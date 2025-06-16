using System;
using Facepunch.Steamworks;

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
        platform = (Platform.Interface) new Platform.Win64(pointer);
      else if (Platform.IsWindows32)
        platform = (Platform.Interface) new Platform.Win32(pointer);
      else if (Platform.IsLinux32)
        platform = (Platform.Interface) new Platform.Linux32(pointer);
      else if (Platform.IsLinux64)
      {
        platform = (Platform.Interface) new Platform.Linux64(pointer);
      }
      else
      {
        if (!Platform.IsOsx)
          return;
        platform = (Platform.Interface) new Platform.Mac(pointer);
      }
    }

    public bool IsValid => platform != null && platform.IsValid;

    public virtual void Dispose()
    {
      if (platform == null)
        return;
      platform.Dispose();
      platform = (Platform.Interface) null;
    }

    public ScreenshotHandle AddScreenshotToLibrary(
      string pchFilename,
      string pchThumbnailFilename,
      int nWidth,
      int nHeight)
    {
      return platform.ISteamScreenshots_AddScreenshotToLibrary(pchFilename, pchThumbnailFilename, nWidth, nHeight);
    }

    public ScreenshotHandle AddVRScreenshotToLibrary(
      VRScreenshotType eType,
      string pchFilename,
      string pchVRFilename)
    {
      return platform.ISteamScreenshots_AddVRScreenshotToLibrary(eType, pchFilename, pchVRFilename);
    }

    public void HookScreenshots(bool bHook)
    {
      platform.ISteamScreenshots_HookScreenshots(bHook);
    }

    public bool IsScreenshotsHooked() => platform.ISteamScreenshots_IsScreenshotsHooked();

    public bool SetLocation(ScreenshotHandle hScreenshot, string pchLocation)
    {
      return platform.ISteamScreenshots_SetLocation(hScreenshot.Value, pchLocation);
    }

    public bool TagPublishedFile(ScreenshotHandle hScreenshot, PublishedFileId_t unPublishedFileID)
    {
      return platform.ISteamScreenshots_TagPublishedFile(hScreenshot.Value, unPublishedFileID.Value);
    }

    public bool TagUser(ScreenshotHandle hScreenshot, CSteamID steamID)
    {
      return platform.ISteamScreenshots_TagUser(hScreenshot.Value, steamID.Value);
    }

    public void TriggerScreenshot() => platform.ISteamScreenshots_TriggerScreenshot();

    public ScreenshotHandle WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
    {
      return platform.ISteamScreenshots_WriteScreenshot(pubRGB, cubRGB, nWidth, nHeight);
    }
  }
}
