﻿namespace SteamNative
{
  internal enum BroadcastUploadResult
  {
    None,
    OK,
    InitFailed,
    FrameFailed,
    Timeout,
    BandwidthExceeded,
    LowFPS,
    MissingKeyFrames,
    NoConnection,
    RelayFailed,
    SettingsChanged,
    MissingAudio,
    TooFarBehind,
    TranscodeBehind,
  }
}
