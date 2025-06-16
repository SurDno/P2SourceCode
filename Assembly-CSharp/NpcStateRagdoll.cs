using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateRagdoll : INpcState, INpcStateRagdoll
{
  private NpcState npcState;
  private NPCEnemy enemy;
  private Behaviour behavior;
  private NavMeshAgent agent;
  private Pivot pivot;
  private Rigidbody rigidbody;
  private Animator animator;
  private AnimatorState45 animatorState;
  private NPCWeaponService weaponService;
  private CapsuleCollider collider;
  private bool initiallyKinematic;
  private bool initiallyColliderEnabled;
  private int ragdollLayerIndex;
  private bool inited;
  private bool failed;
  private bool agentWasEnabled;
  private float initialRagdollLayerWeight;
  private float initialRagdollWeight;
  private float actualRagdollWeight;
  private AnimatorUpdateMode initialAnimatorUpdateMode;
  private AnimatorCullingMode initialAnimatorCullingMode;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status { get; private set; }

  private bool TryInit()
  {
    if (inited)
      return true;
    enemy = pivot.GetNpcEnemy();
    behavior = pivot.GetBehavior();
    rigidbody = pivot.GetRigidbody();
    collider = GameObject.GetComponent<CapsuleCollider>();
    animator = pivot.GetAnimator();
    weaponService = pivot.GetNpcWeaponService();
    agent = pivot.GetAgent();
    if (agent == null)
    {
      Debug.LogError("No navmesh agent " + GameObject.name, GameObject);
      failed = true;
      return false;
    }
    if (animator == null)
    {
      Debug.LogError("Null animator " + GameObject.name, GameObject);
      Debug.LogError("Null animator " + GameObject.GetFullName());
      failed = true;
      return false;
    }
    ragdollLayerIndex = animator.GetLayerIndex("Ragdoll Layer");
    animatorState = AnimatorState45.GetAnimatorState(animator);
    failed = false;
    inited = true;
    return true;
  }

  public NpcStateRagdoll(NpcState npcState, Pivot pivot)
  {
    GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(bool internalCollisions, INpcState previousState)
  {
    if (!TryInit())
      return;
    if ((bool) (Object) rigidbody)
      initiallyKinematic = rigidbody.isKinematic;
    if ((bool) (Object) collider)
      initiallyColliderEnabled = collider.enabled;
    if (weaponService != null)
      weaponService.Weapon = npcState.Weapon;
    npcState.WeaponChangeEvent += State_WeaponChangeEvent;
    Status = NpcStateStatusEnum.Running;
    if ((bool) (Object) rigidbody)
      rigidbody.isKinematic = true;
    if ((bool) (Object) collider)
      collider.enabled = false;
    if (pivot.HierarhyStructure == Pivot.HierarhyStructureEnum.PuppetMaster)
      animatorState.SetTrigger("Triggers/Ragdoll");
    initialRagdollWeight = pivot.RagdollWeight;
    if (previousState is INpcStateRagdoll)
    {
      pivot.RagdollWeight = (previousState as INpcStateRagdoll).GetActualRagdollWeight();
      actualRagdollWeight = pivot.RagdollWeight;
    }
    else
      pivot.RagdollWeight = Mathf.Max(pivot.RagdollWeight, 0.5f);
    pivot.RgdollInternalCollisions = internalCollisions;
    initialAnimatorUpdateMode = animator.updateMode;
    animator.updateMode = AnimatorUpdateMode.Normal;
    initialAnimatorCullingMode = animator.cullingMode;
    animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    agentWasEnabled = agent.enabled;
    agent.enabled = true;
    animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    animatorState.SetTrigger("Fight.Triggers/CancelEscape");
    if (!(bool) (Object) enemy)
      return;
    enemy.RotationTarget = null;
    enemy.RotateByPath = false;
  }

  private void State_WeaponChangeEvent(WeaponEnum weapon)
  {
    if (!(weaponService != null))
      return;
    weaponService.Weapon = weapon;
  }

  public void Shutdown()
  {
    if (failed)
      return;
    if ((bool) (Object) rigidbody)
      rigidbody.isKinematic = initiallyKinematic;
    if ((bool) (Object) collider)
      collider.enabled = initiallyColliderEnabled;
    if (pivot.HierarhyStructure == Pivot.HierarhyStructureEnum.PuppetMaster)
      animator.SetLayerWeight(ragdollLayerIndex, initialRagdollLayerWeight);
    pivot.RagdollWeight = initialRagdollWeight;
    agent.enabled = agentWasEnabled;
    animator.updateMode = initialAnimatorUpdateMode;
    animator.cullingMode = initialAnimatorCullingMode;
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
    if (failed || InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    pivot.RagdollWeight = Mathf.MoveTowards(pivot.RagdollWeight, 1f, Time.deltaTime / 0.5f);
    actualRagdollWeight = pivot.RagdollWeight;
    if (!animator.isActiveAndEnabled || !agent.isActiveAndEnabled)
      return;
    Vector3 vector3 = GameObject.transform.position + animator.deltaPosition * (1f - pivot.RagdollWeight);
    vector3.y = Mathf.MoveTowards(vector3.y, agent.nextPosition.y, Time.deltaTime * 0.1f);
    agent.nextPosition = vector3;
    GameObject.transform.position = agent.nextPosition;
  }

  public float GetActualRagdollWeight() => actualRagdollWeight;

  public void OnLodStateChanged(bool enabled)
  {
  }
}
