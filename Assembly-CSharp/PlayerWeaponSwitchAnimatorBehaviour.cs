using Engine.Behaviours.Engines.Services;
using Engine.Common.Components.AttackerPlayer;

public class PlayerWeaponSwitchAnimatorBehaviour : StateMachineBehaviour
{
  [SerializeField]
  private WeaponKind Kind;
  [SerializeField]
  private bool SwitchOn;

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    if ((double) animator.GetLayerWeight(layerIndex) <= 0.0)
      return;
    animator.gameObject.GetComponent<PlayerWeaponServiceNew>().OnWeaponSwitch(Kind, SwitchOn);
  }
}
