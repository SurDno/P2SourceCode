using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;

public class NpcStateFightFollowTarget : INpcState
{
  private NpcState npcState;
  private Pivot pivot;
  private NPCEnemy enemy;
  private NavMeshAgent agent;
  private Animator animator;
  private NPCWeaponService weaponService;
  private Transform target;
  private IKController ikController;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private float stopDistance;
  private float runDistance;
  private float retreatDistance;
  private Vector3 lastPlayerPosition;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status { get; protected set; }

  private bool TryInit()
  {
    if (inited)
      return true;
    enemy = pivot.GetNpcEnemy();
    agent = pivot.GetAgent();
    animator = pivot.GetAnimator();
    weaponService = pivot.GetNpcWeaponService();
    ikController = GameObject.GetComponent<IKController>();
    failed = false;
    inited = true;
    return true;
  }

  public NpcStateFightFollowTarget(NpcState npcState, Pivot pivot)
  {
    GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(
    float stopDistance,
    float runDistance,
    float retreatDistance,
    Transform target,
    bool aim)
  {
    if (!TryInit())
      return;
    prevAreaMask = agent.areaMask;
    agentWasEnabled = agent.enabled;
    this.stopDistance = stopDistance;
    this.runDistance = runDistance;
    this.retreatDistance = retreatDistance;
    this.target = target;
    weaponService.Weapon = npcState.Weapon;
    npcState.WeaponChangeEvent += State_WeaponChangeEvent;
    Status = NpcStateStatusEnum.Running;
    LocationItemComponent component = (LocationItemComponent) npcState.Owner.GetComponent<ILocationItemComponent>();
    if (component == null)
    {
      Debug.LogWarning((object) (GameObject.name + ": location component not found"));
      Status = NpcStateStatusEnum.Failed;
    }
    else
    {
      bool isIndoor = component.IsIndoor;
      NPCStateHelper.SetAgentAreaMask(agent, isIndoor);
      agent.enabled = true;
      if (!agent.isOnNavMesh)
      {
        Vector3 position = GameObject.transform.position;
        if (NavMeshUtility.SampleRaycastPosition(ref position, isIndoor ? 1f : 5f, isIndoor ? 2f : 10f, agent.areaMask))
        {
          agent.Warp(position);
        }
        else
        {
          Status = NpcStateStatusEnum.Failed;
          return;
        }
      }
      if (!((UnityEngine.Object) ikController != (UnityEngine.Object) null & aim))
        return;
      ikController.WeaponTarget = target;
    }
  }

  private void State_WeaponChangeEvent(WeaponEnum weapon) => weaponService.Weapon = weapon;

  public void Shutdown()
  {
    if (failed)
      return;
    npcState.WeaponChangeEvent -= State_WeaponChangeEvent;
    agent.areaMask = prevAreaMask;
    agent.enabled = agentWasEnabled;
    if (!((UnityEngine.Object) ikController != (UnityEngine.Object) null))
      return;
    ikController.WeaponTarget = (Transform) null;
  }

  public void OnAnimatorMove()
  {
    if (failed)
      return;
    enemy?.OnExternalAnimatorMove();
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
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
    {
      agent.nextPosition = GameObject.transform.position;
    }
    else
    {
      UpdatePath();
      enemy.RotationTarget = (Transform) null;
      enemy.RotateByPath = false;
      enemy.RetreatAngle = new float?();
      Vector3 vector3 = target.position - enemy.transform.position;
      float magnitude = vector3.magnitude;
      vector3.Normalize();
      if (enemy.IsAttacking || enemy.IsContrReacting)
      {
        enemy.RotationTarget = target;
        Status = NpcStateStatusEnum.Running;
      }
      else
      {
        float num = 0.0f;
        if (NavMesh.Raycast(enemy.transform.position, target.position, out NavMeshHit _, -1))
        {
          if (!agent.hasPath)
          {
            Status = NpcStateStatusEnum.Running;
            return;
          }
          if ((double) agent.remainingDistance > stopDistance)
          {
            num = (double) agent.remainingDistance > runDistance ? 2f : 1f;
            enemy.RotateByPath = true;
          }
          else if (magnitude < (double) retreatDistance)
            num = -1f;
          enemy.RotationTarget = target;
        }
        else
          num = magnitude <= (double) stopDistance ? (magnitude >= (double) retreatDistance ? 0.0f : -1f) : ((double) agent.remainingDistance > runDistance ? 2f : 1f);
        enemy.RotationTarget = target;
        enemy.DesiredWalkSpeed = num;
        agent.nextPosition = animator.rootPosition;
        Status = NpcStateStatusEnum.Running;
      }
    }
  }

  private void UpdatePath()
  {
    if ((double) UnityEngine.Random.value < (double) Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(agent))
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo(npcState.Owner), (UnityEngine.Object) GameObject);
      Vector3 destination = agent.destination;
      agent.ResetPath();
      agent.SetDestination(destination);
    }
    else
    {
      if ((double) (lastPlayerPosition - target.position).magnitude <= 0.33000001311302185)
        return;
      if (!agent.isOnNavMesh)
        agent.Warp(enemy.transform.position);
      if (agent.isOnNavMesh)
        agent.destination = target.position;
      NavMeshUtility.DrawPath(agent);
      lastPlayerPosition = target.position;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
