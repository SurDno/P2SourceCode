﻿using System;
using System.IO;
using Facepunch.Steamworks.Interop;
using SteamNative;

namespace Facepunch.Steamworks
{
  public class Client : BaseSteamworks, IDisposable
  {
    private Auth _auth;
    private Friends _friends;
    private Lobby _lobby;
    private Overlay _overlay;
    private Screenshots _screenshots;

    public static Client Instance { get; private set; }

    public string Username { get; private set; }

    public ulong SteamId { get; private set; }

    public ulong OwnerSteamId { get; private set; }

    public string BetaName { get; private set; }

    public int BuildId { get; private set; }

    public DirectoryInfo InstallFolder { get; private set; }

    public string CurrentLanguage { get; }

    public string[] AvailableLanguages { get; }

    public Voice Voice { get; private set; }

    public ServerList ServerList { get; private set; }

    public LobbyList LobbyList { get; private set; }

    public App App { get; private set; }

    public Achievements Achievements { get; private set; }

    public Stats Stats { get; private set; }

    public MicroTransactions MicroTransactions { get; private set; }

    public User User { get; private set; }

    public RemoteStorage RemoteStorage { get; private set; }

    public Client(uint appId)
    {
      Instance = Instance == null ? this : throw new Exception("Only one Facepunch.Steamworks.Client can exist - dispose the old one before trying to create a new one.");
      native = new NativeInterface();
      if (!native.InitClient(this))
      {
        native.Dispose();
        native = null;
        Instance = null;
      }
      else
      {
        SetupCommonInterfaces();
        Voice = new Voice(this);
        ServerList = new ServerList(this);
        LobbyList = new LobbyList(this);
        App = new App(this);
        Stats = new Stats(this);
        Achievements = new Achievements(this);
        MicroTransactions = new MicroTransactions(this);
        User = new User(this);
        RemoteStorage = new RemoteStorage(this);
        Workshop.friends = Friends;
        Stats.UpdateStats();
        AppId = appId;
        Username = native.friends.GetPersonaName();
        SteamId = native.user.GetSteamID();
        BetaName = native.apps.GetCurrentBetaName();
        OwnerSteamId = native.apps.GetAppOwner();
        string appInstallDir = native.apps.GetAppInstallDir(AppId);
        if (!string.IsNullOrEmpty(appInstallDir) && Directory.Exists(appInstallDir))
          InstallFolder = new DirectoryInfo(appInstallDir);
        BuildId = native.apps.GetAppBuildId();
        CurrentLanguage = native.apps.GetCurrentGameLanguage();
        AvailableLanguages = native.apps.GetAvailableGameLanguages().Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries);
        Update();
      }
    }

    public override void Update()
    {
      if (!IsValid)
        return;
      RunCallbacks();
      Voice.Update();
      base.Update();
    }

    public void RunCallbacks() => native.api.SteamAPI_RunCallbacks();

    public override void Dispose()
    {
      if (Voice != null)
        Voice = null;
      if (ServerList != null)
      {
        ServerList.Dispose();
        ServerList = null;
      }
      if (LobbyList != null)
      {
        LobbyList.Dispose();
        LobbyList = null;
      }
      if (App != null)
      {
        App.Dispose();
        App = null;
      }
      if (Stats != null)
      {
        Stats.Dispose();
        Stats = null;
      }
      if (Achievements != null)
      {
        Achievements.Dispose();
        Achievements = null;
      }
      if (MicroTransactions != null)
      {
        MicroTransactions.Dispose();
        MicroTransactions = null;
      }
      if (User != null)
      {
        User.Dispose();
        User = null;
      }
      if (RemoteStorage != null)
      {
        RemoteStorage.Dispose();
        RemoteStorage = null;
      }
      if (Instance == this)
        Instance = null;
      base.Dispose();
    }

    public Leaderboard GetLeaderboard(
      string name,
      LeaderboardSortMethod sortMethod = LeaderboardSortMethod.None,
      LeaderboardDisplayType displayType = LeaderboardDisplayType.None)
    {
      Leaderboard leaderboard = new Leaderboard(this);
      native.userstats.FindOrCreateLeaderboard(name, (SteamNative.LeaderboardSortMethod) sortMethod, (SteamNative.LeaderboardDisplayType) displayType, leaderboard.OnBoardCreated);
      return leaderboard;
    }

    public bool IsSubscribed => native.apps.BIsSubscribed();

    public bool IsCybercafe => native.apps.BIsCybercafe();

    public bool IsSubscribedFromFreeWeekend => native.apps.BIsSubscribedFromFreeWeekend();

    public bool IsLowViolence => native.apps.BIsLowViolence();

    public static bool RestartIfNecessary(uint appid)
    {
      using (SteamApi steamApi = new SteamApi())
        return steamApi.SteamAPI_RestartAppIfNecessary(appid);
    }

    public Auth Auth
    {
      get
      {
        if (_auth == null)
          _auth = new Auth { client = this };
        return _auth;
      }
    }

    public Friends Friends
    {
      get
      {
        if (_friends == null)
          _friends = new Friends(this);
        return _friends;
      }
    }

    public Lobby Lobby
    {
      get
      {
        if (_lobby == null)
          _lobby = new Lobby(this);
        return _lobby;
      }
    }

    public Overlay Overlay
    {
      get
      {
        if (_overlay == null)
          _overlay = new Overlay { client = this };
        return _overlay;
      }
    }

    public Screenshots Screenshots
    {
      get
      {
        if (_screenshots == null)
          _screenshots = new Screenshots(this);
        return _screenshots;
      }
    }

    public enum LeaderboardSortMethod
    {
      None,
      Ascending,
      Descending,
    }

    public enum LeaderboardDisplayType
    {
      None,
      Numeric,
      TimeSeconds,
      TimeMilliSeconds,
    }
  }
}
