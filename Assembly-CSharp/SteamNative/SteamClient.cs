using Facepunch.Steamworks;
using System;

namespace SteamNative
{
  internal class SteamClient : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamClient(BaseSteamworks steamworks, IntPtr pointer)
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

    public bool BReleaseSteamPipe(HSteamPipe hSteamPipe)
    {
      return this.platform.ISteamClient_BReleaseSteamPipe(hSteamPipe.Value);
    }

    public bool BShutdownIfAllPipesClosed()
    {
      return this.platform.ISteamClient_BShutdownIfAllPipesClosed();
    }

    public HSteamUser ConnectToGlobalUser(HSteamPipe hSteamPipe)
    {
      return this.platform.ISteamClient_ConnectToGlobalUser(hSteamPipe.Value);
    }

    public HSteamUser CreateLocalUser(out HSteamPipe phSteamPipe, AccountType eAccountType)
    {
      return this.platform.ISteamClient_CreateLocalUser(out phSteamPipe.Value, eAccountType);
    }

    public HSteamPipe CreateSteamPipe() => this.platform.ISteamClient_CreateSteamPipe();

    public uint GetIPCCallCount() => this.platform.ISteamClient_GetIPCCallCount();

    public SteamAppList GetISteamAppList(
      HSteamUser hSteamUser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamAppList(this.steamworks, this.platform.ISteamClient_GetISteamAppList(hSteamUser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamApps GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
      return new SteamApps(this.steamworks, this.platform.ISteamClient_GetISteamApps(hSteamUser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamController GetISteamController(
      HSteamUser hSteamUser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamController(this.steamworks, this.platform.ISteamClient_GetISteamController(hSteamUser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamFriends GetISteamFriends(
      HSteamUser hSteamUser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamFriends(this.steamworks, this.platform.ISteamClient_GetISteamFriends(hSteamUser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamGameServer GetISteamGameServer(
      HSteamUser hSteamUser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamGameServer(this.steamworks, this.platform.ISteamClient_GetISteamGameServer(hSteamUser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamGameServerStats GetISteamGameServerStats(
      HSteamUser hSteamuser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamGameServerStats(this.steamworks, this.platform.ISteamClient_GetISteamGameServerStats(hSteamuser.Value, hSteamPipe.Value, pchVersion));
    }

    public IntPtr GetISteamGenericInterface(
      HSteamUser hSteamUser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return this.platform.ISteamClient_GetISteamGenericInterface(hSteamUser.Value, hSteamPipe.Value, pchVersion);
    }

    public SteamHTMLSurface GetISteamHTMLSurface(
      HSteamUser hSteamuser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamHTMLSurface(this.steamworks, this.platform.ISteamClient_GetISteamHTMLSurface(hSteamuser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamHTTP GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
      return new SteamHTTP(this.steamworks, this.platform.ISteamClient_GetISteamHTTP(hSteamuser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamInventory GetISteamInventory(
      HSteamUser hSteamuser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamInventory(this.steamworks, this.platform.ISteamClient_GetISteamInventory(hSteamuser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamMatchmaking GetISteamMatchmaking(
      HSteamUser hSteamUser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamMatchmaking(this.steamworks, this.platform.ISteamClient_GetISteamMatchmaking(hSteamUser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamMatchmakingServers GetISteamMatchmakingServers(
      HSteamUser hSteamUser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamMatchmakingServers(this.steamworks, this.platform.ISteamClient_GetISteamMatchmakingServers(hSteamUser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamMusic GetISteamMusic(
      HSteamUser hSteamuser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamMusic(this.steamworks, this.platform.ISteamClient_GetISteamMusic(hSteamuser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamMusicRemote GetISteamMusicRemote(
      HSteamUser hSteamuser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamMusicRemote(this.steamworks, this.platform.ISteamClient_GetISteamMusicRemote(hSteamuser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamNetworking GetISteamNetworking(
      HSteamUser hSteamUser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamNetworking(this.steamworks, this.platform.ISteamClient_GetISteamNetworking(hSteamUser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamRemoteStorage GetISteamRemoteStorage(
      HSteamUser hSteamuser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamRemoteStorage(this.steamworks, this.platform.ISteamClient_GetISteamRemoteStorage(hSteamuser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamScreenshots GetISteamScreenshots(
      HSteamUser hSteamuser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamScreenshots(this.steamworks, this.platform.ISteamClient_GetISteamScreenshots(hSteamuser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamUGC GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
      return new SteamUGC(this.steamworks, this.platform.ISteamClient_GetISteamUGC(hSteamUser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamUnifiedMessages GetISteamUnifiedMessages(
      HSteamUser hSteamuser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamUnifiedMessages(this.steamworks, this.platform.ISteamClient_GetISteamUnifiedMessages(hSteamuser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamUser GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
      return new SteamUser(this.steamworks, this.platform.ISteamClient_GetISteamUser(hSteamUser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamUserStats GetISteamUserStats(
      HSteamUser hSteamUser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamUserStats(this.steamworks, this.platform.ISteamClient_GetISteamUserStats(hSteamUser.Value, hSteamPipe.Value, pchVersion));
    }

    public SteamUtils GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion)
    {
      return new SteamUtils(this.steamworks, this.platform.ISteamClient_GetISteamUtils(hSteamPipe.Value, pchVersion));
    }

    public SteamVideo GetISteamVideo(
      HSteamUser hSteamuser,
      HSteamPipe hSteamPipe,
      string pchVersion)
    {
      return new SteamVideo(this.steamworks, this.platform.ISteamClient_GetISteamVideo(hSteamuser.Value, hSteamPipe.Value, pchVersion));
    }

    public void ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser)
    {
      this.platform.ISteamClient_ReleaseUser(hSteamPipe.Value, hUser.Value);
    }

    public void SetLocalIPBinding(uint unIP, ushort usPort)
    {
      this.platform.ISteamClient_SetLocalIPBinding(unIP, usPort);
    }

    public void SetWarningMessageHook(IntPtr pFunction)
    {
      this.platform.ISteamClient_SetWarningMessageHook(pFunction);
    }
  }
}
