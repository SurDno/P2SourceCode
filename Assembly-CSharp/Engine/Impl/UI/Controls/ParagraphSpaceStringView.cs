// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ParagraphSpaceStringView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
