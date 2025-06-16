// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamGameServer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;

#nullable disable
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

    public CallbackHandle AssociateWithClan(
      CSteamID steamIDClan,
      Action<AssociateWithClanResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamGameServer_AssociateWithClan(steamIDClan.Value);
      return CallbackFunction == null ? (CallbackHandle) null : AssociateWithClanResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public BeginAuthSessionResult BeginAuthSession(
      IntPtr pAuthTicket,
      int cbAuthTicket,
      CSteamID steamID)
    {
      return this.platform.ISteamGameServer_BeginAuthSession(pAuthTicket, cbAuthTicket, steamID.Value);
    }

    public bool BLoggedOn() => this.platform.ISteamGameServer_BLoggedOn();

    public bool BSecure() => this.platform.ISteamGameServer_BSecure();

    public bool BUpdateUserData(CSteamID steamIDUser, string pchPlayerName, uint uScore)
    {
      return this.platform.ISteamGameServer_BUpdateUserData(steamIDUser.Value, pchPlayerName, uScore);
    }

    public void CancelAuthTicket(HAuthTicket hAuthTicket)
    {
      this.platform.ISteamGameServer_CancelAuthTicket(hAuthTicket.Value);
    }

    public void ClearAllKeyValues() => this.platform.ISteamGameServer_ClearAllKeyValues();

    public CallbackHandle ComputeNewPlayerCompatibility(
      CSteamID steamIDNewPlayer,
      Action<ComputeNewPlayerCompatibilityResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t playerCompatibility = this.platform.ISteamGameServer_ComputeNewPlayerCompatibility(steamIDNewPlayer.Value);
      return CallbackFunction == null ? (CallbackHandle) null : ComputeNewPlayerCompatibilityResult_t.CallResult(this.steamworks, playerCompatibility, CallbackFunction);
    }

    public ulong CreateUnauthenticatedUserConnection()
    {
      return (ulong) this.platform.ISteamGameServer_CreateUnauthenticatedUserConnection();
    }

    public void EnableHeartbeats(bool bActive)
    {
      this.platform.ISteamGameServer_EnableHeartbeats(bActive);
    }

    public void EndAuthSession(CSteamID steamID)
    {
      this.platform.ISteamGameServer_EndAuthSession(steamID.Value);
    }

    public void ForceHeartbeat() => this.platform.ISteamGameServer_ForceHeartbeat();

    public HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, out uint pcbTicket)
    {
      return this.platform.ISteamGameServer_GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket);
    }

    public void GetGameplayStats() => this.platform.ISteamGameServer_GetGameplayStats();

    public int GetNextOutgoingPacket(
      IntPtr pOut,
      int cbMaxOut,
      out uint pNetAdr,
      out ushort pPort)
    {
      return this.platform.ISteamGameServer_GetNextOutgoingPacket(pOut, cbMaxOut, out pNetAdr, out pPort);
    }

    public uint GetPublicIP() => this.platform.ISteamGameServer_GetPublicIP();

    public CallbackHandle GetServerReputation(Action<GSReputation_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t serverReputation = this.platform.ISteamGameServer_GetServerReputation();
      return CallbackFunction == null ? (CallbackHandle) null : GSReputation_t.CallResult(this.steamworks, serverReputation, CallbackFunction);
    }

    public ulong GetSteamID() => (ulong) this.platform.ISteamGameServer_GetSteamID();

    public bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, ushort srcPort)
    {
      return this.platform.ISteamGameServer_HandleIncomingPacket(pData, cbData, srcIP, srcPort);
    }

    public bool InitGameServer(
      uint unIP,
      ushort usGamePort,
      ushort usQueryPort,
      uint unFlags,
      AppId_t nGameAppId,
      string pchVersionString)
    {
      return this.platform.ISteamGameServer_InitGameServer(unIP, usGamePort, usQueryPort, unFlags, nGameAppId.Value, pchVersionString);
    }

    public void LogOff() => this.platform.ISteamGameServer_LogOff();

    public void LogOn(string pszToken) => this.platform.ISteamGameServer_LogOn(pszToken);

    public void LogOnAnonymous() => this.platform.ISteamGameServer_LogOnAnonymous();

    public bool RequestUserGroupStatus(CSteamID steamIDUser, CSteamID steamIDGroup)
    {
      return this.platform.ISteamGameServer_RequestUserGroupStatus(steamIDUser.Value, steamIDGroup.Value);
    }

    public bool SendUserConnectAndAuthenticate(
      uint unIPClient,
      IntPtr pvAuthBlob,
      uint cubAuthBlobSize,
      out CSteamID pSteamIDUser)
    {
      return this.platform.ISteamGameServer_SendUserConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, out pSteamIDUser.Value);
    }

    public void SendUserDisconnect(CSteamID steamIDUser)
    {
      this.platform.ISteamGameServer_SendUserDisconnect(steamIDUser.Value);
    }

    public void SetBotPlayerCount(int cBotplayers)
    {
      this.platform.ISteamGameServer_SetBotPlayerCount(cBotplayers);
    }

    public void SetDedicatedServer(bool bDedicated)
    {
      this.platform.ISteamGameServer_SetDedicatedServer(bDedicated);
    }

    public void SetGameData(string pchGameData)
    {
      this.platform.ISteamGameServer_SetGameData(pchGameData);
    }

    public void SetGameDescription(string pszGameDescription)
    {
      this.platform.ISteamGameServer_SetGameDescription(pszGameDescription);
    }

    public void SetGameTags(string pchGameTags)
    {
      this.platform.ISteamGameServer_SetGameTags(pchGameTags);
    }

    public void SetHeartbeatInterval(int iHeartbeatInterval)
    {
      this.platform.ISteamGameServer_SetHeartbeatInterval(iHeartbeatInterval);
    }

    public void SetKeyValue(string pKey, string pValue)
    {
      this.platform.ISteamGameServer_SetKeyValue(pKey, pValue);
    }

    public void SetMapName(string pszMapName)
    {
      this.platform.ISteamGameServer_SetMapName(pszMapName);
    }

    public void SetMaxPlayerCount(int cPlayersMax)
    {
      this.platform.ISteamGameServer_SetMaxPlayerCount(cPlayersMax);
    }

    public void SetModDir(string pszModDir) => this.platform.ISteamGameServer_SetModDir(pszModDir);

    public void SetPasswordProtected(bool bPasswordProtected)
    {
      this.platform.ISteamGameServer_SetPasswordProtected(bPasswordProtected);
    }

    public void SetProduct(string pszProduct)
    {
      this.platform.ISteamGameServer_SetProduct(pszProduct);
    }

    public void SetRegion(string pszRegion) => this.platform.ISteamGameServer_SetRegion(pszRegion);

    public void SetServerName(string pszServerName)
    {
      this.platform.ISteamGameServer_SetServerName(pszServerName);
    }

    public void SetSpectatorPort(ushort unSpectatorPort)
    {
      this.platform.ISteamGameServer_SetSpectatorPort(unSpectatorPort);
    }

    public void SetSpectatorServerName(string pszSpectatorServerName)
    {
      this.platform.ISteamGameServer_SetSpectatorServerName(pszSpectatorServerName);
    }

    public UserHasLicenseForAppResult UserHasLicenseForApp(CSteamID steamID, AppId_t appID)
    {
      return this.platform.ISteamGameServer_UserHasLicenseForApp(steamID.Value, appID.Value);
    }

    public bool WasRestartRequested() => this.platform.ISteamGameServer_WasRestartRequested();
  }
}
