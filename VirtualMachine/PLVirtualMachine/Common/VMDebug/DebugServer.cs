// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.VMDebug.DebugServer
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

#nullable disable
namespace PLVirtualMachine.Common.VMDebug
{
  public class DebugServer : DebugIpcController
  {
    private Queue<ReciveIpcMessage> fromClientMessagesQueue = new Queue<ReciveIpcMessage>();
    private Queue<SendIpcMessage> toClientMessagesQueue = new Queue<SendIpcMessage>();
    private Socket currentClient;
    private bool isNeedStop;
    private object fromClientMessagesLocker = new object();
    private object serverMessagesLocker = new object();
    private static readonly int MAX_CLIENTS_COUNT = 10;

    public DebugServer()
      : base(EDebugIPCControllerType.IPC_DEBUG_SERVER)
    {
    }

    protected override void Start()
    {
      IPAddress ipAddress = this.GetIPAddress();
      if (ipAddress == null)
        return;
      IPEndPoint localEP = new IPEndPoint(ipAddress, this.portAddress);
      this.currentClient = (Socket) null;
      try
      {
        this.socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        this.socket.Bind((EndPoint) localEP);
        this.socket.Listen(DebugServer.MAX_CLIENTS_COUNT);
        this.ProcessLoop();
      }
      catch (Exception ex)
      {
        this.OnError("Debug controller server socket error: " + (object) ex);
      }
    }

    protected override void StartAsync()
    {
      IPAddress ipAddress = this.GetIPAddress();
      if (ipAddress == null)
      {
        this.OnError(string.Format("Server debugger error: not finded addres by host {0}!", (object) this.serverAddress));
      }
      else
      {
        IPEndPoint localEP = new IPEndPoint(ipAddress, this.portAddress);
        try
        {
          this.socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
          this.socket.Bind((EndPoint) localEP);
          this.socket.Listen(DebugServer.MAX_CLIENTS_COUNT);
          Logger.AddInfo("Start async debug server listening");
          this.socket.BeginAccept(new AsyncCallback(this.OnAsyncConnect), (object) this.socket);
        }
        catch (Exception ex)
        {
          this.OnError("Server debugger error: " + ex.ToString());
        }
      }
    }

    protected void OnAsyncConnect(IAsyncResult ar)
    {
      this.currentClient = ((Socket) ar.AsyncState).EndAccept(ar);
      Logger.AddInfo("Client " + (object) this.currentClient.RemoteEndPoint + " connected");
      this.SetWorkMode(EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_DEBUG);
      this.isNeedStop = false;
      this.StartAsyncListening();
    }

    protected void StartAsyncListening()
    {
      DebugServer.AsyncDataReceiver state = new DebugServer.AsyncDataReceiver(this.currentClient, this.dataBuffer, DebugIpcController.MAX_DEBUG_MESSAGE_SIZE);
      this.currentClient.BeginReceive(state.DataBuffer, 0, state.BufferSize, SocketFlags.None, new AsyncCallback(this.OnAsyncReceive), (object) state);
    }

    protected void OnAsyncReceive(IAsyncResult ar)
    {
      DebugServer.AsyncDataReceiver asyncState = (DebugServer.AsyncDataReceiver) ar.AsyncState;
      int length = asyncState.WorkSocket.EndReceive(ar);
      if (length <= 0)
        return;
      ReciveIpcMessage debugMessage = new ReciveIpcMessage();
      if (!debugMessage.IsValid)
        this.OnError(debugMessage.LastError);
      debugMessage.Deserialize(asyncState.DataBuffer, length);
      this.ProcessMessage(debugMessage);
    }

    protected void AsyncSendMessage(Socket client, SendIpcMessage serverResponse)
    {
      byte[] buffer = serverResponse.Serialize();
      client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(this.OnAsyncSend), (object) client);
    }

    protected void OnAsyncSend(IAsyncResult ar)
    {
      this.socket.EndAccept(ar);
      if (this.isNeedStop)
        this.CloseClient();
      else
        this.StartAsyncListening();
    }

    protected override ReciveIpcMessage OnListen()
    {
      Socket client = this.GetClient();
      return client == null ? (ReciveIpcMessage) null : this.ReceiveMessage(client);
    }

