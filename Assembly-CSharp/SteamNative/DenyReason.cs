﻿namespace SteamNative
{
  internal enum DenyReason
  {
    Invalid,
    InvalidVersion,
    Generic,
    NotLoggedOn,
    NoLicense,
    Cheater,
    LoggedInElseWhere,
    UnknownText,
    IncompatibleAnticheat,
    MemoryCorruption,
    IncompatibleSoftware,
    SteamConnectionLost,
    SteamConnectionError,
    SteamResponseTimedOut,
    SteamValidationStalled,
    SteamOwnerLeftGuestUser,
  }
}
