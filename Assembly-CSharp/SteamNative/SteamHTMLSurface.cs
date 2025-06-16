// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamHTMLSurface
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;

#nullable disable
namespace SteamNative
{
  internal class SteamHTMLSurface : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamHTMLSurface(BaseSteamworks steamworks, IntPtr pointer)
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

    public void AddHeader(HHTMLBrowser unBrowserHandle, string pchKey, string pchValue)
    {
      this.platform.ISteamHTMLSurface_AddHeader(unBrowserHandle.Value, pchKey, pchValue);
    }

    public void AllowStartRequest(HHTMLBrowser unBrowserHandle, bool bAllowed)
    {
      this.platform.ISteamHTMLSurface_AllowStartRequest(unBrowserHandle.Value, bAllowed);
    }

    public void CopyToClipboard(HHTMLBrowser unBrowserHandle)
    {
      this.platform.ISteamHTMLSurface_CopyToClipboard(unBrowserHandle.Value);
    }

    public CallbackHandle CreateBrowser(
      string pchUserAgent,
      string pchUserCSS,
      Action<HTML_BrowserReady_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t browser = this.platform.ISteamHTMLSurface_CreateBrowser(pchUserAgent, pchUserCSS);
      return CallbackFunction == null ? (CallbackHandle) null : HTML_BrowserReady_t.CallResult(this.steamworks, browser, CallbackFunction);
    }

    public void DestructISteamHTMLSurface()
    {
      this.platform.ISteamHTMLSurface_DestructISteamHTMLSurface();
    }

    public void ExecuteJavascript(HHTMLBrowser unBrowserHandle, string pchScript)
    {
      this.platform.ISteamHTMLSurface_ExecuteJavascript(unBrowserHandle.Value, pchScript);
    }

    public void Find(
      HHTMLBrowser unBrowserHandle,
      string pchSearchStr,
      bool bCurrentlyInFind,
      bool bReverse)
    {
      this.platform.ISteamHTMLSurface_Find(unBrowserHandle.Value, pchSearchStr, bCurrentlyInFind, bReverse);
    }

    public void GetLinkAtPosition(HHTMLBrowser unBrowserHandle, int x, int y)
    {
      this.platform.ISteamHTMLSurface_GetLinkAtPosition(unBrowserHandle.Value, x, y);
    }

    public void GoBack(HHTMLBrowser unBrowserHandle)
    {
      this.platform.ISteamHTMLSurface_GoBack(unBrowserHandle.Value);
    }

    public void GoForward(HHTMLBrowser unBrowserHandle)
    {
      this.platform.ISteamHTMLSurface_GoForward(unBrowserHandle.Value);
    }

    public bool Init() => this.platform.ISteamHTMLSurface_Init();

    public void JSDialogResponse(HHTMLBrowser unBrowserHandle, bool bResult)
    {
      this.platform.ISteamHTMLSurface_JSDialogResponse(unBrowserHandle.Value, bResult);
    }

    public void KeyChar(
      HHTMLBrowser unBrowserHandle,
      uint cUnicodeChar,
      HTMLKeyModifiers eHTMLKeyModifiers)
    {
      this.platform.ISteamHTMLSurface_KeyChar(unBrowserHandle.Value, cUnicodeChar, eHTMLKeyModifiers);
    }

    public void KeyDown(
      HHTMLBrowser unBrowserHandle,
      uint nNativeKeyCode,
      HTMLKeyModifiers eHTMLKeyModifiers)
    {
      this.platform.ISteamHTMLSurface_KeyDown(unBrowserHandle.Value, nNativeKeyCode, eHTMLKeyModifiers);
    }

    public void KeyUp(
      HHTMLBrowser unBrowserHandle,
      uint nNativeKeyCode,
      HTMLKeyModifiers eHTMLKeyModifiers)
    {
      this.platform.ISteamHTMLSurface_KeyUp(unBrowserHandle.Value, nNativeKeyCode, eHTMLKeyModifiers);
    }

    public void LoadURL(HHTMLBrowser unBrowserHandle, string pchURL, string pchPostData)
    {
      this.platform.ISteamHTMLSurface_LoadURL(unBrowserHandle.Value, pchURL, pchPostData);
    }

