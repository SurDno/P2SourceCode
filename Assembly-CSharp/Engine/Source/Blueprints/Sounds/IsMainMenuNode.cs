using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu;
using Engine.Source.UI;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsMainMenuNode : FlowControlNode
  {
    [Port("Value")]
    private bool Value()
    {
      UIService service = ServiceLocator.GetService<UIService>();
      if (service == null || !service.IsInitialize)
        return false;
      UIWindow active = service.Active;
      return !((Object) active == (Object) null) && active is IMainMenu;
    }
  }
}
