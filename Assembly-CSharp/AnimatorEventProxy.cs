using System;

public class AnimatorEventProxy : MonoBehaviour
{
  public event Action AnimatorMoveEvent;

  public event Action<string> AnimatorEventEvent;

  private void OnAnimatorMove()
  {
    Action animatorMoveEvent = AnimatorMoveEvent;
    if (animatorMoveEvent == null)
      return;
    animatorMoveEvent();
  }

  private void AnimatorEvent(string name)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(name);
    Action<string> animatorEventEvent = AnimatorEventEvent;
    if (animatorEventEvent != null)
      animatorEventEvent(name);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }
}
