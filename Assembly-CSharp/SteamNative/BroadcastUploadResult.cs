// Decompiled with JetBrains decompiler
// Type: SteamNative.BroadcastUploadResult
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
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
