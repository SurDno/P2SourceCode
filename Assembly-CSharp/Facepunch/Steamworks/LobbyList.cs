using System;
using System.Collections.Generic;
using SteamNative;

namespace Facepunch.Steamworks;

public class LobbyList : IDisposable {
	internal Client client;
	internal List<ulong> requests;
	public Action OnLobbiesUpdated;

	public List<Lobby> Lobbies { get; private set; }

	public bool Finished { get; private set; }

	internal LobbyList(Client client) {
		this.client = client;
		Lobbies = new List<Lobby>();
		requests = new List<ulong>();
	}

	public void Refresh(Filter filter = null) {
		Lobbies.Clear();
		requests.Clear();
		Finished = false;
		if (filter == null) {
			filter = new Filter();
			filter.StringFilters.Add("appid", client.AppId.ToString());
		}

		client.native.matchmaking.AddRequestLobbyListDistanceFilter((LobbyDistanceFilter)filter.DistanceFilter);
		var nullable = filter.SlotsAvailable;
		if (nullable.HasValue) {
			var matchmaking = client.native.matchmaking;
			nullable = filter.SlotsAvailable;
			var nSlotsAvailable = nullable.Value;
			matchmaking.AddRequestLobbyListFilterSlotsAvailable(nSlotsAvailable);
		}

		nullable = filter.MaxResults;
		if (nullable.HasValue) {
			var matchmaking = client.native.matchmaking;
			nullable = filter.MaxResults;
			var cMaxResults = nullable.Value;
			matchmaking.AddRequestLobbyListResultCountFilter(cMaxResults);
		}

		foreach (var stringFilter in filter.StringFilters)
			client.native.matchmaking.AddRequestLobbyListStringFilter(stringFilter.Key, stringFilter.Value,
				LobbyComparison.Equal);
		foreach (var nearFilter in filter.NearFilters)
			client.native.matchmaking.AddRequestLobbyListNearValueFilter(nearFilter.Key, nearFilter.Value);
		client.native.matchmaking.RequestLobbyList(OnLobbyList);
	}

	private void OnLobbyList(LobbyMatchList_t callback, bool error) {
		if (error)
			return;
		var lobbiesMatching = callback.LobbiesMatching;
		for (var iLobby = 0; iLobby < lobbiesMatching; ++iLobby) {
			var lobbyByIndex = client.native.matchmaking.GetLobbyByIndex(iLobby);
			requests.Add(lobbyByIndex);
			var lobby = Lobby.FromSteam(client, lobbyByIndex);
			if (lobby.Name != "") {
				Lobbies.Add(lobby);
				checkFinished();
			} else {
				client.native.matchmaking.RequestLobbyData(lobbyByIndex);
				LobbyDataUpdate_t.RegisterCallback(client, OnLobbyDataUpdated);
			}
		}

		checkFinished();
		if (OnLobbiesUpdated == null)
			return;
		OnLobbiesUpdated();
	}

	private void checkFinished() {
		if (Lobbies.Count == requests.Count)
			Finished = true;
		else
			Finished = false;
	}

	private void OnLobbyDataUpdated(LobbyDataUpdate_t callback, bool error) {
		if (callback.Success != 1)
			return;
		var lobby = Lobbies.Find(x => (long)x.LobbyID == (long)callback.SteamIDLobby);
		if (lobby == null) {
			Lobbies.Add(lobby);
			checkFinished();
		}

		if (OnLobbiesUpdated != null)
			OnLobbiesUpdated();
	}

	public void Dispose() {
		client = null;
	}

	public class Filter {
		public Dictionary<string, string> StringFilters = new();
		public Dictionary<string, int> NearFilters = new();
		public Distance DistanceFilter = Distance.Worldwide;

		public int? SlotsAvailable { get; set; }

		public int? MaxResults { get; set; }

		public enum Distance {
			Close,
			Default,
			Far,
			Worldwide
		}

		public enum Comparison {
			EqualToOrLessThan = -2,
			LessThan = -1,
			Equal = 0,
			GreaterThan = 1,
			EqualToOrGreaterThan = 2,
			NotEqual = 3
		}
	}

	public class Lobby {
		internal Client Client;

		public string Name { get; private set; }

		public ulong LobbyID { get; private set; }

		public ulong Owner { get; private set; }

		public int MemberLimit { get; private set; }

		public int NumMembers { get; private set; }

		internal static Lobby FromSteam(Client client, ulong lobby) {
			return new Lobby {
				Client = client,
				LobbyID = lobby,
				Name = client.native.matchmaking.GetLobbyData(lobby, "name"),
				MemberLimit = client.native.matchmaking.GetLobbyMemberLimit(lobby),
				Owner = client.native.matchmaking.GetLobbyOwner(lobby),
				NumMembers = client.native.matchmaking.GetNumLobbyMembers(lobby)
			};
		}
	}
}