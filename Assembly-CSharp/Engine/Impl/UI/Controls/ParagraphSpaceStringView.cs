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
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if ((Object) this.view == (Object) null)
        return;
      this.view.StringValue = this.StringValue?.Replace("\n", "\n<size=" + this.size.ToString() + ">\n</size>");
    }
  }
}
