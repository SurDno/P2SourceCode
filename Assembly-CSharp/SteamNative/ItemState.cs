namespace SteamNative
{
  internal enum ItemState
  {
    None = 0,
    Subscribed = 1,
    LegacyItem = 2,
    Installed = 4,
    NeedsUpdate = 8,
    Downloading = 16, // 0x00000010
    DownloadPending = 32, // 0x00000020
  }
}
