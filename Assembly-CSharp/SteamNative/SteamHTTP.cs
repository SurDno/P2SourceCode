﻿using System;
using Facepunch.Steamworks;

namespace SteamNative;

internal class SteamHTTP : IDisposable {
	internal Platform.Interface platform;
	internal BaseSteamworks steamworks;

	internal SteamHTTP(BaseSteamworks steamworks, IntPtr pointer) {
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

	public HTTPCookieContainerHandle CreateCookieContainer(bool bAllowResponsesToModify) {
		return platform.ISteamHTTP_CreateCookieContainer(bAllowResponsesToModify);
	}

	public HTTPRequestHandle CreateHTTPRequest(HTTPMethod eHTTPRequestMethod, string pchAbsoluteURL) {
		return platform.ISteamHTTP_CreateHTTPRequest(eHTTPRequestMethod, pchAbsoluteURL);
	}

	public bool DeferHTTPRequest(HTTPRequestHandle hRequest) {
		return platform.ISteamHTTP_DeferHTTPRequest(hRequest.Value);
	}

	public bool GetHTTPDownloadProgressPct(HTTPRequestHandle hRequest, out float pflPercentOut) {
		return platform.ISteamHTTP_GetHTTPDownloadProgressPct(hRequest.Value, out pflPercentOut);
	}

	public bool GetHTTPRequestWasTimedOut(HTTPRequestHandle hRequest, ref bool pbWasTimedOut) {
		return platform.ISteamHTTP_GetHTTPRequestWasTimedOut(hRequest.Value, ref pbWasTimedOut);
	}

	public bool GetHTTPResponseBodyData(
		HTTPRequestHandle hRequest,
		out byte pBodyDataBuffer,
		uint unBufferSize) {
		return platform.ISteamHTTP_GetHTTPResponseBodyData(hRequest.Value, out pBodyDataBuffer, unBufferSize);
	}

	public bool GetHTTPResponseBodySize(HTTPRequestHandle hRequest, out uint unBodySize) {
		return platform.ISteamHTTP_GetHTTPResponseBodySize(hRequest.Value, out unBodySize);
	}

	public bool GetHTTPResponseHeaderSize(
		HTTPRequestHandle hRequest,
		string pchHeaderName,
		out uint unResponseHeaderSize) {
		return platform.ISteamHTTP_GetHTTPResponseHeaderSize(hRequest.Value, pchHeaderName, out unResponseHeaderSize);
	}

	public bool GetHTTPResponseHeaderValue(
		HTTPRequestHandle hRequest,
		string pchHeaderName,
		out byte pHeaderValueBuffer,
		uint unBufferSize) {
		return platform.ISteamHTTP_GetHTTPResponseHeaderValue(hRequest.Value, pchHeaderName, out pHeaderValueBuffer,
			unBufferSize);
	}

	public bool GetHTTPStreamingResponseBodyData(
		HTTPRequestHandle hRequest,
		uint cOffset,
		out byte pBodyDataBuffer,
		uint unBufferSize) {
		return platform.ISteamHTTP_GetHTTPStreamingResponseBodyData(hRequest.Value, cOffset, out pBodyDataBuffer,
			unBufferSize);
	}

	public bool PrioritizeHTTPRequest(HTTPRequestHandle hRequest) {
		return platform.ISteamHTTP_PrioritizeHTTPRequest(hRequest.Value);
	}

	public bool ReleaseCookieContainer(HTTPCookieContainerHandle hCookieContainer) {
		return platform.ISteamHTTP_ReleaseCookieContainer(hCookieContainer.Value);
	}

	public bool ReleaseHTTPRequest(HTTPRequestHandle hRequest) {
		return platform.ISteamHTTP_ReleaseHTTPRequest(hRequest.Value);
	}

	public bool SendHTTPRequest(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle) {
		return platform.ISteamHTTP_SendHTTPRequest(hRequest.Value, ref pCallHandle.Value);
	}

	public bool SendHTTPRequestAndStreamResponse(
		HTTPRequestHandle hRequest,
		ref SteamAPICall_t pCallHandle) {
		return platform.ISteamHTTP_SendHTTPRequestAndStreamResponse(hRequest.Value, ref pCallHandle.Value);
	}

	public bool SetCookie(
		HTTPCookieContainerHandle hCookieContainer,
		string pchHost,
		string pchUrl,
		string pchCookie) {
		return platform.ISteamHTTP_SetCookie(hCookieContainer.Value, pchHost, pchUrl, pchCookie);
	}

	public bool SetHTTPRequestAbsoluteTimeoutMS(HTTPRequestHandle hRequest, uint unMilliseconds) {
		return platform.ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS(hRequest.Value, unMilliseconds);
	}

	public bool SetHTTPRequestContextValue(HTTPRequestHandle hRequest, ulong ulContextValue) {
		return platform.ISteamHTTP_SetHTTPRequestContextValue(hRequest.Value, ulContextValue);
	}

	public bool SetHTTPRequestCookieContainer(
		HTTPRequestHandle hRequest,
		HTTPCookieContainerHandle hCookieContainer) {
		return platform.ISteamHTTP_SetHTTPRequestCookieContainer(hRequest.Value, hCookieContainer.Value);
	}

	public bool SetHTTPRequestGetOrPostParameter(
		HTTPRequestHandle hRequest,
		string pchParamName,
		string pchParamValue) {
		return platform.ISteamHTTP_SetHTTPRequestGetOrPostParameter(hRequest.Value, pchParamName, pchParamValue);
	}

	public bool SetHTTPRequestHeaderValue(
		HTTPRequestHandle hRequest,
		string pchHeaderName,
		string pchHeaderValue) {
		return platform.ISteamHTTP_SetHTTPRequestHeaderValue(hRequest.Value, pchHeaderName, pchHeaderValue);
	}

	public bool SetHTTPRequestNetworkActivityTimeout(
		HTTPRequestHandle hRequest,
		uint unTimeoutSeconds) {
		return platform.ISteamHTTP_SetHTTPRequestNetworkActivityTimeout(hRequest.Value, unTimeoutSeconds);
	}

	public bool SetHTTPRequestRawPostBody(
		HTTPRequestHandle hRequest,
		string pchContentType,
		out byte pubBody,
		uint unBodyLen) {
		return platform.ISteamHTTP_SetHTTPRequestRawPostBody(hRequest.Value, pchContentType, out pubBody, unBodyLen);
	}

	public bool SetHTTPRequestRequiresVerifiedCertificate(
		HTTPRequestHandle hRequest,
		bool bRequireVerifiedCertificate) {
		return platform.ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate(hRequest.Value,
			bRequireVerifiedCertificate);
	}

	public bool SetHTTPRequestUserAgentInfo(HTTPRequestHandle hRequest, string pchUserAgentInfo) {
		return platform.ISteamHTTP_SetHTTPRequestUserAgentInfo(hRequest.Value, pchUserAgentInfo);
	}
}