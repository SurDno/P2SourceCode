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
    if (this.inited)
      return true;
    this.behavior = (Behaviour) this.pivot.GetBehavior();
    this.rigidbody = this.pivot.GetRigidbody();
    this.collider = this.GameObject.GetComponent<CapsuleCollider>();
    this.animator = this.pivot.GetAnimator();
    if ((Object) this.animator == (Object) null)
    {
      Debug.LogError((object) ("Null animator " + this.GameObject.name), (Object) this.GameObject);
      Debug.LogError((object) ("Null animator " + this.GameObject.GetFullName()));
      this.failed = true;
      return false;
    }
    this.fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(this.animator);
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    this.ragdollLayerIndex = this.animator.GetLayerIndex("Ragdoll Layer");
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateRagdollRessurect(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate()
  {
    if (!this.TryInit())
      return;
    if (this.pivot.HierarhyStructure != Pivot.HierarhyStructureEnum.PuppetMaster)
    {
      Debug.LogWarning((object) "Only PupperMaster ragdoll can be ressurected");
    }
    else
    {
      this.Status = NpcStateStatusEnum.Running;
      this.ressurectStarted = false;
      this.initialRagdollWeight = this.pivot.RagdollWeight;
      this.initialRagdollLayerWeight = this.animator.GetLayerWeight(this.ragdollLayerIndex);
      this.animator.SetLayerWeight(this.ragdollLayerIndex, 1f);
      this.animatorWasEnabled = this.npcState.AnimatorEnabled;
      bool flag;
      if ((bool) (Object) this.pivot.RootBone)
      {
        flag = (double) Vector3.Dot(Vector3.up, this.pivot.RootBone.up) > 0.0;
      }
      else
      {
        flag = true;
        Debug.LogError((object) "To use Ressurect you need to specify RootBone");
      }
      if (flag)
      {
        this.animatorState.ResetAllTriggers();
        this.animatorState.SetTrigger("Triggers/RagdollGetUpFromBack");
      }
      else
      {
        this.animatorState.ResetAllTriggers();
        this.animatorState.SetTrigger("Triggers/RagdollGetUpFromBelly");
      }
    }
  }

  public void Shutdown()
  {
    if (this.pivot.HierarhyStructure != Pivot.HierarhyStructureEnum.PuppetMaster)
      return;
    this.pivot.RagdollWeight = this.initialRagdollWeight;
    this.animator.SetLayerWeight(this.ragdollLayerIndex, this.initialRagdollLayerWeight);
    this.npcState.AnimatorEnabled = this.animatorWasEnabled;
  }

  public void OnAnimatorMove()
  {
    if (!this.failed)
      ;
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (this.failed)
      return;
    this.pivot.RagdollWeight = 0.0f;
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused || this.Status != 0)
      return;
    if (this.pivot.HierarhyStructure == Pivot.HierarhyStructureEnum.PuppetMaster)
    {
      if (!this.ressurectStarted)
      {
        if (this.fightAnimatorState.IsRessurect)
          this.ressurectStarted = true;
      }
      else if (!this.fightAnimatorState.IsRessurect)
      {
        this.Status = NpcStateStatusEnum.Success;
        this.pivot.RagdollWeight = 0.0f;
        return;
      }
    }
    else if ((double) this.pivot.RagdollWeight == 0.0)
      this.Status = NpcStateStatusEnum.Success;
    if (this.pivot.HierarhyStructure == Pivot.HierarhyStructureEnum.PuppetMaster)
      this.pivot.RagdollWeight = Mathf.MoveTowards(this.pivot.RagdollWeight, 0.0f, Time.deltaTime / 0.5f);
    else
      this.pivot.RagdollWeight = 0.0f;
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
