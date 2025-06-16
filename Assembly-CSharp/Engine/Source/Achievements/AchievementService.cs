using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Achievements.Controllers;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Achievements
{
  [RuntimeService(typeof (AchievementService), typeof (IAchievementService))]
  public class AchievementService : IInitialisable, IUpdatable, IAchievementService
  {
    [Inspected]
    private Dictionary<string, IAchievementController> controllers = new Dictionary<string, IAchievementController>();
    [Inspected]
    private IAchievementServiceImpl service;

    public void Initialise()
    {
      service = CreateService();
      service.Initialise();
      foreach (KeyValuePair<string, Type> keyValuePair in AchievementControllerAttribute.Factory)
      {
        string key = keyValuePair.Key;
        if (!service.IsUnlocked(key))
          CreateAddInitialise(key, keyValuePair.Value);
      }
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
      foreach (KeyValuePair<string, IAchievementController> controller in controllers)
        controller.Value.Terminate();
      controllers.Clear();
      service.Shutdown();
      service = null;
    }

    public void Unlock(string id)
    {
      if (service.IsUnlocked(id))
        return;
      service.Unlock(id);
      IAchievementController achievementController;
      if (!controllers.TryGetValue(id, out achievementController))
        return;
      achievementController.Terminate();
      controllers.Remove(id);
    }

    public void Reset(string id)
    {
      if (!service.IsUnlocked(id))
        return;
      service.Reset(id);
      Type type;
      if (!AchievementControllerAttribute.Factory.TryGetValue(id, out type))
        return;
      CreateAddInitialise(id, type);
    }

    private IAchievementServiceImpl CreateService()
    {
      return ScriptableObjectInstance<BuildData>.Instance.Steam && AchievementUtility.IsSteamAvailable ? new AchievementServiceSteam() : new AchievementServiceStub();
    }

    public void ComputeUpdate() => service.Update();

    [Inspected]
    public void UnlockAll()
    {
      foreach (string id in service.Ids)
        Unlock(id);
    }

    [Inspected]
    public void ResetAll()
    {
      foreach (string id in service.Ids)
        Reset(id);
    }

    private void CreateAddInitialise(string id, Type type)
    {
      IAchievementController achievementController = (IAchievementController) ServiceLocator.GetService<Factory>().Create(type, Guid.Empty);
      controllers[id] = achievementController;
      achievementController.Initialise(id);
    }
  }
}
