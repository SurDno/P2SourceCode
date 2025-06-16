// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.LoopProgressAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class LoopProgressAnimation : FloatView
  {
    [SerializeField]
    private FloatView progressView;
    [SerializeField]
    private float rate;
    [SerializeField]
    private bool randomStart;
    private float phase;

    public override float FloatValue
    {
      get => this.rate;
      set => this.rate = value;
    }

    private void ApplyPhase()
    {
      if ((Object) this.progressView == (Object) null)
        return;
      this.progressView.FloatValue = this.phase;
    }

    public override void SkipAnimation()
    {
    }

    private void OnEnable()
    {
      this.phase = this.randomStart ? Random.value : 0.0f;
      this.ApplyPhase();
    }

    private void Update()
    {
      this.phase += Time.deltaTime * this.rate;
      this.phase %= 1f;
      this.ApplyPhase();
    }
  }
}
