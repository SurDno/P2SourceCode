﻿using System;
using SteamNative;

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
      isServer = false;
      api = new SteamApi();
      if (!api.SteamAPI_Init())
      {
        Console.Error.WriteLine("InitClient: SteamAPI_Init returned false");
        return false;
      }
      HSteamUser hsteamUser = api.SteamAPI_GetHSteamUser();
      HSteamPipe hsteamPipe = api.SteamAPI_GetHSteamPipe();
      if (hsteamPipe == 0)
      {
        Console.Error.WriteLine("InitClient: hPipe == 0");
        return false;
      }
      FillInterfaces(steamworks, hsteamUser, hsteamPipe);
      if (!user.IsValid)
      {
        Console.Error.WriteLine("InitClient: ISteamUser is null");
        return false;
      }
      if (user.BLoggedOn())
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
      isServer = true;
      api = new SteamApi();
      if (!api.SteamInternal_GameServer_Init(IpAddress, usPort, GamePort, QueryPort, eServerMode, pchVersionString))
      {
        Console.Error.WriteLine("InitServer: GameServer_Init returned false");
        return false;
      }
      HSteamUser hsteamUser = api.SteamGameServer_GetHSteamUser();
      HSteamPipe hsteamPipe = api.SteamGameServer_GetHSteamPipe();
      if (hsteamPipe == 0)
      {
        Console.Error.WriteLine("InitServer: hPipe == 0");
        return false;
      }
      FillInterfaces(steamworks, hsteamPipe, hsteamUser);
      if (!gameServer.IsValid)
      {
        gameServer = null;
        throw new Exception("Steam Server: Couldn't load SteamGameServer012");
      }
      return true;
    }

    public void FillInterfaces(BaseSteamworks steamworks, int hpipe, int huser)
    {
      IntPtr pointer = api.SteamInternal_CreateInterface("SteamClient017");
      client = !(pointer == IntPtr.Zero) ? new SteamClient(steamworks, pointer) : throw new Exception("Steam Server: Couldn't load SteamClient017");
      user = client.GetISteamUser(huser, hpipe, "SteamUser019");
      utils = client.GetISteamUtils(hpipe, "SteamUtils009");
      networking = client.GetISteamNetworking(huser, hpipe, "SteamNetworking005");
      gameServerStats = client.GetISteamGameServerStats(huser, hpipe, "SteamGameServerStats001");
      http = client.GetISteamHTTP(huser, hpipe, "STEAMHTTP_INTERFACE_VERSION002");
      inventory = client.GetISteamInventory(huser, hpipe, "STEAMINVENTORY_INTERFACE_V002");
      ugc = client.GetISteamUGC(huser, hpipe, "STEAMUGC_INTERFACE_VERSION010");
      apps = client.GetISteamApps(huser, hpipe, "STEAMAPPS_INTERFACE_VERSION008");
      gameServer = client.GetISteamGameServer(huser, hpipe, "SteamGameServer012");
      friends = client.GetISteamFriends(huser, hpipe, "SteamFriends015");
      servers = client.GetISteamMatchmakingServers(huser, hpipe, "SteamMatchMakingServers002");
      userstats = client.GetISteamUserStats(huser, hpipe, "STEAMUSERSTATS_INTERFACE_VERSION011");
      screenshots = client.GetISteamScreenshots(huser, hpipe, "STEAMSCREENSHOTS_INTERFACE_VERSION003");
      remoteStorage = client.GetISteamRemoteStorage(huser, hpipe, "STEAMREMOTESTORAGE_INTERFACE_VERSION014");
      matchmaking = client.GetISteamMatchmaking(huser, hpipe, "SteamMatchMaking009");
      applist = client.GetISteamAppList(huser, hpipe, "STEAMAPPLIST_INTERFACE_VERSION001");
    }

    public void Dispose()
    {
      if (user != null)
      {
        user.Dispose();
        user = null;
      }
      if (utils != null)
      {
        utils.Dispose();
        utils = null;
      }
      if (networking != null)
      {
        networking.Dispose();
        networking = null;
      }
      if (gameServerStats != null)
      {
        gameServerStats.Dispose();
        gameServerStats = null;
      }
      if (http != null)
      {
        http.Dispose();
        http = null;
      }
      if (inventory != null)
      {
        inventory.Dispose();
        inventory = null;
      }
      if (ugc != null)
      {
        ugc.Dispose();
        ugc = null;
      }
      if (apps != null)
      {
        apps.Dispose();
        apps = null;
      }
      if (gameServer != null)
      {
        gameServer.Dispose();
        gameServer = null;
      }
      if (friends != null)
      {
        friends.Dispose();
        friends = null;
      }
      if (servers != null)
      {
        servers.Dispose();
        servers = null;
      }
      if (userstats != null)
      {
        userstats.Dispose();
        userstats = null;
      }
      if (screenshots != null)
      {
        screenshots.Dispose();
        screenshots = null;
      }
      if (remoteStorage != null)
      {
        remoteStorage.Dispose();
        remoteStorage = null;
      }
      if (matchmaking != null)
      {
        matchmaking.Dispose();
        matchmaking = null;
      }
      if (applist != null)
      {
        applist.Dispose();
        applist = null;
      }
      if (client != null)
      {
        client.Dispose();
        client = null;
      }
      if (api == null)
        return;
      if (isServer)
        api.SteamGameServer_Shutdown();
      else
        api.SteamAPI_Shutdown();
      api.Dispose();
      api = null;
    }
  }
}
