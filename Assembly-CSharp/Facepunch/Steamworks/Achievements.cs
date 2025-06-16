// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Achievements
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Achievements : IDisposable
  {
    internal Client client;
    private List<Achievement> unlockedRecently = new List<Achievement>();

    public Achievement[] All { get; private set; }

    public event Action OnUpdated;

    public event Action<Achievement> OnAchievementStateChanged;

    internal Achievements(Client c)
    {
      this.client = c;
      this.All = new Achievement[0];
      UserStatsReceived_t.RegisterCallback((BaseSteamworks) c, new Action<UserStatsReceived_t, bool>(this.UserStatsReceived));
      UserStatsStored_t.RegisterCallback((BaseSteamworks) c, new Action<UserStatsStored_t, bool>(this.UserStatsStored));
      this.Refresh();
    }

    public void Refresh()
    {
      Achievement[] old = this.All;
      this.All = Enumerable.Range(0, (int) this.client.native.userstats.GetNumAchievements()).Select<int, Achievement>((Func<int, Achievement>) (x =>
      {
        if (old != null)
        {
          string name = this.client.native.userstats.GetAchievementName((uint) x);
          Achievement achievement = ((IEnumerable<Achievement>) old).FirstOrDefault<Achievement>((Func<Achievement, bool>) (y => y.Id == name));
          if (achievement != null)
          {
            if (achievement.Refresh())
              this.unlockedRecently.Add(achievement);
            return achievement;
          }
        }
        return new Achievement(this.client, x);
      })).ToArray<Achievement>();
      foreach (Achievement a in this.unlockedRecently)
        this.OnUnlocked(a);
      this.unlockedRecently.Clear();
    }

    internal void OnUnlocked(Achievement a)
    {
      Action<Achievement> achievementStateChanged = this.OnAchievementStateChanged;
      if (achievementStateChanged == null)
        return;
      achievementStateChanged(a);
    }

    public void Dispose() => this.client = (Client) null;

    public Achievement Find(string identifier)
    {
      return ((IEnumerable<Achievement>) this.All).FirstOrDefault<Achievement>((Func<Achievement, bool>) (x => x.Id == identifier));
    }

    public bool Trigger(string identifier, bool apply = true)
    {
      Achievement achievement = this.Find(identifier);
      return achievement != null && achievement.Trigger(apply);
    }

    public bool Reset(string identifier)
    {
      return this.client.native.userstats.ClearAchievement(identifier);
    }

    private void UserStatsReceived(UserStatsReceived_t stats, bool isError)
    {
      if (isError || (long) stats.GameID != (long) this.client.AppId)
        return;
      this.Refresh();
      Action onUpdated = this.OnUpdated;
      if (onUpdated == null)
        return;
      onUpdated();
    }

    private void UserStatsStored(UserStatsStored_t stats, bool isError)
    {
      if (isError || (long) stats.GameID != (long) this.client.AppId)
        return;
      this.Refresh();
      Action onUpdated = this.OnUpdated;
      if (onUpdated == null)
        return;
      onUpdated();
    }
  }
}
