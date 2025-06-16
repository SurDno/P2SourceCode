using SteamNative;
using System;

namespace Facepunch.Steamworks.Interop
{
  internal class NativeInterface : IDisposable
  {
    internal SteamApi api;
    internal SteamClient client;
    internal SteamUser user;
    internal SteamApps apps;
    internal SteamAppList applist;
    internal SteamFriends friends;
    internal SteamMatchmakingServers servers;
    internal SteamMatchmaking matchmaking;
    internal SteamInventory inventory;
    internal SteamNetworking networking;
    internal SteamUserStats userstats;
    internal SteamUtils utils;
    internal SteamScreenshots screenshots;
    internal SteamHTTP http;
    internal SteamUGC ugc;
    internal SteamGameServer gameServer;
    internal SteamGameServerStats gameServerStats;
    internal SteamRemoteStorage remoteStorage;
    private bool isServer;

    internal bool InitClient(BaseSteamworks steamworks)
    {
      if (Server.Instance != null)
        throw new Exception("Steam client should be initialized before steam server - or there's big trouble.");
      this.isServer = false;
      this.api = new SteamApi();
      if (!this.api.SteamAPI_Init())
      {
        Console.Error.WriteLine("InitClient: SteamAPI_Init returned false");
        return false;
      }
      HSteamUser hsteamUser = this.api.SteamAPI_GetHSteamUser();
      HSteamPipe hsteamPipe = this.api.SteamAPI_GetHSteamPipe();
      if ((int) hsteamPipe == 0)
      {
        Console.Error.WriteLine("InitClient: hPipe == 0");
        return false;
      }
      this.FillInterfaces(steamworks, (int) hsteamUser, (int) hsteamPipe);
      if (!this.user.IsValid)
      {
        Console.Error.WriteLine("InitClient: ISteamUser is null");
        return false;
      }
      if (this.user.BLoggedOn())
        return true;
      Console.Error.WriteLine("InitClient: Not Logged On");
      return false;
    }

    internal bool InitServer(
      BaseSteamworks steamworks,
      uint IpAddress,
      ushort usPort,
      ushort GamePort,
      ushort QueryPort,
      int eServerMode,
      string pchVersionString)
    {
      this.isServer = true;
      this.api = new SteamApi();
      if (!this.api.SteamInternal_GameServer_Init(IpAddress, usPort, GamePort, QueryPort, eServerMode, pchVersionString))
      {
        Console.Error.WriteLine("InitServer: GameServer_Init returned false");
        return false;
      }
      HSteamUser hsteamUser = this.api.SteamGameServer_GetHSteamUser();
      HSteamPipe hsteamPipe = this.api.SteamGameServer_GetHSteamPipe();
      if ((int) hsteamPipe == 0)
      {
        Console.Error.WriteLine("InitServer: hPipe == 0");
        return false;
      }
      this.FillInterfaces(steamworks, (int) hsteamPipe, (int) hsteamUser);
      if (!this.gameServer.IsValid)
      {
        this.gameServer = (SteamGameServer) null;
        throw new Exception("Steam Server: Couldn't load SteamGameServer012");
      }
      return true;
    }

