﻿namespace SteamNative
{
  internal enum VoiceResult
  {
    OK,
    NotInitialized,
    NotRecording,
    NoData,
    BufferTooSmall,
    DataCorrupted,
    Restricted,
    UnsupportedCodec,
    ReceiverOutOfDate,
    ReceiverDidNotAnswer,
  }
}
