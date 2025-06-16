// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.WaveDistortionEffectNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class WaveDistortionEffectNode : FlowControlNode, IUpdatable
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    private WaveDistortion waveDistortion;

    public void Update()
    {
      if ((Object) this.waveDistortion == (Object) null)
        this.waveDistortion = GameCamera.Instance.Camera.GetComponent<WaveDistortion>();
      if ((Object) this.waveDistortion == (Object) null)
        return;
      this.waveDistortion.enabled = (double) this.valueInput.value > 0.0;
      this.waveDistortion.Intensity = this.valueInput.value;
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (Object) this.waveDistortion)
        return;
      this.waveDistortion.enabled = false;
    }
  }
}
