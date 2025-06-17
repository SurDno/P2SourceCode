using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common.VMDebug
{
  public class DebugIpcController(EDebugIPCControllerType type) : IDisposable 
  {
    protected string serverAddress;
    protected int portAddress;
    protected Socket socket;
    protected byte[] dataBuffer;
    private EDebugIPCApplicationWorkMode workMode = EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY;
    private Thread ipcMessageLoopThread;
    private List<string> lastErrors = [];
    private object mainLoopThreadLocker = new();
    private bool isInited;
    protected static readonly int MAX_DEBUG_MESSAGE_SIZE = 65536;
    protected static readonly int LOOP_SLEEP_TIME = 1;

    public List<string> LastErrors
    {
      get
      {
        lock (mainLoopThreadLocker)
          return lastErrors;
      }
    }

    public virtual void Init(string serverAddressStr = "")
    {
      if (isInited)
        return;
      if (serverAddressStr != "")
      {
        string[] strArray = serverAddressStr.Split(';');
        serverAddress = strArray[0];
        portAddress = StringUtility.ToInt32(strArray[1]);
      }
      else
      {
        serverAddress = "localhost";
        portAddress = 11000;
      }
      dataBuffer = new byte[MAX_DEBUG_MESSAGE_SIZE];
      isInited = true;
      if (IsAsyncWork)
      {
        StartAsync();
      }
      else
      {
        ipcMessageLoopThread = new Thread(Start);
        ipcMessageLoopThread.Start();
      }
    }

    public void Close()
    {
      Stop();
      if (ipcMessageLoopThread == null)
        return;
      ipcMessageLoopThread.Join();
      ipcMessageLoopThread = null;
    }

    protected virtual void Start()
    {
    }

    protected virtual void StartAsync()
    {
    }

    protected virtual void Stop() => isInited = false;

    public void Dispose() => Close();

    public EDebugIPCControllerType ControllerType => type;

    public EDebugIPCApplicationWorkMode ControllerWorkMode => workMode;

    protected void ProcessLoop()
    {
      while (true)
      {
        ReciveIpcMessage debugMessage = OnListen();
        if (debugMessage != null)
        {
          ProcessMessage(debugMessage);
          if (ControllerType != EDebugIPCControllerType.IPC_DEBUG_CLIENT || ControllerWorkMode != EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY)
            Thread.Sleep(LOOP_SLEEP_TIME);
          else
            goto label_4;
        }
        else
          break;
      }
      OnNeedClose();
      return;
label_4:;
    }

    protected virtual ReciveIpcMessage OnListen() => null;

    protected virtual void ProcessMessage(ReciveIpcMessage debugMessage)
    {
    }

    protected ReciveIpcMessage ReceiveMessage(Socket target)
    {
      try
      {
        int length = target.Receive(dataBuffer);
        if (length > 0)
        {
          ReciveIpcMessage message = new ReciveIpcMessage();
          message.Deserialize(dataBuffer, length);
          return message;
        }
      }
      catch (Exception ex)
      {
      }
      return null;
    }

    protected void SendMessage(Socket target, SendIpcMessage debugMessage)
    {
      byte[] buffer = debugMessage.Serialize();
      target.Send(buffer);
    }

    protected virtual void SetWorkMode(EDebugIPCApplicationWorkMode workMode)
    {
      this.workMode = workMode;
    }

    protected bool IsAsyncWork => type == EDebugIPCControllerType.IPC_DEBUG_SERVER;

    protected IPAddress GetIPAddress()
    {
      try
      {
        IPHostEntry hostEntry = Dns.GetHostEntry(serverAddress);
        IPAddress ipAddress = null;
        foreach (IPAddress address in hostEntry.AddressList)
        {
          if (address.AddressFamily == AddressFamily.InterNetwork)
            ipAddress = address;
        }
        return ipAddress;
      }
      catch (Exception ex)
      {
        OnError(string.Format("Cannot find ipc controler ip address by host {0}, error: {1}", serverAddress, ex));
        return null;
      }
    }

    protected virtual void OnNeedClose()
    {
    }

    protected virtual void OnError(string sErrorText)
    {
      lock (mainLoopThreadLocker)
        lastErrors.Add(sErrorText);
    }

    protected void ResetErrors() => lastErrors.Clear();
  }
}
