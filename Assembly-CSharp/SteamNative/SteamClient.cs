using System;
using Facepunch.Steamworks;

namespace SteamNative;

internal class SteamClient : IDisposable {
	internal Platform.Interface platform;
	internal BaseSteamworks steamworks;

	internal SteamClient(BaseSteamworks steamworks, IntPtr pointer) {
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

	public bool BReleaseSteamPipe(HSteamPipe hSteamPipe) {
		return platform.ISteamClient_BReleaseSteamPipe(hSteamPipe.Value);
	}

	public bool BShutdownIfAllPipesClosed() {
		return platform.ISteamClient_BShutdownIfAllPipesClosed();
	}

	public HSteamUser ConnectToGlobalUser(HSteamPipe hSteamPipe) {
		return platform.ISteamClient_ConnectToGlobalUser(hSteamPipe.Value);
	}

	public HSteamUser CreateLocalUser(out HSteamPipe phSteamPipe, AccountType eAccountType) {
		return platform.ISteamClient_CreateLocalUser(out phSteamPipe.Value, eAccountType);
	}

	public HSteamPipe CreateSteamPipe() {
		return platform.ISteamClient_CreateSteamPipe();
	}

	public uint GetIPCCallCount() {
		return platform.ISteamClient_GetIPCCallCount();
	}

	public SteamAppList GetISteamAppList(
		HSteamUser hSteamUser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamAppList(steamworks,
			platform.ISteamClient_GetISteamAppList(hSteamUser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamApps GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) {
		return new SteamApps(steamworks,
			platform.ISteamClient_GetISteamApps(hSteamUser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamController GetISteamController(
		HSteamUser hSteamUser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamController(steamworks,
			platform.ISteamClient_GetISteamController(hSteamUser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamFriends GetISteamFriends(
		HSteamUser hSteamUser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamFriends(steamworks,
			platform.ISteamClient_GetISteamFriends(hSteamUser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamGameServer GetISteamGameServer(
		HSteamUser hSteamUser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamGameServer(steamworks,
			platform.ISteamClient_GetISteamGameServer(hSteamUser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamGameServerStats GetISteamGameServerStats(
		HSteamUser hSteamuser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamGameServerStats(steamworks,
			platform.ISteamClient_GetISteamGameServerStats(hSteamuser.Value, hSteamPipe.Value, pchVersion));
	}

	public IntPtr GetISteamGenericInterface(
		HSteamUser hSteamUser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return platform.ISteamClient_GetISteamGenericInterface(hSteamUser.Value, hSteamPipe.Value, pchVersion);
	}

	public SteamHTMLSurface GetISteamHTMLSurface(
		HSteamUser hSteamuser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamHTMLSurface(steamworks,
			platform.ISteamClient_GetISteamHTMLSurface(hSteamuser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamHTTP GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion) {
		return new SteamHTTP(steamworks,
			platform.ISteamClient_GetISteamHTTP(hSteamuser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamInventory GetISteamInventory(
		HSteamUser hSteamuser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamInventory(steamworks,
			platform.ISteamClient_GetISteamInventory(hSteamuser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamMatchmaking GetISteamMatchmaking(
		HSteamUser hSteamUser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamMatchmaking(steamworks,
			platform.ISteamClient_GetISteamMatchmaking(hSteamUser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamMatchmakingServers GetISteamMatchmakingServers(
		HSteamUser hSteamUser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamMatchmakingServers(steamworks,
			platform.ISteamClient_GetISteamMatchmakingServers(hSteamUser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamMusic GetISteamMusic(
		HSteamUser hSteamuser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamMusic(steamworks,
			platform.ISteamClient_GetISteamMusic(hSteamuser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamMusicRemote GetISteamMusicRemote(
		HSteamUser hSteamuser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamMusicRemote(steamworks,
			platform.ISteamClient_GetISteamMusicRemote(hSteamuser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamNetworking GetISteamNetworking(
		HSteamUser hSteamUser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamNetworking(steamworks,
			platform.ISteamClient_GetISteamNetworking(hSteamUser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamRemoteStorage GetISteamRemoteStorage(
		HSteamUser hSteamuser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamRemoteStorage(steamworks,
			platform.ISteamClient_GetISteamRemoteStorage(hSteamuser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamScreenshots GetISteamScreenshots(
		HSteamUser hSteamuser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamScreenshots(steamworks,
			platform.ISteamClient_GetISteamScreenshots(hSteamuser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamUGC GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) {
		return new SteamUGC(steamworks,
			platform.ISteamClient_GetISteamUGC(hSteamUser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamUnifiedMessages GetISteamUnifiedMessages(
		HSteamUser hSteamuser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamUnifiedMessages(steamworks,
			platform.ISteamClient_GetISteamUnifiedMessages(hSteamuser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamUser GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) {
		return new SteamUser(steamworks,
			platform.ISteamClient_GetISteamUser(hSteamUser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamUserStats GetISteamUserStats(
		HSteamUser hSteamUser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamUserStats(steamworks,
			platform.ISteamClient_GetISteamUserStats(hSteamUser.Value, hSteamPipe.Value, pchVersion));
	}

	public SteamUtils GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion) {
		return new SteamUtils(steamworks, platform.ISteamClient_GetISteamUtils(hSteamPipe.Value, pchVersion));
	}

	public SteamVideo GetISteamVideo(
		HSteamUser hSteamuser,
		HSteamPipe hSteamPipe,
		string pchVersion) {
		return new SteamVideo(steamworks,
			platform.ISteamClient_GetISteamVideo(hSteamuser.Value, hSteamPipe.Value, pchVersion));
	}

	public void ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser) {
		platform.ISteamClient_ReleaseUser(hSteamPipe.Value, hUser.Value);
	}

	public void SetLocalIPBinding(uint unIP, ushort usPort) {
		platform.ISteamClient_SetLocalIPBinding(unIP, usPort);
	}

	public void SetWarningMessageHook(IntPtr pFunction) {
		platform.ISteamClient_SetWarningMessageHook(pFunction);
	}
}