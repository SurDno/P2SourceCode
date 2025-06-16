using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class VignetteEffectNode : FlowControlNode, IUpdatable
  {
    [Port("Value")]
    private ValueInput<IntensityParameter<Color>> valueInput;
    private PostProcessingStackOverride postProcessingOverride = null;

    public void Update()
    {
      if ((Object) postProcessingOverride == (Object) null)
      {
        postProcessingOverride = GameCamera.Instance.GamePostProcessingOverride;
        if ((Object) postProcessingOverride != (Object) null)
        {
          postProcessingOverride.Vignette.Override = true;
          postProcessingOverride.Vignette.Enabled = true;
        }
      }
      if ((Object) postProcessingOverride == (Object) null)
        return;
      postProcessingOverride.Vignette.Intensity = valueInput.value.Intensity;
      postProcessingOverride.Vignette.Color = valueInput.value.Value;
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (!((Object) postProcessingOverride != (Object) null))
        return;
      postProcessingOverride.Vignette.Override = false;
    }
  }
}
