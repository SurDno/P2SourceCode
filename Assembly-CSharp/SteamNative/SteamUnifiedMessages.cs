using System;
using Facepunch.Steamworks;

namespace SteamNative;

internal class SteamUnifiedMessages : IDisposable {
	internal Platform.Interface platform;
	internal BaseSteamworks steamworks;

	internal SteamUnifiedMessages(BaseSteamworks steamworks, IntPtr pointer) {
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

	public bool GetMethodResponseData(
		ClientUnifiedMessageHandle hHandle,
		IntPtr pResponseBuffer,
		uint unResponseBufferSize,
		bool bAutoRelease) {
		return platform.ISteamUnifiedMessages_GetMethodResponseData(hHandle.Value, pResponseBuffer,
			unResponseBufferSize, bAutoRelease);
	}

	public bool GetMethodResponseInfo(
		ClientUnifiedMessageHandle hHandle,
		out uint punResponseSize,
		out Result peResult) {
		return platform.ISteamUnifiedMessages_GetMethodResponseInfo(hHandle.Value, out punResponseSize, out peResult);
	}

	public bool ReleaseMethod(ClientUnifiedMessageHandle hHandle) {
		return platform.ISteamUnifiedMessages_ReleaseMethod(hHandle.Value);
	}

	public ClientUnifiedMessageHandle SendMethod(
		string pchServiceMethod,
		IntPtr pRequestBuffer,
		uint unRequestBufferSize,
		ulong unContext) {
		return platform.ISteamUnifiedMessages_SendMethod(pchServiceMethod, pRequestBuffer, unRequestBufferSize,
			unContext);
	}

	public bool SendNotification(
		string pchServiceNotification,
		IntPtr pNotificationBuffer,
		uint unNotificationBufferSize) {
		return platform.ISteamUnifiedMessages_SendNotification(pchServiceNotification, pNotificationBuffer,
			unNotificationBufferSize);
	}
}