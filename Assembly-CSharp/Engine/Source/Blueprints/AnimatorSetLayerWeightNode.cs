// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.AnimatorSetLayerWeightNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class AnimatorSetLayerWeightNode : FlowControlNode
  {
    [Port("Animator")]
    private ValueInput<Animator> animatorInput;
    [Port("LayerName")]
    private ValueInput<string> layerNameInput;
    [Port("LayerWeight")]
    private ValueInput<float> layerWeightInput;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      Animator animator = this.animatorInput.value;
      if ((Object) animator != (Object) null)
      {
        int layerIndex = animator.GetLayerIndex(this.layerNameInput.value);
        animator.SetLayerWeight(layerIndex, this.layerWeightInput.value);
      }
      this.output.Call();
    }
  }
}
