using System;
using Facepunch.Steamworks;

namespace SteamNative
{
  internal class SteamGameServerStats : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamGameServerStats(BaseSteamworks steamworks, IntPtr pointer)
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

    public bool ClearUserAchievement(CSteamID steamIDUser, string pchName)
    {
      return platform.ISteamGameServerStats_ClearUserAchievement(steamIDUser.Value, pchName);
    }

    public bool GetUserAchievement(CSteamID steamIDUser, string pchName, ref bool pbAchieved)
    {
      return platform.ISteamGameServerStats_GetUserAchievement(steamIDUser.Value, pchName, ref pbAchieved);
    }

    public bool GetUserStat(CSteamID steamIDUser, string pchName, out int pData)
    {
      return platform.ISteamGameServerStats_GetUserStat(steamIDUser.Value, pchName, out pData);
    }

    public bool GetUserStat0(CSteamID steamIDUser, string pchName, out float pData)
    {
      return platform.ISteamGameServerStats_GetUserStat0(steamIDUser.Value, pchName, out pData);
    }

    public CallbackHandle RequestUserStats(
      CSteamID steamIDUser,
      Action<GSStatsReceived_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamGameServerStats_RequestUserStats(steamIDUser.Value);
      return CallbackFunction == null ? null : GSStatsReceived_t.CallResult(steamworks, call, CallbackFunction);
    }

    public bool SetUserAchievement(CSteamID steamIDUser, string pchName)
    {
      return platform.ISteamGameServerStats_SetUserAchievement(steamIDUser.Value, pchName);
    }

    public bool SetUserStat(CSteamID steamIDUser, string pchName, int nData)
    {
      return platform.ISteamGameServerStats_SetUserStat(steamIDUser.Value, pchName, nData);
    }

    public bool SetUserStat0(CSteamID steamIDUser, string pchName, float fData)
    {
      return platform.ISteamGameServerStats_SetUserStat0(steamIDUser.Value, pchName, fData);
    }

    public CallbackHandle StoreUserStats(
      CSteamID steamIDUser,
      Action<GSStatsStored_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamGameServerStats_StoreUserStats(steamIDUser.Value);
      return CallbackFunction == null ? null : GSStatsStored_t.CallResult(steamworks, call, CallbackFunction);
    }

    public bool UpdateUserAvgRateStat(
      CSteamID steamIDUser,
      string pchName,
      float flCountThisSession,
      double dSessionLength)
    {
      return platform.ISteamGameServerStats_UpdateUserAvgRateStat(steamIDUser.Value, pchName, flCountThisSession, dSessionLength);
    }
  }
}
