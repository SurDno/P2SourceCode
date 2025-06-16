// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.LobbyList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;
using System.Collections.Generic;

#nullable disable
namespace Facepunch.Steamworks
{
  public class LobbyList : IDisposable
  {
    internal Client client;
    internal List<ulong> requests;
    public Action OnLobbiesUpdated;

    public List<LobbyList.Lobby> Lobbies { get; private set; }

    public bool Finished { get; private set; }

    internal LobbyList(Client client)
    {
      this.client = client;
      this.Lobbies = new List<LobbyList.Lobby>();
      this.requests = new List<ulong>();
    }

    public void Refresh(LobbyList.Filter filter = null)
    {
      this.Lobbies.Clear();
      this.requests.Clear();
      this.Finished = false;
      if (filter == null)
      {
        filter = new LobbyList.Filter();
        filter.StringFilters.Add("appid", this.client.AppId.ToString());
      }
      this.client.native.matchmaking.AddRequestLobbyListDistanceFilter((LobbyDistanceFilter) filter.DistanceFilter);
      int? nullable = filter.SlotsAvailable;
      if (nullable.HasValue)
      {
        SteamMatchmaking matchmaking = this.client.native.matchmaking;
        nullable = filter.SlotsAvailable;
        int nSlotsAvailable = nullable.Value;
        matchmaking.AddRequestLobbyListFilterSlotsAvailable(nSlotsAvailable);
      }
      nullable = filter.MaxResults;
      if (nullable.HasValue)
      {
        SteamMatchmaking matchmaking = this.client.native.matchmaking;
        nullable = filter.MaxResults;
        int cMaxResults = nullable.Value;
        matchmaking.AddRequestLobbyListResultCountFilter(cMaxResults);
      }
      foreach (KeyValuePair<string, string> stringFilter in filter.StringFilters)
        this.client.native.matchmaking.AddRequestLobbyListStringFilter(stringFilter.Key, stringFilter.Value, LobbyComparison.Equal);
      foreach (KeyValuePair<string, int> nearFilter in filter.NearFilters)
        this.client.native.matchmaking.AddRequestLobbyListNearValueFilter(nearFilter.Key, nearFilter.Value);
      this.client.native.matchmaking.RequestLobbyList(new Action<LobbyMatchList_t, bool>(this.OnLobbyList));
    }

    private void OnLobbyList(LobbyMatchList_t callback, bool error)
    {
      if (error)
        return;
      uint lobbiesMatching = callback.LobbiesMatching;
      for (int iLobby = 0; (long) iLobby < (long) lobbiesMatching; ++iLobby)
      {
        ulong lobbyByIndex = this.client.native.matchmaking.GetLobbyByIndex(iLobby);
        this.requests.Add(lobbyByIndex);
        LobbyList.Lobby lobby = LobbyList.Lobby.FromSteam(this.client, lobbyByIndex);
        if (lobby.Name != "")
        {
          this.Lobbies.Add(lobby);
          this.checkFinished();
        }
        else
        {
          this.client.native.matchmaking.RequestLobbyData((CSteamID) lobbyByIndex);
          LobbyDataUpdate_t.RegisterCallback((BaseSteamworks) this.client, new Action<LobbyDataUpdate_t, bool>(this.OnLobbyDataUpdated));
        }
      }
      this.checkFinished();
      if (this.OnLobbiesUpdated == null)
        return;
      this.OnLobbiesUpdated();
    }

    private void checkFinished()
    {
      if (this.Lobbies.Count == this.requests.Count)
        this.Finished = true;
      else
        this.Finished = false;
    }

    private void OnLobbyDataUpdated(LobbyDataUpdate_t callback, bool error)
    {
      if (callback.Success != (byte) 1)
        return;
      LobbyList.Lobby lobby = this.Lobbies.Find((Predicate<LobbyList.Lobby>) (x => (long) x.LobbyID == (long) callback.SteamIDLobby));
      if (lobby == null)
      {
        this.Lobbies.Add(lobby);
        this.checkFinished();
      }
      if (this.OnLobbiesUpdated != null)
        this.OnLobbiesUpdated();
    }

    public void Dispose() => this.client = (Client) null;

    public class Filter
    {
      public Dictionary<string, string> StringFilters = new Dictionary<string, string>();
      public Dictionary<string, int> NearFilters = new Dictionary<string, int>();
      public LobbyList.Filter.Distance DistanceFilter = LobbyList.Filter.Distance.Worldwide;

      public int? SlotsAvailable { get; set; }

      public int? MaxResults { get; set; }

      public enum Distance
      {
        Close,
        Default,
        Far,
        Worldwide,
      }

      public enum Comparison
      {
        EqualToOrLessThan = -2, // 0xFFFFFFFE
        LessThan = -1, // 0xFFFFFFFF
        Equal = 0,
        GreaterThan = 1,
        EqualToOrGreaterThan = 2,
        NotEqual = 3,
      }
    }

    public class Lobby
    {
      internal Client Client;

      public string Name { get; private set; }

      public ulong LobbyID { get; private set; }

      public ulong Owner { get; private set; }

      public int MemberLimit { get; private set; }

      public int NumMembers { get; private set; }

      internal static LobbyList.Lobby FromSteam(Client client, ulong lobby)
      {
        return new LobbyList.Lobby()
        {
          Client = client,
          LobbyID = lobby,
          Name = client.native.matchmaking.GetLobbyData((CSteamID) lobby, "name"),
          MemberLimit = client.native.matchmaking.GetLobbyMemberLimit((CSteamID) lobby),
          Owner = client.native.matchmaking.GetLobbyOwner((CSteamID) lobby),
          NumMembers = client.native.matchmaking.GetNumLobbyMembers((CSteamID) lobby)
        };
      }
    }
  }
}
