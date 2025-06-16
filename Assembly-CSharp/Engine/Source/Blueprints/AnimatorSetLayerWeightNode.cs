using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

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
