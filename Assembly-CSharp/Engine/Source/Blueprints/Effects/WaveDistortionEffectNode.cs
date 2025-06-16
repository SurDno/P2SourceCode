using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

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
