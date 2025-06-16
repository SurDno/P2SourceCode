using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace Facepunch.Steamworks
{
  public class ServerList : IDisposable
  {
    internal Client client;
    private HashSet<ulong> FavouriteHash = new HashSet<ulong>();
    private HashSet<ulong> HistoryHash = new HashSet<ulong>();

    internal ServerList(Client client)
    {
      this.client = client;
      this.UpdateFavouriteList();
    }

    internal void UpdateFavouriteList()
    {
      this.FavouriteHash.Clear();
      this.HistoryHash.Clear();
      for (int iGame = 0; iGame < this.client.native.matchmaking.GetFavoriteGameCount(); ++iGame)
      {
        AppId_t pnAppID = (AppId_t) 0U;
        uint pnIP;
        ushort pnConnPort;
        uint punFlags;
        this.client.native.matchmaking.GetFavoriteGame(iGame, ref pnAppID, out pnIP, out pnConnPort, out ushort _, out punFlags, out uint _);
        ulong num = (ulong) pnIP << 32 | (ulong) pnConnPort;
        if (((int) punFlags & 1) == 1)
          this.FavouriteHash.Add(num);
        if (((int) punFlags & 1) == 1)
          this.HistoryHash.Add(num);
      }
    }

    public void Dispose() => this.client = (Client) null;

    public ServerList.Request Internet(ServerList.Filter filter = null)
    {
      if (filter == null)
      {
        filter = new ServerList.Filter();
        filter.Add("appid", this.client.AppId.ToString());
      }
      filter.Start();
      ServerList.Request request = new ServerList.Request(this.client);
      request.Filter = filter;
      request.AddRequest((IntPtr) this.client.native.servers.RequestInternetServerList((AppId_t) this.client.AppId, filter.NativeArray, (uint) filter.Count, IntPtr.Zero));
      filter.Free();
      return request;
    }

    public ServerList.Request Custom(IEnumerable<string> serverList)
    {
      ServerList.Request request = new ServerList.Request(this.client);
      request.ServerList = serverList;
      request.StartCustomQuery();
      return request;
    }

    public ServerList.Request History(ServerList.Filter filter = null)
    {
      if (filter == null)
      {
        filter = new ServerList.Filter();
        filter.Add("appid", this.client.AppId.ToString());
      }
      filter.Start();
      ServerList.Request request = new ServerList.Request(this.client);
      request.Filter = filter;
      request.AddRequest((IntPtr) this.client.native.servers.RequestHistoryServerList((AppId_t) this.client.AppId, filter.NativeArray, (uint) filter.Count, IntPtr.Zero));
      filter.Free();
      return request;
    }

    public ServerList.Request Favourites(ServerList.Filter filter = null)
    {
      if (filter == null)
      {
        filter = new ServerList.Filter();
        filter.Add("appid", this.client.AppId.ToString());
      }
      filter.Start();
      ServerList.Request request = new ServerList.Request(this.client);
      request.Filter = filter;
      request.AddRequest((IntPtr) this.client.native.servers.RequestFavoritesServerList((AppId_t) this.client.AppId, filter.NativeArray, (uint) filter.Count, IntPtr.Zero));
      filter.Free();
      return request;
    }

    public ServerList.Request Friends(ServerList.Filter filter = null)
    {
      if (filter == null)
      {
        filter = new ServerList.Filter();
        filter.Add("appid", this.client.AppId.ToString());
      }
      filter.Start();
      ServerList.Request request = new ServerList.Request(this.client);
      request.Filter = filter;
      request.AddRequest((IntPtr) this.client.native.servers.RequestFriendsServerList((AppId_t) this.client.AppId, filter.NativeArray, (uint) filter.Count, IntPtr.Zero));
      filter.Free();
      return request;
    }

    public ServerList.Request Local(ServerList.Filter filter = null)
    {
      if (filter == null)
      {
        filter = new ServerList.Filter();
        filter.Add("appid", this.client.AppId.ToString());
      }
      filter.Start();
      ServerList.Request request = new ServerList.Request(this.client);
      request.Filter = filter;
      request.AddRequest((IntPtr) this.client.native.servers.RequestLANServerList((AppId_t) this.client.AppId, IntPtr.Zero));
      filter.Free();
      return request;
    }

    internal bool IsFavourite(ServerList.Server server)
    {
      return this.FavouriteHash.Contains((ulong) server.Address.IpToInt32() << 32 | (ulong) (uint) server.ConnectionPort);
    }

    internal bool IsHistory(ServerList.Server server)
    {
      return this.HistoryHash.Contains((ulong) server.Address.IpToInt32() << 32 | (ulong) (uint) server.ConnectionPort);
    }

    public class Filter : List<KeyValuePair<string, string>>
    {
      internal IntPtr NativeArray;
      private IntPtr m_pArrayEntries;
      private int AppId = 0;

      public void Add(string k, string v) => this.Add(new KeyValuePair<string, string>(k, v));

      internal void Start()
      {
        MatchMakingKeyValuePair_t[] array = this.Select<KeyValuePair<string, string>, MatchMakingKeyValuePair_t>((Func<KeyValuePair<string, string>, MatchMakingKeyValuePair_t>) (x =>
        {
          if (x.Key == "appid")
            this.AppId = int.Parse(x.Value);
          return new MatchMakingKeyValuePair_t()
          {
            Key = x.Key,
            Value = x.Value
          };
        })).ToArray<MatchMakingKeyValuePair_t>();
        int num = Marshal.SizeOf(typeof (MatchMakingKeyValuePair_t));
        this.NativeArray = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * array.Length);
        this.m_pArrayEntries = Marshal.AllocHGlobal(num * array.Length);
        for (int index = 0; index < array.Length; ++index)
          Marshal.StructureToPtr<MatchMakingKeyValuePair_t>(array[index], new IntPtr(this.m_pArrayEntries.ToInt64() + (long) (index * num)), false);
        Marshal.WriteIntPtr(this.NativeArray, this.m_pArrayEntries);
      }

      internal void Free()
      {
        if (this.m_pArrayEntries != IntPtr.Zero)
          Marshal.FreeHGlobal(this.m_pArrayEntries);
        if (!(this.NativeArray != IntPtr.Zero))
          return;
        Marshal.FreeHGlobal(this.NativeArray);
      }

      internal bool Test(gameserveritem_t info)
      {
        return this.AppId == 0 || (long) this.AppId == (long) info.AppID;
      }
    }

    private struct MatchPair
    {
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string key;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string value;
    }

    public class Request : IDisposable
    {
      internal Client client;
      internal List<ServerList.Request.SubRequest> Requests = new List<ServerList.Request.SubRequest>();
      public Action OnUpdate;
      public Action<ServerList.Server> OnServerResponded;
      public Action OnFinished;
      public List<ServerList.Server> Responded = new List<ServerList.Server>();
      public List<ServerList.Server> Unresponsive = new List<ServerList.Server>();
      public bool Finished = false;

      internal Request(Client c)
      {
        this.client = c;
        this.client.OnUpdate += new Action(this.Update);
      }

      ~Request() => this.Dispose();

      internal IEnumerable<string> ServerList { get; set; }

      internal ServerList.Filter Filter { get; set; }

      internal void StartCustomQuery()
      {
        if (this.ServerList == null)
          return;
        int count1 = 16;
        int count2 = 0;
        while (true)
        {
          IEnumerable<string> source = this.ServerList.Skip<string>(count2).Take<string>(count1);
          if (source.Count<string>() != 0)
          {
            count2 += source.Count<string>();
            ServerList.Filter filter = new ServerList.Filter();
            filter.Add("or", source.Count<string>().ToString());
            foreach (string v in source)
              filter.Add("gameaddr", v);
            filter.Start();
            HServerListRequest id = this.client.native.servers.RequestInternetServerList((AppId_t) this.client.AppId, filter.NativeArray, (uint) filter.Count, IntPtr.Zero);
            filter.Free();
            this.AddRequest((IntPtr) id);
          }
          else
            break;
        }
        this.ServerList = (IEnumerable<string>) null;
      }

      internal void AddRequest(IntPtr id)
      {
        this.Requests.Add(new ServerList.Request.SubRequest()
        {
          Request = id
        });
      }

      private void Update()
      {
        if (this.Requests.Count == 0)
          return;
        for (int index = 0; index < this.Requests.Count<ServerList.Request.SubRequest>(); ++index)
        {
          if (this.Requests[index].Update(this.client.native.servers, new Action<gameserveritem_t>(this.OnServer), this.OnUpdate))
          {
            this.Requests.RemoveAt(index);
            --index;
          }
        }
        if (this.Requests.Count != 0)
          return;
        this.Finished = true;
        this.client.OnUpdate -= new Action(this.Update);
        Action onFinished = this.OnFinished;
        if (onFinished != null)
          onFinished();
      }

      private void OnServer(gameserveritem_t info)
      {
        if (info.HadSuccessfulResponse)
        {
          if (this.Filter != null && !this.Filter.Test(info))
            return;
          ServerList.Server server = ServerList.Server.FromSteam(this.client, info);
          this.Responded.Add(server);
          Action<ServerList.Server> onServerResponded = this.OnServerResponded;
          if (onServerResponded == null)
            return;
          onServerResponded(server);
        }
        else
          this.Unresponsive.Add(ServerList.Server.FromSteam(this.client, info));
      }

      public void Dispose()
      {
        if (this.client.IsValid)
          this.client.OnUpdate -= new Action(this.Update);
        foreach (ServerList.Request.SubRequest request in this.Requests)
        {
          if (this.client.IsValid)
            this.client.native.servers.CancelQuery((HServerListRequest) request.Request);
        }
        this.Requests.Clear();
      }

      internal class SubRequest
      {
        internal IntPtr Request;
        internal int Pointer = 0;
        internal List<int> WatchList = new List<int>();
        internal Stopwatch Timer = Stopwatch.StartNew();

        internal bool Update(
          SteamMatchmakingServers servers,
          Action<gameserveritem_t> OnServer,
          Action OnUpdate)
        {
          if (this.Request == IntPtr.Zero)
            return true;
          if (this.Timer.Elapsed.TotalSeconds < 0.5)
            return false;
          this.Timer.Reset();
          this.Timer.Start();
          bool changes = false;
          int serverCount = servers.GetServerCount((HServerListRequest) this.Request);
          if (serverCount != this.Pointer)
          {
            for (int pointer = this.Pointer; pointer < serverCount; ++pointer)
              this.WatchList.Add(pointer);
          }
          this.Pointer = serverCount;
          this.WatchList.RemoveAll((Predicate<int>) (x =>
          {
            gameserveritem_t serverDetails = servers.GetServerDetails((HServerListRequest) this.Request, x);
            if (!serverDetails.HadSuccessfulResponse)
              return false;
            OnServer(serverDetails);
            changes = true;
            return true;
          }));
          if (!servers.IsRefreshing((HServerListRequest) this.Request))
          {
            this.WatchList.RemoveAll((Predicate<int>) (x =>
            {
              OnServer(servers.GetServerDetails((HServerListRequest) this.Request, x));
              return true;
            }));
            servers.CancelQuery((HServerListRequest) this.Request);
            this.Request = IntPtr.Zero;
            changes = true;
          }
          if (changes && OnUpdate != null)
            OnUpdate();
          return this.Request == IntPtr.Zero;
        }
      }
    }

    public class Server
    {
      internal Client Client;
      public Action<bool> OnReceivedRules;
      public Dictionary<string, string> Rules;
      internal ServerRules RulesRequest;
      internal const uint k_unFavoriteFlagNone = 0;
      internal const uint k_unFavoriteFlagFavorite = 1;
      internal const uint k_unFavoriteFlagHistory = 2;

      public string Name { get; set; }

      public int Ping { get; set; }

      public string GameDir { get; set; }

      public string Map { get; set; }

      public string Description { get; set; }

      public uint AppId { get; set; }

      public int Players { get; set; }

      public int MaxPlayers { get; set; }

      public int BotPlayers { get; set; }

      public bool Passworded { get; set; }

      public bool Secure { get; set; }

      public uint LastTimePlayed { get; set; }

      public int Version { get; set; }

      public string[] Tags { get; set; }

      public ulong SteamId { get; set; }

      public IPAddress Address { get; set; }

      public int ConnectionPort { get; set; }

      public int QueryPort { get; set; }

      public bool Favourite => this.Client.ServerList.IsFavourite(this);

      internal static ServerList.Server FromSteam(Client client, gameserveritem_t item)
      {
        ServerList.Server server1 = new ServerList.Server();
        server1.Client = client;
        server1.Address = Utility.Int32ToIp(item.NetAdr.IP);
        server1.ConnectionPort = (int) item.NetAdr.ConnectionPort;
        server1.QueryPort = (int) item.NetAdr.QueryPort;
        server1.Name = item.ServerName;
        server1.Ping = item.Ping;
        server1.GameDir = item.GameDir;
        server1.Map = item.Map;
        server1.Description = item.GameDescription;
        server1.AppId = item.AppID;
        server1.Players = item.Players;
        server1.MaxPlayers = item.MaxPlayers;
        server1.BotPlayers = item.BotPlayers;
        server1.Passworded = item.Password;
        server1.Secure = item.Secure;
        server1.LastTimePlayed = item.TimeLastPlayed;
        server1.Version = item.ServerVersion;
        ServerList.Server server2 = server1;
        string[] strArray;
        if (item.GameTags != null)
          strArray = item.GameTags.Split(',');
        else
          strArray = (string[]) null;
        server2.Tags = strArray;
        server1.SteamId = item.SteamID;
        return server1;
      }

      public bool HasRules => this.Rules != null && this.Rules.Count > 0;

      public void FetchRules()
      {
        if (this.RulesRequest != null)
          return;
        this.Rules = new Dictionary<string, string>();
        this.RulesRequest = new ServerRules(this, this.Address, this.QueryPort);
      }

      internal void OnServerRulesReceiveFinished(bool Success)
      {
        this.RulesRequest.Dispose();
        this.RulesRequest = (ServerRules) null;
        if (this.OnReceivedRules == null)
          return;
        this.OnReceivedRules(Success);
      }

      public void AddToHistory()
      {
        this.Client.native.matchmaking.AddFavoriteGame((AppId_t) this.AppId, this.Address.IpToInt32(), (ushort) this.ConnectionPort, (ushort) this.QueryPort, 2U, (uint) Utility.Epoch.Current);
        this.Client.ServerList.UpdateFavouriteList();
      }

      public void RemoveFromHistory()
      {
        this.Client.native.matchmaking.RemoveFavoriteGame((AppId_t) this.AppId, this.Address.IpToInt32(), (ushort) this.ConnectionPort, (ushort) this.QueryPort, 2U);
        this.Client.ServerList.UpdateFavouriteList();
      }

      public void AddToFavourites()
      {
        this.Client.native.matchmaking.AddFavoriteGame((AppId_t) this.AppId, this.Address.IpToInt32(), (ushort) this.ConnectionPort, (ushort) this.QueryPort, 1U, (uint) Utility.Epoch.Current);
        this.Client.ServerList.UpdateFavouriteList();
      }

      public void RemoveFromFavourites()
      {
        this.Client.native.matchmaking.RemoveFavoriteGame((AppId_t) this.AppId, this.Address.IpToInt32(), (ushort) this.ConnectionPort, (ushort) this.QueryPort, 1U);
        this.Client.ServerList.UpdateFavouriteList();
      }
    }
  }
}
