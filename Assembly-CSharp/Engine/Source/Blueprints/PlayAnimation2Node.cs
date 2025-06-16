using System.Collections;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlayAnimation2Node : FlowControlNode
  {
    private ValueInput<Animation> inputAnimation;
    private ValueInput<AnimationClip> inputAnimationClip;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Complete");
      AddFlowInput("In", () =>
      {
        Animation animation = inputAnimation.value;
        if (!(animation != null))
          return;
        animation.wrapMode = WrapMode.Once;
        AnimationClip clip = inputAnimationClip.value;
        if (clip != null)
        {
          animation.clip = clip;
          animation.AddClip(clip, clip.name);
          animation.Play();
          StartCoroutine(WaitComplete(animation, clip, output));
        }
      });
      inputAnimation = AddValueInput<Animation>("Animation");
      inputAnimationClip = AddValueInput<AnimationClip>("Clip");
    }

    private IEnumerator WaitComplete(Animation animation, AnimationClip clip, FlowOutput output)
    {
      while (animation.isPlaying)
        yield return null;
      animation.RemoveClip(clip);
      output.Call();
    }
  }
}
