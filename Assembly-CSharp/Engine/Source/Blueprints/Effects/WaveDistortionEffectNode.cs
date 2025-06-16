using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

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
      if ((Object) waveDistortion == (Object) null)
        waveDistortion = GameCamera.Instance.Camera.GetComponent<WaveDistortion>();
      if ((Object) waveDistortion == (Object) null)
        return;
      waveDistortion.enabled = valueInput.value > 0.0;
      waveDistortion.Intensity = valueInput.value;
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (Object) waveDistortion)
        return;
      waveDistortion.enabled = false;
    }
  }
}