    public void FillInterfaces(BaseSteamworks steamworks, int hpipe, int huser)
    {
      IntPtr pointer = this.api.SteamInternal_CreateInterface("SteamClient017");
      this.client = !(pointer == IntPtr.Zero) ? new SteamClient(steamworks, pointer) : throw new Exception("Steam Server: Couldn't load SteamClient017");
      this.user = this.client.GetISteamUser((HSteamUser) huser, (HSteamPipe) hpipe, "SteamUser019");
      this.utils = this.client.GetISteamUtils((HSteamPipe) hpipe, "SteamUtils009");
      this.networking = this.client.GetISteamNetworking((HSteamUser) huser, (HSteamPipe) hpipe, "SteamNetworking005");
      this.gameServerStats = this.client.GetISteamGameServerStats((HSteamUser) huser, (HSteamPipe) hpipe, "SteamGameServerStats001");
      this.http = this.client.GetISteamHTTP((HSteamUser) huser, (HSteamPipe) hpipe, "STEAMHTTP_INTERFACE_VERSION002");
      this.inventory = this.client.GetISteamInventory((HSteamUser) huser, (HSteamPipe) hpipe, "STEAMINVENTORY_INTERFACE_V002");
      this.ugc = this.client.GetISteamUGC((HSteamUser) huser, (HSteamPipe) hpipe, "STEAMUGC_INTERFACE_VERSION010");
      this.apps = this.client.GetISteamApps((HSteamUser) huser, (HSteamPipe) hpipe, "STEAMAPPS_INTERFACE_VERSION008");
      this.gameServer = this.client.GetISteamGameServer((HSteamUser) huser, (HSteamPipe) hpipe, "SteamGameServer012");
      this.friends = this.client.GetISteamFriends((HSteamUser) huser, (HSteamPipe) hpipe, "SteamFriends015");
      this.servers = this.client.GetISteamMatchmakingServers((HSteamUser) huser, (HSteamPipe) hpipe, "SteamMatchMakingServers002");
      this.userstats = this.client.GetISteamUserStats((HSteamUser) huser, (HSteamPipe) hpipe, "STEAMUSERSTATS_INTERFACE_VERSION011");
      this.screenshots = this.client.GetISteamScreenshots((HSteamUser) huser, (HSteamPipe) hpipe, "STEAMSCREENSHOTS_INTERFACE_VERSION003");
      this.remoteStorage = this.client.GetISteamRemoteStorage((HSteamUser) huser, (HSteamPipe) hpipe, "STEAMREMOTESTORAGE_INTERFACE_VERSION014");
      this.matchmaking = this.client.GetISteamMatchmaking((HSteamUser) huser, (HSteamPipe) hpipe, "SteamMatchMaking009");
      this.applist = this.client.GetISteamAppList((HSteamUser) huser, (HSteamPipe) hpipe, "STEAMAPPLIST_INTERFACE_VERSION001");
    }

    public void Dispose()
    {
      if (this.user != null)
      {
        this.user.Dispose();
        this.user = (SteamUser) null;
      }
      if (this.utils != null)
      {
        this.utils.Dispose();
        this.utils = (SteamUtils) null;
      }
      if (this.networking != null)
      {
        this.networking.Dispose();
        this.networking = (SteamNetworking) null;
      }
      if (this.gameServerStats != null)
      {
        this.gameServerStats.Dispose();
        this.gameServerStats = (SteamGameServerStats) null;
      }
      if (this.http != null)
      {
        this.http.Dispose();
        this.http = (SteamHTTP) null;
      }
      if (this.inventory != null)
      {
        this.inventory.Dispose();
        this.inventory = (SteamInventory) null;
      }
      if (this.ugc != null)
      {
        this.ugc.Dispose();
        this.ugc = (SteamUGC) null;
      }
      if (this.apps != null)
      {
        this.apps.Dispose();
        this.apps = (SteamApps) null;
      }
      if (this.gameServer != null)
      {
        this.gameServer.Dispose();
        this.gameServer = (SteamGameServer) null;
      }
      if (this.friends != null)
      {
        this.friends.Dispose();
        this.friends = (SteamFriends) null;
      }
      if (this.servers != null)
      {
        this.servers.Dispose();
        this.servers = (SteamMatchmakingServers) null;
      }
      if (this.userstats != null)
      {
        this.userstats.Dispose();
        this.userstats = (SteamUserStats) null;
      }
      if (this.screenshots != null)
      {
        this.screenshots.Dispose();
        this.screenshots = (SteamScreenshots) null;
      }
      if (this.remoteStorage != null)
      {
        this.remoteStorage.Dispose();
        this.remoteStorage = (SteamRemoteStorage) null;
      }
      if (this.matchmaking != null)
      {
        this.matchmaking.Dispose();
        this.matchmaking = (SteamMatchmaking) null;
      }
      if (this.applist != null)
      {
        this.applist.Dispose();
        this.applist = (SteamAppList) null;
      }
      if (this.client != null)
      {
        this.client.Dispose();
        this.client = (SteamClient) null;
      }
      if (this.api == null)
        return;
      if (this.isServer)
        this.api.SteamGameServer_Shutdown();
      else
        this.api.SteamAPI_Shutdown();
      this.api.Dispose();
      this.api = (SteamApi) null;
    }
  }
}
