using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateInFire(NpcState npcState, Pivot pivot) : INpcState 
  {
  private NpcState npcState = npcState;
  private EngineBehavior behavior;
  private NavMeshAgent agent;
  private Animator animator;
  private AnimatorState45 animatorState;
  private int fireLayerIndex;
  private float fireLayerWeight;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; private set; } = npcState.gameObject;

  [Inspected]
  public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

  private bool TryInit()
  {
    if (inited)
      return true;
    behavior = pivot.GetBehavior();
    agent = pivot.GetAgent();
    animator = pivot.GetAnimator();
    if (animator == null)
    {
      Debug.LogError("Null animator " + GameObject.name, GameObject);
      Debug.LogError("Null animator " + GameObject.GetFullName());
      failed = true;
      return false;
    }
    animatorState = AnimatorState45.GetAnimatorState(animator);
    failed = false;
    inited = true;
    return true;
  }

  public void Activate()
  {
    if (!TryInit())
      return;
    animatorState.ResetAllTriggers();
    animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    fireLayerIndex = animator.GetLayerIndex("Fight Fire Layer");
    fireLayerWeight = animator.GetLayerWeight(fireLayerIndex);
    agent.enabled = true;
    Pivot component = GameObject.GetComponent<Pivot>();
    if (!(component != null) || !(component.HidingOuterWeapon != null))
      return;
    component.HidingOuterWeapon.SetActive(false);
  }

  public void Shutdown()
  {
    if (!failed)
      ;
  }

  public void OnAnimatorMove()
  {
    if (failed)
      return;
    behavior.OnExternalAnimatorMove();
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!failed)
      ;
  }

  public void Update()
  {
    if (failed)
      return;
    fireLayerWeight = Mathf.MoveTowards(fireLayerWeight, 1f, Time.deltaTime / 0.5f);
    animator.SetLayerWeight(fireLayerIndex, fireLayerWeight);
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
