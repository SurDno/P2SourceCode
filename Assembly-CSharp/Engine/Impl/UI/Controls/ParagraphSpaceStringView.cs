using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ParagraphSpaceStringView : StringView
  {
    [SerializeField]
    private StringView view;
    [SerializeField]
    private int size;

    public override void SkipAnimation()
    {
      if (!(view != null))
        return;
      view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if (view == null)
        return;
      view.StringValue = StringValue?.Replace("\n", "\n<size=" + size + ">\n</size>");
    }
  }
}
