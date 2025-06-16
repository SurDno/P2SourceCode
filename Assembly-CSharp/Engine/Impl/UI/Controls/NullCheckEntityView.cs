using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class NullCheckEntityView : EntityViewBase
  {
    [SerializeField]
    private HideableView view;

    protected override void ApplyValue()
    {
      if (!(view != null))
        return;
      view.Visible = Value == null;
    }

    public override void SkipAnimation()
    {
      if (!(view != null))
        return;
      view.SkipAnimation();
    }
  }
}
