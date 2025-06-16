// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.ServerStats
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;

#nullable disable
namespace Facepunch.Steamworks
{
  public class ServerStats
  {
    internal Server server;

    internal ServerStats(Server s) => this.server = s;

    public void Refresh(ulong steamid, Action<ulong, bool> Callback = null)
    {
      if (Callback == null)
        this.server.native.gameServerStats.RequestUserStats((CSteamID) steamid);
      else
        this.server.native.gameServerStats.RequestUserStats((CSteamID) steamid, (Action<GSStatsReceived_t, bool>) ((o, failed) => Callback(steamid, o.Result == SteamNative.Result.OK && !failed)));
    }

    public void Commit(ulong steamid, Action<ulong, bool> Callback = null)
    {
      if (Callback == null)
        this.server.native.gameServerStats.StoreUserStats((CSteamID) steamid);
      else
        this.server.native.gameServerStats.StoreUserStats((CSteamID) steamid, (Action<GSStatsStored_t, bool>) ((o, failed) => Callback(steamid, o.Result == SteamNative.Result.OK && !failed)));
    }

    public bool SetInt(ulong steamid, string name, int stat)
    {
      return this.server.native.gameServerStats.SetUserStat((CSteamID) steamid, name, stat);
    }

    public bool SetFloat(ulong steamid, string name, float stat)
    {
      return this.server.native.gameServerStats.SetUserStat0((CSteamID) steamid, name, stat);
    }

    public int GetInt(ulong steamid, string name, int defaultValue = 0)
    {
      int pData = defaultValue;
      return !this.server.native.gameServerStats.GetUserStat((CSteamID) steamid, name, out pData) ? defaultValue : pData;
    }

    public float GetFloat(ulong steamid, string name, float defaultValue = 0.0f)
    {
      float pData = defaultValue;
      return !this.server.native.gameServerStats.GetUserStat0((CSteamID) steamid, name, out pData) ? defaultValue : pData;
    }

    public struct StatsReceived
    {
      public int Result;
      public ulong SteamId;
    }
  }
}
