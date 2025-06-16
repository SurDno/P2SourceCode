using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class HideableDecorator : HideableView
  {
    [SerializeField]
    private HideableView nestedView = (HideableView) null;

    protected override void ApplyVisibility()
    {
      if (!((Object) this.nestedView != (Object) null))
        return;
      this.nestedView.Visible = this.Visible;
    }
  }
}
