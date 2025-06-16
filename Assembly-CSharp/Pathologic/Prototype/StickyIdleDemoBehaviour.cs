namespace Pathologic.Prototype
{
  public class StickyIdleDemoBehaviour : StateMachineBehaviour
  {
    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      if ((double) Random.value < 0.5)
      {
        animator.SetInteger("Next", 0);
      }
      else
      {
        int num = Random.Range(1, 7);
        animator.SetInteger("Next", num);
      }
    }
  }
}
