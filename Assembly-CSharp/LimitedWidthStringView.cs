// Decompiled with JetBrains decompiler
// Type: LimitedWidthStringView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
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
