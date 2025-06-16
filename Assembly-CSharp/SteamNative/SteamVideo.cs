using Facepunch.Steamworks;
using System;
using System.Text;

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

    public void GetOPFSettings(AppId_t unVideoAppID)
    {
      this.platform.ISteamVideo_GetOPFSettings(unVideoAppID.Value);
    }

    public string GetOPFStringForApp(AppId_t unVideoAppID)
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int pnBufferSize = 4096;
      return !this.platform.ISteamVideo_GetOPFStringForApp(unVideoAppID.Value, stringBuilder, out pnBufferSize) ? (string) null : stringBuilder.ToString();
    }

    public void GetVideoURL(AppId_t unVideoAppID)
    {
      this.platform.ISteamVideo_GetVideoURL(unVideoAppID.Value);
    }

    public bool IsBroadcasting(IntPtr pnNumViewers)
    {
      return this.platform.ISteamVideo_IsBroadcasting(pnNumViewers);
    }
  }
}
