using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class ChromaticAbberationEffectNode : FlowControlNode, IUpdatable
  {
    [Port("Intensity")]
    private ValueInput<float> intensityInput;
    private float prevIntensity;
    private PostProcessingStackOverride postProcessingOverride = null;

    public void Update()
    {
      if ((Object) postProcessingOverride == (Object) null)
      {
        GetOverrideColorGrading();
      }
      else
      {
        float num = intensityInput.value;
        if (prevIntensity == (double) num)
          return;
        postProcessingOverride.ChromaticAberration.Intensity = num;
        prevIntensity = num;
      }
    }

    private void GetOverrideColorGrading()
    {
      postProcessingOverride = GameCamera.Instance.GamePostProcessingOverride;
      if (!((Object) postProcessingOverride != (Object) null))
        return;
      postProcessingOverride.ChromaticAberration.Override = true;
      postProcessingOverride.ChromaticAberration.Enabled = true;
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (!((Object) postProcessingOverride != (Object) null))
        return;
      postProcessingOverride.ChromaticAberration.Override = false;
    }
  }
}
