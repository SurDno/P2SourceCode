using System.Collections;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
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
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        AnimationClip animationClip = animationClipInput.value;
        GameObject go = goInput.value;
        Debug.LogError("TEST, before : " + go.transform.position);
        animationClip.SampleAnimation(go, timeInput.value);
        StartCoroutine(Delay(output));
      });
      animationClipInput = AddValueInput<AnimationClip>("AnimationClip");
      goInput = AddValueInput<GameObject>("Target");
      timeInput = AddValueInput<float>("Time");
    }

    private IEnumerator Delay(FlowOutput output)
    {
      yield return null;
      yield return null;
      yield return null;
      yield return null;
      yield return null;
      yield return null;
      yield return null;
      Debug.LogError("TEST, after : " + goInput.value.transform.position);
      output.Call();
    }
  }
}
