using System;
using System.Collections.Generic;
using System.Linq;
using SteamNative;

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
      client = c;
      All = new Achievement[0];
      UserStatsReceived_t.RegisterCallback(c, UserStatsReceived);
      UserStatsStored_t.RegisterCallback(c, UserStatsStored);
      Refresh();
    }

    public void Refresh()
    {
      Achievement[] old = All;
      All = Enumerable.Range(0, (int) client.native.userstats.GetNumAchievements()).Select(x =>
      {
        if (old != null)
        {
          string name = client.native.userstats.GetAchievementName((uint) x);
          Achievement achievement = old.FirstOrDefault(y => y.Id == name);
          if (achievement != null)
          {
            if (achievement.Refresh())
              unlockedRecently.Add(achievement);
            return achievement;
          }
        }
        return new Achievement(client, x);
      }).ToArray();
      foreach (Achievement a in unlockedRecently)
        OnUnlocked(a);
      unlockedRecently.Clear();
    }

    internal void OnUnlocked(Achievement a)
    {
      Action<Achievement> achievementStateChanged = OnAchievementStateChanged;
      if (achievementStateChanged == null)
        return;
      achievementStateChanged(a);
    }

    public void Dispose() => client = null;

    public Achievement Find(string identifier)
    {
      return All.FirstOrDefault(x => x.Id == identifier);
    }

    public bool Trigger(string identifier, bool apply = true)
    {
      Achievement achievement = Find(identifier);
      return achievement != null && achievement.Trigger(apply);
    }

    public bool Reset(string identifier)
    {
      return client.native.userstats.ClearAchievement(identifier);
    }

    private void UserStatsReceived(UserStatsReceived_t stats, bool isError)
    {
      if (isError || (long) stats.GameID != client.AppId)
        return;
      Refresh();
      Action onUpdated = OnUpdated;
      if (onUpdated == null)
        return;
      onUpdated();
    }

    private void UserStatsStored(UserStatsStored_t stats, bool isError)
    {
      if (isError || (long) stats.GameID != client.AppId)
        return;
      Refresh();
      Action onUpdated = OnUpdated;
      if (onUpdated == null)
        return;
      onUpdated();
    }
  }
}