    public void MouseDoubleClick(HHTMLBrowser unBrowserHandle, HTMLMouseButton eMouseButton)
    {
      this.platform.ISteamHTMLSurface_MouseDoubleClick(unBrowserHandle.Value, eMouseButton);
    }

    public void MouseDown(HHTMLBrowser unBrowserHandle, HTMLMouseButton eMouseButton)
    {
      this.platform.ISteamHTMLSurface_MouseDown(unBrowserHandle.Value, eMouseButton);
    }

    public void MouseMove(HHTMLBrowser unBrowserHandle, int x, int y)
    {
      this.platform.ISteamHTMLSurface_MouseMove(unBrowserHandle.Value, x, y);
    }

    public void MouseUp(HHTMLBrowser unBrowserHandle, HTMLMouseButton eMouseButton)
    {
      this.platform.ISteamHTMLSurface_MouseUp(unBrowserHandle.Value, eMouseButton);
    }

    public void MouseWheel(HHTMLBrowser unBrowserHandle, int nDelta)
    {
      this.platform.ISteamHTMLSurface_MouseWheel(unBrowserHandle.Value, nDelta);
    }

    public void PasteFromClipboard(HHTMLBrowser unBrowserHandle)
    {
      this.platform.ISteamHTMLSurface_PasteFromClipboard(unBrowserHandle.Value);
    }

    public void Reload(HHTMLBrowser unBrowserHandle)
    {
      this.platform.ISteamHTMLSurface_Reload(unBrowserHandle.Value);
    }

    public void RemoveBrowser(HHTMLBrowser unBrowserHandle)
    {
      this.platform.ISteamHTMLSurface_RemoveBrowser(unBrowserHandle.Value);
    }

    public void SetBackgroundMode(HHTMLBrowser unBrowserHandle, bool bBackgroundMode)
    {
      this.platform.ISteamHTMLSurface_SetBackgroundMode(unBrowserHandle.Value, bBackgroundMode);
    }

    public void SetCookie(
      string pchHostname,
      string pchKey,
      string pchValue,
      string pchPath,
      RTime32 nExpires,
      bool bSecure,
      bool bHTTPOnly)
    {
      this.platform.ISteamHTMLSurface_SetCookie(pchHostname, pchKey, pchValue, pchPath, nExpires.Value, bSecure, bHTTPOnly);
    }

    public void SetHorizontalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
    {
      this.platform.ISteamHTMLSurface_SetHorizontalScroll(unBrowserHandle.Value, nAbsolutePixelScroll);
    }

    public void SetKeyFocus(HHTMLBrowser unBrowserHandle, bool bHasKeyFocus)
    {
      this.platform.ISteamHTMLSurface_SetKeyFocus(unBrowserHandle.Value, bHasKeyFocus);
    }

    public void SetPageScaleFactor(
      HHTMLBrowser unBrowserHandle,
      float flZoom,
      int nPointX,
      int nPointY)
    {
      this.platform.ISteamHTMLSurface_SetPageScaleFactor(unBrowserHandle.Value, flZoom, nPointX, nPointY);
    }

    public void SetSize(HHTMLBrowser unBrowserHandle, uint unWidth, uint unHeight)
    {
      this.platform.ISteamHTMLSurface_SetSize(unBrowserHandle.Value, unWidth, unHeight);
    }

    public void SetVerticalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
    {
      this.platform.ISteamHTMLSurface_SetVerticalScroll(unBrowserHandle.Value, nAbsolutePixelScroll);
    }

    public bool Shutdown() => this.platform.ISteamHTMLSurface_Shutdown();

    public void StopFind(HHTMLBrowser unBrowserHandle)
    {
      this.platform.ISteamHTMLSurface_StopFind(unBrowserHandle.Value);
    }

    public void StopLoad(HHTMLBrowser unBrowserHandle)
    {
      this.platform.ISteamHTMLSurface_StopLoad(unBrowserHandle.Value);
    }

    public void ViewSource(HHTMLBrowser unBrowserHandle)
    {
      this.platform.ISteamHTMLSurface_ViewSource(unBrowserHandle.Value);
    }
  }
}
