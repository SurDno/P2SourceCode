using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
      Animator animator = animatorInput.value;
      string name = triggerNameInput.value;
      if ((Object) animator != (Object) null)
        animator.SetTrigger(name);
      output.Call();
    }
  }
}
