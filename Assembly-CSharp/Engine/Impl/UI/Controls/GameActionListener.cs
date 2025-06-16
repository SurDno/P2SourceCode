// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.GameActionListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class GameActionListener : MonoBehaviour
  {
    [SerializeField]
    private EventView view;
    [SerializeField]
    private GameActionType action;

    private void OnDisable()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(this.action, new GameActionHandle(this.OnGameAction));
    }

    private void OnEnable()
    {
      ServiceLocator.GetService<GameActionService>().AddListener(this.action, new GameActionHandle(this.OnGameAction));
    }

    private bool OnGameAction(GameActionType type, bool down)
    {
      if (!down || (Object) this.view == (Object) null)
        return false;
      this.view.Invoke();
      return true;
    }
  }
}
