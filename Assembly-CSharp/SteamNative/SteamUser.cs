﻿using System;
using System.Text;
using Facepunch.Steamworks;

namespace SteamNative;

internal class SteamUser : IDisposable {
	internal Platform.Interface platform;
	internal BaseSteamworks steamworks;

	internal SteamUser(BaseSteamworks steamworks, IntPtr pointer) {
		this.steamworks = steamworks;
		if (Platform.IsWindows64)
			platform = (Platform.Interface)new Platform.Win64(pointer);
		else if (Platform.IsWindows32)
			platform = (Platform.Interface)new Platform.Win32(pointer);
		else if (Platform.IsLinux32)
			platform = (Platform.Interface)new Platform.Linux32(pointer);
		else if (Platform.IsLinux64)
			platform = (Platform.Interface)new Platform.Linux64(pointer);
		else {
			if (!Platform.IsOsx)
				return;
			platform = (Platform.Interface)new Platform.Mac(pointer);
		}
	}

	public bool IsValid => platform != null && platform.IsValid;

	public virtual void Dispose() {
		if (platform == null)
			return;
		platform.Dispose();
		platform = (Platform.Interface)null;
	}

	public void AdvertiseGame(CSteamID steamIDGameServer, uint unIPServer, ushort usPortServer) {
		platform.ISteamUser_AdvertiseGame(steamIDGameServer.Value, unIPServer, usPortServer);
	}

	public BeginAuthSessionResult BeginAuthSession(
		IntPtr pAuthTicket,
		int cbAuthTicket,
		CSteamID steamID) {
		return platform.ISteamUser_BeginAuthSession(pAuthTicket, cbAuthTicket, steamID.Value);
	}

	public bool BIsBehindNAT() {
		return platform.ISteamUser_BIsBehindNAT();
	}

	public bool BIsPhoneIdentifying() {
		return platform.ISteamUser_BIsPhoneIdentifying();
	}

	public bool BIsPhoneRequiringVerification() {
		return platform.ISteamUser_BIsPhoneRequiringVerification();
	}

	public bool BIsPhoneVerified() {
		return platform.ISteamUser_BIsPhoneVerified();
	}

	public bool BIsTwoFactorEnabled() {
		return platform.ISteamUser_BIsTwoFactorEnabled();
	}

	public bool BLoggedOn() {
		return platform.ISteamUser_BLoggedOn();
	}

	public void CancelAuthTicket(HAuthTicket hAuthTicket) {
		platform.ISteamUser_CancelAuthTicket(hAuthTicket.Value);
	}

	public VoiceResult DecompressVoice(
		IntPtr pCompressed,
		uint cbCompressed,
		IntPtr pDestBuffer,
		uint cbDestBufferSize,
		out uint nBytesWritten,
		uint nDesiredSampleRate) {
		return platform.ISteamUser_DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize,
			out nBytesWritten, nDesiredSampleRate);
	}

	public void EndAuthSession(CSteamID steamID) {
		platform.ISteamUser_EndAuthSession(steamID.Value);
	}

	public HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, out uint pcbTicket) {
		return platform.ISteamUser_GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket);
	}

	public VoiceResult GetAvailableVoice(
		out uint pcbCompressed,
		out uint pcbUncompressed_Deprecated,
		uint nUncompressedVoiceDesiredSampleRate_Deprecated) {
		return platform.ISteamUser_GetAvailableVoice(out pcbCompressed, out pcbUncompressed_Deprecated,
			nUncompressedVoiceDesiredSampleRate_Deprecated);
	}

	public bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, out uint pcbTicket) {
		return platform.ISteamUser_GetEncryptedAppTicket(pTicket, cbMaxTicket, out pcbTicket);
	}

	public int GetGameBadgeLevel(int nSeries, bool bFoil) {
		return platform.ISteamUser_GetGameBadgeLevel(nSeries, bFoil);
	}

	public HSteamUser GetHSteamUser() {
		return platform.ISteamUser_GetHSteamUser();
	}

	public int GetPlayerSteamLevel() {
		return platform.ISteamUser_GetPlayerSteamLevel();
	}

	public ulong GetSteamID() {
		return (ulong)platform.ISteamUser_GetSteamID();
	}

	public string GetUserDataFolder() {
		var stringBuilder = Helpers.TakeStringBuilder();
		var cubBuffer = 4096;
		return !platform.ISteamUser_GetUserDataFolder(stringBuilder, cubBuffer) ? null : stringBuilder.ToString();
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
		uint nUncompressedVoiceDesiredSampleRate_Deprecated) {
		return platform.ISteamUser_GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, out nBytesWritten,
			bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated,
			out nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
	}

	public uint GetVoiceOptimalSampleRate() {
		return platform.ISteamUser_GetVoiceOptimalSampleRate();
	}

	public int InitiateGameConnection(
		IntPtr pAuthBlob,
		int cbMaxAuthBlob,
		CSteamID steamIDGameServer,
		uint unIPServer,
		ushort usPortServer,
		bool bSecure) {
		return platform.ISteamUser_InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer.Value, unIPServer,
			usPortServer, bSecure);
	}

	public CallbackHandle RequestEncryptedAppTicket(
		IntPtr pDataToInclude,
		int cbDataToInclude,
		Action<EncryptedAppTicketResponse_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUser_RequestEncryptedAppTicket(pDataToInclude, cbDataToInclude);
		return CallbackFunction == null
			? null
			: EncryptedAppTicketResponse_t.CallResult(steamworks, call, CallbackFunction);
	}

	public CallbackHandle RequestStoreAuthURL(
		string pchRedirectURL,
		Action<StoreAuthURLResponse_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUser_RequestStoreAuthURL(pchRedirectURL);
		return CallbackFunction == null ? null : StoreAuthURLResponse_t.CallResult(steamworks, call, CallbackFunction);
	}

	public void StartVoiceRecording() {
		platform.ISteamUser_StartVoiceRecording();
	}

	public void StopVoiceRecording() {
		platform.ISteamUser_StopVoiceRecording();
	}

	public void TerminateGameConnection(uint unIPServer, ushort usPortServer) {
		platform.ISteamUser_TerminateGameConnection(unIPServer, usPortServer);
	}

	public void TrackAppUsageEvent(CGameID gameID, int eAppUsageEvent, string pchExtraInfo) {
		platform.ISteamUser_TrackAppUsageEvent(gameID.Value, eAppUsageEvent, pchExtraInfo);
	}

	public UserHasLicenseForAppResult UserHasLicenseForApp(CSteamID steamID, AppId_t appID) {
		return platform.ISteamUser_UserHasLicenseForApp(steamID.Value, appID.Value);
	}
}