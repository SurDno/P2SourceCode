﻿using System;
using Facepunch.Steamworks;

namespace SteamNative;

internal class SteamNetworking : IDisposable {
	internal Platform.Interface platform;
	internal BaseSteamworks steamworks;

	internal SteamNetworking(BaseSteamworks steamworks, IntPtr pointer) {
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

	public bool AcceptP2PSessionWithUser(CSteamID steamIDRemote) {
		return platform.ISteamNetworking_AcceptP2PSessionWithUser(steamIDRemote.Value);
	}

	public bool AllowP2PPacketRelay(bool bAllow) {
		return platform.ISteamNetworking_AllowP2PPacketRelay(bAllow);
	}

	public bool CloseP2PChannelWithUser(CSteamID steamIDRemote, int nChannel) {
		return platform.ISteamNetworking_CloseP2PChannelWithUser(steamIDRemote.Value, nChannel);
	}

	public bool CloseP2PSessionWithUser(CSteamID steamIDRemote) {
		return platform.ISteamNetworking_CloseP2PSessionWithUser(steamIDRemote.Value);
	}

	public SNetSocket_t CreateConnectionSocket(uint nIP, ushort nPort, int nTimeoutSec) {
		return platform.ISteamNetworking_CreateConnectionSocket(nIP, nPort, nTimeoutSec);
	}

	public SNetListenSocket_t CreateListenSocket(
		int nVirtualP2PPort,
		uint nIP,
		ushort nPort,
		bool bAllowUseOfPacketRelay) {
		return platform.ISteamNetworking_CreateListenSocket(nVirtualP2PPort, nIP, nPort, bAllowUseOfPacketRelay);
	}

	public SNetSocket_t CreateP2PConnectionSocket(
		CSteamID steamIDTarget,
		int nVirtualPort,
		int nTimeoutSec,
		bool bAllowUseOfPacketRelay) {
		return platform.ISteamNetworking_CreateP2PConnectionSocket(steamIDTarget.Value, nVirtualPort, nTimeoutSec,
			bAllowUseOfPacketRelay);
	}

	public bool DestroyListenSocket(SNetListenSocket_t hSocket, bool bNotifyRemoteEnd) {
		return platform.ISteamNetworking_DestroyListenSocket(hSocket.Value, bNotifyRemoteEnd);
	}

	public bool DestroySocket(SNetSocket_t hSocket, bool bNotifyRemoteEnd) {
		return platform.ISteamNetworking_DestroySocket(hSocket.Value, bNotifyRemoteEnd);
	}

	public bool GetListenSocketInfo(
		SNetListenSocket_t hListenSocket,
		out uint pnIP,
		out ushort pnPort) {
		return platform.ISteamNetworking_GetListenSocketInfo(hListenSocket.Value, out pnIP, out pnPort);
	}

	public int GetMaxPacketSize(SNetSocket_t hSocket) {
		return platform.ISteamNetworking_GetMaxPacketSize(hSocket.Value);
	}

	public bool GetP2PSessionState(CSteamID steamIDRemote, ref P2PSessionState_t pConnectionState) {
		return platform.ISteamNetworking_GetP2PSessionState(steamIDRemote.Value, ref pConnectionState);
	}

	public SNetSocketConnectionType GetSocketConnectionType(SNetSocket_t hSocket) {
		return platform.ISteamNetworking_GetSocketConnectionType(hSocket.Value);
	}

	public bool GetSocketInfo(
		SNetSocket_t hSocket,
		out CSteamID pSteamIDRemote,
		IntPtr peSocketStatus,
		out uint punIPRemote,
		out ushort punPortRemote) {
		return platform.ISteamNetworking_GetSocketInfo(hSocket.Value, out pSteamIDRemote.Value, peSocketStatus,
			out punIPRemote, out punPortRemote);
	}

	public bool IsDataAvailable(
		SNetListenSocket_t hListenSocket,
		out uint pcubMsgSize,
		ref SNetSocket_t phSocket) {
		return platform.ISteamNetworking_IsDataAvailable(hListenSocket.Value, out pcubMsgSize, ref phSocket.Value);
	}

	public bool IsDataAvailableOnSocket(SNetSocket_t hSocket, out uint pcubMsgSize) {
		return platform.ISteamNetworking_IsDataAvailableOnSocket(hSocket.Value, out pcubMsgSize);
	}

	public bool IsP2PPacketAvailable(out uint pcubMsgSize, int nChannel) {
		return platform.ISteamNetworking_IsP2PPacketAvailable(out pcubMsgSize, nChannel);
	}

	public bool ReadP2PPacket(
		IntPtr pubDest,
		uint cubDest,
		out uint pcubMsgSize,
		out CSteamID psteamIDRemote,
		int nChannel) {
		return platform.ISteamNetworking_ReadP2PPacket(pubDest, cubDest, out pcubMsgSize, out psteamIDRemote.Value,
			nChannel);
	}

	public bool RetrieveData(
		SNetListenSocket_t hListenSocket,
		IntPtr pubDest,
		uint cubDest,
		out uint pcubMsgSize,
		ref SNetSocket_t phSocket) {
		return platform.ISteamNetworking_RetrieveData(hListenSocket.Value, pubDest, cubDest, out pcubMsgSize,
			ref phSocket.Value);
	}

	public bool RetrieveDataFromSocket(
		SNetSocket_t hSocket,
		IntPtr pubDest,
		uint cubDest,
		out uint pcubMsgSize) {
		return platform.ISteamNetworking_RetrieveDataFromSocket(hSocket.Value, pubDest, cubDest, out pcubMsgSize);
	}

	public bool SendDataOnSocket(
		SNetSocket_t hSocket,
		IntPtr pubData,
		uint cubData,
		bool bReliable) {
		return platform.ISteamNetworking_SendDataOnSocket(hSocket.Value, pubData, cubData, bReliable);
	}

	public bool SendP2PPacket(
		CSteamID steamIDRemote,
		IntPtr pubData,
		uint cubData,
		P2PSend eP2PSendType,
		int nChannel) {
		return platform.ISteamNetworking_SendP2PPacket(steamIDRemote.Value, pubData, cubData, eP2PSendType, nChannel);
	}
}