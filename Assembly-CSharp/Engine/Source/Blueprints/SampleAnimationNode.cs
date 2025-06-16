using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class SampleAnimationNode : FlowControlNode
  {
    private ValueInput<AnimationClip> animationClipInput;
    private ValueInput<GameObject> goInput;
    private ValueInput<float> timeInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        AnimationClip animationClip = this.animationClipInput.value;
        GameObject go = this.goInput.value;
        Debug.LogError((object) ("TEST, before : " + (object) go.transform.position));
        animationClip.SampleAnimation(go, this.timeInput.value);
        this.StartCoroutine(this.Delay(output));
      }));
      this.animationClipInput = this.AddValueInput<AnimationClip>("AnimationClip");
      this.goInput = this.AddValueInput<GameObject>("Target");
      this.timeInput = this.AddValueInput<float>("Time");
    }

    private IEnumerator Delay(FlowOutput output)
    {
      yield return (object) null;
      yield return (object) null;
      yield return (object) null;
      yield return (object) null;
      yield return (object) null;
      yield return (object) null;
      yield return (object) null;
      Debug.LogError((object) ("TEST, after : " + (object) this.goInput.value.transform.position));
      output.Call();
    }
  }
}
