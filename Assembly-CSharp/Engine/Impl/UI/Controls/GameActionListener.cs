using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using UnityEngine;

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
