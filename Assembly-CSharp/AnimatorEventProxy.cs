using System;
using UnityEngine;
using UnityEngine.Profiling;

public class AnimatorEventProxy : MonoBehaviour
{
  public event Action AnimatorMoveEvent;

  public event Action<string> AnimatorEventEvent;

  private void OnAnimatorMove()
  {
    Action animatorMoveEvent = this.AnimatorMoveEvent;
    if (animatorMoveEvent == null)
      return;
    animatorMoveEvent();
  }

  private void AnimatorEvent(string name)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(name);
    Action<string> animatorEventEvent = this.AnimatorEventEvent;
    if (animatorEventEvent != null)
      animatorEventEvent(name);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }
}
