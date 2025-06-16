using System;
using SteamNative;

namespace Facepunch.Steamworks;

public class ServerStats {
	internal Server server;

	internal ServerStats(Server s) {
		server = s;
	}

	public void Refresh(ulong steamid, Action<ulong, bool> Callback = null) {
		if (Callback == null)
			server.native.gameServerStats.RequestUserStats(steamid);
		else
			server.native.gameServerStats.RequestUserStats(steamid,
				(o, failed) => Callback(steamid, o.Result == Result.OK && !failed));
	}

	public void Commit(ulong steamid, Action<ulong, bool> Callback = null) {
		if (Callback == null)
			server.native.gameServerStats.StoreUserStats(steamid);
		else
			server.native.gameServerStats.StoreUserStats(steamid,
				(o, failed) => Callback(steamid, o.Result == Result.OK && !failed));
	}

	public bool SetInt(ulong steamid, string name, int stat) {
		return server.native.gameServerStats.SetUserStat(steamid, name, stat);
	}

	public bool SetFloat(ulong steamid, string name, float stat) {
		return server.native.gameServerStats.SetUserStat0(steamid, name, stat);
	}

	public int GetInt(ulong steamid, string name, int defaultValue = 0) {
		var pData = defaultValue;
		return !server.native.gameServerStats.GetUserStat(steamid, name, out pData) ? defaultValue : pData;
	}

	public float GetFloat(ulong steamid, string name, float defaultValue = 0.0f) {
		var pData = defaultValue;
		return !server.native.gameServerStats.GetUserStat0(steamid, name, out pData) ? defaultValue : pData;
	}

	public struct StatsReceived {
		public int Result;
		public ulong SteamId;
	}
}