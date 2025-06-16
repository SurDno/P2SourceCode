using Engine.Behaviours.Components;
using UnityEngine;

public class SetRandomDialogAnimationBehaviour : StateMachineBehaviour
{
  private int lastUsedAnimation = 0;

  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    Pivot component = animator.gameObject.GetComponent<Pivot>();
    if ((Object) component == (Object) null)
    {
      component = animator.gameObject.transform.parent.GetComponent<Pivot>();
      if ((Object) component == (Object) null)
        return;
    }
    int num1 = this.lastUsedAnimation == 0 ? 1 : 0;
    int num2 = Random.Range(0, component.DialogIdleAnimationCount + 1);
    this.lastUsedAnimation = num2;
    animator.SetInteger("Dialog.AnimationControl", num2);
  }
}
