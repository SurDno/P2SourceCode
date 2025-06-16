using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class HideableDecorator : HideableView
  {
    [SerializeField]
    private HideableView nestedView;

    protected override void ApplyVisibility()
    {
      if (!(nestedView != null))
        return;
      nestedView.Visible = Visible;
    }
  }
}
