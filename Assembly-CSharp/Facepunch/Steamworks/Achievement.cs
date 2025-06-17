﻿using System;

namespace Facepunch.Steamworks
{
  public class Achievement
  {
    private Client client;
    private int refreshCount;
    private Image _icon;

    public string Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public bool State { get; private set; }

    public DateTime UnlockTime { get; private set; }

    private int iconId { get; set; } = -1;

    public float GlobalUnlockedPercentage
    {
      get
      {
        if (State)
          return 1f;
        return !client.native.userstats.GetAchievementAchievedPercent(Id, out float pflPercent) ? -1f : pflPercent;
      }
    }

    public Image Icon
    {
      get
      {
        if (iconId <= 0)
          return null;
        if (_icon == null)
        {
          _icon = new Image();
          _icon.Id = iconId;
        }
        return _icon.IsLoaded || _icon.TryLoad(client.native.utils) ? _icon : null;
      }
    }

    public Achievement(Client client, int index)
    {
      this.client = client;
      Id = client.native.userstats.GetAchievementName((uint) index);
      Name = client.native.userstats.GetAchievementDisplayAttribute(Id, "name");
      Description = client.native.userstats.GetAchievementDisplayAttribute(Id, "desc");
      iconId = client.native.userstats.GetAchievementIcon(Id);
      Refresh();
    }

    public bool Trigger(bool apply = true)
    {
      if (State)
        return false;
      State = true;
      UnlockTime = DateTime.Now;
      bool flag = client.native.userstats.SetAchievement(Id);
      if (apply)
        client.Stats.StoreStats();
      client.Achievements.OnUnlocked(this);
      return flag;
    }

    public bool Reset()
    {
      State = false;
      UnlockTime = DateTime.Now;
      return client.native.userstats.ClearAchievement(Id);
    }

    public bool Refresh()
    {
      bool state = State;
      bool pbAchieved = false;
      State = false;
      if (client.native.userstats.GetAchievementAndUnlockTime(Id, ref pbAchieved, out uint punUnlockTime))
      {
        State = pbAchieved;
        UnlockTime = Utility.Epoch.ToDateTime(punUnlockTime);
      }
      ++refreshCount;
      return state != State && refreshCount > 1;
    }
  }
}
