using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Cofe.Loggers;

namespace PLVirtualMachine.Common.VMDebug;

public class DebugServer : DebugIpcController {
	private Queue<ReciveIpcMessage> fromClientMessagesQueue = new();
	private Queue<SendIpcMessage> toClientMessagesQueue = new();
	private Socket currentClient;
	private bool isNeedStop;
	private object fromClientMessagesLocker = new();
	private object serverMessagesLocker = new();
	private static readonly int MAX_CLIENTS_COUNT = 10;

	public DebugServer()
		: base(EDebugIPCControllerType.IPC_DEBUG_SERVER) { }

	protected override void Start() {
		var ipAddress = GetIPAddress();
		if (ipAddress == null)
			return;
		var localEP = new IPEndPoint(ipAddress, portAddress);
		currentClient = null;
		try {
			socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			socket.Bind(localEP);
			socket.Listen(MAX_CLIENTS_COUNT);
			ProcessLoop();
		} catch (Exception ex) {
			OnError("Debug controller server socket error: " + ex);
		}
	}

	protected override void StartAsync() {
		var ipAddress = GetIPAddress();
		if (ipAddress == null)
			OnError(string.Format("Server debugger error: not finded addres by host {0}!", serverAddress));
		else {
			var localEP = new IPEndPoint(ipAddress, portAddress);
			try {
				socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				socket.Bind(localEP);
				socket.Listen(MAX_CLIENTS_COUNT);
				Logger.AddInfo("Start async debug server listening");
				socket.BeginAccept(OnAsyncConnect, socket);
			} catch (Exception ex) {
				OnError("Server debugger error: " + ex);
			}
		}
	}

	protected void OnAsyncConnect(IAsyncResult ar) {
		currentClient = ((Socket)ar.AsyncState).EndAccept(ar);
		Logger.AddInfo("Client " + currentClient.RemoteEndPoint + " connected");
		SetWorkMode(EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_DEBUG);
		isNeedStop = false;
		StartAsyncListening();
	}

	protected void StartAsyncListening() {
		var state = new AsyncDataReceiver(currentClient, dataBuffer, MAX_DEBUG_MESSAGE_SIZE);
		currentClient.BeginReceive(state.DataBuffer, 0, state.BufferSize, SocketFlags.None, OnAsyncReceive, state);
	}

	protected void OnAsyncReceive(IAsyncResult ar) {
		var asyncState = (AsyncDataReceiver)ar.AsyncState;
		var length = asyncState.WorkSocket.EndReceive(ar);
		if (length <= 0)
			return;
		var debugMessage = new ReciveIpcMessage();
		if (!debugMessage.IsValid)
			OnError(debugMessage.LastError);
		debugMessage.Deserialize(asyncState.DataBuffer, length);
		ProcessMessage(debugMessage);
	}

	protected void AsyncSendMessage(Socket client, SendIpcMessage serverResponse) {
		var buffer = serverResponse.Serialize();
		client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, OnAsyncSend, client);
	}

	protected void OnAsyncSend(IAsyncResult ar) {
		socket.EndAccept(ar);
		if (isNeedStop)
			CloseClient();
		else
			StartAsyncListening();
	}

	protected override ReciveIpcMessage OnListen() {
		var client = GetClient();
		return client == null ? null : ReceiveMessage(client);
	}

	protected override void ProcessMessage(ReciveIpcMessage clientMessage) {
		if (clientMessage == null)
			return;
		var flag = false;
		SendIpcMessage sendIpcMessage = null;
		if (clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT ||
		    clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT ||
		    clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT ||
		    clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_INTO ||
		    clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_OVER ||
		    clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_CONTINUE ||
		    clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_CAMERA ||
		    clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_RAISE_EVENT) {
			lock (fromClientMessagesLocker) {
				fromClientMessagesQueue.Enqueue(clientMessage);
			}

			sendIpcMessage = new SendIpcMessage(clientMessage.MessageType + 11);
			if (!sendIpcMessage.IsValid)
				OnError(sendIpcMessage.LastError);
			sendIpcMessage.CopyData(clientMessage);
		} else if (clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STOP_DEBUG) {
			flag = true;
			sendIpcMessage = new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_STOP_DEBUG);
			if (!sendIpcMessage.IsValid)
				OnError(sendIpcMessage.LastError);
			sendIpcMessage.CopyData(clientMessage);
		} else if (clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SERVER_EVENT) {
			lock (serverMessagesLocker) {
				sendIpcMessage = toClientMessagesQueue.Count <= 0
					? new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_NONE)
					: toClientMessagesQueue.Dequeue();
			}

			if (!sendIpcMessage.IsValid)
				OnError(sendIpcMessage.LastError);
		}

		if (sendIpcMessage != null) {
			if (IsAsyncWork)
				AsyncSendMessage(currentClient, sendIpcMessage);
			else
				SendMessage(currentClient, sendIpcMessage);
		} else
			OnError(string.Format("Invalid debug client message type: {0}", clientMessage.MessageType.ToString()));

		if (flag && !IsAsyncWork)
			CloseClient();
		else {
			if (!flag)
				return;
			isNeedStop = true;
		}
	}

	protected void ClearMessages() {
		lock (serverMessagesLocker) {
			toClientMessagesQueue.Clear();
		}
	}

	protected void AddMessageToClient(SendIpcMessage messageToClient) {
		lock (serverMessagesLocker) {
			toClientMessagesQueue.Enqueue(messageToClient);
		}
	}

	protected int MessagesToClienCount() {
		lock (serverMessagesLocker) {
			return toClientMessagesQueue.Count;
		}
	}

	protected ReciveIpcMessage PopMessageFromClient() {
		ReciveIpcMessage reciveIpcMessage = null;
		lock (fromClientMessagesLocker) {
			if (fromClientMessagesQueue.Count > 0)
				reciveIpcMessage = fromClientMessagesQueue.Dequeue();
		}

		return reciveIpcMessage;
	}

	private void CloseClient() {
		currentClient.Shutdown(SocketShutdown.Both);
		currentClient.Close();
		currentClient = null;
		lock (serverMessagesLocker) {
			toClientMessagesQueue.Clear();
		}

		SetWorkMode(EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY);
		Logger.AddInfo("Start async debug server listening");
		socket.BeginAccept(OnAsyncConnect, socket);
	}

	private Socket GetClient() {
		if (currentClient != null && !currentClient.Connected)
			CloseClient();
		do {
			try {
				if (currentClient == null) {
					Logger.AddInfo(string.Format("Start listen to client: address={0}, port={1}", serverAddress,
						portAddress));
					currentClient = socket.Accept();
				}
			} catch (Exception ex) {
				currentClient = null;
				OnError("Debug controller server socket error: " + ex);
			}
		} while (currentClient == null);

		if (ControllerWorkMode == EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY) {
			SetWorkMode(EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_DEBUG);
			Logger.AddInfo("Client has connected!");
		}

		return currentClient;
	}

	public class AsyncDataReceiver {
		public Socket WorkSocket;
		public int BufferSize;
		public byte[] DataBuffer;

		public AsyncDataReceiver(Socket workSocket, byte[] buffer, int bufferSize) {
			WorkSocket = workSocket;
			DataBuffer = buffer;
			BufferSize = bufferSize;
		}
	}
}