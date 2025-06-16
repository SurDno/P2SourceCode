using System;
using System.Collections.Generic;
using System.Net;
using Facepunch.Steamworks.Interop;

namespace Facepunch.Steamworks
{
  public class Server : BaseSteamworks
  {
    private bool _dedicatedServer;
    private int _maxplayers;
    private int _botcount;
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
      Instance = Instance == null ? this : throw new Exception("Only one Facepunch.Steamworks.Server can exist - dispose the old one before trying to create a new one.");
      native = new NativeInterface();
      if (init.SteamPort == 0)
        init.RandomSteamPort();
      if (!native.InitServer(this, init.IpAddress, init.SteamPort, init.GamePort, init.QueryPort, init.Secure ? 3 : 2, init.VersionString))
      {
        native.Dispose();
        native = null;
        Instance = null;
      }
      else
      {
        SetupCommonInterfaces();
        AppId = appId;
        native.gameServer.EnableHeartbeats(true);
        MaxPlayers = 32;
        BotCount = 0;
        Product = string.Format("{0}", AppId);
        ModDir = init.ModDir;
        GameDescription = init.GameDescription;
        Passworded = false;
        DedicatedServer = true;
        Query = new ServerQuery(this);
        Stats = new ServerStats(this);
        Auth = new ServerAuth(this);
        Update();
      }
    }

    public override void Update()
    {
      if (!IsValid)
        return;
      native.api.SteamGameServer_RunCallbacks();
      base.Update();
    }

    public bool DedicatedServer
    {
      get => _dedicatedServer;
      set
      {
        if (_dedicatedServer == value)
          return;
        native.gameServer.SetDedicatedServer(value);
        _dedicatedServer = value;
      }
    }

    public int MaxPlayers
    {
      get => _maxplayers;
      set
      {
        if (_maxplayers == value)
          return;
        native.gameServer.SetMaxPlayerCount(value);
        _maxplayers = value;
      }
    }

    public int BotCount
    {
      get => _botcount;
      set
      {
        if (_botcount == value)
          return;
        native.gameServer.SetBotPlayerCount(value);
        _botcount = value;
      }
    }

    public string MapName
    {
      get => _mapname;
      set
      {
        if (_mapname == value)
          return;
        native.gameServer.SetMapName(value);
        _mapname = value;
      }
    }

    public string ModDir
    {
      get => _modDir;
      internal set
      {
        if (_modDir == value)
          return;
        native.gameServer.SetModDir(value);
        _modDir = value;
      }
    }

    public string Product
    {
      get => _product;
      internal set
      {
        if (_product == value)
          return;
        native.gameServer.SetProduct(value);
        _product = value;
      }
    }

    public string GameDescription
    {
      get => _gameDescription;
      internal set
      {
        if (_gameDescription == value)
          return;
        native.gameServer.SetGameDescription(value);
        _gameDescription = value;
      }
    }

    public string ServerName
    {
      get => _serverName;
      set
      {
        if (_serverName == value)
          return;
        native.gameServer.SetServerName(value);
        _serverName = value;
      }
    }

    public bool Passworded
    {
      get => _passworded;
      set
      {
        if (_passworded == value)
          return;
        native.gameServer.SetPasswordProtected(value);
        _passworded = value;
      }
    }

    public string GameTags
    {
      get => _gametags;
      set
      {
        if (_gametags == value)
          return;
        native.gameServer.SetGameTags(value);
        _gametags = value;
      }
    }

    public void LogOnAnonymous()
    {
      native.gameServer.LogOnAnonymous();
      ForceHeartbeat();
    }

    public bool LoggedOn => native.gameServer.BLoggedOn();

    public void SetKey(string Key, string Value)
    {
      if (KeyValue.ContainsKey(Key))
      {
        if (KeyValue[Key] == Value)
          return;
        KeyValue[Key] = Value;
      }
      else
        KeyValue.Add(Key, Value);
      native.gameServer.SetKeyValue(Key, Value);
    }

    public void UpdatePlayer(ulong steamid, string name, int score)
    {
      native.gameServer.BUpdateUserData(steamid, name, (uint) score);
    }

    public override void Dispose()
    {
      if (Query != null)
        Query = null;
      if (Stats != null)
        Stats = null;
      if (Auth != null)
        Auth = null;
      if (Instance == this)
        Instance = null;
      base.Dispose();
    }

    public IPAddress PublicIp
    {
      get
      {
        uint publicIp = native.gameServer.GetPublicIP();
        return publicIp == 0U ? null : new IPAddress(Utility.SwapBytes(publicIp));
      }
    }

    public bool AutomaticHeartbeats
    {
      set => native.gameServer.EnableHeartbeats(value);
    }

    public int AutomaticHeartbeatRate
    {
      set => native.gameServer.SetHeartbeatInterval(value);
    }

    public void ForceHeartbeat() => native.gameServer.ForceHeartbeat();
  }
}
