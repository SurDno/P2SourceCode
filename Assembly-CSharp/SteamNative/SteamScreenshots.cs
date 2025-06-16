// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamScreenshots
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;

#nullable disable
namespace SteamNative
{
  internal class SteamScreenshots : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamScreenshots(BaseSteamworks steamworks, IntPtr pointer)
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

    public ScreenshotHandle AddScreenshotToLibrary(
      string pchFilename,
      string pchThumbnailFilename,
      int nWidth,
      int nHeight)
    {
      return this.platform.ISteamScreenshots_AddScreenshotToLibrary(pchFilename, pchThumbnailFilename, nWidth, nHeight);
    }

    public ScreenshotHandle AddVRScreenshotToLibrary(
      VRScreenshotType eType,
      string pchFilename,
      string pchVRFilename)
    {
      return this.platform.ISteamScreenshots_AddVRScreenshotToLibrary(eType, pchFilename, pchVRFilename);
    }

    public void HookScreenshots(bool bHook)
    {
      this.platform.ISteamScreenshots_HookScreenshots(bHook);
    }

    public bool IsScreenshotsHooked() => this.platform.ISteamScreenshots_IsScreenshotsHooked();

    public bool SetLocation(ScreenshotHandle hScreenshot, string pchLocation)
    {
      return this.platform.ISteamScreenshots_SetLocation(hScreenshot.Value, pchLocation);
    }

    public bool TagPublishedFile(ScreenshotHandle hScreenshot, PublishedFileId_t unPublishedFileID)
    {
      return this.platform.ISteamScreenshots_TagPublishedFile(hScreenshot.Value, unPublishedFileID.Value);
    }

    public bool TagUser(ScreenshotHandle hScreenshot, CSteamID steamID)
    {
      return this.platform.ISteamScreenshots_TagUser(hScreenshot.Value, steamID.Value);
    }

    public void TriggerScreenshot() => this.platform.ISteamScreenshots_TriggerScreenshot();

    public ScreenshotHandle WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
    {
      return this.platform.ISteamScreenshots_WriteScreenshot(pubRGB, cubRGB, nWidth, nHeight);
    }
  }
}
