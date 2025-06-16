using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Achievements.Controllers;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;

namespace Engine.Source.Achievements
{
  [RuntimeService(new Type[] {typeof (AchievementService), typeof (IAchievementService)})]
  public class AchievementService : IInitialisable, IUpdatable, IAchievementService
  {
    [Inspected]
    private Dictionary<string, IAchievementController> controllers = new Dictionary<string, IAchievementController>();
    [Inspected]
    private IAchievementServiceImpl service;

    public void Initialise()
    {
      this.service = this.CreateService();
      this.service.Initialise();
      foreach (KeyValuePair<string, Type> keyValuePair in (IEnumerable<KeyValuePair<string, Type>>) AchievementControllerAttribute.Factory)
      {
        string key = keyValuePair.Key;
        if (!this.service.IsUnlocked(key))
          this.CreateAddInitialise(key, keyValuePair.Value);
      }
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      foreach (KeyValuePair<string, IAchievementController> controller in this.controllers)
        controller.Value.Terminate();
      this.controllers.Clear();
      this.service.Shutdown();
      this.service = (IAchievementServiceImpl) null;
    }

    public void Unlock(string id)
    {
      if (this.service.IsUnlocked(id))
        return;
      this.service.Unlock(id);
      IAchievementController achievementController;
      if (!this.controllers.TryGetValue(id, out achievementController))
        return;
      achievementController.Terminate();
      this.controllers.Remove(id);
    }

    public void Reset(string id)
    {
      if (!this.service.IsUnlocked(id))
        return;
      this.service.Reset(id);
      Type type;
      if (!AchievementControllerAttribute.Factory.TryGetValue(id, out type))
        return;
      this.CreateAddInitialise(id, type);
    }

    private IAchievementServiceImpl CreateService()
    {
      return ScriptableObjectInstance<BuildData>.Instance.Steam && AchievementUtility.IsSteamAvailable ? (IAchievementServiceImpl) new AchievementServiceSteam() : (IAchievementServiceImpl) new AchievementServiceStub();
    }

    public void ComputeUpdate() => this.service.Update();

    [Inspected]
    public void UnlockAll()
    {
      foreach (string id in this.service.Ids)
        this.Unlock(id);
    }

    [Inspected]
    public void ResetAll()
    {
      foreach (string id in this.service.Ids)
        this.Reset(id);
    }

    private void CreateAddInitialise(string id, Type type)
    {
      IAchievementController achievementController = (IAchievementController) ServiceLocator.GetService<Factory>().Create(type, Guid.Empty);
      this.controllers[id] = achievementController;
      achievementController.Initialise(id);
    }
  }
}
