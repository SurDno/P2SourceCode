using UnityEngine;

namespace Engine.Behaviours.Unity.Mecanim
{
  public class AnimatorBehaviorBase45_PrimaryIdle : StateMachineBehaviour
  {
    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      bool flag = (double) Random.value < (double) AnimatorState45.GetAnimatorState(animator).PrimaryIdleProbability;
      animator.SetBool("Movable.Idle.PrimaryIdle", flag);
    }
  }
}
