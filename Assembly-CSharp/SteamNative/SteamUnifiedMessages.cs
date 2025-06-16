using Facepunch.Steamworks;
using System;

namespace SteamNative
{
  internal class SteamUnifiedMessages : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamUnifiedMessages(BaseSteamworks steamworks, IntPtr pointer)
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

    public bool GetMethodResponseData(
      ClientUnifiedMessageHandle hHandle,
      IntPtr pResponseBuffer,
      uint unResponseBufferSize,
      bool bAutoRelease)
    {
      return this.platform.ISteamUnifiedMessages_GetMethodResponseData(hHandle.Value, pResponseBuffer, unResponseBufferSize, bAutoRelease);
    }

    public bool GetMethodResponseInfo(
      ClientUnifiedMessageHandle hHandle,
      out uint punResponseSize,
      out Result peResult)
    {
      return this.platform.ISteamUnifiedMessages_GetMethodResponseInfo(hHandle.Value, out punResponseSize, out peResult);
    }

    public bool ReleaseMethod(ClientUnifiedMessageHandle hHandle)
    {
      return this.platform.ISteamUnifiedMessages_ReleaseMethod(hHandle.Value);
    }

    public ClientUnifiedMessageHandle SendMethod(
      string pchServiceMethod,
      IntPtr pRequestBuffer,
      uint unRequestBufferSize,
      ulong unContext)
    {
      return this.platform.ISteamUnifiedMessages_SendMethod(pchServiceMethod, pRequestBuffer, unRequestBufferSize, unContext);
    }

    public bool SendNotification(
      string pchServiceNotification,
      IntPtr pNotificationBuffer,
      uint unNotificationBufferSize)
    {
      return this.platform.ISteamUnifiedMessages_SendNotification(pchServiceNotification, pNotificationBuffer, unNotificationBufferSize);
    }
  }
}
