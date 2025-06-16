using System;
using System.Runtime.InteropServices;

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
