using Cofe.Utility;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

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
      FlowOutput output = this.AddFlowOutput("Complete");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Animation animation1 = this.inputAnimation.value;
        if (!((Object) animation1 != (Object) null))
          return;
        animation1.wrapMode = WrapMode.Once;
        string animation2 = this.inputName.value;
        if (!animation2.IsNullOrEmpty())
          animation1.Play(animation2);
        else
          animation1.Play();
        this.StartCoroutine(this.WaitComplete(animation1, output));
      }));
      this.inputAnimation = this.AddValueInput<Animation>("Animation");
      this.inputName = this.AddValueInput<string>("Name");
    }

    private IEnumerator WaitComplete(Animation animation, FlowOutput output)
    {
      while (animation.isPlaying)
        yield return (object) null;
      output.Call();
    }
  }
}
