// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.HideableColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class HideableColor : HideableView, IValueView<Color>
  {
    [SerializeField]
    private ColorViewHandle view;
    [SerializeField]
    private Color falseColor;
    [SerializeField]
    private Color trueColor;

    public override void SkipAnimation() => this.ApplyVisibility(true);

    protected override void ApplyVisibility() => this.ApplyVisibility(false);

    private void ApplyVisibility(bool instant)
    {
      this.view.SetValue(this.Visible ? this.trueColor : this.falseColor, instant);
    }

    Color IValueView<Color>.GetValue(int id) => id <= 0 ? this.falseColor : this.trueColor;

    void IValueView<Color>.SetValue(int id, Color value, bool instant)
    {
      if (id <= 0)
      {
        if (!instant && this.falseColor == value)
          return;
        this.falseColor = value;
        this.ApplyVisibility(instant);
      }
      else
      {
        if (!instant && this.trueColor == value)
          return;
        this.trueColor = value;
        this.ApplyVisibility(instant);
      }
    }
  }
}
