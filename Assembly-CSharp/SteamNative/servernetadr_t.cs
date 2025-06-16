// Decompiled with JetBrains decompiler
// Type: SteamNative.servernetadr_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
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
