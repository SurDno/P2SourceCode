﻿using System;

namespace SteamNative
{
  internal class SteamApi : IDisposable
  {
    internal Platform.Interface platform;

    internal SteamApi()
    {
      if (Platform.IsWindows64)
        this.platform = (Platform.Interface) new Platform.Win64((IntPtr) 1);
      else if (Platform.IsWindows32)
        this.platform = (Platform.Interface) new Platform.Win32((IntPtr) 1);
      else if (Platform.IsLinux32)
        this.platform = (Platform.Interface) new Platform.Linux32((IntPtr) 1);
      else if (Platform.IsLinux64)
      {
        this.platform = (Platform.Interface) new Platform.Linux64((IntPtr) 1);
      }
      else
      {
        if (!Platform.IsOsx)
          return;
        this.platform = (Platform.Interface) new Platform.Mac((IntPtr) 1);
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

    public HSteamPipe SteamAPI_GetHSteamPipe() => this.platform.SteamApi_SteamAPI_GetHSteamPipe();

    public HSteamUser SteamAPI_GetHSteamUser() => this.platform.SteamApi_SteamAPI_GetHSteamUser();

    public bool SteamAPI_Init() => this.platform.SteamApi_SteamAPI_Init();

    public void SteamAPI_RegisterCallback(IntPtr pCallback, int callback)
    {
      this.platform.SteamApi_SteamAPI_RegisterCallback(pCallback, callback);
    }

    public void SteamAPI_RegisterCallResult(IntPtr pCallback, SteamAPICall_t callback)
    {
      this.platform.SteamApi_SteamAPI_RegisterCallResult(pCallback, callback.Value);
    }

    public bool SteamAPI_RestartAppIfNecessary(uint unOwnAppID)
    {
      return this.platform.SteamApi_SteamAPI_RestartAppIfNecessary(unOwnAppID);
    }

    public void SteamAPI_RunCallbacks() => this.platform.SteamApi_SteamAPI_RunCallbacks();

    public void SteamAPI_Shutdown() => this.platform.SteamApi_SteamAPI_Shutdown();

    public void SteamAPI_UnregisterCallback(IntPtr pCallback)
    {
      this.platform.SteamApi_SteamAPI_UnregisterCallback(pCallback);
    }

    public void SteamAPI_UnregisterCallResult(IntPtr pCallback, SteamAPICall_t callback)
    {
      this.platform.SteamApi_SteamAPI_UnregisterCallResult(pCallback, callback.Value);
    }

    public HSteamPipe SteamGameServer_GetHSteamPipe()
    {
      return this.platform.SteamApi_SteamGameServer_GetHSteamPipe();
    }

    public HSteamUser SteamGameServer_GetHSteamUser()
    {
      return this.platform.SteamApi_SteamGameServer_GetHSteamUser();
    }

    public void SteamGameServer_RunCallbacks()
    {
      this.platform.SteamApi_SteamGameServer_RunCallbacks();
    }

    public void SteamGameServer_Shutdown() => this.platform.SteamApi_SteamGameServer_Shutdown();

    public IntPtr SteamInternal_CreateInterface(string version)
    {
      return this.platform.SteamApi_SteamInternal_CreateInterface(version);
    }

    public bool SteamInternal_GameServer_Init(
      uint unIP,
      ushort usPort,
      ushort usGamePort,
      ushort usQueryPort,
      int eServerMode,
      string pchVersionString)
    {
      return this.platform.SteamApi_SteamInternal_GameServer_Init(unIP, usPort, usGamePort, usQueryPort, eServerMode, pchVersionString);
    }
  }
}
