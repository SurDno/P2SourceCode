using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class AnimatorTriggerNode : FlowControlNode
  {
    [Port("Animator")]
    private ValueInput<Animator> animatorInput;
    [Port("TriggerName")]
    private ValueInput<string> triggerNameInput;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      Animator animator = this.animatorInput.value;
      string name = this.triggerNameInput.value;
      if ((Object) animator != (Object) null)
        animator.SetTrigger(name);
      this.output.Call();
    }
  }
}
