using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SetHideableEventView : EventView
  {
    [SerializeField]
    private HideableView view;
    [SerializeField]
    private bool value;

    public override void Invoke()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.Visible = this.value;
    }
  }
}
