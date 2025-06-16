// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Stats
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Stats : IDisposable
  {
    internal Client client;

    internal Stats(Client c) => this.client = c;

    public bool StoreStats() => this.client.native.userstats.StoreStats();

    public void UpdateStats() => this.client.native.userstats.RequestCurrentStats();

    public void UpdateGlobalStats(int days = 1)
    {
      this.client.native.userstats.GetNumberOfCurrentPlayers();
      this.client.native.userstats.RequestGlobalAchievementPercentages();
      this.client.native.userstats.RequestGlobalStats(days);
    }

    public int GetInt(string name)
    {
      int pData = 0;
      this.client.native.userstats.GetStat(name, out pData);
      return pData;
    }

    public long GetGlobalInt(string name)
    {
      long pData = 0;
      this.client.native.userstats.GetGlobalStat(name, out pData);
      return pData;
    }

    public float GetFloat(string name)
    {
      float pData = 0.0f;
      this.client.native.userstats.GetStat0(name, out pData);
      return pData;
    }

    public double GetGlobalFloat(string name)
    {
      double pData = 0.0;
      this.client.native.userstats.GetGlobalStat0(name, out pData);
      return pData;
    }

    public bool Set(string name, int value, bool store = true)
    {
      bool flag = this.client.native.userstats.SetStat(name, value);
      return store ? flag && this.client.native.userstats.StoreStats() : flag;
    }

    public bool Set(string name, float value, bool store = true)
    {
      bool flag = this.client.native.userstats.SetStat0(name, value);
      return store ? flag && this.client.native.userstats.StoreStats() : flag;
    }

    public bool Add(string name, int amount = 1, bool store = true)
    {
      int num = this.GetInt(name) + amount;
      return this.Set(name, num, store);
    }

    public bool ResetAll(bool includeAchievements)
    {
      return this.client.native.userstats.ResetAllStats(includeAchievements);
    }

    public void Dispose() => this.client = (Client) null;
  }
}
