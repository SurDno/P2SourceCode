namespace SteamNative
{
  internal enum SteamAPICallFailure
  {
    None = -1, // 0xFFFFFFFF
    SteamGone = 0,
    NetworkFailure = 1,
    InvalidHandle = 2,
    MismatchedCallback = 3,
  }
}
