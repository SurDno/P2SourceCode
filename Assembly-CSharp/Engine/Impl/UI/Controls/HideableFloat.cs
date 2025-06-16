using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableFloat : HideableView
  {
    [SerializeField]
    private FloatView view;

    protected override void ApplyVisibility()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.FloatValue = this.Visible ? 1f : 0.0f;
    }
  }
}
