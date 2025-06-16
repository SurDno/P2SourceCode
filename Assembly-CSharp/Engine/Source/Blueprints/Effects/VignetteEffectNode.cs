using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class VignetteEffectNode : FlowControlNode, IUpdatable
  {
    [Port("Value")]
    private ValueInput<IntensityParameter<Color>> valueInput;
    private PostProcessingStackOverride postProcessingOverride = (PostProcessingStackOverride) null;

    public void Update()
    {
      if ((Object) this.postProcessingOverride == (Object) null)
      {
        this.postProcessingOverride = GameCamera.Instance.GamePostProcessingOverride;
        if ((Object) this.postProcessingOverride != (Object) null)
        {
          this.postProcessingOverride.Vignette.Override = true;
          this.postProcessingOverride.Vignette.Enabled = true;
        }
      }
      if ((Object) this.postProcessingOverride == (Object) null)
        return;
      this.postProcessingOverride.Vignette.Intensity = this.valueInput.value.Intensity;
      this.postProcessingOverride.Vignette.Color = this.valueInput.value.Value;
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (!((Object) this.postProcessingOverride != (Object) null))
        return;
      this.postProcessingOverride.Vignette.Override = false;
    }
  }
}
