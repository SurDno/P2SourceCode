using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class NullCheckEntityView : EntityViewBase
  {
    [SerializeField]
    private HideableView view;

    protected override void ApplyValue()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.Visible = this.Value == null;
    }

    public override void SkipAnimation()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }
  }
}
