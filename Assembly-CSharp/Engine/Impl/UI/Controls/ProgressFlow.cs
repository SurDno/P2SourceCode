// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressFlow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  public class ProgressFlow : ProgressView
  {
    [SerializeField]
    [FormerlySerializedAs("Invert")]
    private bool invert = false;
    [SerializeField]
    [FormerlySerializedAs("Overlap")]
    private float overlap = 1f;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      Image component = this.GetComponent<Image>();
      if ((Object) component == (Object) null)
        return;
      float num1 = Mathf.Clamp01((float) ((1.0 - (double) this.Progress) * (1.0 + (double) this.overlap)));
      float num2 = Mathf.Clamp01(this.Progress * (1f + this.overlap));
      Color color = component.color;
      if (this.invert)
      {
        color.r = num2;
        color.g = num1;
      }
      else
      {
        color.r = num1;
        color.g = num2;
      }
      component.color = color;
    }
  }
}
