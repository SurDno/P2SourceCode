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

public class NpcStateIdle(NpcState npcState, Pivot pivot) : INpcState 
  {
  private EngineBehavior behavior;
  private NavMeshAgent agent;
  private Rigidbody rigidbody;
  private Animator animator;
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

  public GameObject GameObject { get; private set; } = npcState.gameObject;

  [Inspected]
  public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

  private bool TryInit()
  {
    if (inited)
      return true;
    behavior = pivot.GetBehavior();
    rigidbody = pivot.GetRigidbody();
    weaponService = pivot.GetNpcWeaponService();
    agent = pivot.GetAgent();
    if (agent == null)
    {
      Debug.LogError("No navmesh agent " + GameObject.name, GameObject);
      failed = true;
      return false;
    }
    animator = pivot.GetAnimator();
    if (animator == null)
    {
      failed = true;
      return false;
    }
    animatorState = AnimatorState45.GetAnimatorState(animator);
    failed = false;
    inited = true;
    return true;
  }

  public void Activate(float primaryIdleProbability, bool makeObstacle = false)
  {
    if (!TryInit())
      return;
    MovementControllerUtility.SetRandomAnimation(animator, pivot.SecondaryIdleAnimationCount, pivot.SecondaryLowIdleAnimationCount);
    this.makeObstacle = makeObstacle;
    animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
    animatorState.PrimaryIdleProbability = primaryIdleProbability;
    agentWasEnabled = agent.enabled;
    if (rigidbody != null)
    {
      rigidbodyWasKinematic = rigidbody.isKinematic;
      rigidbodyWasGravity = rigidbody.useGravity;
      rigidbody.useGravity = false;
    }
    bool indoor = false;
    LocationItemComponent component1 = npcState.Owner?.GetComponent<LocationItemComponent>();
    if (component1 == null)
    {
      Debug.LogWarning(GameObject.name + ": location component not found");
    }
    else
    {
      indoor = component1.IsIndoor;
      NPCStateHelper.SetAgentAreaMask(agent, indoor);
    }
    if (!agent.enabled)
      agent.enabled = true;
    initialAnimatorUpdateMode = animator.updateMode;
    animator.updateMode = AnimatorUpdateMode.Normal;
    initialAnimatorCullingMode = animator.cullingMode;
    animator.cullingMode = AnimatorCullingMode.CullCompletely;
    if (npcState.Owner != null)
    {
      ParametersComponent component2 = npcState.Owner.GetComponent<ParametersComponent>();
      if (component2 != null)
      {
        IParameter<bool> byName = component2.GetByName<bool>(ParameterNameEnum.SayReplicsInIdle);
        if (byName != null)
        {
          sayReplics = byName.Value;
          if (sayReplics)
            timeToNextReplic = Random.Range(ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
        }
      }
    }
    if (weaponService != null)
      weaponService.Weapon = WeaponEnum.Unknown;
    if (agent.isOnNavMesh)
      return;
    Vector3 position = GameObject.transform.position;
    if (NavMeshUtility.SampleRaycastPosition(ref position, indoor ? 1f : 5f, indoor ? 2f : 10f, agent.areaMask))
      agent.Warp(position);
    else
      Debug.Log("Can't sample navmesh", GameObject);
  }

  private void CreateObstacle()
  {
  }

  public void Shutdown()
  {
    if (failed)
      return;
    if (agent.enabled != agentWasEnabled)
      agent.enabled = agentWasEnabled;
    animator.updateMode = initialAnimatorUpdateMode;
    animator.cullingMode = initialAnimatorCullingMode;
    NavMeshObstacle obstacle = pivot.GetObstacle();
    if (obstacle != null)
      obstacle.enabled = false;
    if ((bool) (Object) rigidbody)
    {
      rigidbody.isKinematic = rigidbodyWasKinematic;
      rigidbody.useGravity = rigidbodyWasGravity;
    }
    if (!(weaponService != null))
      return;
    weaponService.Weapon = npcState.Weapon;
  }

  public void OnAnimatorMove()
  {
    if (failed)
      return;
    float num = animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
    Vector3 vector3 = GameObject.transform.position + animator.deltaPosition;
    if (agent.isActiveAndEnabled)
    {
      long monoHeapSizeLong1 = Profiler.GetMonoHeapSizeLong();
      if (agent.isOnNavMesh)
      {
        agent.nextPosition = vector3;
        GameObject.transform.position = agent.nextPosition;
      }
      else
      {
        long monoHeapSizeLong2 = Profiler.GetMonoHeapSizeLong();
        if (NavMesh.FindClosestEdge(GameObject.transform.position, out NavMeshHit hit, agent.areaMask))
        {
          agent.Warp(hit.position);
          GameObject.transform.position = agent.nextPosition;
          Debug.Log("Agent was teleported to closest edge", agent.gameObject);
        }
        else
        {
          long monoHeapSizeLong3 = Profiler.GetMonoHeapSizeLong();
          if (NavMesh.Raycast(GameObject.transform.position + new Vector3(0.0f, 20f, 0.0f), GameObject.transform.position + new Vector3(0.0f, -20f, 0.0f), out hit, agent.areaMask))
          {
            agent.Warp(hit.position);
            GameObject.transform.position = agent.nextPosition;
            Debug.Log("Agent was teleported (Raycast)", agent.gameObject);
          }
          else
          {
            long monoHeapSizeLong4 = Profiler.GetMonoHeapSizeLong();
            Debug.LogWarning(ObjectInfoUtility.GetStream().Append("Agent was not able to teleport to closest edge and raycast also failed, memory : ").Append(monoHeapSizeLong1).Append(" , ").Append(monoHeapSizeLong2).Append(" , ").Append(monoHeapSizeLong3).Append(" , ").Append(monoHeapSizeLong4), agent.gameObject);
          }
        }
      }
    }
    else
      GameObject.transform.position = vector3;
    GameObject.transform.rotation *= Quaternion.AngleAxis(57.29578f * animator.angularVelocity.y * num, Vector3.up);
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!failed)
      ;
  }

  public void Update()
  {
    if (failed || InstanceByRequest<EngineApplication>.Instance.IsPaused || !sayReplics)
      return;
    timeToNextReplic -= Time.deltaTime;
    if (timeToNextReplic <= 0.0)
    {
      timeToNextReplic = Random.Range(ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
      NPCStateHelper.SayIdleReplic(npcState.Owner);
    }
  }

  public void OnLodStateChanged(bool inLodState)
  {
    npcState.AnimatorEnabled = !inLodState;
    EffectsComponent component = npcState.Owner?.GetComponent<EffectsComponent>();
    if (component == null)
      return;
    component.Disabled = inLodState;
  }
}
