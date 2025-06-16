using Facepunch.Steamworks;
using System;

namespace SteamNative
{
  internal class SteamMusic : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamMusic(BaseSteamworks steamworks, IntPtr pointer)
    {
      this.steamworks = steamworks;
      if (Platform.IsWindows64)
        this.platform = (Platform.Interface) new Platform.Win64(pointer);
      else if (Platform.IsWindows32)
        this.platform = (Platform.Interface) new Platform.Win32(pointer);
      else if (Platform.IsLinux32)
        this.platform = (Platform.Interface) new Platform.Linux32(pointer);
      else if (Platform.IsLinux64)
      {
        this.platform = (Platform.Interface) new Platform.Linux64(pointer);
      }
      else
      {
        if (!Platform.IsOsx)
          return;
        this.platform = (Platform.Interface) new Platform.Mac(pointer);
      }
    }

    public bool IsValid => this.platform != null && this.platform.IsValid;

    public virtual void Dispose()
    {
      if (this.platform == null)
        return;
      this.platform.Dispose();
      this.platform = (Platform.Interface) null;
    }

    public bool BIsEnabled() => this.platform.ISteamMusic_BIsEnabled();

    public bool BIsPlaying() => this.platform.ISteamMusic_BIsPlaying();

    public AudioPlayback_Status GetPlaybackStatus()
    {
      return this.platform.ISteamMusic_GetPlaybackStatus();
    }

    public float GetVolume() => this.platform.ISteamMusic_GetVolume();

    public void Pause() => this.platform.ISteamMusic_Pause();

    public void Play() => this.platform.ISteamMusic_Play();

    public void PlayNext() => this.platform.ISteamMusic_PlayNext();

    public void PlayPrevious() => this.platform.ISteamMusic_PlayPrevious();

    public void SetVolume(float flVolume) => this.platform.ISteamMusic_SetVolume(flVolume);
  }
}
