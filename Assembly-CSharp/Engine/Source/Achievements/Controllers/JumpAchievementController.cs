using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using System;

namespace Engine.Source.Achievements.Controllers
{
  [AchievementController("jump")]
  public class JumpAchievementController : IAchievementController
  {
    private string id;

    public void Initialise(string id)
    {
      this.id = id;
      ServiceLocator.GetService<GameActionService>().OnGameAction += new Action<GameActionType>(this.OnGameAction);
    }

    private void OnGameAction(GameActionType action)
    {
      if (action != GameActionType.Jump)
        return;
      ServiceLocator.GetService<AchievementService>().Unlock(this.id);
    }

    public void Terminate()
    {
      ServiceLocator.GetService<GameActionService>().OnGameAction -= new Action<GameActionType>(this.OnGameAction);
    }
  }
}
