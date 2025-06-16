using System;

namespace Facepunch.Steamworks
{
  public class ServerQuery
  {
    internal Server server;
    internal static byte[] buffer = new byte[65536];

    internal ServerQuery(Server s) => server = s;

    public unsafe bool GetOutgoingPacket(out Packet packet)
    {
      packet = new Packet();
      fixed (byte* pOut = buffer)
      {
        uint pNetAdr = 0;
        ushort pPort = 0;
        int nextOutgoingPacket = server.native.gameServer.GetNextOutgoingPacket((IntPtr) pOut, buffer.Length, out pNetAdr, out pPort);
        if (nextOutgoingPacket == 0)
          return false;
        packet.Size = nextOutgoingPacket;
        packet.Data = buffer;
        packet.Address = pNetAdr;
        packet.Port = pPort;
        return true;
      }
    }

    public unsafe void Handle(byte[] data, int size, uint address, ushort port)
    {
      fixed (byte* pData = data)
        server.native.gameServer.HandleIncomingPacket((IntPtr) pData, size, address, port);
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
