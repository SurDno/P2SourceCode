// Decompiled with JetBrains decompiler
// Type: SteamNative.FriendGameInfo_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  internal struct FriendGameInfo_t
  {
    public ulong GameID;
    public uint GameIP;
    public ushort GamePort;
    public ushort QueryPort;
    public ulong SteamIDLobby;

    public static FriendGameInfo_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (FriendGameInfo_t) (FriendGameInfo_t.PackSmall) Marshal.PtrToStructure(p, typeof (FriendGameInfo_t.PackSmall)) : (FriendGameInfo_t) Marshal.PtrToStructure(p, typeof (FriendGameInfo_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public ulong GameID;
      public uint GameIP;
      public ushort GamePort;
      public ushort QueryPort;
      public ulong SteamIDLobby;

      public static implicit operator FriendGameInfo_t(FriendGameInfo_t.PackSmall d)
      {
        return new FriendGameInfo_t()
        {
          GameID = d.GameID,
          GameIP = d.GameIP,
          GamePort = d.GamePort,
          QueryPort = d.QueryPort,
          SteamIDLobby = d.SteamIDLobby
        };
      }
    }
  }
}
