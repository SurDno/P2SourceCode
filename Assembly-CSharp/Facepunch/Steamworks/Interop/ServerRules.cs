using System;
using System.Net;
using System.Runtime.InteropServices;

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
      Server = server;
      InstallVTable();
      Server.Client.native.servers.ServerRules(address.IpToInt32(), (ushort) queryPort, GetPtr());
    }

    public void Dispose()
    {
      if (vTablePtr != IntPtr.Zero)
      {
        Marshal.FreeHGlobal(vTablePtr);
        vTablePtr = IntPtr.Zero;
      }
      if (vTablePin.IsAllocated)
        vTablePin.Free();
      if (RulesRespondPin.IsAllocated)
        RulesRespondPin.Free();
      if (FailedRespondPin.IsAllocated)
        FailedRespondPin.Free();
      if (!CompletePin.IsAllocated)
        return;
      CompletePin.Free();
    }

    private void InstallVTable()
    {
      if (Config.UseThisCall)
      {
        ThisVTable.InternalRulesResponded internalRulesResponded = (_, k, v) => InternalOnRulesResponded(k, v);
        ThisVTable.InternalRulesFailedToRespond rulesFailedToRespond = _ => InternalOnRulesFailedToRespond();
        ThisVTable.InternalRulesRefreshComplete rulesRefreshComplete = _ => InternalOnRulesRefreshComplete();
        RulesRespondPin = GCHandle.Alloc(internalRulesResponded);
        FailedRespondPin = GCHandle.Alloc(rulesFailedToRespond);
        CompletePin = GCHandle.Alloc(rulesRefreshComplete);
        ThisVTable structure = new ThisVTable {
          m_VTRulesResponded = internalRulesResponded,
          m_VTRulesFailedToRespond = rulesFailedToRespond,
          m_VTRulesRefreshComplete = rulesRefreshComplete
        };
        vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (ThisVTable)));
        Marshal.StructureToPtr(structure, vTablePtr, false);
        vTablePin = GCHandle.Alloc(vTablePtr, GCHandleType.Pinned);
      }
      else
      {
        StdVTable.InternalRulesResponded internalRulesResponded = InternalOnRulesResponded;
        StdVTable.InternalRulesFailedToRespond rulesFailedToRespond = InternalOnRulesFailedToRespond;
        StdVTable.InternalRulesRefreshComplete rulesRefreshComplete = InternalOnRulesRefreshComplete;
        RulesRespondPin = GCHandle.Alloc(internalRulesResponded);
        FailedRespondPin = GCHandle.Alloc(rulesFailedToRespond);
        CompletePin = GCHandle.Alloc(rulesRefreshComplete);
        StdVTable structure = new StdVTable {
          m_VTRulesResponded = internalRulesResponded,
          m_VTRulesFailedToRespond = rulesFailedToRespond,
          m_VTRulesRefreshComplete = rulesRefreshComplete
        };
        vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (StdVTable)));
        Marshal.StructureToPtr(structure, vTablePtr, false);
        vTablePin = GCHandle.Alloc(vTablePtr, GCHandleType.Pinned);
      }
    }

    private void InternalOnRulesResponded(string k, string v) => Server.Rules.Add(k, v);

    private void InternalOnRulesFailedToRespond()
    {
      Server.OnServerRulesReceiveFinished(false);
    }

    private void InternalOnRulesRefreshComplete() => Server.OnServerRulesReceiveFinished(true);

    public IntPtr GetPtr() => vTablePin.AddrOfPinnedObject();

    [StructLayout(LayoutKind.Sequential)]
    private class StdVTable
    {
      [MarshalAs(UnmanagedType.FunctionPtr)]
      public InternalRulesResponded m_VTRulesResponded;
      [MarshalAs(UnmanagedType.FunctionPtr)]
      public InternalRulesFailedToRespond m_VTRulesFailedToRespond;
      [MarshalAs(UnmanagedType.FunctionPtr)]
      public InternalRulesRefreshComplete m_VTRulesRefreshComplete;

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
      public InternalRulesResponded m_VTRulesResponded;
      [MarshalAs(UnmanagedType.FunctionPtr)]
      public InternalRulesFailedToRespond m_VTRulesFailedToRespond;
      [MarshalAs(UnmanagedType.FunctionPtr)]
      public InternalRulesRefreshComplete m_VTRulesRefreshComplete;

      [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
      public delegate void InternalRulesResponded(IntPtr thisptr, string pchRule, string pchValue);

      [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
      public delegate void InternalRulesFailedToRespond(IntPtr thisptr);

      [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
      public delegate void InternalRulesRefreshComplete(IntPtr thisptr);
    }
  }
}
