﻿namespace SteamNative
{
  internal enum FriendFlags
  {
    None = 0,
    Blocked = 1,
    FriendshipRequested = 2,
    Immediate = 4,
    ClanMember = 8,
    OnGameServer = 16,
    RequestingFriendship = 128,
    RequestingInfo = 256,
    Ignored = 512,
    IgnoredFriend = 1024,
    ChatMember = 4096,
    All = 65535,
  }
}
