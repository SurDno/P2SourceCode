// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Client
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.IO;

#nullable disable
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
      Client.Instance = Client.Instance == null ? this : throw new Exception("Only one Facepunch.Steamworks.Client can exist - dispose the old one before trying to create a new one.");
      this.native = new NativeInterface();
      if (!this.native.InitClient((BaseSteamworks) this))
      {
        this.native.Dispose();
        this.native = (NativeInterface) null;
        Client.Instance = (Client) null;
      }
      else
      {
        this.SetupCommonInterfaces();
        this.Voice = new Voice(this);
        this.ServerList = new ServerList(this);
        this.LobbyList = new LobbyList(this);
        this.App = new App(this);
        this.Stats = new Stats(this);
        this.Achievements = new Achievements(this);
        this.MicroTransactions = new MicroTransactions(this);
        this.User = new User(this);
        this.RemoteStorage = new RemoteStorage(this);
        this.Workshop.friends = this.Friends;
        this.Stats.UpdateStats();
        this.AppId = appId;
        this.Username = this.native.friends.GetPersonaName();
        this.SteamId = this.native.user.GetSteamID();
        this.BetaName = this.native.apps.GetCurrentBetaName();
        this.OwnerSteamId = this.native.apps.GetAppOwner();
        string appInstallDir = this.native.apps.GetAppInstallDir((AppId_t) this.AppId);
        if (!string.IsNullOrEmpty(appInstallDir) && Directory.Exists(appInstallDir))
          this.InstallFolder = new DirectoryInfo(appInstallDir);
        this.BuildId = this.native.apps.GetAppBuildId();
        this.CurrentLanguage = this.native.apps.GetCurrentGameLanguage();
        this.AvailableLanguages = this.native.apps.GetAvailableGameLanguages().Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries);
        this.Update();
      }
    }

    public override void Update()
    {
      if (!this.IsValid)
        return;
      this.RunCallbacks();
      this.Voice.Update();
      base.Update();
    }

    public void RunCallbacks() => this.native.api.SteamAPI_RunCallbacks();

    public override void Dispose()
    {
      if (this.Voice != null)
        this.Voice = (Voice) null;
      if (this.ServerList != null)
      {
        this.ServerList.Dispose();
        this.ServerList = (ServerList) null;
      }
      if (this.LobbyList != null)
      {
        this.LobbyList.Dispose();
        this.LobbyList = (LobbyList) null;
      }
      if (this.App != null)
      {
        this.App.Dispose();
        this.App = (App) null;
      }
      if (this.Stats != null)
      {
        this.Stats.Dispose();
        this.Stats = (Stats) null;
      }
      if (this.Achievements != null)
      {
        this.Achievements.Dispose();
        this.Achievements = (Achievements) null;
      }
      if (this.MicroTransactions != null)
      {
        this.MicroTransactions.Dispose();
        this.MicroTransactions = (MicroTransactions) null;
      }
      if (this.User != null)
      {
        this.User.Dispose();
        this.User = (User) null;
      }
      if (this.RemoteStorage != null)
      {
        this.RemoteStorage.Dispose();
        this.RemoteStorage = (RemoteStorage) null;
      }
      if (Client.Instance == this)
        Client.Instance = (Client) null;
      base.Dispose();
    }

    public Leaderboard GetLeaderboard(
      string name,
      Client.LeaderboardSortMethod sortMethod = Client.LeaderboardSortMethod.None,
      Client.LeaderboardDisplayType displayType = Client.LeaderboardDisplayType.None)
    {
      Leaderboard leaderboard = new Leaderboard(this);
      this.native.userstats.FindOrCreateLeaderboard(name, (SteamNative.LeaderboardSortMethod) sortMethod, (SteamNative.LeaderboardDisplayType) displayType, new Action<LeaderboardFindResult_t, bool>(leaderboard.OnBoardCreated));
      return leaderboard;
    }

    public bool IsSubscribed => this.native.apps.BIsSubscribed();

    public bool IsCybercafe => this.native.apps.BIsCybercafe();

    public bool IsSubscribedFromFreeWeekend => this.native.apps.BIsSubscribedFromFreeWeekend();

    public bool IsLowViolence => this.native.apps.BIsLowViolence();

    public static bool RestartIfNecessary(uint appid)
    {
      using (SteamApi steamApi = new SteamApi())
        return steamApi.SteamAPI_RestartAppIfNecessary(appid);
    }

    public Auth Auth
    {
      get
      {
        if (this._auth == null)
          this._auth = new Auth() { client = this };
        return this._auth;
      }
    }

    public Friends Friends
    {
      get
      {
        if (this._friends == null)
          this._friends = new Friends(this);
        return this._friends;
      }
    }

    public Lobby Lobby
    {
      get
      {
        if (this._lobby == null)
          this._lobby = new Lobby(this);
        return this._lobby;
      }
    }

    public Overlay Overlay
    {
      get
      {
        if (this._overlay == null)
          this._overlay = new Overlay() { client = this };
        return this._overlay;
      }
    }

    public Screenshots Screenshots
    {
      get
      {
        if (this._screenshots == null)
          this._screenshots = new Screenshots(this);
        return this._screenshots;
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
