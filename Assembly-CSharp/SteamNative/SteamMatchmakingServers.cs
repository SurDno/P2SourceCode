using Facepunch.Steamworks;
using System;

namespace SteamNative
{
  internal class SteamMatchmakingServers : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamMatchmakingServers(BaseSteamworks steamworks, IntPtr pointer)
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

    public void CancelQuery(HServerListRequest hRequest)
    {
      this.platform.ISteamMatchmakingServers_CancelQuery(hRequest.Value);
    }

    public void CancelServerQuery(HServerQuery hServerQuery)
    {
      this.platform.ISteamMatchmakingServers_CancelServerQuery(hServerQuery.Value);
    }

    public int GetServerCount(HServerListRequest hRequest)
    {
      return this.platform.ISteamMatchmakingServers_GetServerCount(hRequest.Value);
    }

    public gameserveritem_t GetServerDetails(HServerListRequest hRequest, int iServer)
    {
      IntPtr serverDetails = this.platform.ISteamMatchmakingServers_GetServerDetails(hRequest.Value, iServer);
      return serverDetails == IntPtr.Zero ? new gameserveritem_t() : gameserveritem_t.FromPointer(serverDetails);
    }

    public bool IsRefreshing(HServerListRequest hRequest)
    {
      return this.platform.ISteamMatchmakingServers_IsRefreshing(hRequest.Value);
    }

    public HServerQuery PingServer(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
    {
      return this.platform.ISteamMatchmakingServers_PingServer(unIP, usPort, pRequestServersResponse);
    }

    public HServerQuery PlayerDetails(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
    {
      return this.platform.ISteamMatchmakingServers_PlayerDetails(unIP, usPort, pRequestServersResponse);
    }

    public void RefreshQuery(HServerListRequest hRequest)
    {
      this.platform.ISteamMatchmakingServers_RefreshQuery(hRequest.Value);
    }

    public void RefreshServer(HServerListRequest hRequest, int iServer)
    {
      this.platform.ISteamMatchmakingServers_RefreshServer(hRequest.Value, iServer);
    }

    public void ReleaseRequest(HServerListRequest hServerListRequest)
    {
      this.platform.ISteamMatchmakingServers_ReleaseRequest(hServerListRequest.Value);
    }

    public HServerListRequest RequestFavoritesServerList(
      AppId_t iApp,
      IntPtr ppchFilters,
      uint nFilters,
      IntPtr pRequestServersResponse)
    {
      return this.platform.ISteamMatchmakingServers_RequestFavoritesServerList(iApp.Value, ppchFilters, nFilters, pRequestServersResponse);
    }

    public HServerListRequest RequestFriendsServerList(
      AppId_t iApp,
      IntPtr ppchFilters,
      uint nFilters,
      IntPtr pRequestServersResponse)
    {
      return this.platform.ISteamMatchmakingServers_RequestFriendsServerList(iApp.Value, ppchFilters, nFilters, pRequestServersResponse);
    }

    public HServerListRequest RequestHistoryServerList(
      AppId_t iApp,
      IntPtr ppchFilters,
      uint nFilters,
      IntPtr pRequestServersResponse)
    {
      return this.platform.ISteamMatchmakingServers_RequestHistoryServerList(iApp.Value, ppchFilters, nFilters, pRequestServersResponse);
    }

    public HServerListRequest RequestInternetServerList(
      AppId_t iApp,
      IntPtr ppchFilters,
      uint nFilters,
      IntPtr pRequestServersResponse)
    {
      return this.platform.ISteamMatchmakingServers_RequestInternetServerList(iApp.Value, ppchFilters, nFilters, pRequestServersResponse);
    }

    public HServerListRequest RequestLANServerList(AppId_t iApp, IntPtr pRequestServersResponse)
    {
      return this.platform.ISteamMatchmakingServers_RequestLANServerList(iApp.Value, pRequestServersResponse);
    }

    public HServerListRequest RequestSpectatorServerList(
      AppId_t iApp,
      IntPtr ppchFilters,
      uint nFilters,
      IntPtr pRequestServersResponse)
    {
      return this.platform.ISteamMatchmakingServers_RequestSpectatorServerList(iApp.Value, ppchFilters, nFilters, pRequestServersResponse);
    }

    public HServerQuery ServerRules(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
    {
      return this.platform.ISteamMatchmakingServers_ServerRules(unIP, usPort, pRequestServersResponse);
    }
  }
}