    protected override void ProcessMessage(ReciveIpcMessage clientMessage)
    {
      if (clientMessage == null)
        return;
      bool flag = false;
      SendIpcMessage sendIpcMessage = (SendIpcMessage) null;
      if (clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT || clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT || clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT || clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_INTO || clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_OVER || clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_CONTINUE || clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_CAMERA || clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_RAISE_EVENT)
      {
        lock (this.fromClientMessagesLocker)
          this.fromClientMessagesQueue.Enqueue(clientMessage);
        sendIpcMessage = new SendIpcMessage(clientMessage.MessageType + 11);
        if (!sendIpcMessage.IsValid)
          this.OnError(sendIpcMessage.LastError);
        sendIpcMessage.CopyData(clientMessage);
      }
      else if (clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STOP_DEBUG)
      {
        flag = true;
        sendIpcMessage = new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_STOP_DEBUG);
        if (!sendIpcMessage.IsValid)
          this.OnError(sendIpcMessage.LastError);
        sendIpcMessage.CopyData(clientMessage);
      }
      else if (clientMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SERVER_EVENT)
      {
        lock (this.serverMessagesLocker)
          sendIpcMessage = this.toClientMessagesQueue.Count <= 0 ? new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_NONE) : this.toClientMessagesQueue.Dequeue();
        if (!sendIpcMessage.IsValid)
          this.OnError(sendIpcMessage.LastError);
      }
      if (sendIpcMessage != null)
      {
        if (this.IsAsyncWork)
          this.AsyncSendMessage(this.currentClient, sendIpcMessage);
        else
          this.SendMessage(this.currentClient, sendIpcMessage);
      }
      else
        this.OnError(string.Format("Invalid debug client message type: {0}", (object) clientMessage.MessageType.ToString()));
      if (flag && !this.IsAsyncWork)
      {
        this.CloseClient();
      }
      else
      {
        if (!flag)
          return;
        this.isNeedStop = true;
      }
    }

    protected void ClearMessages()
    {
      lock (this.serverMessagesLocker)
        this.toClientMessagesQueue.Clear();
    }

    protected void AddMessageToClient(SendIpcMessage messageToClient)
    {
      lock (this.serverMessagesLocker)
        this.toClientMessagesQueue.Enqueue(messageToClient);
    }

    protected int MessagesToClienCount()
    {
      lock (this.serverMessagesLocker)
        return this.toClientMessagesQueue.Count;
    }

    protected ReciveIpcMessage PopMessageFromClient()
    {
      ReciveIpcMessage reciveIpcMessage = (ReciveIpcMessage) null;
      lock (this.fromClientMessagesLocker)
      {
        if (this.fromClientMessagesQueue.Count > 0)
          reciveIpcMessage = this.fromClientMessagesQueue.Dequeue();
      }
      return reciveIpcMessage;
    }

    private void CloseClient()
    {
      this.currentClient.Shutdown(SocketShutdown.Both);
      this.currentClient.Close();
      this.currentClient = (Socket) null;
      lock (this.serverMessagesLocker)
        this.toClientMessagesQueue.Clear();
      this.SetWorkMode(EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY);
      Logger.AddInfo("Start async debug server listening");
      this.socket.BeginAccept(new AsyncCallback(this.OnAsyncConnect), (object) this.socket);
    }

    private Socket GetClient()
    {
      if (this.currentClient != null && !this.currentClient.Connected)
        this.CloseClient();
      do
      {
        try
        {
          if (this.currentClient == null)
          {
            Logger.AddInfo(string.Format("Start listen to client: address={0}, port={1}", (object) this.serverAddress, (object) this.portAddress));
            this.currentClient = this.socket.Accept();
          }
        }
        catch (Exception ex)
        {
          this.currentClient = (Socket) null;
          this.OnError("Debug controller server socket error: " + (object) ex);
        }
      }
      while (this.currentClient == null);
      if (this.ControllerWorkMode == EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY)
      {
        this.SetWorkMode(EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_DEBUG);
        Logger.AddInfo("Client has connected!");
      }
      return this.currentClient;
    }

    public class AsyncDataReceiver
    {
      public Socket WorkSocket;
      public int BufferSize;
      public byte[] DataBuffer;

      public AsyncDataReceiver(Socket workSocket, byte[] buffer, int bufferSize)
      {
        this.WorkSocket = workSocket;
        this.DataBuffer = buffer;
        this.BufferSize = bufferSize;
      }
    }
  }
}
