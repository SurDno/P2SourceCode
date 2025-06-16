// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Interop.ServerRules
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Net;
using System.Runtime.InteropServices;

#nullable disable
namespace Facepunch.Steamworks.Interop
{
  internal class ServerRules : IDisposable
  {
    private GCHandle vTablePin;
    private IntPtr vTablePtr;
    private GCHandle RulesRespondPin;
    private GCHandle FailedRespondPin;
    private GCHandle CompletePin;
    private ServerList.Server Server;

    public ServerRules(ServerList.Server server, IPAddress address, int queryPort)
    {
      this.Server = server;
      this.InstallVTable();
      this.Server.Client.native.servers.ServerRules(address.IpToInt32(), (ushort) queryPort, this.GetPtr());
    }

    public void Dispose()
    {
      if (this.vTablePtr != IntPtr.Zero)
      {
        Marshal.FreeHGlobal(this.vTablePtr);
        this.vTablePtr = IntPtr.Zero;
      }
      if (this.vTablePin.IsAllocated)
        this.vTablePin.Free();
      if (this.RulesRespondPin.IsAllocated)
        this.RulesRespondPin.Free();
      if (this.FailedRespondPin.IsAllocated)
        this.FailedRespondPin.Free();
      if (!this.CompletePin.IsAllocated)
        return;
      this.CompletePin.Free();
    }

    private void InstallVTable()
    {
      if (Config.UseThisCall)
      {
        ServerRules.ThisVTable.InternalRulesResponded internalRulesResponded = (ServerRules.ThisVTable.InternalRulesResponded) ((_, k, v) => this.InternalOnRulesResponded(k, v));
        ServerRules.ThisVTable.InternalRulesFailedToRespond rulesFailedToRespond = (ServerRules.ThisVTable.InternalRulesFailedToRespond) (_ => this.InternalOnRulesFailedToRespond());
        ServerRules.ThisVTable.InternalRulesRefreshComplete rulesRefreshComplete = (ServerRules.ThisVTable.InternalRulesRefreshComplete) (_ => this.InternalOnRulesRefreshComplete());
        this.RulesRespondPin = GCHandle.Alloc((object) internalRulesResponded);
        this.FailedRespondPin = GCHandle.Alloc((object) rulesFailedToRespond);
        this.CompletePin = GCHandle.Alloc((object) rulesRefreshComplete);
        ServerRules.ThisVTable structure = new ServerRules.ThisVTable()
        {
          m_VTRulesResponded = internalRulesResponded,
          m_VTRulesFailedToRespond = rulesFailedToRespond,
          m_VTRulesRefreshComplete = rulesRefreshComplete
        };
        this.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (ServerRules.ThisVTable)));
        Marshal.StructureToPtr<ServerRules.ThisVTable>(structure, this.vTablePtr, false);
        this.vTablePin = GCHandle.Alloc((object) this.vTablePtr, GCHandleType.Pinned);
      }
      else
      {
        ServerRules.StdVTable.InternalRulesResponded internalRulesResponded = new ServerRules.StdVTable.InternalRulesResponded(this.InternalOnRulesResponded);
        ServerRules.StdVTable.InternalRulesFailedToRespond rulesFailedToRespond = new ServerRules.StdVTable.InternalRulesFailedToRespond(this.InternalOnRulesFailedToRespond);
        ServerRules.StdVTable.InternalRulesRefreshComplete rulesRefreshComplete = new ServerRules.StdVTable.InternalRulesRefreshComplete(this.InternalOnRulesRefreshComplete);
        this.RulesRespondPin = GCHandle.Alloc((object) internalRulesResponded);
        this.FailedRespondPin = GCHandle.Alloc((object) rulesFailedToRespond);
        this.CompletePin = GCHandle.Alloc((object) rulesRefreshComplete);
        ServerRules.StdVTable structure = new ServerRules.StdVTable()
        {
          m_VTRulesResponded = internalRulesResponded,
          m_VTRulesFailedToRespond = rulesFailedToRespond,
          m_VTRulesRefreshComplete = rulesRefreshComplete
        };
        this.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (ServerRules.StdVTable)));
        Marshal.StructureToPtr<ServerRules.StdVTable>(structure, this.vTablePtr, false);
        this.vTablePin = GCHandle.Alloc((object) this.vTablePtr, GCHandleType.Pinned);
      }
    }

    private void InternalOnRulesResponded(string k, string v) => this.Server.Rules.Add(k, v);

    private void InternalOnRulesFailedToRespond()
    {
      this.Server.OnServerRulesReceiveFinished(false);
    }

    private void InternalOnRulesRefreshComplete() => this.Server.OnServerRulesReceiveFinished(true);

    public IntPtr GetPtr() => this.vTablePin.AddrOfPinnedObject();

    [StructLayout(LayoutKind.Sequential)]
    private class StdVTable
    {
      [MarshalAs(UnmanagedType.FunctionPtr)]
      public ServerRules.StdVTable.InternalRulesResponded m_VTRulesResponded;
      [MarshalAs(UnmanagedType.FunctionPtr)]
      public ServerRules.StdVTable.InternalRulesFailedToRespond m_VTRulesFailedToRespond;
      [MarshalAs(UnmanagedType.FunctionPtr)]
      public ServerRules.StdVTable.InternalRulesRefreshComplete m_VTRulesRefreshComplete;

      [UnmanagedFunctionPointer(CallingConvention.StdCall)]
      public delegate void InternalRulesResponded(string pchRule, string pchValue);

      [UnmanagedFunctionPointer(CallingConvention.StdCall)]
      public delegate void InternalRulesFailedToRespond();

      [UnmanagedFunctionPointer(CallingConvention.StdCall)]
      public delegate void InternalRulesRefreshComplete();
    }

    [StructLayout(LayoutKind.Sequential)]
    private class ThisVTable
    {
      [MarshalAs(UnmanagedType.FunctionPtr)]
      public ServerRules.ThisVTable.InternalRulesResponded m_VTRulesResponded;
      [MarshalAs(UnmanagedType.FunctionPtr)]
      public ServerRules.ThisVTable.InternalRulesFailedToRespond m_VTRulesFailedToRespond;
      [MarshalAs(UnmanagedType.FunctionPtr)]
      public ServerRules.ThisVTable.InternalRulesRefreshComplete m_VTRulesRefreshComplete;

      [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
      public delegate void InternalRulesResponded(IntPtr thisptr, string pchRule, string pchValue);

      [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
      public delegate void InternalRulesFailedToRespond(IntPtr thisptr);

      [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
      public delegate void InternalRulesRefreshComplete(IntPtr thisptr);
    }
  }
}
