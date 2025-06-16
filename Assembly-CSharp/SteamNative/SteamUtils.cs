﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using Facepunch.Steamworks;

namespace SteamNative;

internal class SteamUtils : IDisposable {
	internal Platform.Interface platform;
	internal BaseSteamworks steamworks;

	internal SteamUtils(BaseSteamworks steamworks, IntPtr pointer) {
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

	public bool BOverlayNeedsPresent() {
		return platform.ISteamUtils_BOverlayNeedsPresent();
	}

	public CallbackHandle CheckFileSignature(
		string szFileName,
		Action<CheckFileSignature_t, bool> CallbackFunction = null) {
		SteamAPICall_t steamApiCallT = 0UL;
		var call = platform.ISteamUtils_CheckFileSignature(szFileName);
		return CallbackFunction == null ? null : CheckFileSignature_t.CallResult(steamworks, call, CallbackFunction);
	}

	public SteamAPICallFailure GetAPICallFailureReason(SteamAPICall_t hSteamAPICall) {
		return platform.ISteamUtils_GetAPICallFailureReason(hSteamAPICall.Value);
	}

	public bool GetAPICallResult(
		SteamAPICall_t hSteamAPICall,
		IntPtr pCallback,
		int cubCallback,
		int iCallbackExpected,
		ref bool pbFailed) {
		return platform.ISteamUtils_GetAPICallResult(hSteamAPICall.Value, pCallback, cubCallback, iCallbackExpected,
			ref pbFailed);
	}

	public uint GetAppID() {
		return platform.ISteamUtils_GetAppID();
	}

	public Universe GetConnectedUniverse() {
		return platform.ISteamUtils_GetConnectedUniverse();
	}

	public bool GetCSERIPPort(out uint unIP, out ushort usPort) {
		return platform.ISteamUtils_GetCSERIPPort(out unIP, out usPort);
	}

	public byte GetCurrentBatteryPower() {
		return platform.ISteamUtils_GetCurrentBatteryPower();
	}

	public string GetEnteredGamepadTextInput() {
		var stringBuilder = Helpers.TakeStringBuilder();
		uint cchText = 4096;
		return !platform.ISteamUtils_GetEnteredGamepadTextInput(stringBuilder, cchText)
			? null
			: stringBuilder.ToString();
	}

	public uint GetEnteredGamepadTextLength() {
		return platform.ISteamUtils_GetEnteredGamepadTextLength();
	}

	public bool GetImageRGBA(int iImage, IntPtr pubDest, int nDestBufferSize) {
		return platform.ISteamUtils_GetImageRGBA(iImage, pubDest, nDestBufferSize);
	}

	public bool GetImageSize(int iImage, out uint pnWidth, out uint pnHeight) {
		return platform.ISteamUtils_GetImageSize(iImage, out pnWidth, out pnHeight);
	}

	public uint GetIPCCallCount() {
		return platform.ISteamUtils_GetIPCCallCount();
	}

	public string GetIPCountry() {
		return Marshal.PtrToStringAnsi(platform.ISteamUtils_GetIPCountry());
	}

	public uint GetSecondsSinceAppActive() {
		return platform.ISteamUtils_GetSecondsSinceAppActive();
	}

	public uint GetSecondsSinceComputerActive() {
		return platform.ISteamUtils_GetSecondsSinceComputerActive();
	}

	public uint GetServerRealTime() {
		return platform.ISteamUtils_GetServerRealTime();
	}

	public string GetSteamUILanguage() {
		return Marshal.PtrToStringAnsi(platform.ISteamUtils_GetSteamUILanguage());
	}

	public bool IsAPICallCompleted(SteamAPICall_t hSteamAPICall, ref bool pbFailed) {
		return platform.ISteamUtils_IsAPICallCompleted(hSteamAPICall.Value, ref pbFailed);
	}

	public bool IsOverlayEnabled() {
		return platform.ISteamUtils_IsOverlayEnabled();
	}

	public bool IsSteamInBigPictureMode() {
		return platform.ISteamUtils_IsSteamInBigPictureMode();
	}

	public bool IsSteamRunningInVR() {
		return platform.ISteamUtils_IsSteamRunningInVR();
	}

	public bool IsVRHeadsetStreamingEnabled() {
		return platform.ISteamUtils_IsVRHeadsetStreamingEnabled();
	}

	public void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset) {
		platform.ISteamUtils_SetOverlayNotificationInset(nHorizontalInset, nVerticalInset);
	}

	public void SetOverlayNotificationPosition(NotificationPosition eNotificationPosition) {
		platform.ISteamUtils_SetOverlayNotificationPosition(eNotificationPosition);
	}

	public void SetVRHeadsetStreamingEnabled(bool bEnabled) {
		platform.ISteamUtils_SetVRHeadsetStreamingEnabled(bEnabled);
	}

	public void SetWarningMessageHook(IntPtr pFunction) {
		platform.ISteamUtils_SetWarningMessageHook(pFunction);
	}

	public bool ShowGamepadTextInput(
		GamepadTextInputMode eInputMode,
		GamepadTextInputLineMode eLineInputMode,
		string pchDescription,
		uint unCharMax,
		string pchExistingText) {
		return platform.ISteamUtils_ShowGamepadTextInput(eInputMode, eLineInputMode, pchDescription, unCharMax,
			pchExistingText);
	}

	public void StartVRDashboard() {
		platform.ISteamUtils_StartVRDashboard();
	}
}