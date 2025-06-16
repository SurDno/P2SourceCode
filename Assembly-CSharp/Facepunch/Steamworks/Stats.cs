using System;

namespace Facepunch.Steamworks
{
  public class Stats : IDisposable
  {
    internal Client client;

    internal Stats(Client c) => client = c;

    public bool StoreStats() => client.native.userstats.StoreStats();

    public void UpdateStats() => client.native.userstats.RequestCurrentStats();

    public void UpdateGlobalStats(int days = 1)
    {
      client.native.userstats.GetNumberOfCurrentPlayers();
      client.native.userstats.RequestGlobalAchievementPercentages();
      client.native.userstats.RequestGlobalStats(days);
    }

    public int GetInt(string name)
    {
      int pData = 0;
      client.native.userstats.GetStat(name, out pData);
      return pData;
    }

    public long GetGlobalInt(string name)
    {
      long pData = 0;
      client.native.userstats.GetGlobalStat(name, out pData);
      return pData;
    }

    public float GetFloat(string name)
    {
      float pData = 0.0f;
      client.native.userstats.GetStat0(name, out pData);
      return pData;
    }

    public double GetGlobalFloat(string name)
    {
      double pData = 0.0;
      client.native.userstats.GetGlobalStat0(name, out pData);
      return pData;
    }

    public bool Set(string name, int value, bool store = true)
    {
      bool flag = client.native.userstats.SetStat(name, value);
      return store ? flag && client.native.userstats.StoreStats() : flag;
    }

    public bool Set(string name, float value, bool store = true)
    {
      bool flag = client.native.userstats.SetStat0(name, value);
      return store ? flag && client.native.userstats.StoreStats() : flag;
    }

    public bool Add(string name, int amount = 1, bool store = true)
    {
      int num = GetInt(name) + amount;
      return Set(name, num, store);
    }

    public bool ResetAll(bool includeAchievements)
    {
      return client.native.userstats.ResetAllStats(includeAchievements);
    }

    public void Dispose() => client = null;
  }
}
