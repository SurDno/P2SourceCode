using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateFightFollow(NpcState npcState, Pivot pivot) : INpcState 
  {
  private Animator animator;
  private NPCEnemy enemy;
  private NavMeshAgent agent;
  private NPCWeaponService weaponService;
  private IKController ikController;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private float stopDistance;
  private float runDistance;
  private float timeAfterHitToRun;
  private Vector3 lastPlayerPosition;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; protected set; } = npcState.gameObject;

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

  private bool IsEnemyRunningAway()
  {
    return enemy.Enemy.Velocity.magnitude >= 0.5 && Vector3.Dot(enemy.transform.forward, (enemy.Enemy.transform.position - enemy.transform.position).normalized) > 0.25;
  }

  public void Activate(float stopDistance, float runDistance, bool aim, float timeAfterHitToRun = 7f)
  {
    if (!TryInit())
      return;
    prevAreaMask = agent.areaMask;
    agentWasEnabled = agent.enabled;
    this.stopDistance = stopDistance;
    this.runDistance = runDistance;
    this.timeAfterHitToRun = timeAfterHitToRun;
    weaponService.Weapon = npcState.Weapon;
    npcState.WeaponChangeEvent += State_WeaponChangeEvent;
    Status = NpcStateStatusEnum.Running;
    LocationItemComponent component = (LocationItemComponent) npcState.Owner.GetComponent<ILocationItemComponent>();
    if (component == null)
    {
      Debug.LogWarning(GameObject.name + ": location component not found");
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
      if (FightAnimatorBehavior.GetAnimatorState(animator).IsReaction)
        weaponService.SwitchWeaponOnImmediate();
      if (!(ikController != null & aim) || !(enemy != null) || !(enemy.Enemy != null))
        return;
      ikController.WeaponTarget = enemy.Enemy.transform;
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
    if (!(ikController != null))
      return;
    ikController.WeaponTarget = null;
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
      enemy.RotationTarget = null;
      enemy.RotateByPath = false;
      enemy.RetreatAngle = new float?();
      if (enemy.IsReacting || enemy.IsQuickBlock)
      {
        if (enemy.IsContrReacting && enemy.CounterReactionEnemy != null)
          enemy.RotationTarget = enemy.CounterReactionEnemy.transform;
        else if (enemy.IsQuickBlock && enemy.PrePunchEnemy != null)
          enemy.RotationTarget = enemy.PrePunchEnemy.transform;
        enemy.DesiredWalkSpeed = 0.0f;
        Status = NpcStateStatusEnum.Running;
      }
      else
      {
        if (enemy.Enemy == null)
          return;
        Vector3 vector3 = enemy.Enemy.transform.position - enemy.transform.position;
        float magnitude = vector3.magnitude;
        vector3.Normalize();
        if (enemy.IsAttacking || enemy.IsContrReacting)
        {
          enemy.RotationTarget = enemy.Enemy.transform;
          Status = NpcStateStatusEnum.Running;
        }
        else
        {
          float num = 0.0f;
          bool flag = enemy.TimeFromLastHit > (double) timeAfterHitToRun;
          if (NavMesh.Raycast(enemy.transform.position, enemy.Enemy.transform.position, out NavMeshHit _, -1))
          {
            if (!agent.hasPath || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
            {
              Status = NpcStateStatusEnum.Running;
              return;
            }
            if (agent.remainingDistance > (double) stopDistance)
            {
              num = !IsEnemyRunningAway() ? (agent.remainingDistance > (double) runDistance | flag ? 2f : 1f) : 2f;
              enemy.RotationTarget = enemy.Enemy.transform;
              enemy.RotateByPath = true;
              enemy.RetreatAngle = new float?();
            }
          }
          else if (magnitude > (double) stopDistance)
          {
            num = !IsEnemyRunningAway() ? (((!agent.hasPath || !agent.isActiveAndEnabled || !agent.isOnNavMesh ? 0 : (agent.remainingDistance > (double) runDistance ? 1 : 0)) | (flag ? 1 : 0)) != 0 ? 2f : 1f) : 2f;
            enemy.RotationTarget = enemy.Enemy.transform;
          }
          else
          {
            num = 0.0f;
            if (enemy.IsContrReacting || !enemy.IsReacting)
              enemy.RotationTarget = enemy.Enemy.transform;
            if (enemy.IsAttacking)
              enemy.RotationTarget = enemy.Enemy.transform;
          }
          enemy.DesiredWalkSpeed = num;
          agent.nextPosition = animator.rootPosition;
          Status = NpcStateStatusEnum.Running;
        }
      }
    }
  }

  private void UpdatePath()
  {
    if (enemy.Enemy == null)
      return;
    if (Random.value < Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(agent))
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo(npcState.Owner), GameObject);
      Vector3 destination = agent.destination;
      agent.ResetPath();
      agent.SetDestination(destination);
    }
    else
    {
      if ((lastPlayerPosition - enemy.Enemy.transform.position).magnitude <= 0.33000001311302185)
        return;
      if (!agent.isOnNavMesh)
        agent.Warp(enemy.transform.position);
      if (agent.isOnNavMesh)
        agent.destination = enemy.Enemy.transform.position;
      NavMeshUtility.DrawPath(agent);
      lastPlayerPosition = enemy.Enemy.transform.position;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
