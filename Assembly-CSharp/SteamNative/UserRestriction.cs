namespace SteamNative
{
  internal enum UserRestriction
  {
    None = 0,
    Unknown = 1,
    AnyChat = 2,
    VoiceChat = 4,
    GroupChat = 8,
    Rating = 16, // 0x00000010
    GameInvites = 32, // 0x00000020
    Trading = 64, // 0x00000040
  }
}
