// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamAppList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;
using System.Text;

#nullable disable
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

    public int GetAppBuildId(AppId_t nAppID)
    {
      return this.platform.ISteamAppList_GetAppBuildId(nAppID.Value);
    }

    public string GetAppInstallDir(AppId_t nAppID)
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int cchNameMax = 4096;
      return this.platform.ISteamAppList_GetAppInstallDir(nAppID.Value, stringBuilder, cchNameMax) <= 0 ? (string) null : stringBuilder.ToString();
    }

    public string GetAppName(AppId_t nAppID)
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int cchNameMax = 4096;
      return this.platform.ISteamAppList_GetAppName(nAppID.Value, stringBuilder, cchNameMax) <= 0 ? (string) null : stringBuilder.ToString();
    }

    public unsafe uint GetInstalledApps(AppId_t[] pvecAppID)
    {
      uint length = (uint) pvecAppID.Length;
      fixed (AppId_t* pvecAppID1 = pvecAppID)
        return this.platform.ISteamAppList_GetInstalledApps((IntPtr) (void*) pvecAppID1, length);
    }

    public uint GetNumInstalledApps() => this.platform.ISteamAppList_GetNumInstalledApps();
  }
}
