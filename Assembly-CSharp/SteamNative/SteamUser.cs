// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamUser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;
using System.Text;

#nullable disable
namespace SteamNative
{
  internal class SteamUser : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamUser(BaseSteamworks steamworks, IntPtr pointer)
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

    public void AdvertiseGame(CSteamID steamIDGameServer, uint unIPServer, ushort usPortServer)
    {
      this.platform.ISteamUser_AdvertiseGame(steamIDGameServer.Value, unIPServer, usPortServer);
    }

    public BeginAuthSessionResult BeginAuthSession(
      IntPtr pAuthTicket,
      int cbAuthTicket,
      CSteamID steamID)
    {
      return this.platform.ISteamUser_BeginAuthSession(pAuthTicket, cbAuthTicket, steamID.Value);
    }

    public bool BIsBehindNAT() => this.platform.ISteamUser_BIsBehindNAT();

    public bool BIsPhoneIdentifying() => this.platform.ISteamUser_BIsPhoneIdentifying();

    public bool BIsPhoneRequiringVerification()
    {
      return this.platform.ISteamUser_BIsPhoneRequiringVerification();
    }

    public bool BIsPhoneVerified() => this.platform.ISteamUser_BIsPhoneVerified();

    public bool BIsTwoFactorEnabled() => this.platform.ISteamUser_BIsTwoFactorEnabled();

    public bool BLoggedOn() => this.platform.ISteamUser_BLoggedOn();

    public void CancelAuthTicket(HAuthTicket hAuthTicket)
    {
      this.platform.ISteamUser_CancelAuthTicket(hAuthTicket.Value);
    }

    public VoiceResult DecompressVoice(
      IntPtr pCompressed,
      uint cbCompressed,
      IntPtr pDestBuffer,
      uint cbDestBufferSize,
      out uint nBytesWritten,
      uint nDesiredSampleRate)
    {
      return this.platform.ISteamUser_DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, out nBytesWritten, nDesiredSampleRate);
    }

    public void EndAuthSession(CSteamID steamID)
    {
      this.platform.ISteamUser_EndAuthSession(steamID.Value);
    }

    public HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, out uint pcbTicket)
    {
      return this.platform.ISteamUser_GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket);
    }

    public VoiceResult GetAvailableVoice(
      out uint pcbCompressed,
      out uint pcbUncompressed_Deprecated,
      uint nUncompressedVoiceDesiredSampleRate_Deprecated)
    {
      return this.platform.ISteamUser_GetAvailableVoice(out pcbCompressed, out pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
    }

    public bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, out uint pcbTicket)
    {
      return this.platform.ISteamUser_GetEncryptedAppTicket(pTicket, cbMaxTicket, out pcbTicket);
    }

    public int GetGameBadgeLevel(int nSeries, bool bFoil)
    {
      return this.platform.ISteamUser_GetGameBadgeLevel(nSeries, bFoil);
    }

    public HSteamUser GetHSteamUser() => this.platform.ISteamUser_GetHSteamUser();

    public int GetPlayerSteamLevel() => this.platform.ISteamUser_GetPlayerSteamLevel();

    public ulong GetSteamID() => (ulong) this.platform.ISteamUser_GetSteamID();

    public string GetUserDataFolder()
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      int cubBuffer = 4096;
      return !this.platform.ISteamUser_GetUserDataFolder(stringBuilder, cubBuffer) ? (string) null : stringBuilder.ToString();
    }

    public VoiceResult GetVoice(
      bool bWantCompressed,
      IntPtr pDestBuffer,
      uint cbDestBufferSize,
      out uint nBytesWritten,
      bool bWantUncompressed_Deprecated,
      IntPtr pUncompressedDestBuffer_Deprecated,
      uint cbUncompressedDestBufferSize_Deprecated,
      out uint nUncompressBytesWritten_Deprecated,
      uint nUncompressedVoiceDesiredSampleRate_Deprecated)
    {
      return this.platform.ISteamUser_GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, out nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, out nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
    }

    public uint GetVoiceOptimalSampleRate() => this.platform.ISteamUser_GetVoiceOptimalSampleRate();

    public int InitiateGameConnection(
      IntPtr pAuthBlob,
      int cbMaxAuthBlob,
      CSteamID steamIDGameServer,
      uint unIPServer,
      ushort usPortServer,
      bool bSecure)
    {
      return this.platform.ISteamUser_InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer.Value, unIPServer, usPortServer, bSecure);
    }

    public CallbackHandle RequestEncryptedAppTicket(
      IntPtr pDataToInclude,
      int cbDataToInclude,
      Action<EncryptedAppTicketResponse_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUser_RequestEncryptedAppTicket(pDataToInclude, cbDataToInclude);
      return CallbackFunction == null ? (CallbackHandle) null : EncryptedAppTicketResponse_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public CallbackHandle RequestStoreAuthURL(
      string pchRedirectURL,
      Action<StoreAuthURLResponse_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUser_RequestStoreAuthURL(pchRedirectURL);
      return CallbackFunction == null ? (CallbackHandle) null : StoreAuthURLResponse_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public void StartVoiceRecording() => this.platform.ISteamUser_StartVoiceRecording();

    public void StopVoiceRecording() => this.platform.ISteamUser_StopVoiceRecording();

    public void TerminateGameConnection(uint unIPServer, ushort usPortServer)
    {
      this.platform.ISteamUser_TerminateGameConnection(unIPServer, usPortServer);
    }

    public void TrackAppUsageEvent(CGameID gameID, int eAppUsageEvent, string pchExtraInfo)
    {
      this.platform.ISteamUser_TrackAppUsageEvent(gameID.Value, eAppUsageEvent, pchExtraInfo);
    }

    public UserHasLicenseForAppResult UserHasLicenseForApp(CSteamID steamID, AppId_t appID)
    {
      return this.platform.ISteamUser_UserHasLicenseForApp(steamID.Value, appID.Value);
    }
  }
}
