using System;
using System.Runtime.InteropServices;
using System.Text;
using Facepunch.Steamworks;

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

    public bool BGetDLCDataByIndex(
      int iDLC,
      ref AppId_t pAppID,
      ref bool pbAvailable,
      out string pchName)
    {
      pchName = string.Empty;
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int cchNameBufferSize = 4096;
      bool flag = platform.ISteamApps_BGetDLCDataByIndex(iDLC, ref pAppID.Value, ref pbAvailable, stringBuilder, cchNameBufferSize);
      if (!flag)
        return flag;
      pchName = stringBuilder.ToString();
      return flag;
    }

    public bool BIsAppInstalled(AppId_t appID)
    {
      return platform.ISteamApps_BIsAppInstalled(appID.Value);
    }

    public bool BIsCybercafe() => platform.ISteamApps_BIsCybercafe();

    public bool BIsDlcInstalled(AppId_t appID)
    {
      return platform.ISteamApps_BIsDlcInstalled(appID.Value);
    }

    public bool BIsLowViolence() => platform.ISteamApps_BIsLowViolence();

    public bool BIsSubscribed() => platform.ISteamApps_BIsSubscribed();

    public bool BIsSubscribedApp(AppId_t appID)
    {
      return platform.ISteamApps_BIsSubscribedApp(appID.Value);
    }

    public bool BIsSubscribedFromFreeWeekend()
    {
      return platform.ISteamApps_BIsSubscribedFromFreeWeekend();
    }

    public bool BIsVACBanned() => platform.ISteamApps_BIsVACBanned();

    public int GetAppBuildId() => platform.ISteamApps_GetAppBuildId();

    public string GetAppInstallDir(AppId_t appID)
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      uint cchFolderBufferSize = 4096;
      return platform.ISteamApps_GetAppInstallDir(appID.Value, stringBuilder, cchFolderBufferSize) <= 0U ? null : stringBuilder.ToString();
    }

    public ulong GetAppOwner() => (ulong) platform.ISteamApps_GetAppOwner();

    public string GetAvailableGameLanguages()
    {
      return Marshal.PtrToStringAnsi(platform.ISteamApps_GetAvailableGameLanguages());
    }

    public string GetCurrentBetaName()
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int cchNameBufferSize = 4096;
      return !platform.ISteamApps_GetCurrentBetaName(stringBuilder, cchNameBufferSize) ? null : stringBuilder.ToString();
    }

    public string GetCurrentGameLanguage()
    {
      return Marshal.PtrToStringAnsi(platform.ISteamApps_GetCurrentGameLanguage());
    }

    public int GetDLCCount() => platform.ISteamApps_GetDLCCount();

    public bool GetDlcDownloadProgress(
      AppId_t nAppID,
      out ulong punBytesDownloaded,
      out ulong punBytesTotal)
    {
      return platform.ISteamApps_GetDlcDownloadProgress(nAppID.Value, out punBytesDownloaded, out punBytesTotal);
    }

    public uint GetEarliestPurchaseUnixTime(AppId_t nAppID)
    {
      return platform.ISteamApps_GetEarliestPurchaseUnixTime(nAppID.Value);
    }

    public CallbackHandle GetFileDetails(
      string pszFileName,
      Action<FileDetailsResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t fileDetails = platform.ISteamApps_GetFileDetails(pszFileName);
      return CallbackFunction == null ? null : FileDetailsResult_t.CallResult(steamworks, fileDetails, CallbackFunction);
    }

    public uint GetInstalledDepots(AppId_t appID, IntPtr pvecDepots, uint cMaxDepots)
    {
      return platform.ISteamApps_GetInstalledDepots(appID.Value, pvecDepots, cMaxDepots);
    }

    public string GetLaunchQueryParam(string pchKey)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamApps_GetLaunchQueryParam(pchKey));
    }

    public void InstallDLC(AppId_t nAppID) => platform.ISteamApps_InstallDLC(nAppID.Value);

    public bool MarkContentCorrupt(bool bMissingFilesOnly)
    {
      return platform.ISteamApps_MarkContentCorrupt(bMissingFilesOnly);
    }

    public void RequestAllProofOfPurchaseKeys()
    {
      platform.ISteamApps_RequestAllProofOfPurchaseKeys();
    }

    public void RequestAppProofOfPurchaseKey(AppId_t nAppID)
    {
      platform.ISteamApps_RequestAppProofOfPurchaseKey(nAppID.Value);
    }

    public void UninstallDLC(AppId_t nAppID) => platform.ISteamApps_UninstallDLC(nAppID.Value);
  }
}
