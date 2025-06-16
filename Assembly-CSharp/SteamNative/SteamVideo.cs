using System;
using System.Text;
using Facepunch.Steamworks;

namespace SteamNative
{
  internal class SteamVideo : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamVideo(BaseSteamworks steamworks, IntPtr pointer)
    {
      this.steamworks = steamworks;
      if (Platform.IsWindows64)
        platform = (Platform.Interface) new Platform.Win64(pointer);
      else if (Platform.IsWindows32)
        platform = (Platform.Interface) new Platform.Win32(pointer);
      else if (Platform.IsLinux32)
        platform = (Platform.Interface) new Platform.Linux32(pointer);
      else if (Platform.IsLinux64)
      {
        platform = (Platform.Interface) new Platform.Linux64(pointer);
      }
      else
      {
        if (!Platform.IsOsx)
          return;
        platform = (Platform.Interface) new Platform.Mac(pointer);
      }
    }

    public bool IsValid => platform != null && platform.IsValid;

    public virtual void Dispose()
    {
      if (platform == null)
        return;
      platform.Dispose();
      platform = (Platform.Interface) null;
    }

    public void GetOPFSettings(AppId_t unVideoAppID)
    {
      platform.ISteamVideo_GetOPFSettings(unVideoAppID.Value);
    }

    public string GetOPFStringForApp(AppId_t unVideoAppID)
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int pnBufferSize = 4096;
      return !platform.ISteamVideo_GetOPFStringForApp(unVideoAppID.Value, stringBuilder, out pnBufferSize) ? null : stringBuilder.ToString();
    }

    public void GetVideoURL(AppId_t unVideoAppID)
    {
      platform.ISteamVideo_GetVideoURL(unVideoAppID.Value);
    }

    public bool IsBroadcasting(IntPtr pnNumViewers)
    {
      return platform.ISteamVideo_IsBroadcasting(pnNumViewers);
    }
  }
}
