using System;
using UnityEngine;

public class CutsceneMecanimEvents : MonoBehaviour
{
  public event Action<string> OnEndAnimationEnd;

  public void AnimationEnd(string name)
  {
    Action<string> onEndAnimationEnd = OnEndAnimationEnd;
    if (onEndAnimationEnd == null)
      return;
    onEndAnimationEnd(name);
  }
}
