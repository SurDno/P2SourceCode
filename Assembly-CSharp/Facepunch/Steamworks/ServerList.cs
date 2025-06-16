using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Facepunch.Steamworks.Interop;
using SteamNative;

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
      UpdateFavouriteList();
    }

    internal void UpdateFavouriteList()
    {
      FavouriteHash.Clear();
      HistoryHash.Clear();
      for (int iGame = 0; iGame < client.native.matchmaking.GetFavoriteGameCount(); ++iGame)
      {
        AppId_t pnAppID = 0U;
        uint pnIP;
        ushort pnConnPort;
        uint punFlags;
        client.native.matchmaking.GetFavoriteGame(iGame, ref pnAppID, out pnIP, out pnConnPort, out ushort _, out punFlags, out uint _);
        ulong num = (ulong) pnIP << 32 | pnConnPort;
        if (((int) punFlags & 1) == 1)
          FavouriteHash.Add(num);
        if (((int) punFlags & 1) == 1)
          HistoryHash.Add(num);
      }
    }

    public void Dispose() => client = null;

    public Request Internet(Filter filter = null)
    {
      if (filter == null)
      {
        filter = new Filter();
        filter.Add("appid", client.AppId.ToString());
      }
      filter.Start();
      Request request = new Request(client);
      request.Filter = filter;
      request.AddRequest(client.native.servers.RequestInternetServerList(client.AppId, filter.NativeArray, (uint) filter.Count, IntPtr.Zero));
      filter.Free();
      return request;
    }

    public Request Custom(IEnumerable<string> serverList)
    {
      Request request = new Request(client);
      request.ServerList = serverList;
      request.StartCustomQuery();
      return request;
    }

    public Request History(Filter filter = null)
    {
      if (filter == null)
      {
        filter = new Filter();
        filter.Add("appid", client.AppId.ToString());
      }
      filter.Start();
      Request request = new Request(client);
      request.Filter = filter;
      request.AddRequest(client.native.servers.RequestHistoryServerList(client.AppId, filter.NativeArray, (uint) filter.Count, IntPtr.Zero));
      filter.Free();
      return request;
    }

    public Request Favourites(Filter filter = null)
    {
      if (filter == null)
      {
        filter = new Filter();
        filter.Add("appid", client.AppId.ToString());
      }
      filter.Start();
      Request request = new Request(client);
      request.Filter = filter;
      request.AddRequest(client.native.servers.RequestFavoritesServerList(client.AppId, filter.NativeArray, (uint) filter.Count, IntPtr.Zero));
      filter.Free();
      return request;
    }

    public Request Friends(Filter filter = null)
    {
      if (filter == null)
      {
        filter = new Filter();
        filter.Add("appid", client.AppId.ToString());
      }
      filter.Start();
      Request request = new Request(client);
      request.Filter = filter;
      request.AddRequest(client.native.servers.RequestFriendsServerList(client.AppId, filter.NativeArray, (uint) filter.Count, IntPtr.Zero));
      filter.Free();
      return request;
    }

    public Request Local(Filter filter = null)
    {
      if (filter == null)
      {
        filter = new Filter();
        filter.Add("appid", client.AppId.ToString());
      }
      filter.Start();
      Request request = new Request(client);
      request.Filter = filter;
      request.AddRequest(client.native.servers.RequestLANServerList(client.AppId, IntPtr.Zero));
      filter.Free();
      return request;
    }

    internal bool IsFavourite(Server server)
    {
      return FavouriteHash.Contains((ulong) server.Address.IpToInt32() << 32 | (uint) server.ConnectionPort);
    }

    internal bool IsHistory(Server server)
    {
      return HistoryHash.Contains((ulong) server.Address.IpToInt32() << 32 | (uint) server.ConnectionPort);
    }

    public class Filter : List<KeyValuePair<string, string>>
    {
      internal IntPtr NativeArray;
      private IntPtr m_pArrayEntries;
      private int AppId;

      public void Add(string k, string v) => Add(new KeyValuePair<string, string>(k, v));

      internal void Start()
      {
        MatchMakingKeyValuePair_t[] array = this.Select(x =>
        {
          if (x.Key == "appid")
            AppId = int.Parse(x.Value);
          return new MatchMakingKeyValuePair_t {
            Key = x.Key,
            Value = x.Value
          };
        }).ToArray();
        int num = Marshal.SizeOf(typeof (MatchMakingKeyValuePair_t));
        NativeArray = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (IntPtr)) * array.Length);
        m_pArrayEntries = Marshal.AllocHGlobal(num * array.Length);
        for (int index = 0; index < array.Length; ++index)
          Marshal.StructureToPtr(array[index], new IntPtr(m_pArrayEntries.ToInt64() + index * num), false);
        Marshal.WriteIntPtr(NativeArray, m_pArrayEntries);
      }

      internal void Free()
      {
        if (m_pArrayEntries != IntPtr.Zero)
          Marshal.FreeHGlobal(m_pArrayEntries);
        if (!(NativeArray != IntPtr.Zero))
          return;
        Marshal.FreeHGlobal(NativeArray);
      }

      internal bool Test(gameserveritem_t info)
      {
        return AppId == 0 || AppId == info.AppID;
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
      internal List<SubRequest> Requests = new List<SubRequest>();
      public Action OnUpdate;
      public Action<Server> OnServerResponded;
      public Action OnFinished;
      public List<Server> Responded = new List<Server>();
      public List<Server> Unresponsive = new List<Server>();
      public bool Finished;

      internal Request(Client c)
      {
        client = c;
        client.OnUpdate += Update;
      }

      ~Request() => Dispose();

      internal IEnumerable<string> ServerList { get; set; }

      internal Filter Filter { get; set; }

      internal void StartCustomQuery()
      {
        if (ServerList == null)
          return;
        int count1 = 16;
        int count2 = 0;
        while (true)
        {
          IEnumerable<string> source = ServerList.Skip(count2).Take(count1);
          if (source.Count() != 0)
          {
            count2 += source.Count();
            Filter filter = new Filter();
            filter.Add("or", source.Count().ToString());
            foreach (string v in source)
              filter.Add("gameaddr", v);
            filter.Start();
            HServerListRequest id = client.native.servers.RequestInternetServerList(client.AppId, filter.NativeArray, (uint) filter.Count, IntPtr.Zero);
            filter.Free();
            AddRequest(id);
          }
          else
            break;
        }
        ServerList = null;
      }

      internal void AddRequest(IntPtr id)
      {
        Requests.Add(new SubRequest {
          Request = id
        });
      }

      private void Update()
      {
        if (Requests.Count == 0)
          return;
        for (int index = 0; index < Requests.Count(); ++index)
        {
          if (Requests[index].Update(client.native.servers, OnServer, OnUpdate))
          {
            Requests.RemoveAt(index);
            --index;
          }
        }
        if (Requests.Count != 0)
          return;
        Finished = true;
        client.OnUpdate -= Update;
        Action onFinished = OnFinished;
        if (onFinished != null)
          onFinished();
      }

      private void OnServer(gameserveritem_t info)
      {
        if (info.HadSuccessfulResponse)
        {
          if (Filter != null && !Filter.Test(info))
            return;
          Server server = ServerList.Server.FromSteam(client, info);
          Responded.Add(server);
          Action<Server> onServerResponded = OnServerResponded;
          if (onServerResponded == null)
            return;
          onServerResponded(server);
        }
        else
          Unresponsive.Add(ServerList.Server.FromSteam(client, info));
      }

      public void Dispose()
      {
        if (client.IsValid)
          client.OnUpdate -= Update;
        foreach (SubRequest request in Requests)
        {
          if (client.IsValid)
            client.native.servers.CancelQuery(request.Request);
        }
        Requests.Clear();
      }

      internal class SubRequest
      {
        internal IntPtr Request;
        internal int Pointer;
        internal List<int> WatchList = new List<int>();
        internal Stopwatch Timer = Stopwatch.StartNew();

        internal bool Update(
          SteamMatchmakingServers servers,
          Action<gameserveritem_t> OnServer,
          Action OnUpdate)
        {
          if (Request == IntPtr.Zero)
            return true;
          if (Timer.Elapsed.TotalSeconds < 0.5)
            return false;
          Timer.Reset();
          Timer.Start();
          bool changes = false;
          int serverCount = servers.GetServerCount(Request);
          if (serverCount != Pointer)
          {
            for (int pointer = Pointer; pointer < serverCount; ++pointer)
              WatchList.Add(pointer);
          }
          Pointer = serverCount;
          WatchList.RemoveAll(x =>
          {
            gameserveritem_t serverDetails = servers.GetServerDetails(Request, x);
            if (!serverDetails.HadSuccessfulResponse)
              return false;
            OnServer(serverDetails);
            changes = true;
            return true;
          });
          if (!servers.IsRefreshing(Request))
          {
            WatchList.RemoveAll(x =>
            {
              OnServer(servers.GetServerDetails(Request, x));
              return true;
            });
            servers.CancelQuery(Request);
            Request = IntPtr.Zero;
            changes = true;
          }
          if (changes && OnUpdate != null)
            OnUpdate();
          return Request == IntPtr.Zero;
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

      public bool Favourite => Client.ServerList.IsFavourite(this);

      internal static Server FromSteam(Client client, gameserveritem_t item)
      {
        Server server1 = new Server();
        server1.Client = client;
        server1.Address = Utility.Int32ToIp(item.NetAdr.IP);
        server1.ConnectionPort = item.NetAdr.ConnectionPort;
        server1.QueryPort = item.NetAdr.QueryPort;
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
        Server server2 = server1;
        string[] strArray;
        if (item.GameTags != null)
          strArray = item.GameTags.Split(',');
        else
          strArray = null;
        server2.Tags = strArray;
        server1.SteamId = item.SteamID;
        return server1;
      }

      public bool HasRules => Rules != null && Rules.Count > 0;

      public void FetchRules()
      {
        if (RulesRequest != null)
          return;
        Rules = new Dictionary<string, string>();
        RulesRequest = new ServerRules(this, Address, QueryPort);
      }

      internal void OnServerRulesReceiveFinished(bool Success)
      {
        RulesRequest.Dispose();
        RulesRequest = null;
        if (OnReceivedRules == null)
          return;
        OnReceivedRules(Success);
      }

      public void AddToHistory()
      {
        Client.native.matchmaking.AddFavoriteGame(AppId, Address.IpToInt32(), (ushort) ConnectionPort, (ushort) QueryPort, 2U, (uint) Utility.Epoch.Current);
        Client.ServerList.UpdateFavouriteList();
      }

      public void RemoveFromHistory()
      {
        Client.native.matchmaking.RemoveFavoriteGame(AppId, Address.IpToInt32(), (ushort) ConnectionPort, (ushort) QueryPort, 2U);
        Client.ServerList.UpdateFavouriteList();
      }

      public void AddToFavourites()
      {
        Client.native.matchmaking.AddFavoriteGame(AppId, Address.IpToInt32(), (ushort) ConnectionPort, (ushort) QueryPort, 1U, (uint) Utility.Epoch.Current);
        Client.ServerList.UpdateFavouriteList();
      }

      public void RemoveFromFavourites()
      {
        Client.native.matchmaking.RemoveFavoriteGame(AppId, Address.IpToInt32(), (ushort) ConnectionPort, (ushort) QueryPort, 1U);
        Client.ServerList.UpdateFavouriteList();
      }
    }
  }
}
