using System;
using Facepunch.Steamworks;

namespace SteamNative
{
  internal class SteamGameServer : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamGameServer(BaseSteamworks steamworks, IntPtr pointer)
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

    public CallbackHandle AssociateWithClan(
      CSteamID steamIDClan,
      Action<AssociateWithClanResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamGameServer_AssociateWithClan(steamIDClan.Value);
      return CallbackFunction == null ? null : AssociateWithClanResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public BeginAuthSessionResult BeginAuthSession(
      IntPtr pAuthTicket,
      int cbAuthTicket,
      CSteamID steamID)
    {
      return platform.ISteamGameServer_BeginAuthSession(pAuthTicket, cbAuthTicket, steamID.Value);
    }

    public bool BLoggedOn() => platform.ISteamGameServer_BLoggedOn();

    public bool BSecure() => platform.ISteamGameServer_BSecure();

    public bool BUpdateUserData(CSteamID steamIDUser, string pchPlayerName, uint uScore)
    {
      return platform.ISteamGameServer_BUpdateUserData(steamIDUser.Value, pchPlayerName, uScore);
    }

    public void CancelAuthTicket(HAuthTicket hAuthTicket)
    {
      platform.ISteamGameServer_CancelAuthTicket(hAuthTicket.Value);
    }

    public void ClearAllKeyValues() => platform.ISteamGameServer_ClearAllKeyValues();

    public CallbackHandle ComputeNewPlayerCompatibility(
      CSteamID steamIDNewPlayer,
      Action<ComputeNewPlayerCompatibilityResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t playerCompatibility = platform.ISteamGameServer_ComputeNewPlayerCompatibility(steamIDNewPlayer.Value);
      return CallbackFunction == null ? null : ComputeNewPlayerCompatibilityResult_t.CallResult(steamworks, playerCompatibility, CallbackFunction);
    }

    public ulong CreateUnauthenticatedUserConnection()
    {
      return (ulong) platform.ISteamGameServer_CreateUnauthenticatedUserConnection();
    }

    public void EnableHeartbeats(bool bActive)
    {
      platform.ISteamGameServer_EnableHeartbeats(bActive);
    }

    public void EndAuthSession(CSteamID steamID)
    {
      platform.ISteamGameServer_EndAuthSession(steamID.Value);
    }

    public void ForceHeartbeat() => platform.ISteamGameServer_ForceHeartbeat();

    public HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, out uint pcbTicket)
    {
      return platform.ISteamGameServer_GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket);
    }

    public void GetGameplayStats() => platform.ISteamGameServer_GetGameplayStats();

    public int GetNextOutgoingPacket(
      IntPtr pOut,
      int cbMaxOut,
      out uint pNetAdr,
      out ushort pPort)
    {
      return platform.ISteamGameServer_GetNextOutgoingPacket(pOut, cbMaxOut, out pNetAdr, out pPort);
    }

    public uint GetPublicIP() => platform.ISteamGameServer_GetPublicIP();

    public CallbackHandle GetServerReputation(Action<GSReputation_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t serverReputation = platform.ISteamGameServer_GetServerReputation();
      return CallbackFunction == null ? null : GSReputation_t.CallResult(steamworks, serverReputation, CallbackFunction);
    }

    public ulong GetSteamID() => (ulong) platform.ISteamGameServer_GetSteamID();

    public bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, ushort srcPort)
    {
      return platform.ISteamGameServer_HandleIncomingPacket(pData, cbData, srcIP, srcPort);
    }

    public bool InitGameServer(
      uint unIP,
      ushort usGamePort,
      ushort usQueryPort,
      uint unFlags,
      AppId_t nGameAppId,
      string pchVersionString)
    {
      return platform.ISteamGameServer_InitGameServer(unIP, usGamePort, usQueryPort, unFlags, nGameAppId.Value, pchVersionString);
    }

    public void LogOff() => platform.ISteamGameServer_LogOff();

    public void LogOn(string pszToken) => platform.ISteamGameServer_LogOn(pszToken);

    public void LogOnAnonymous() => platform.ISteamGameServer_LogOnAnonymous();

    public bool RequestUserGroupStatus(CSteamID steamIDUser, CSteamID steamIDGroup)
    {
      return platform.ISteamGameServer_RequestUserGroupStatus(steamIDUser.Value, steamIDGroup.Value);
    }

    public bool SendUserConnectAndAuthenticate(
      uint unIPClient,
      IntPtr pvAuthBlob,
      uint cubAuthBlobSize,
      out CSteamID pSteamIDUser)
    {
      return platform.ISteamGameServer_SendUserConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, out pSteamIDUser.Value);
    }

    public void SendUserDisconnect(CSteamID steamIDUser)
    {
      platform.ISteamGameServer_SendUserDisconnect(steamIDUser.Value);
    }

    public void SetBotPlayerCount(int cBotplayers)
    {
      platform.ISteamGameServer_SetBotPlayerCount(cBotplayers);
    }

    public void SetDedicatedServer(bool bDedicated)
    {
      platform.ISteamGameServer_SetDedicatedServer(bDedicated);
    }

    public void SetGameData(string pchGameData)
    {
      platform.ISteamGameServer_SetGameData(pchGameData);
    }

    public void SetGameDescription(string pszGameDescription)
    {
      platform.ISteamGameServer_SetGameDescription(pszGameDescription);
    }

    public void SetGameTags(string pchGameTags)
    {
      platform.ISteamGameServer_SetGameTags(pchGameTags);
    }

    public void SetHeartbeatInterval(int iHeartbeatInterval)
    {
      platform.ISteamGameServer_SetHeartbeatInterval(iHeartbeatInterval);
    }

    public void SetKeyValue(string pKey, string pValue)
    {
      platform.ISteamGameServer_SetKeyValue(pKey, pValue);
    }

    public void SetMapName(string pszMapName)
    {
      platform.ISteamGameServer_SetMapName(pszMapName);
    }

    public void SetMaxPlayerCount(int cPlayersMax)
    {
      platform.ISteamGameServer_SetMaxPlayerCount(cPlayersMax);
    }

    public void SetModDir(string pszModDir) => platform.ISteamGameServer_SetModDir(pszModDir);

    public void SetPasswordProtected(bool bPasswordProtected)
    {
      platform.ISteamGameServer_SetPasswordProtected(bPasswordProtected);
    }

    public void SetProduct(string pszProduct)
    {
      platform.ISteamGameServer_SetProduct(pszProduct);
    }

    public void SetRegion(string pszRegion) => platform.ISteamGameServer_SetRegion(pszRegion);

    public void SetServerName(string pszServerName)
    {
      platform.ISteamGameServer_SetServerName(pszServerName);
    }

    public void SetSpectatorPort(ushort unSpectatorPort)
    {
      platform.ISteamGameServer_SetSpectatorPort(unSpectatorPort);
    }

    public void SetSpectatorServerName(string pszSpectatorServerName)
    {
      platform.ISteamGameServer_SetSpectatorServerName(pszSpectatorServerName);
    }

    public UserHasLicenseForAppResult UserHasLicenseForApp(CSteamID steamID, AppId_t appID)
    {
      return platform.ISteamGameServer_UserHasLicenseForApp(steamID.Value, appID.Value);
    }

    public bool WasRestartRequested() => platform.ISteamGameServer_WasRestartRequested();
  }
}
