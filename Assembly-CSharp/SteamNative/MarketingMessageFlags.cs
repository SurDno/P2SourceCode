﻿namespace SteamNative
{
  internal enum MarketingMessageFlags
  {
    None = 0,
    HighPriority = 1,
    PlatformWindows = 2,
    PlatformMac = 4,
    PlatformLinux = 8,
    PlatformRestrictions = 14, // 0x0000000E
  }
}
