using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateFightSurrender(NpcState npcState, Pivot pivot) : INpcState 
  {
  private NPCEnemy enemy;
  private NavMeshAgent agent;
  private Animator animator;
  private AnimatorState45 animatorState;
  private NPCWeaponService weaponService;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private float moveTime;
  private float moveTimeLeft;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; private set; } = npcState.gameObject;

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

  public void Activate(float moveTime)
  {
    if (!TryInit())
      return;
    prevAreaMask = agent.areaMask;
    agentWasEnabled = agent.enabled;
    this.moveTime = moveTime;
    moveTimeLeft = moveTime;
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
      enemy.DesiredWalkSpeed = 0.0f;
      animatorState.ResetAllTriggers();
      animatorState.SetTrigger("Fight.Triggers/CancelAttack");
      animatorState.SetTrigger("Fight.Triggers/CancelStagger");
      animatorState.SetTrigger("Fight.Triggers/Surrender");
    }
  }

  private void State_WeaponChangeEvent(WeaponEnum weapon) => weaponService.Weapon = weapon;

  public void Shutdown()
  {
    if (failed)
      return;
    animatorState.SetTrigger("Fight.Triggers/CancelWalk");
    npcState.WeaponChangeEvent -= State_WeaponChangeEvent;
    agent.areaMask = prevAreaMask;
    agent.enabled = agentWasEnabled;
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
    if (failed || InstanceByRequest<EngineApplication>.Instance.IsPaused || Status != 0)
      return;
    if (enemy == null || enemy.Enemy == null)
    {
      Status = NpcStateStatusEnum.Failed;
    }
    else
    {
      enemy.RotationTarget = enemy.Enemy.transform;
      enemy.RotateByPath = false;
      enemy.RetreatAngle = new float?();
      enemy.DesiredWalkSpeed = -Mathf.Clamp01((float) ((PathfindingHelper.GetSurrenderRange(enemy.transform.position, -enemy.transform.forward) - 0.5) / 1.0));
      if (moveTime != 0.0)
      {
        moveTimeLeft -= Time.deltaTime;
        if (moveTimeLeft < 0.0)
        {
          Status = NpcStateStatusEnum.Success;
          return;
        }
      }
      agent.nextPosition = animator.rootPosition;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
