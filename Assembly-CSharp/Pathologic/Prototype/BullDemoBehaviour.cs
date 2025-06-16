using UnityEngine;

namespace Pathologic.Prototype
{
  public class BullDemoBehaviour : StateMachineBehaviour
  {
    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      float num = Random.value;
      if ((double) num < 0.60000002384185791)
        animator.SetInteger("Next", 0);
      else if ((double) num < 0.8)
        animator.SetInteger("Next", 1);
      else
        animator.SetInteger("Next", 2);
    }
  }
}
