using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Settings.External;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public class NpcStateIdle : INpcState
{
  private EngineBehavior behavior;
  private NavMeshAgent agent;
  private Rigidbody rigidbody;
  private Pivot pivot;
  private Animator animator;
  private NpcState npcState;
  private NPCWeaponService weaponService;
  private bool agentWasEnabled;
  private bool rigidbodyWasKinematic;
  private bool rigidbodyWasGravity;
  private bool makeObstacle;
  private bool obstacleCreated;
  private AnimatorUpdateMode initialAnimatorUpdateMode;
  private AnimatorCullingMode initialAnimatorCullingMode;
  private AnimatorState45 animatorState;
  private bool inited;
  private bool failed;
  private bool sayReplics;
  private float timeToNextReplic;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.behavior = this.pivot.GetBehavior();
    this.rigidbody = this.pivot.GetRigidbody();
    this.weaponService = this.pivot.GetNpcWeaponService();
    this.agent = this.pivot.GetAgent();
    if ((Object) this.agent == (Object) null)
    {
      Debug.LogError((object) ("No navmesh agent " + this.GameObject.name), (Object) this.GameObject);
      this.failed = true;
      return false;
    }
    this.animator = this.pivot.GetAnimator();
    if ((Object) this.animator == (Object) null)
    {
      this.failed = true;
      return false;
    }
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateIdle(NpcState npcState, Pivot pivot)
  {
    this.npcState = npcState;
    this.pivot = pivot;
    this.GameObject = npcState.gameObject;
  }

  public void Activate(float primaryIdleProbability, bool makeObstacle = false)
  {
    if (!this.TryInit())
      return;
    MovementControllerUtility.SetRandomAnimation(this.animator, this.pivot.SecondaryIdleAnimationCount, this.pivot.SecondaryLowIdleAnimationCount);
    this.makeObstacle = makeObstacle;
    this.animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
    this.animatorState.PrimaryIdleProbability = primaryIdleProbability;
    this.agentWasEnabled = this.agent.enabled;
    if ((Object) this.rigidbody != (Object) null)
    {
      this.rigidbodyWasKinematic = this.rigidbody.isKinematic;
      this.rigidbodyWasGravity = this.rigidbody.useGravity;
      this.rigidbody.useGravity = false;
    }
    bool indoor = false;
    LocationItemComponent component1 = this.npcState.Owner?.GetComponent<LocationItemComponent>();
    if (component1 == null)
    {
      Debug.LogWarning((object) (this.GameObject.name + ": location component not found"));
    }
    else
    {
      indoor = component1.IsIndoor;
      NPCStateHelper.SetAgentAreaMask(this.agent, indoor);
    }
    if (!this.agent.enabled)
      this.agent.enabled = true;
    this.initialAnimatorUpdateMode = this.animator.updateMode;
    this.animator.updateMode = AnimatorUpdateMode.Normal;
    this.initialAnimatorCullingMode = this.animator.cullingMode;
    this.animator.cullingMode = AnimatorCullingMode.CullCompletely;
    if (this.npcState.Owner != null)
    {
      ParametersComponent component2 = this.npcState.Owner.GetComponent<ParametersComponent>();
      if (component2 != null)
      {
        IParameter<bool> byName = component2.GetByName<bool>(ParameterNameEnum.SayReplicsInIdle);
        if (byName != null)
        {
          this.sayReplics = byName.Value;
          if (this.sayReplics)
            this.timeToNextReplic = Random.Range(ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
        }
      }
    }
    if ((Object) this.weaponService != (Object) null)
      this.weaponService.Weapon = WeaponEnum.Unknown;
    if (this.agent.isOnNavMesh)
      return;
    Vector3 position = this.GameObject.transform.position;
    if (NavMeshUtility.SampleRaycastPosition(ref position, indoor ? 1f : 5f, indoor ? 2f : 10f, this.agent.areaMask))
      this.agent.Warp(position);
    else
      Debug.Log((object) "Can't sample navmesh", (Object) this.GameObject);
  }

  private void CreateObstacle()
  {
  }

  public void Shutdown()
  {
    if (this.failed)
      return;
    if (this.agent.enabled != this.agentWasEnabled)
      this.agent.enabled = this.agentWasEnabled;
    this.animator.updateMode = this.initialAnimatorUpdateMode;
    this.animator.cullingMode = this.initialAnimatorCullingMode;
    NavMeshObstacle obstacle = this.pivot.GetObstacle();
    if ((Object) obstacle != (Object) null)
      obstacle.enabled = false;
    if ((bool) (Object) this.rigidbody)
    {
      this.rigidbody.isKinematic = this.rigidbodyWasKinematic;
      this.rigidbody.useGravity = this.rigidbodyWasGravity;
    }
    if (!((Object) this.weaponService != (Object) null))
      return;
    this.weaponService.Weapon = this.npcState.Weapon;
  }

  public void OnAnimatorMove()
  {
    if (this.failed)
      return;
    float num = this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
    Vector3 vector3 = this.GameObject.transform.position + this.animator.deltaPosition;
    if (this.agent.isActiveAndEnabled)
    {
      long monoHeapSizeLong1 = Profiler.GetMonoHeapSizeLong();
      if (this.agent.isOnNavMesh)
      {
        this.agent.nextPosition = vector3;
        this.GameObject.transform.position = this.agent.nextPosition;
      }
      else
      {
        long monoHeapSizeLong2 = Profiler.GetMonoHeapSizeLong();
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(this.GameObject.transform.position, out hit, this.agent.areaMask))
        {
          this.agent.Warp(hit.position);
          this.GameObject.transform.position = this.agent.nextPosition;
          Debug.Log((object) "Agent was teleported to closest edge", (Object) this.agent.gameObject);
        }
        else
        {
          long monoHeapSizeLong3 = Profiler.GetMonoHeapSizeLong();
          if (NavMesh.Raycast(this.GameObject.transform.position + new Vector3(0.0f, 20f, 0.0f), this.GameObject.transform.position + new Vector3(0.0f, -20f, 0.0f), out hit, this.agent.areaMask))
          {
            this.agent.Warp(hit.position);
            this.GameObject.transform.position = this.agent.nextPosition;
            Debug.Log((object) "Agent was teleported (Raycast)", (Object) this.agent.gameObject);
          }
          else
          {
            long monoHeapSizeLong4 = Profiler.GetMonoHeapSizeLong();
            Debug.LogWarning((object) ObjectInfoUtility.GetStream().Append("Agent was not able to teleport to closest edge and raycast also failed, memory : ").Append(monoHeapSizeLong1).Append(" , ").Append(monoHeapSizeLong2).Append(" , ").Append(monoHeapSizeLong3).Append(" , ").Append(monoHeapSizeLong4), (Object) this.agent.gameObject);
          }
        }
      }
    }
    else
      this.GameObject.transform.position = vector3;
    this.GameObject.transform.rotation *= Quaternion.AngleAxis(57.29578f * this.animator.angularVelocity.y * num, Vector3.up);
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (this.failed || InstanceByRequest<EngineApplication>.Instance.IsPaused || !this.sayReplics)
      return;
    this.timeToNextReplic -= Time.deltaTime;
    if ((double) this.timeToNextReplic <= 0.0)
    {
      this.timeToNextReplic = Random.Range(ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
      NPCStateHelper.SayIdleReplic(this.npcState.Owner);
    }
  }

  public void OnLodStateChanged(bool inLodState)
  {
    this.npcState.AnimatorEnabled = !inLodState;
    EffectsComponent component = this.npcState.Owner?.GetComponent<EffectsComponent>();
    if (component == null)
      return;
    component.Disabled = inLodState;
  }
}
