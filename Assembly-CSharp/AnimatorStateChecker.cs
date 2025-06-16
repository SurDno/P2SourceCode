using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Inspectors;

public class AnimatorStateChecker : MonoBehaviour
{
  [Inspected]
  private AnimatorState45 animatorState;

  private void Awake()
  {
    animatorState = AnimatorState45.GetAnimatorState(this.GetComponent<Pivot>().GetAnimator());
  }
}
