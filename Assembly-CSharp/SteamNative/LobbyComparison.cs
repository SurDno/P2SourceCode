namespace SteamNative
{
  internal enum LobbyComparison
  {
    EqualToOrLessThan = -2, // 0xFFFFFFFE
    LessThan = -1, // 0xFFFFFFFF
    Equal = 0,
    GreaterThan = 1,
    EqualToOrGreaterThan = 2,
    NotEqual = 3,
  }
}
