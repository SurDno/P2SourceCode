// Decompiled with JetBrains decompiler
// Type: SteamNative.P2PSessionState_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct P2PSessionState_t
  {
    public byte ConnectionActive;
    public byte Connecting;
    public byte P2PSessionError;
    public byte UsingRelay;
    public int BytesQueuedForSend;
    public int PacketsQueuedForSend;
    public uint RemoteIP;
    public ushort RemotePort;

    public static P2PSessionState_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (P2PSessionState_t) (P2PSessionState_t.PackSmall) Marshal.PtrToStructure(p, typeof (P2PSessionState_t.PackSmall)) : (P2PSessionState_t) Marshal.PtrToStructure(p, typeof (P2PSessionState_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public byte ConnectionActive;
      public byte Connecting;
      public byte P2PSessionError;
      public byte UsingRelay;
      public int BytesQueuedForSend;
      public int PacketsQueuedForSend;
      public uint RemoteIP;
      public ushort RemotePort;

      public static implicit operator P2PSessionState_t(P2PSessionState_t.PackSmall d)
      {
        return new P2PSessionState_t()
        {
          ConnectionActive = d.ConnectionActive,
          Connecting = d.Connecting,
          P2PSessionError = d.P2PSessionError,
          UsingRelay = d.UsingRelay,
          BytesQueuedForSend = d.BytesQueuedForSend,
          PacketsQueuedForSend = d.PacketsQueuedForSend,
          RemoteIP = d.RemoteIP,
          RemotePort = d.RemotePort
        };
      }
    }
  }
}
