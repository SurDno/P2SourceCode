using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Source.Commons;
using Facepunch.Steamworks;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Achievements
{
  public class AchievementServiceSteam : IAchievementServiceImpl
  {
    [Inspected]
    public IEnumerable<string> Ids
    {
      get
      {
        return Client.Instance == null ? Array.Empty<string>() : Client.Instance.Achievements.All.Select(o => o.Id);
      }
    }

    public void Initialise()
    {
      uint steamAppId = AchievementUtility.GetSteamAppId();
      if (steamAppId == 0U)
      {
        Debug.LogWarning("Steam AppId not found");
      }
      else
      {
        Debug.Log("Steam AppId: " + steamAppId);
        Config.ForUnity(Application.platform.ToString());
        Client client = new Client(steamAppId);
        if (Client.Instance == null)
          Debug.LogWarning("Error starting Steam!");
        else
          InstanceByRequest<EngineApplication>.Instance.OnApplicationQuit += OnApplicationQuit;
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
      Achievement achievement = Client.Instance.Achievements.All.FirstOrDefault(o => o.Id == id);
      return achievement == null || achievement.State;
    }

    public void Unlock(string id)
    {
      if (Client.Instance == null)
        return;
      Achievement achievement = Client.Instance.Achievements.All.FirstOrDefault(o => o.Id == id);
      if (achievement == null)
      {
        Debug.LogError("Achievement not found : " + id);
      }
      else
      {
        achievement.Trigger();
        Debug.Log("Unlock achievement : " + id);
      }
    }

    public void Reset(string id)
    {
      if (Client.Instance == null)
        return;
      Achievement achievement = Client.Instance.Achievements.All.FirstOrDefault(o => o.Id == id);
      if (achievement == null)
        Debug.LogError("Achievement not found : " + id);
      else
        achievement.Reset();
    }

    private void OnApplicationQuit() => Shutdown();
  }
}
