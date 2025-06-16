// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressHiddenCircle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  public class ProgressHiddenCircle : ProgressViewBase
  {
    [SerializeField]
    [FormerlySerializedAs("CircleBase")]
    private Image circleBase = (Image) null;
    [SerializeField]
    [FormerlySerializedAs("CircleFill")]
    private Image circleFill = (Image) null;
    private float progress = 0.0f;

    [Inspected]
    public override float Progress
    {
      get => this.progress;
      set
      {
        if ((double) value <= 0.0)
        {
          this.gameObject.SetActive(false);
        }
        else
        {
          if ((double) this.progress <= 0.0)
            this.gameObject.SetActive(true);
          if ((Object) this.circleFill != (Object) null)
            this.circleFill.fillAmount = value;
          if ((Object) this.circleBase != (Object) null)
            this.circleBase.fillAmount = 1f - value;
        }
        this.progress = value;
      }
    }

    public override void SkipAnimation()
    {
    }
  }
}
