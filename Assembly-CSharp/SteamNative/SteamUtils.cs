// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamUtils
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace SteamNative
{
  internal class SteamUtils : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamUtils(BaseSteamworks steamworks, IntPtr pointer)
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

    public bool BOverlayNeedsPresent() => this.platform.ISteamUtils_BOverlayNeedsPresent();

    public CallbackHandle CheckFileSignature(
      string szFileName,
      Action<CheckFileSignature_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamUtils_CheckFileSignature(szFileName);
      return CallbackFunction == null ? (CallbackHandle) null : CheckFileSignature_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public SteamAPICallFailure GetAPICallFailureReason(SteamAPICall_t hSteamAPICall)
    {
      return this.platform.ISteamUtils_GetAPICallFailureReason(hSteamAPICall.Value);
    }

    public bool GetAPICallResult(
      SteamAPICall_t hSteamAPICall,
      IntPtr pCallback,
      int cubCallback,
      int iCallbackExpected,
      ref bool pbFailed)
    {
      return this.platform.ISteamUtils_GetAPICallResult(hSteamAPICall.Value, pCallback, cubCallback, iCallbackExpected, ref pbFailed);
    }

    public uint GetAppID() => this.platform.ISteamUtils_GetAppID();

    public Universe GetConnectedUniverse() => this.platform.ISteamUtils_GetConnectedUniverse();

    public bool GetCSERIPPort(out uint unIP, out ushort usPort)
    {
      return this.platform.ISteamUtils_GetCSERIPPort(out unIP, out usPort);
    }

    public byte GetCurrentBatteryPower() => this.platform.ISteamUtils_GetCurrentBatteryPower();

    public string GetEnteredGamepadTextInput()
    {
      StringBuilder stringBuilder = Helpers.TakeStringBuilder();
      uint cchText = 4096;
      return !this.platform.ISteamUtils_GetEnteredGamepadTextInput(stringBuilder, cchText) ? (string) null : stringBuilder.ToString();
    }

    public uint GetEnteredGamepadTextLength()
    {
      return this.platform.ISteamUtils_GetEnteredGamepadTextLength();
    }

    public bool GetImageRGBA(int iImage, IntPtr pubDest, int nDestBufferSize)
    {
      return this.platform.ISteamUtils_GetImageRGBA(iImage, pubDest, nDestBufferSize);
    }

    public bool GetImageSize(int iImage, out uint pnWidth, out uint pnHeight)
    {
      return this.platform.ISteamUtils_GetImageSize(iImage, out pnWidth, out pnHeight);
    }

    public uint GetIPCCallCount() => this.platform.ISteamUtils_GetIPCCallCount();

    public string GetIPCountry()
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamUtils_GetIPCountry());
    }

    public uint GetSecondsSinceAppActive() => this.platform.ISteamUtils_GetSecondsSinceAppActive();

    public uint GetSecondsSinceComputerActive()
    {
      return this.platform.ISteamUtils_GetSecondsSinceComputerActive();
    }

    public uint GetServerRealTime() => this.platform.ISteamUtils_GetServerRealTime();

    public string GetSteamUILanguage()
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamUtils_GetSteamUILanguage());
    }

    public bool IsAPICallCompleted(SteamAPICall_t hSteamAPICall, ref bool pbFailed)
    {
      return this.platform.ISteamUtils_IsAPICallCompleted(hSteamAPICall.Value, ref pbFailed);
    }

    public bool IsOverlayEnabled() => this.platform.ISteamUtils_IsOverlayEnabled();

    public bool IsSteamInBigPictureMode() => this.platform.ISteamUtils_IsSteamInBigPictureMode();

    public bool IsSteamRunningInVR() => this.platform.ISteamUtils_IsSteamRunningInVR();

    public bool IsVRHeadsetStreamingEnabled()
    {
      return this.platform.ISteamUtils_IsVRHeadsetStreamingEnabled();
    }

    public void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset)
    {
      this.platform.ISteamUtils_SetOverlayNotificationInset(nHorizontalInset, nVerticalInset);
    }

    public void SetOverlayNotificationPosition(NotificationPosition eNotificationPosition)
    {
      this.platform.ISteamUtils_SetOverlayNotificationPosition(eNotificationPosition);
    }

    public void SetVRHeadsetStreamingEnabled(bool bEnabled)
    {
      this.platform.ISteamUtils_SetVRHeadsetStreamingEnabled(bEnabled);
    }

    public void SetWarningMessageHook(IntPtr pFunction)
    {
      this.platform.ISteamUtils_SetWarningMessageHook(pFunction);
    }

    public bool ShowGamepadTextInput(
      GamepadTextInputMode eInputMode,
      GamepadTextInputLineMode eLineInputMode,
      string pchDescription,
      uint unCharMax,
      string pchExistingText)
    {
      return this.platform.ISteamUtils_ShowGamepadTextInput(eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText);
    }

    public void StartVRDashboard() => this.platform.ISteamUtils_StartVRDashboard();
  }
}
