using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.Net;

namespace Facepunch.Steamworks
{
  public class Server : BaseSteamworks
  {
    private bool _dedicatedServer;
    private int _maxplayers = 0;
    private int _botcount = 0;
    private string _mapname;
    private string _modDir = "";
    private string _product = "";
    private string _gameDescription = "";
    private string _serverName = "";
    private bool _passworded;
    private string _gametags = "";
    private Dictionary<string, string> KeyValue = new Dictionary<string, string>();

    public static Server Instance { get; private set; }

    internal override bool IsGameServer => true;

    public ServerQuery Query { get; internal set; }

    public ServerStats Stats { get; internal set; }

    public ServerAuth Auth { get; internal set; }

    public Server(uint appId, ServerInit init)
    {
      Server.Instance = Server.Instance == null ? this : throw new Exception("Only one Facepunch.Steamworks.Server can exist - dispose the old one before trying to create a new one.");
      this.native = new NativeInterface();
      if (init.SteamPort == (ushort) 0)
        init.RandomSteamPort();
      if (!this.native.InitServer((BaseSteamworks) this, init.IpAddress, init.SteamPort, init.GamePort, init.QueryPort, init.Secure ? 3 : 2, init.VersionString))
      {
        this.native.Dispose();
        this.native = (NativeInterface) null;
        Server.Instance = (Server) null;
      }
      else
      {
        this.SetupCommonInterfaces();
        this.AppId = appId;
        this.native.gameServer.EnableHeartbeats(true);
        this.MaxPlayers = 32;
        this.BotCount = 0;
        this.Product = string.Format("{0}", (object) this.AppId);
        this.ModDir = init.ModDir;
        this.GameDescription = init.GameDescription;
        this.Passworded = false;
        this.DedicatedServer = true;
        this.Query = new ServerQuery(this);
        this.Stats = new ServerStats(this);
        this.Auth = new ServerAuth(this);
        this.Update();
      }
    }

    public override void Update()
    {
      if (!this.IsValid)
        return;
      this.native.api.SteamGameServer_RunCallbacks();
      base.Update();
    }

    public bool DedicatedServer
    {
      get => this._dedicatedServer;
      set
      {
        if (this._dedicatedServer == value)
          return;
        this.native.gameServer.SetDedicatedServer(value);
        this._dedicatedServer = value;
      }
    }

    public int MaxPlayers
    {
      get => this._maxplayers;
      set
      {
        if (this._maxplayers == value)
          return;
        this.native.gameServer.SetMaxPlayerCount(value);
        this._maxplayers = value;
      }
    }

    public int BotCount
    {
      get => this._botcount;
      set
      {
        if (this._botcount == value)
          return;
        this.native.gameServer.SetBotPlayerCount(value);
        this._botcount = value;
      }
    }

    public string MapName
    {
      get => this._mapname;
      set
      {
        if (this._mapname == value)
          return;
        this.native.gameServer.SetMapName(value);
        this._mapname = value;
      }
    }

    public string ModDir
    {
      get => this._modDir;
      internal set
      {
        if (this._modDir == value)
          return;
        this.native.gameServer.SetModDir(value);
        this._modDir = value;
      }
    }

    public string Product
    {
      get => this._product;
      internal set
      {
        if (this._product == value)
          return;
        this.native.gameServer.SetProduct(value);
        this._product = value;
      }
    }

    public string GameDescription
    {
      get => this._gameDescription;
      internal set
      {
        if (this._gameDescription == value)
          return;
        this.native.gameServer.SetGameDescription(value);
        this._gameDescription = value;
      }
    }

    public string ServerName
    {
      get => this._serverName;
      set
      {
        if (this._serverName == value)
          return;
        this.native.gameServer.SetServerName(value);
        this._serverName = value;
      }
    }

    public bool Passworded
    {
      get => this._passworded;
      set
      {
        if (this._passworded == value)
          return;
        this.native.gameServer.SetPasswordProtected(value);
        this._passworded = value;
      }
    }

    public string GameTags
    {
      get => this._gametags;
      set
      {
        if (this._gametags == value)
          return;
        this.native.gameServer.SetGameTags(value);
        this._gametags = value;
      }
    }

    public void LogOnAnonymous()
    {
      this.native.gameServer.LogOnAnonymous();
      this.ForceHeartbeat();
    }

    public bool LoggedOn => this.native.gameServer.BLoggedOn();

    public void SetKey(string Key, string Value)
    {
      if (this.KeyValue.ContainsKey(Key))
      {
        if (this.KeyValue[Key] == Value)
          return;
        this.KeyValue[Key] = Value;
      }
      else
        this.KeyValue.Add(Key, Value);
      this.native.gameServer.SetKeyValue(Key, Value);
    }

    public void UpdatePlayer(ulong steamid, string name, int score)
    {
      this.native.gameServer.BUpdateUserData((CSteamID) steamid, name, (uint) score);
    }

    public override void Dispose()
    {
      if (this.Query != null)
        this.Query = (ServerQuery) null;
      if (this.Stats != null)
        this.Stats = (ServerStats) null;
      if (this.Auth != null)
        this.Auth = (ServerAuth) null;
      if (Server.Instance == this)
        Server.Instance = (Server) null;
      base.Dispose();
    }

    public IPAddress PublicIp
    {
      get
      {
        uint publicIp = this.native.gameServer.GetPublicIP();
        return publicIp == 0U ? (IPAddress) null : new IPAddress((long) Utility.SwapBytes(publicIp));
      }
    }

    public bool AutomaticHeartbeats
    {
      set => this.native.gameServer.EnableHeartbeats(value);
    }

    public int AutomaticHeartbeatRate
    {
      set => this.native.gameServer.SetHeartbeatInterval(value);
    }

    public void ForceHeartbeat() => this.native.gameServer.ForceHeartbeat();
  }
}
