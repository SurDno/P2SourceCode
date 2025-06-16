// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressBar
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
  public class ProgressBar : ProgressViewBase
  {
    [SerializeField]
    [FormerlySerializedAs("_Background")]
    private RawImage background = (RawImage) null;
    [SerializeField]
    [FormerlySerializedAs("_Progress")]
    private float progress;
    [SerializeField]
    [FormerlySerializedAs("_ProgressSlider")]
    private RawImage progressSlider = (RawImage) null;

    public override float Progress
    {
      get => this.progress;
      set
      {
        this.progress = value;
        if ((double) this.progress > 1.0)
          this.progress = 1f;
        else if ((double) this.progress < 0.0)
          this.progress = 0.0f;
        if ((Object) this.background == (Object) null)
          return;
        RectTransform component = this.background.gameObject.GetComponent<RectTransform>();
        if ((Object) this.progressSlider == (Object) null)
          return;
        this.progressSlider.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (component.rect.width - 2f) * this.progress);
      }
    }

    public Color Background
    {
      get => (Object) this.background == (Object) null ? Color.black : this.background.color;
      set
      {
        if ((Object) this.background == (Object) null)
          return;
        this.background.color = value;
      }
    }

    public Color Foreground
    {
      get => (Object) this.progressSlider == (Object) null ? Color.red : this.progressSlider.color;
      set
      {
        if ((Object) this.progressSlider == (Object) null)
          return;
        this.progressSlider.color = value;
      }
    }

    public override void SkipAnimation()
    {
    }
  }
}
