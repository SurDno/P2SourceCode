// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamGameServerStats
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;

#nullable disable
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

    public bool ClearUserAchievement(CSteamID steamIDUser, string pchName)
    {
      return this.platform.ISteamGameServerStats_ClearUserAchievement(steamIDUser.Value, pchName);
    }

    public bool GetUserAchievement(CSteamID steamIDUser, string pchName, ref bool pbAchieved)
    {
      return this.platform.ISteamGameServerStats_GetUserAchievement(steamIDUser.Value, pchName, ref pbAchieved);
    }

    public bool GetUserStat(CSteamID steamIDUser, string pchName, out int pData)
    {
      return this.platform.ISteamGameServerStats_GetUserStat(steamIDUser.Value, pchName, out pData);
    }

    public bool GetUserStat0(CSteamID steamIDUser, string pchName, out float pData)
    {
      return this.platform.ISteamGameServerStats_GetUserStat0(steamIDUser.Value, pchName, out pData);
    }

    public CallbackHandle RequestUserStats(
      CSteamID steamIDUser,
      Action<GSStatsReceived_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamGameServerStats_RequestUserStats(steamIDUser.Value);
      return CallbackFunction == null ? (CallbackHandle) null : GSStatsReceived_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool SetUserAchievement(CSteamID steamIDUser, string pchName)
    {
      return this.platform.ISteamGameServerStats_SetUserAchievement(steamIDUser.Value, pchName);
    }

    public bool SetUserStat(CSteamID steamIDUser, string pchName, int nData)
    {
      return this.platform.ISteamGameServerStats_SetUserStat(steamIDUser.Value, pchName, nData);
    }

    public bool SetUserStat0(CSteamID steamIDUser, string pchName, float fData)
    {
      return this.platform.ISteamGameServerStats_SetUserStat0(steamIDUser.Value, pchName, fData);
    }

    public CallbackHandle StoreUserStats(
      CSteamID steamIDUser,
      Action<GSStatsStored_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamGameServerStats_StoreUserStats(steamIDUser.Value);
      return CallbackFunction == null ? (CallbackHandle) null : GSStatsStored_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool UpdateUserAvgRateStat(
      CSteamID steamIDUser,
      string pchName,
      float flCountThisSession,
      double dSessionLength)
    {
      return this.platform.ISteamGameServerStats_UpdateUserAvgRateStat(steamIDUser.Value, pchName, flCountThisSession, dSessionLength);
    }
  }
}
