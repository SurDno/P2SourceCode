using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Inspectors;
using UnityEngine;

public class AnimatorStateChecker : MonoBehaviour
{
  [Inspected]
  private AnimatorState45 animatorState;

  private void Awake()
  {
    this.animatorState = AnimatorState45.GetAnimatorState(this.GetComponent<Pivot>().GetAnimator());
  }
}
