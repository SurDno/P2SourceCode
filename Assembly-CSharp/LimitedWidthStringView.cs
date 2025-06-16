using Engine.Impl.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

public class LimitedWidthStringView : StringView
{
  [SerializeField]
  private StringView view;
  [SerializeField]
  private LayoutElement layout;
  [SerializeField]
  private Text text;
  [SerializeField]
  private float maxWidth;

  public override void SkipAnimation()
  {
  }

  protected override void ApplyStringValue()
  {
    if (!((Object) this.view != (Object) null))
      return;
    this.view.StringValue = this.StringValue;
    if ((Object) this.layout != (Object) null && (Object) this.text != (Object) null)
      this.layout.preferredWidth = (double) this.text.preferredWidth > (double) this.maxWidth ? this.maxWidth : -1f;
  }
}
