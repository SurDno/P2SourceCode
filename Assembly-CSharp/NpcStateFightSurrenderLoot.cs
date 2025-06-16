using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateFightSurrenderLoot : INpcState
{
  private NpcState npcState;
  private Pivot pivot;
  private NPCEnemy enemy;
  private NavMeshAgent agent;
  private Animator animator;
  private AnimatorState45 animatorState;
  private NPCWeaponService weaponService;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private float lootTime;
  private float lootTimeLeft;
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

  private void SetSurrenderValue(bool b)
  {
    if (npcState.Owner == null)
      return;
    ParametersComponent component = npcState.Owner.GetComponent<ParametersComponent>();
    if (component == null)
      return;
    IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.Surrender);
    if (byName == null)
      return;
    byName.Value = b;
  }

  public NpcStateFightSurrenderLoot(NpcState npcState, Pivot pivot)
  {
    GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(float lootTime)
  {
    if (!TryInit())
      return;
    prevAreaMask = agent.areaMask;
    agentWasEnabled = agent.enabled;
    this.lootTime = lootTime;
    lootTimeLeft = lootTime;
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
          Debug.Log("Can't sample navmesh", GameObject);
          Status = NpcStateStatusEnum.Failed;
          return;
        }
      }
      enemy.DesiredWalkSpeed = 0.0f;
      animatorState.ResetAllTriggers();
      animatorState.SetTrigger("Fight.Triggers/CancelAttack");
      animatorState.SetTrigger("Fight.Triggers/CancelStagger");
      animatorState.SetTrigger("Fight.Triggers/TakeMyMoney");
      SetSurrenderValue(true);
      if (enemy.Enemy != null)
        enemy.RotationTarget = enemy.Enemy.transform;
      enemy.RotateByPath = false;
      enemy.RetreatAngle = new float?();
    }
  }

  private void State_WeaponChangeEvent(WeaponEnum weapon) => weaponService.Weapon = weapon;

  public void Shutdown()
  {
    if (failed)
      return;
    animatorState.ResetAllTriggers();
    animatorState.SetTrigger("Fight.Triggers/CancelWalk");
    SetSurrenderValue(false);
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
      if (lootTime != 0.0)
      {
        lootTimeLeft -= Time.deltaTime;
        if (lootTimeLeft < 0.0)
        {
          animator.SetTrigger("Fight.Triggers/CancelWalk");
          Status = NpcStateStatusEnum.Success;
          return;
        }
      }
      agent.nextPosition = animator.rootPosition;
      Status = NpcStateStatusEnum.Running;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
