using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class ChromaticAbberationEffectNode : FlowControlNode, IUpdatable
  {
    [Port("Intensity")]
    private ValueInput<float> intensityInput;
    private float prevIntensity;
    private PostProcessingStackOverride postProcessingOverride = (PostProcessingStackOverride) null;

    public void Update()
    {
      if ((Object) this.postProcessingOverride == (Object) null)
      {
        this.GetOverrideColorGrading();
      }
      else
      {
        float num = this.intensityInput.value;
        if ((double) this.prevIntensity == (double) num)
          return;
        this.postProcessingOverride.ChromaticAberration.Intensity = num;
        this.prevIntensity = num;
      }
    }

    private void GetOverrideColorGrading()
    {
      this.postProcessingOverride = GameCamera.Instance.GamePostProcessingOverride;
      if (!((Object) this.postProcessingOverride != (Object) null))
        return;
      this.postProcessingOverride.ChromaticAberration.Override = true;
      this.postProcessingOverride.ChromaticAberration.Enabled = true;
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (!((Object) this.postProcessingOverride != (Object) null))
        return;
      this.postProcessingOverride.ChromaticAberration.Override = false;
    }
  }
}
