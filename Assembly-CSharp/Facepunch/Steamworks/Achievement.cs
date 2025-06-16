// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Achievement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Achievement
  {
    private Client client;
    private int refreshCount = 0;
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
        if (this.State)
          return 1f;
        float pflPercent = 0.0f;
        return !this.client.native.userstats.GetAchievementAchievedPercent(this.Id, out pflPercent) ? -1f : pflPercent;
      }
    }

    public Image Icon
    {
      get
      {
        if (this.iconId <= 0)
          return (Image) null;
        if (this._icon == null)
        {
          this._icon = new Image();
          this._icon.Id = this.iconId;
        }
        return this._icon.IsLoaded || this._icon.TryLoad(this.client.native.utils) ? this._icon : (Image) null;
      }
    }

    public Achievement(Client client, int index)
    {
      this.client = client;
      this.Id = client.native.userstats.GetAchievementName((uint) index);
      this.Name = client.native.userstats.GetAchievementDisplayAttribute(this.Id, "name");
      this.Description = client.native.userstats.GetAchievementDisplayAttribute(this.Id, "desc");
      this.iconId = client.native.userstats.GetAchievementIcon(this.Id);
      this.Refresh();
    }

    public bool Trigger(bool apply = true)
    {
      if (this.State)
        return false;
      this.State = true;
      this.UnlockTime = DateTime.Now;
      bool flag = this.client.native.userstats.SetAchievement(this.Id);
      if (apply)
        this.client.Stats.StoreStats();
      this.client.Achievements.OnUnlocked(this);
      return flag;
    }

    public bool Reset()
    {
      this.State = false;
      this.UnlockTime = DateTime.Now;
      return this.client.native.userstats.ClearAchievement(this.Id);
    }

    public bool Refresh()
    {
      bool state = this.State;
      bool pbAchieved = false;
      this.State = false;
      uint punUnlockTime;
      if (this.client.native.userstats.GetAchievementAndUnlockTime(this.Id, ref pbAchieved, out punUnlockTime))
      {
        this.State = pbAchieved;
        this.UnlockTime = Utility.Epoch.ToDateTime((Decimal) punUnlockTime);
      }
      ++this.refreshCount;
      return state != this.State && this.refreshCount > 1;
    }
  }
}
