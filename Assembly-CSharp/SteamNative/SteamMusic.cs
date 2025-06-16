﻿using System;
using Facepunch.Steamworks;

namespace SteamNative;

internal class SteamMusic : IDisposable {
	internal Platform.Interface platform;
	internal BaseSteamworks steamworks;

	internal SteamMusic(BaseSteamworks steamworks, IntPtr pointer) {
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

	public bool BIsEnabled() {
		return platform.ISteamMusic_BIsEnabled();
	}

	public bool BIsPlaying() {
		return platform.ISteamMusic_BIsPlaying();
	}

	public AudioPlayback_Status GetPlaybackStatus() {
		return platform.ISteamMusic_GetPlaybackStatus();
	}

	public float GetVolume() {
		return platform.ISteamMusic_GetVolume();
	}

	public void Pause() {
		platform.ISteamMusic_Pause();
	}

	public void Play() {
		platform.ISteamMusic_Play();
	}

	public void PlayNext() {
		platform.ISteamMusic_PlayNext();
	}

	public void PlayPrevious() {
		platform.ISteamMusic_PlayPrevious();
	}

	public void SetVolume(float flVolume) {
		platform.ISteamMusic_SetVolume(flVolume);
	}
}