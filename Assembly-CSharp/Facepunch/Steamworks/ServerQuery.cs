// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.ServerQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Facepunch.Steamworks
{
  public class ServerQuery
  {
    internal Server server;
    internal static byte[] buffer = new byte[65536];

    internal ServerQuery(Server s) => this.server = s;

    public unsafe bool GetOutgoingPacket(out ServerQuery.Packet packet)
    {
      packet = new ServerQuery.Packet();
      fixed (byte* pOut = ServerQuery.buffer)
      {
        uint pNetAdr = 0;
        ushort pPort = 0;
        int nextOutgoingPacket = this.server.native.gameServer.GetNextOutgoingPacket((IntPtr) (void*) pOut, ServerQuery.buffer.Length, out pNetAdr, out pPort);
        if (nextOutgoingPacket == 0)
          return false;
        packet.Size = nextOutgoingPacket;
        packet.Data = ServerQuery.buffer;
        packet.Address = pNetAdr;
        packet.Port = pPort;
        return true;
      }
    }

    public unsafe void Handle(byte[] data, int size, uint address, ushort port)
    {
      fixed (byte* pData = data)
        this.server.native.gameServer.HandleIncomingPacket((IntPtr) (void*) pData, size, address, port);
    }

    public struct Packet
    {
      public uint Address { get; internal set; }

      public ushort Port { get; internal set; }

      public byte[] Data { get; internal set; }

      public int Size { get; internal set; }
    }
  }
}
