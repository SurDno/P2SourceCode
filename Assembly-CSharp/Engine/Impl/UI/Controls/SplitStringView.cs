using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SplitStringView : StringView
  {
    [SerializeField]
    private StringView[] views;

    public override void SkipAnimation()
    {
      if (this.views == null)
        return;
      for (int index = 0; index < this.views.Length; ++index)
      {
        StringView view = this.views[index];
        if ((Object) view != (Object) null)
          view.SkipAnimation();
      }
    }

    protected override void ApplyStringValue()
    {
      if (this.views == null)
        return;
      for (int index = 0; index < this.views.Length; ++index)
      {
        StringView view = this.views[index];
        if ((Object) view != (Object) null)
          view.StringValue = this.StringValue;
      }
    }
  }
}
