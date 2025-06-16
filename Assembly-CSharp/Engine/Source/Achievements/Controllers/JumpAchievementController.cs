// Decompiled with JetBrains decompiler
// Type: Engine.Source.Achievements.Controllers.JumpAchievementController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using System;

#nullable disable
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
