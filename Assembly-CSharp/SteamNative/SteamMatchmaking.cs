using System;
using System.Runtime.InteropServices;
using System.Text;
using Facepunch.Steamworks;

namespace SteamNative;

internal class SteamMatchmaking : IDisposable {
	internal Platform.Interface platform;
	internal BaseSteamworks steamworks;

	internal SteamMatchmaking(BaseSteamworks steamworks, IntPtr pointer) {
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

	public int AddFavoriteGame(
		AppId_t nAppID,
		uint nIP,
		ushort nConnPort,
		ushort nQueryPort,
		uint unFlags,
		uint rTime32LastPlayedOnServer) {
		return platform.ISteamMatchmaking_AddFavoriteGame(nAppID.Value, nIP, nConnPort, nQueryPort, unFlags,
			rTime32LastPlayedOnServer);
	}

	public void AddRequestLobbyListCompatibleMembersFilter(CSteamID steamIDLobby) {
		platform.ISteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(steamIDLobby.Value);
	}

	public void AddRequestLobbyListDistanceFilter(LobbyDistanceFilter eLobbyDistanceFilter) {
		platform.ISteamMatchmaking_AddRequestLobbyListDistanceFilter(eLobbyDistanceFilter);
	}

	public void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable) {
		platform.ISteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(nSlotsAvailable);
	}

	public void AddRequestLobbyListNearValueFilter(string pchKeyToMatch, int nValueToBeCloseTo) {
		platform.ISteamMatchmaking_AddRequestLobbyListNearValueFilter(pchKeyToMatch, nValueToBeCloseTo);
	}

	public void AddRequestLobbyListNumericalFilter(
		string pchKeyToMatch,
		int nValueToMatch,
		LobbyComparison eComparisonType) {
		platform.ISteamMatchmaking_AddRequestLobbyListNumericalFilter(pchKeyToMatch, nValueToMatch, eComparisonType);
	}

	public void AddRequestLobbyListResultCountFilter(int cMaxResults) {
		platform.ISteamMatchmaking_AddRequestLobbyListResultCountFilter(cMaxResults);
	}

	public void AddRequestLobbyListStringFilter(
		string pchKeyToMatch,
		string pchValueToMatch,
		LobbyComparison eComparisonType) {
		platform.ISteamMatchmaking_AddRequestLobbyListStringFilter(pchKeyToMatch, pchValueToMatch, eComparisonType);
	}

	public CallbackHandle CreateLobby(
		LobbyType eLobbyType,
		int cMaxMembers,
		Action<LobbyCreated_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var lobby = platform.ISteamMatchmaking_CreateLobby(eLobbyType, cMaxMembers);
		return CallbackFunction == null ? null : LobbyCreated_t.CallResult(steamworks, lobby, CallbackFunction);
	}

	public bool DeleteLobbyData(CSteamID steamIDLobby, string pchKey) {
		return platform.ISteamMatchmaking_DeleteLobbyData(steamIDLobby.Value, pchKey);
	}

	public bool GetFavoriteGame(
		int iGame,
		ref AppId_t pnAppID,
		out uint pnIP,
		out ushort pnConnPort,
		out ushort pnQueryPort,
		out uint punFlags,
		out uint pRTime32LastPlayedOnServer) {
		return platform.ISteamMatchmaking_GetFavoriteGame(iGame, ref pnAppID.Value, out pnIP, out pnConnPort,
			out pnQueryPort, out punFlags, out pRTime32LastPlayedOnServer);
	}

	public int GetFavoriteGameCount() {
		return platform.ISteamMatchmaking_GetFavoriteGameCount();
	}

	public ulong GetLobbyByIndex(int iLobby) {
		return (ulong)platform.ISteamMatchmaking_GetLobbyByIndex(iLobby);
	}

	public int GetLobbyChatEntry(
		CSteamID steamIDLobby,
		int iChatID,
		out CSteamID pSteamIDUser,
		IntPtr pvData,
		int cubData,
		out ChatEntryType peChatEntryType) {
		return platform.ISteamMatchmaking_GetLobbyChatEntry(steamIDLobby.Value, iChatID, out pSteamIDUser.Value, pvData,
			cubData, out peChatEntryType);
	}

	public string GetLobbyData(CSteamID steamIDLobby, string pchKey) {
		return Marshal.PtrToStringAnsi(platform.ISteamMatchmaking_GetLobbyData(steamIDLobby.Value, pchKey));
	}

	public bool GetLobbyDataByIndex(
		CSteamID steamIDLobby,
		int iLobbyData,
		out string pchKey,
		out string pchValue) {
		pchKey = string.Empty;
		var stringBuilder1 = Helpers.TakeStringBuilder();
		var cchKeyBufferSize = 4096;
		pchValue = string.Empty;
		var stringBuilder2 = Helpers.TakeStringBuilder();
		var cchValueBufferSize = 4096;
		var lobbyDataByIndex = platform.ISteamMatchmaking_GetLobbyDataByIndex(steamIDLobby.Value, iLobbyData,
			stringBuilder1, cchKeyBufferSize, stringBuilder2, cchValueBufferSize);
		if (!lobbyDataByIndex)
			return lobbyDataByIndex;
		pchValue = stringBuilder2.ToString();
		if (!lobbyDataByIndex)
			return lobbyDataByIndex;
		pchKey = stringBuilder1.ToString();
		return lobbyDataByIndex;
	}

