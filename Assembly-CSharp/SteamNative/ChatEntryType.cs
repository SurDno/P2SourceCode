namespace SteamNative
{
  internal enum ChatEntryType
  {
    Invalid = 0,
    ChatMsg = 1,
    Typing = 2,
    InviteGame = 3,
    Emote = 4,
    LeftConversation = 6,
    Entered = 7,
    WasKicked = 8,
    WasBanned = 9,
    Disconnected = 10, // 0x0000000A
    HistoricalChat = 11, // 0x0000000B
    LinkBlocked = 14, // 0x0000000E
  }
}
