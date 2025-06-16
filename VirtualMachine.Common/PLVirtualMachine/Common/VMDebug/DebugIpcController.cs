using PLVirtualMachine.Common.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PLVirtualMachine.Common.VMDebug
{
  public class DebugIpcController : IDisposable
  {
    protected string serverAddress;
    protected int portAddress;
    protected Socket socket;
    protected byte[] dataBuffer;
    private EDebugIPCControllerType ipcControllerType;
    private EDebugIPCApplicationWorkMode workMode;
    private Thread ipcMessageLoopThread;
    private List<string> lastErrors = new List<string>();
    private object mainLoopThreadLocker = new object();
    private bool isInited;
    protected static readonly int MAX_DEBUG_MESSAGE_SIZE = 65536;
    protected static readonly int LOOP_SLEEP_TIME = 1;

    public DebugIpcController(EDebugIPCControllerType type)
    {
      this.ipcControllerType = type;
      this.workMode = EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY;
    }

    public List<string> LastErrors
    {
      get
      {
        lock (this.mainLoopThreadLocker)
          return this.lastErrors;
      }
    }

    public virtual void Init(string serverAddressStr = "")
    {
      if (this.isInited)
        return;
      if (serverAddressStr != "")
      {
        string[] strArray = serverAddressStr.Split(';');
        this.serverAddress = strArray[0];
        this.portAddress = StringUtility.ToInt32(strArray[1]);
      }
      else
      {
        this.serverAddress = "localhost";
        this.portAddress = 11000;
      }
      this.dataBuffer = new byte[DebugIpcController.MAX_DEBUG_MESSAGE_SIZE];
      this.isInited = true;
      if (this.IsAsyncWork)
      {
        this.StartAsync();
      }
      else
      {
        this.ipcMessageLoopThread = new Thread(new ThreadStart(this.Start));
        this.ipcMessageLoopThread.Start();
      }
    }

    public void Close()
    {
      this.Stop();
      if (this.ipcMessageLoopThread == null)
        return;
      this.ipcMessageLoopThread.Join();
      this.ipcMessageLoopThread = (Thread) null;
    }

    protected virtual void Start()
    {
    }

    protected virtual void StartAsync()
    {
    }

    protected virtual void Stop() => this.isInited = false;

    public void Dispose() => this.Close();

    public EDebugIPCControllerType ControllerType => this.ipcControllerType;

    public EDebugIPCApplicationWorkMode ControllerWorkMode => this.workMode;

    protected void ProcessLoop()
    {
      while (true)
      {
        ReciveIpcMessage debugMessage = this.OnListen();
        if (debugMessage != null)
        {
          this.ProcessMessage(debugMessage);
          if (this.ControllerType != EDebugIPCControllerType.IPC_DEBUG_CLIENT || this.ControllerWorkMode != EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY)
            Thread.Sleep(DebugIpcController.LOOP_SLEEP_TIME);
          else
            goto label_4;
        }
        else
          break;
      }
      this.OnNeedClose();
      return;
label_4:;
    }

    protected virtual ReciveIpcMessage OnListen() => (ReciveIpcMessage) null;

    protected virtual void ProcessMessage(ReciveIpcMessage debugMessage)
    {
    }

    protected ReciveIpcMessage ReceiveMessage(Socket target)
    {
      try
      {
        int length = target.Receive(this.dataBuffer);
        if (length > 0)
        {
          ReciveIpcMessage message = new ReciveIpcMessage();
          message.Deserialize(this.dataBuffer, length);
          return message;
        }
      }
      catch (Exception ex)
      {
      }
      return (ReciveIpcMessage) null;
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

    protected bool IsAsyncWork
    {
      get => this.ipcControllerType == EDebugIPCControllerType.IPC_DEBUG_SERVER;
    }

    protected IPAddress GetIPAddress()
    {
      try
      {
        IPHostEntry hostEntry = Dns.GetHostEntry(this.serverAddress);
        IPAddress ipAddress = (IPAddress) null;
        foreach (IPAddress address in hostEntry.AddressList)
        {
          if (address.AddressFamily == AddressFamily.InterNetwork)
            ipAddress = address;
        }
        return ipAddress;
      }
      catch (Exception ex)
      {
        this.OnError(string.Format("Cannot find ipc controler ip address by host {0}, error: {1}", (object) this.serverAddress, (object) ex));
        return (IPAddress) null;
      }
    }

    protected virtual void OnNeedClose()
    {
    }

    protected virtual void OnError(string sErrorText)
    {
      lock (this.mainLoopThreadLocker)
        this.lastErrors.Add(sErrorText);
    }

    protected void ResetErrors() => this.lastErrors.Clear();
  }
}
