using Engine.Behaviours.Engines.Controllers;
using Engine.Behaviours.Unity.Mecanim;

namespace Engine.Behaviours.Components
{
  [RequireComponent(typeof (IKController))]
  [DisallowMultipleComponent]
  public class PivotDiseased : MonoBehaviour
  {
    private Animator animator;
    private int stanceLayerIndex = -1;
    private int reactionLayerIndex = -1;
    private float stanceOnPoseWeigth;

    public bool AttackStance { get; set; }

    private void Awake()
    {
      animator = this.GetComponent<Animator>();
      if ((Object) animator == (Object) null)
      {
        Debug.LogErrorFormat("{0} doesn't contain {1} unity component.", (object) this.gameObject.name, (object) typeof (Animator).Name);
      }
      else
      {
        stanceLayerIndex = animator.GetLayerIndex("Attack Stance Layer");
        reactionLayerIndex = animator.GetLayerIndex("Reaction Layer");
      }
    }

    public void Push(GameObject whoPushes)
    {
      DiseasedAnimatorState animatorState = DiseasedAnimatorState.GetAnimatorState(animator);
      animatorState.TriggerPlayerPush();
      Vector3 vector3 = this.transform.InverseTransformDirection(-whoPushes.transform.forward);
      float num = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
      animatorState.PlayerPushAngle = num;
    }

    public void AttackPlayerFendOff(GameObject player)
    {
      DiseasedAnimatorState.GetAnimatorState(animator).TriggerPlayerFendOff();
    }

    private void Update()
    {
      stanceOnPoseWeigth = Mathf.Clamp01(stanceOnPoseWeigth + (AttackStance ? 1f : -1f) * Time.deltaTime);
      animator.SetLayerWeight(stanceLayerIndex, stanceOnPoseWeigth);
    }
  }
}
