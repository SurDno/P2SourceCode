// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.ChromaticAbberationEffectNode
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
