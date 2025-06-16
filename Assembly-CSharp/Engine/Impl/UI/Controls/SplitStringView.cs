using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SplitStringView : StringView
  {
    [SerializeField]
    private StringView[] views;

    public override void SkipAnimation()
    {
      if (views == null)
        return;
      for (int index = 0; index < views.Length; ++index)
      {
        StringView view = views[index];
        if (view != null)
          view.SkipAnimation();
      }
    }

    protected override void ApplyStringValue()
    {
      if (views == null)
        return;
      for (int index = 0; index < views.Length; ++index)
      {
        StringView view = views[index];
        if (view != null)
          view.StringValue = StringValue;
      }
    }
  }
}
