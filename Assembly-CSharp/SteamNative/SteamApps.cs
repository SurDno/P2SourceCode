// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamApps
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace SteamNative
{
  internal class SteamApps : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamApps(BaseSteamworks steamworks, IntPtr pointer)
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

    public bool BGetDLCDataByIndex(
      int iDLC,
      ref AppId_t pAppID,
      ref bool pbAvailable,
      out string pchName)
    {
      pchName = string.Empty;
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int cchNameBufferSize = 4096;
      bool flag = this.platform.ISteamApps_BGetDLCDataByIndex(iDLC, ref pAppID.Value, ref pbAvailable, stringBuilder, cchNameBufferSize);
      if (!flag)
        return flag;
      pchName = stringBuilder.ToString();
      return flag;
    }

    public bool BIsAppInstalled(AppId_t appID)
    {
      return this.platform.ISteamApps_BIsAppInstalled(appID.Value);
    }

    public bool BIsCybercafe() => this.platform.ISteamApps_BIsCybercafe();

    public bool BIsDlcInstalled(AppId_t appID)
    {
      return this.platform.ISteamApps_BIsDlcInstalled(appID.Value);
    }

    public bool BIsLowViolence() => this.platform.ISteamApps_BIsLowViolence();

    public bool BIsSubscribed() => this.platform.ISteamApps_BIsSubscribed();

    public bool BIsSubscribedApp(AppId_t appID)
    {
      return this.platform.ISteamApps_BIsSubscribedApp(appID.Value);
    }

    public bool BIsSubscribedFromFreeWeekend()
    {
      return this.platform.ISteamApps_BIsSubscribedFromFreeWeekend();
    }

    public bool BIsVACBanned() => this.platform.ISteamApps_BIsVACBanned();

    public int GetAppBuildId() => this.platform.ISteamApps_GetAppBuildId();

    public string GetAppInstallDir(AppId_t appID)
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      uint cchFolderBufferSize = 4096;
      return this.platform.ISteamApps_GetAppInstallDir(appID.Value, stringBuilder, cchFolderBufferSize) <= 0U ? (string) null : stringBuilder.ToString();
    }

    public ulong GetAppOwner() => (ulong) this.platform.ISteamApps_GetAppOwner();

    public string GetAvailableGameLanguages()
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamApps_GetAvailableGameLanguages());
    }

    public string GetCurrentBetaName()
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int cchNameBufferSize = 4096;
      return !this.platform.ISteamApps_GetCurrentBetaName(stringBuilder, cchNameBufferSize) ? (string) null : stringBuilder.ToString();
    }

    public string GetCurrentGameLanguage()
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamApps_GetCurrentGameLanguage());
    }

    public int GetDLCCount() => this.platform.ISteamApps_GetDLCCount();

    public bool GetDlcDownloadProgress(
      AppId_t nAppID,
      out ulong punBytesDownloaded,
      out ulong punBytesTotal)
    {
      return this.platform.ISteamApps_GetDlcDownloadProgress(nAppID.Value, out punBytesDownloaded, out punBytesTotal);
    }

    public uint GetEarliestPurchaseUnixTime(AppId_t nAppID)
    {
      return this.platform.ISteamApps_GetEarliestPurchaseUnixTime(nAppID.Value);
    }

    public CallbackHandle GetFileDetails(
      string pszFileName,
      Action<FileDetailsResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t fileDetails = this.platform.ISteamApps_GetFileDetails(pszFileName);
      return CallbackFunction == null ? (CallbackHandle) null : FileDetailsResult_t.CallResult(this.steamworks, fileDetails, CallbackFunction);
    }

    public uint GetInstalledDepots(AppId_t appID, IntPtr pvecDepots, uint cMaxDepots)
    {
      return this.platform.ISteamApps_GetInstalledDepots(appID.Value, pvecDepots, cMaxDepots);
    }

    public string GetLaunchQueryParam(string pchKey)
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamApps_GetLaunchQueryParam(pchKey));
    }

    public void InstallDLC(AppId_t nAppID) => this.platform.ISteamApps_InstallDLC(nAppID.Value);

    public bool MarkContentCorrupt(bool bMissingFilesOnly)
    {
      return this.platform.ISteamApps_MarkContentCorrupt(bMissingFilesOnly);
    }

    public void RequestAllProofOfPurchaseKeys()
    {
      this.platform.ISteamApps_RequestAllProofOfPurchaseKeys();
    }

    public void RequestAppProofOfPurchaseKey(AppId_t nAppID)
    {
      this.platform.ISteamApps_RequestAppProofOfPurchaseKey(nAppID.Value);
    }

    public void UninstallDLC(AppId_t nAppID) => this.platform.ISteamApps_UninstallDLC(nAppID.Value);
  }
}
