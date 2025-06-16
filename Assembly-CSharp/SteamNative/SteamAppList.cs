using System;
using System.Text;
using Facepunch.Steamworks;

namespace SteamNative
{
  internal class SteamAppList : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamAppList(BaseSteamworks steamworks, IntPtr pointer)
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

    public int GetAppBuildId(AppId_t nAppID)
    {
      return platform.ISteamAppList_GetAppBuildId(nAppID.Value);
    }

    public string GetAppInstallDir(AppId_t nAppID)
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int cchNameMax = 4096;
      return platform.ISteamAppList_GetAppInstallDir(nAppID.Value, stringBuilder, cchNameMax) <= 0 ? null : stringBuilder.ToString();
    }

    public string GetAppName(AppId_t nAppID)
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int cchNameMax = 4096;
      return platform.ISteamAppList_GetAppName(nAppID.Value, stringBuilder, cchNameMax) <= 0 ? null : stringBuilder.ToString();
    }

    public unsafe uint GetInstalledApps(AppId_t[] pvecAppID)
    {
      uint length = (uint) pvecAppID.Length;
      fixed (AppId_t* pvecAppID1 = pvecAppID)
        return platform.ISteamAppList_GetInstalledApps((IntPtr) pvecAppID1, length);
    }

    public uint GetNumInstalledApps() => platform.ISteamAppList_GetNumInstalledApps();
  }
}
