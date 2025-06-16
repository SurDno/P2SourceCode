using System;
using System.Runtime.InteropServices;
using System.Text;
using Facepunch.Steamworks;

namespace SteamNative
{
  internal class SteamUserStats : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamUserStats(BaseSteamworks steamworks, IntPtr pointer)
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

    public CallbackHandle AttachLeaderboardUGC(
      SteamLeaderboard_t hSteamLeaderboard,
      UGCHandle_t hUGC,
      Action<LeaderboardUGCSet_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamUserStats_AttachLeaderboardUGC(hSteamLeaderboard.Value, hUGC.Value);
      return CallbackFunction == null ? null : LeaderboardUGCSet_t.CallResult(steamworks, call, CallbackFunction);
    }

    public bool ClearAchievement(string pchName)
    {
      return platform.ISteamUserStats_ClearAchievement(pchName);
    }

    public CallbackHandle DownloadLeaderboardEntries(
      SteamLeaderboard_t hSteamLeaderboard,
      LeaderboardDataRequest eLeaderboardDataRequest,
      int nRangeStart,
      int nRangeEnd,
      Action<LeaderboardScoresDownloaded_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamUserStats_DownloadLeaderboardEntries(hSteamLeaderboard.Value, eLeaderboardDataRequest, nRangeStart, nRangeEnd);
      return CallbackFunction == null ? null : LeaderboardScoresDownloaded_t.CallResult(steamworks, call, CallbackFunction);
    }

    public CallbackHandle DownloadLeaderboardEntriesForUsers(
      SteamLeaderboard_t hSteamLeaderboard,
      IntPtr prgUsers,
      int cUsers,
      Action<LeaderboardScoresDownloaded_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamUserStats_DownloadLeaderboardEntriesForUsers(hSteamLeaderboard.Value, prgUsers, cUsers);
      return CallbackFunction == null ? null : LeaderboardScoresDownloaded_t.CallResult(steamworks, call, CallbackFunction);
    }

    public CallbackHandle FindLeaderboard(
      string pchLeaderboardName,
      Action<LeaderboardFindResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t leaderboard = platform.ISteamUserStats_FindLeaderboard(pchLeaderboardName);
      return CallbackFunction == null ? null : LeaderboardFindResult_t.CallResult(steamworks, leaderboard, CallbackFunction);
    }

    public CallbackHandle FindOrCreateLeaderboard(
      string pchLeaderboardName,
      LeaderboardSortMethod eLeaderboardSortMethod,
      LeaderboardDisplayType eLeaderboardDisplayType,
      Action<LeaderboardFindResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t createLeaderboard = platform.ISteamUserStats_FindOrCreateLeaderboard(pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType);
      return CallbackFunction == null ? null : LeaderboardFindResult_t.CallResult(steamworks, createLeaderboard, CallbackFunction);
    }

    public bool GetAchievement(string pchName, ref bool pbAchieved)
    {
      return platform.ISteamUserStats_GetAchievement(pchName, ref pbAchieved);
    }

    public bool GetAchievementAchievedPercent(string pchName, out float pflPercent)
    {
      return platform.ISteamUserStats_GetAchievementAchievedPercent(pchName, out pflPercent);
    }

    public bool GetAchievementAndUnlockTime(
      string pchName,
      ref bool pbAchieved,
      out uint punUnlockTime)
    {
      return platform.ISteamUserStats_GetAchievementAndUnlockTime(pchName, ref pbAchieved, out punUnlockTime);
    }

