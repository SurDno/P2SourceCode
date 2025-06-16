namespace SteamNative
{
  internal enum ChatRoomEnterResponse
  {
    Success = 1,
    DoesntExist = 2,
    NotAllowed = 3,
    Full = 4,
    Error = 5,
    Banned = 6,
    Limited = 7,
    ClanDisabled = 8,
    CommunityBan = 9,
    MemberBlockedYou = 10, // 0x0000000A
    YouBlockedMember = 11, // 0x0000000B
  }
}
