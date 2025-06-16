using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

public class NpcStateRagdollRessurect : INpcState
{
  private NpcState npcState;
  private Pivot pivot;
  private Behaviour behavior;
  private Rigidbody rigidbody;
  private Animator animator;
  private CapsuleCollider collider;
  private AnimatorState45 animatorState;
  private FightAnimatorBehavior.AnimatorState fightAnimatorState;
  private bool initiallyKinematic;
  private bool initiallyColliderEnabled;
  private int ragdollLayerIndex;
  private float initialRagdollLayerWeight;
  private bool inited;
  private bool failed;
  private bool animatorWasEnabled;
  private float initialRagdollWeight;
  private bool ressurectStarted;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status { get; set; }

  private bool TryInit()
  {
    if (inited)
      return true;
    behavior = pivot.GetBehavior();
    rigidbody = pivot.GetRigidbody();
    collider = GameObject.GetComponent<CapsuleCollider>();
    animator = pivot.GetAnimator();
    if (animator == null)
    {
      Debug.LogError("Null animator " + GameObject.name, GameObject);
      Debug.LogError("Null animator " + GameObject.GetFullName());
      failed = true;
      return false;
    }
    fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(animator);
    animatorState = AnimatorState45.GetAnimatorState(animator);
    ragdollLayerIndex = animator.GetLayerIndex("Ragdoll Layer");
    failed = false;
    inited = true;
    return true;
  }

  public NpcStateRagdollRessurect(NpcState npcState, Pivot pivot)
  {
    GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate()
  {
    if (!TryInit())
      return;
    if (pivot.HierarhyStructure != Pivot.HierarhyStructureEnum.PuppetMaster)
    {
      Debug.LogWarning("Only PupperMaster ragdoll can be ressurected");
    }
    else
    {
      Status = NpcStateStatusEnum.Running;
      ressurectStarted = false;
      initialRagdollWeight = pivot.RagdollWeight;
      initialRagdollLayerWeight = animator.GetLayerWeight(ragdollLayerIndex);
      animator.SetLayerWeight(ragdollLayerIndex, 1f);
      animatorWasEnabled = npcState.AnimatorEnabled;
      bool flag;
      if ((bool) (Object) pivot.RootBone)
      {
        flag = Vector3.Dot(Vector3.up, pivot.RootBone.up) > 0.0;
      }
      else
      {
        flag = true;
        Debug.LogError("To use Ressurect you need to specify RootBone");
      }
      if (flag)
      {
        animatorState.ResetAllTriggers();
        animatorState.SetTrigger("Triggers/RagdollGetUpFromBack");
      }
      else
      {
        animatorState.ResetAllTriggers();
        animatorState.SetTrigger("Triggers/RagdollGetUpFromBelly");
      }
    }
  }

  public void Shutdown()
  {
    if (pivot.HierarhyStructure != Pivot.HierarhyStructureEnum.PuppetMaster)
      return;
    pivot.RagdollWeight = initialRagdollWeight;
    animator.SetLayerWeight(ragdollLayerIndex, initialRagdollLayerWeight);
    npcState.AnimatorEnabled = animatorWasEnabled;
  }

  public void OnAnimatorMove()
  {
    if (!failed)
      ;
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
    pivot.RagdollWeight = 0.0f;
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused || Status != 0)
      return;
    if (pivot.HierarhyStructure == Pivot.HierarhyStructureEnum.PuppetMaster)
    {
      if (!ressurectStarted)
      {
        if (fightAnimatorState.IsRessurect)
          ressurectStarted = true;
      }
      else if (!fightAnimatorState.IsRessurect)
      {
        Status = NpcStateStatusEnum.Success;
        pivot.RagdollWeight = 0.0f;
        return;
      }
    }
    else if (pivot.RagdollWeight == 0.0)
      Status = NpcStateStatusEnum.Success;
    if (pivot.HierarhyStructure == Pivot.HierarhyStructureEnum.PuppetMaster)
      pivot.RagdollWeight = Mathf.MoveTowards(pivot.RagdollWeight, 0.0f, Time.deltaTime / 0.5f);
    else
      pivot.RagdollWeight = 0.0f;
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
