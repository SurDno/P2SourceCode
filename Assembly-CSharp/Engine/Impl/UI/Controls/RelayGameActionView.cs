using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class RelayGameActionView : GameActionViewBase
  {
    [SerializeField]
    private GameActionView view;

    protected override void ApplyValue(bool instant)
    {
      this.view?.SetValue(this.GetValue(), instant);
    }
  }
}
