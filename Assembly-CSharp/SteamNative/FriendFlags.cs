// Decompiled with JetBrains decompiler
// Type: SteamNative.FriendFlags
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal enum FriendFlags
  {
    None = 0,
    Blocked = 1,
    FriendshipRequested = 2,
    Immediate = 4,
    ClanMember = 8,
    OnGameServer = 16, // 0x00000010
    RequestingFriendship = 128, // 0x00000080
    RequestingInfo = 256, // 0x00000100
    Ignored = 512, // 0x00000200
    IgnoredFriend = 1024, // 0x00000400
    ChatMember = 4096, // 0x00001000
    All = 65535, // 0x0000FFFF
  }
}
