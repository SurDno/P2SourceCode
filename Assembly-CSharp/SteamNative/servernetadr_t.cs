using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct servernetadr_t
  {
    public ushort ConnectionPort;
    public ushort QueryPort;
    public uint IP;

    public static servernetadr_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (servernetadr_t) (servernetadr_t.PackSmall) Marshal.PtrToStructure(p, typeof (servernetadr_t.PackSmall)) : (servernetadr_t) Marshal.PtrToStructure(p, typeof (servernetadr_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public ushort ConnectionPort;
      public ushort QueryPort;
      public uint IP;

      public static implicit operator servernetadr_t(servernetadr_t.PackSmall d)
      {
        return new servernetadr_t()
        {
          ConnectionPort = d.ConnectionPort,
          QueryPort = d.QueryPort,
          IP = d.IP
        };
      }
    }
  }
}