	public int GetLobbyDataCount(CSteamID steamIDLobby) {
		return platform.ISteamMatchmaking_GetLobbyDataCount(steamIDLobby.Value);
	}

	public bool GetLobbyGameServer(
		CSteamID steamIDLobby,
		out uint punGameServerIP,
		out ushort punGameServerPort,
		out CSteamID psteamIDGameServer) {
		return platform.ISteamMatchmaking_GetLobbyGameServer(steamIDLobby.Value, out punGameServerIP,
			out punGameServerPort, out psteamIDGameServer.Value);
	}

	public ulong GetLobbyMemberByIndex(CSteamID steamIDLobby, int iMember) {
		return (ulong)platform.ISteamMatchmaking_GetLobbyMemberByIndex(steamIDLobby.Value, iMember);
	}

	public string GetLobbyMemberData(CSteamID steamIDLobby, CSteamID steamIDUser, string pchKey) {
		return Marshal.PtrToStringAnsi(
			platform.ISteamMatchmaking_GetLobbyMemberData(steamIDLobby.Value, steamIDUser.Value, pchKey));
	}

	public int GetLobbyMemberLimit(CSteamID steamIDLobby) {
		return platform.ISteamMatchmaking_GetLobbyMemberLimit(steamIDLobby.Value);
	}

	public ulong GetLobbyOwner(CSteamID steamIDLobby) {
		return (ulong)platform.ISteamMatchmaking_GetLobbyOwner(steamIDLobby.Value);
	}

	public int GetNumLobbyMembers(CSteamID steamIDLobby) {
		return platform.ISteamMatchmaking_GetNumLobbyMembers(steamIDLobby.Value);
	}

	public bool InviteUserToLobby(CSteamID steamIDLobby, CSteamID steamIDInvitee) {
		return platform.ISteamMatchmaking_InviteUserToLobby(steamIDLobby.Value, steamIDInvitee.Value);
	}

	public CallbackHandle JoinLobby(
		CSteamID steamIDLobby,
		Action<LobbyEnter_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamMatchmaking_JoinLobby(steamIDLobby.Value);
		return CallbackFunction == null ? null : LobbyEnter_t.CallResult(steamworks, call, CallbackFunction);
	}

	public void LeaveLobby(CSteamID steamIDLobby) {
		platform.ISteamMatchmaking_LeaveLobby(steamIDLobby.Value);
	}

	public bool RemoveFavoriteGame(
		AppId_t nAppID,
		uint nIP,
		ushort nConnPort,
		ushort nQueryPort,
		uint unFlags) {
		return platform.ISteamMatchmaking_RemoveFavoriteGame(nAppID.Value, nIP, nConnPort, nQueryPort, unFlags);
	}

	public bool RequestLobbyData(CSteamID steamIDLobby) {
		return platform.ISteamMatchmaking_RequestLobbyData(steamIDLobby.Value);
	}

	public CallbackHandle RequestLobbyList(Action<LobbyMatchList_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamMatchmaking_RequestLobbyList();
		return CallbackFunction == null ? null : LobbyMatchList_t.CallResult(steamworks, call, CallbackFunction);
	}

	public bool SendLobbyChatMsg(CSteamID steamIDLobby, IntPtr pvMsgBody, int cubMsgBody) {
		return platform.ISteamMatchmaking_SendLobbyChatMsg(steamIDLobby.Value, pvMsgBody, cubMsgBody);
	}

	public bool SetLinkedLobby(CSteamID steamIDLobby, CSteamID steamIDLobbyDependent) {
		return platform.ISteamMatchmaking_SetLinkedLobby(steamIDLobby.Value, steamIDLobbyDependent.Value);
	}

	public bool SetLobbyData(CSteamID steamIDLobby, string pchKey, string pchValue) {
		return platform.ISteamMatchmaking_SetLobbyData(steamIDLobby.Value, pchKey, pchValue);
	}

	public void SetLobbyGameServer(
		CSteamID steamIDLobby,
		uint unGameServerIP,
		ushort unGameServerPort,
		CSteamID steamIDGameServer) {
		platform.ISteamMatchmaking_SetLobbyGameServer(steamIDLobby.Value, unGameServerIP, unGameServerPort,
			steamIDGameServer.Value);
	}

	public bool SetLobbyJoinable(CSteamID steamIDLobby, bool bLobbyJoinable) {
		return platform.ISteamMatchmaking_SetLobbyJoinable(steamIDLobby.Value, bLobbyJoinable);
	}

	public void SetLobbyMemberData(CSteamID steamIDLobby, string pchKey, string pchValue) {
		platform.ISteamMatchmaking_SetLobbyMemberData(steamIDLobby.Value, pchKey, pchValue);
	}

	public bool SetLobbyMemberLimit(CSteamID steamIDLobby, int cMaxMembers) {
		return platform.ISteamMatchmaking_SetLobbyMemberLimit(steamIDLobby.Value, cMaxMembers);
	}

	public bool SetLobbyOwner(CSteamID steamIDLobby, CSteamID steamIDNewOwner) {
		return platform.ISteamMatchmaking_SetLobbyOwner(steamIDLobby.Value, steamIDNewOwner.Value);
	}

	public bool SetLobbyType(CSteamID steamIDLobby, LobbyType eLobbyType) {
		return platform.ISteamMatchmaking_SetLobbyType(steamIDLobby.Value, eLobbyType);
	}
}