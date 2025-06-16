using System.Collections;
using Cofe.Utility;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlayAnimationNode : FlowControlNode
  {
    private ValueInput<Animation> inputAnimation;
    private ValueInput<string> inputName;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Complete");
      AddFlowInput("In", () =>
      {
        Animation animation1 = inputAnimation.value;
        if (!((Object) animation1 != (Object) null))
          return;
        animation1.wrapMode = WrapMode.Once;
        string animation2 = inputName.value;
        if (!animation2.IsNullOrEmpty())
          animation1.Play(animation2);
        else
          animation1.Play();
        StartCoroutine(WaitComplete(animation1, output));
      });
      inputAnimation = AddValueInput<Animation>("Animation");
      inputName = AddValueInput<string>("Name");
    }

    private IEnumerator WaitComplete(Animation animation, FlowOutput output)
    {
      while (animation.isPlaying)
        yield return null;
      output.Call();
    }
  }
}
