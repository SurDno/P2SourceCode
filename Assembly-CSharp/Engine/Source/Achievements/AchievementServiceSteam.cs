// Decompiled with JetBrains decompiler
// Type: Engine.Source.Achievements.AchievementServiceSteam
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Facepunch.Steamworks;
using Inspectors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Engine.Source.Achievements
{
  public class AchievementServiceSteam : IAchievementServiceImpl
  {
    [Inspected]
    public IEnumerable<string> Ids
    {
      get
      {
        return Client.Instance == null ? (IEnumerable<string>) Array.Empty<string>() : ((IEnumerable<Achievement>) Client.Instance.Achievements.All).Select<Achievement, string>((Func<Achievement, string>) (o => o.Id));
      }
    }

    public void Initialise()
    {
      uint steamAppId = AchievementUtility.GetSteamAppId();
      if (steamAppId == 0U)
      {
        Debug.LogWarning((object) "Steam AppId not found");
      }
      else
      {
        Debug.Log((object) ("Steam AppId: " + (object) steamAppId));
        Config.ForUnity(Application.platform.ToString());
        Client client = new Client(steamAppId);
        if (Client.Instance == null)
          Debug.LogWarning((object) "Error starting Steam!");
        else
          InstanceByRequest<EngineApplication>.Instance.OnApplicationQuit += new Action(this.OnApplicationQuit);
      }
    }

    public void Shutdown()
    {
      if (Client.Instance == null)
        return;
      Client.Instance.Dispose();
    }

    public void Update()
    {
      if (Client.Instance == null)
        return;
      Client.Instance.Update();
    }

    public bool IsUnlocked(string id)
    {
      if (Client.Instance == null)
        return true;
      Achievement achievement = ((IEnumerable<Achievement>) Client.Instance.Achievements.All).FirstOrDefault<Achievement>((Func<Achievement, bool>) (o => o.Id == id));
      return achievement == null || achievement.State;
    }

    public void Unlock(string id)
    {
      if (Client.Instance == null)
        return;
      Achievement achievement = ((IEnumerable<Achievement>) Client.Instance.Achievements.All).FirstOrDefault<Achievement>((Func<Achievement, bool>) (o => o.Id == id));
      if (achievement == null)
      {
        Debug.LogError((object) ("Achievement not found : " + id));
      }
      else
      {
        achievement.Trigger();
        Debug.Log((object) ("Unlock achievement : " + id));
      }
    }

    public void Reset(string id)
    {
      if (Client.Instance == null)
        return;
      Achievement achievement = ((IEnumerable<Achievement>) Client.Instance.Achievements.All).FirstOrDefault<Achievement>((Func<Achievement, bool>) (o => o.Id == id));
      if (achievement == null)
        Debug.LogError((object) ("Achievement not found : " + id));
      else
        achievement.Reset();
    }

    private void OnApplicationQuit() => this.Shutdown();
  }
}
