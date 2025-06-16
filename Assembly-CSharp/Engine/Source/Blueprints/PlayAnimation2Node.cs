using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;
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
      FlowOutput output = this.AddFlowOutput("Complete");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Animation animation = this.inputAnimation.value;
        if (!((Object) animation != (Object) null))
          return;
        animation.wrapMode = WrapMode.Once;
        AnimationClip clip = this.inputAnimationClip.value;
        if ((Object) clip != (Object) null)
        {
          animation.clip = clip;
          animation.AddClip(clip, clip.name);
          animation.Play();
          this.StartCoroutine(this.WaitComplete(animation, clip, output));
        }
      }));
      this.inputAnimation = this.AddValueInput<Animation>("Animation");
      this.inputAnimationClip = this.AddValueInput<AnimationClip>("Clip");
    }

    private IEnumerator WaitComplete(Animation animation, AnimationClip clip, FlowOutput output)
    {
      while (animation.isPlaying)
        yield return (object) null;
      animation.RemoveClip(clip);
      output.Call();
    }
  }
}