    public string GetAchievementDisplayAttribute(string pchName, string pchKey)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamUserStats_GetAchievementDisplayAttribute(pchName, pchKey));
    }

    public int GetAchievementIcon(string pchName)
    {
      return platform.ISteamUserStats_GetAchievementIcon(pchName);
    }

    public string GetAchievementName(uint iAchievement)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamUserStats_GetAchievementName(iAchievement));
    }

    public bool GetDownloadedLeaderboardEntry(
      SteamLeaderboardEntries_t hSteamLeaderboardEntries,
      int index,
      ref LeaderboardEntry_t pLeaderboardEntry,
      IntPtr pDetails,
      int cDetailsMax)
    {
      return platform.ISteamUserStats_GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries.Value, index, ref pLeaderboardEntry, pDetails, cDetailsMax);
    }

    public bool GetGlobalStat(string pchStatName, out long pData)
    {
      return platform.ISteamUserStats_GetGlobalStat(pchStatName, out pData);
    }

    public bool GetGlobalStat0(string pchStatName, out double pData)
    {
      return platform.ISteamUserStats_GetGlobalStat0(pchStatName, out pData);
    }

    public int GetGlobalStatHistory(string pchStatName, out long pData, uint cubData)
    {
      return platform.ISteamUserStats_GetGlobalStatHistory(pchStatName, out pData, cubData);
    }

    public int GetGlobalStatHistory0(string pchStatName, out double pData, uint cubData)
    {
      return platform.ISteamUserStats_GetGlobalStatHistory0(pchStatName, out pData, cubData);
    }

    public LeaderboardDisplayType GetLeaderboardDisplayType(SteamLeaderboard_t hSteamLeaderboard)
    {
      return platform.ISteamUserStats_GetLeaderboardDisplayType(hSteamLeaderboard.Value);
    }

    public int GetLeaderboardEntryCount(SteamLeaderboard_t hSteamLeaderboard)
    {
      return platform.ISteamUserStats_GetLeaderboardEntryCount(hSteamLeaderboard.Value);
    }

    public string GetLeaderboardName(SteamLeaderboard_t hSteamLeaderboard)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamUserStats_GetLeaderboardName(hSteamLeaderboard.Value));
    }

    public LeaderboardSortMethod GetLeaderboardSortMethod(SteamLeaderboard_t hSteamLeaderboard)
    {
      return platform.ISteamUserStats_GetLeaderboardSortMethod(hSteamLeaderboard.Value);
    }

    public int GetMostAchievedAchievementInfo(
      out string pchName,
      out float pflPercent,
      ref bool pbAchieved)
    {
      pchName = string.Empty;
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      uint unNameBufLen = 4096;
      int achievedAchievementInfo = platform.ISteamUserStats_GetMostAchievedAchievementInfo(stringBuilder, unNameBufLen, out pflPercent, ref pbAchieved);
      if (achievedAchievementInfo <= 0)
        return achievedAchievementInfo;
      pchName = stringBuilder.ToString();
      return achievedAchievementInfo;
    }

    public int GetNextMostAchievedAchievementInfo(
      int iIteratorPrevious,
      out string pchName,
      out float pflPercent,
      ref bool pbAchieved)
    {
      pchName = string.Empty;
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      uint unNameBufLen = 4096;
      int achievedAchievementInfo = platform.ISteamUserStats_GetNextMostAchievedAchievementInfo(iIteratorPrevious, stringBuilder, unNameBufLen, out pflPercent, ref pbAchieved);
      if (achievedAchievementInfo <= 0)
        return achievedAchievementInfo;
      pchName = stringBuilder.ToString();
      return achievedAchievementInfo;
    }

    public uint GetNumAchievements() => platform.ISteamUserStats_GetNumAchievements();

    public CallbackHandle GetNumberOfCurrentPlayers(
      Action<NumberOfCurrentPlayers_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t ofCurrentPlayers = platform.ISteamUserStats_GetNumberOfCurrentPlayers();
      return CallbackFunction == null ? null : NumberOfCurrentPlayers_t.CallResult(steamworks, ofCurrentPlayers, CallbackFunction);
    }

    public bool GetStat(string pchName, out int pData)
    {
      return platform.ISteamUserStats_GetStat(pchName, out pData);
    }

    public bool GetStat0(string pchName, out float pData)
    {
      return platform.ISteamUserStats_GetStat0(pchName, out pData);
    }

    public bool GetUserAchievement(CSteamID steamIDUser, string pchName, ref bool pbAchieved)
    {
      return platform.ISteamUserStats_GetUserAchievement(steamIDUser.Value, pchName, ref pbAchieved);
    }

    public bool GetUserAchievementAndUnlockTime(
      CSteamID steamIDUser,
      string pchName,
      ref bool pbAchieved,
      out uint punUnlockTime)
    {
      return platform.ISteamUserStats_GetUserAchievementAndUnlockTime(steamIDUser.Value, pchName, ref pbAchieved, out punUnlockTime);
    }

    public bool GetUserStat(CSteamID steamIDUser, string pchName, out int pData)
    {
      return platform.ISteamUserStats_GetUserStat(steamIDUser.Value, pchName, out pData);
    }

    public bool GetUserStat0(CSteamID steamIDUser, string pchName, out float pData)
    {
      return platform.ISteamUserStats_GetUserStat0(steamIDUser.Value, pchName, out pData);
    }

    public bool IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress)
    {
      return platform.ISteamUserStats_IndicateAchievementProgress(pchName, nCurProgress, nMaxProgress);
    }

    public bool RequestCurrentStats() => platform.ISteamUserStats_RequestCurrentStats();

    public CallbackHandle RequestGlobalAchievementPercentages(
      Action<GlobalAchievementPercentagesReady_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamUserStats_RequestGlobalAchievementPercentages();
      return CallbackFunction == null ? null : GlobalAchievementPercentagesReady_t.CallResult(steamworks, call, CallbackFunction);
    }

    public CallbackHandle RequestGlobalStats(
      int nHistoryDays,
      Action<GlobalStatsReceived_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamUserStats_RequestGlobalStats(nHistoryDays);
      return CallbackFunction == null ? null : GlobalStatsReceived_t.CallResult(steamworks, call, CallbackFunction);
    }

    public CallbackHandle RequestUserStats(
      CSteamID steamIDUser,
      Action<UserStatsReceived_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamUserStats_RequestUserStats(steamIDUser.Value);
      return CallbackFunction == null ? null : UserStatsReceived_t.CallResult(steamworks, call, CallbackFunction);
    }

    public bool ResetAllStats(bool bAchievementsToo)
    {
      return platform.ISteamUserStats_ResetAllStats(bAchievementsToo);
    }

    public bool SetAchievement(string pchName)
    {
      return platform.ISteamUserStats_SetAchievement(pchName);
    }

    public bool SetStat(string pchName, int nData)
    {
      return platform.ISteamUserStats_SetStat(pchName, nData);
    }

    public bool SetStat0(string pchName, float fData)
    {
      return platform.ISteamUserStats_SetStat0(pchName, fData);
    }

    public bool StoreStats() => platform.ISteamUserStats_StoreStats();

    public bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength)
    {
      return platform.ISteamUserStats_UpdateAvgRateStat(pchName, flCountThisSession, dSessionLength);
    }

    public CallbackHandle UploadLeaderboardScore(
      SteamLeaderboard_t hSteamLeaderboard,
      LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod,
      int nScore,
      int[] pScoreDetails,
      int cScoreDetailsCount,
      Action<LeaderboardScoreUploaded_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamUserStats_UploadLeaderboardScore(hSteamLeaderboard.Value, eLeaderboardUploadScoreMethod, nScore, pScoreDetails, cScoreDetailsCount);
      return CallbackFunction == null ? null : LeaderboardScoreUploaded_t.CallResult(steamworks, call, CallbackFunction);
    }
  }
}
