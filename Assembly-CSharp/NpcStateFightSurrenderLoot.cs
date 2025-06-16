using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using System;
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
    if (this.inited)
      return true;
    this.enemy = this.pivot.GetNpcEnemy();
    this.agent = this.pivot.GetAgent();
    this.animator = this.pivot.GetAnimator();
    this.weaponService = this.pivot.GetNpcWeaponService();
    if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("Null animator " + this.GameObject.name), (UnityEngine.Object) this.GameObject);
      Debug.LogError((object) ("Null animator " + this.GameObject.GetFullName()));
      this.failed = true;
      return false;
    }
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    this.failed = false;
    this.inited = true;
    return true;
  }

  private void SetSurrenderValue(bool b)
  {
    if (this.npcState.Owner == null)
      return;
    ParametersComponent component = this.npcState.Owner.GetComponent<ParametersComponent>();
    if (component == null)
      return;
    IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.Surrender);
    if (byName == null)
      return;
    byName.Value = b;
  }

  public NpcStateFightSurrenderLoot(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(float lootTime)
  {
    if (!this.TryInit())
      return;
    this.prevAreaMask = this.agent.areaMask;
    this.agentWasEnabled = this.agent.enabled;
    this.lootTime = lootTime;
    this.lootTimeLeft = lootTime;
    this.weaponService.Weapon = this.npcState.Weapon;
    this.npcState.WeaponChangeEvent += new Action<WeaponEnum>(this.State_WeaponChangeEvent);
    this.Status = NpcStateStatusEnum.Running;
    LocationItemComponent component = (LocationItemComponent) this.npcState.Owner.GetComponent<ILocationItemComponent>();
    if (component == null)
    {
      Debug.LogWarning((object) (this.GameObject.name + ": location component not found"));
      this.Status = NpcStateStatusEnum.Failed;
    }
    else
    {
      bool isIndoor = component.IsIndoor;
      NPCStateHelper.SetAgentAreaMask(this.agent, isIndoor);
      this.agent.enabled = true;
      if (!this.agent.isOnNavMesh)
      {
        Vector3 position = this.GameObject.transform.position;
        if (NavMeshUtility.SampleRaycastPosition(ref position, isIndoor ? 1f : 5f, isIndoor ? 2f : 10f, this.agent.areaMask))
        {
          this.agent.Warp(position);
        }
        else
        {
          Debug.Log((object) "Can't sample navmesh", (UnityEngine.Object) this.GameObject);
          this.Status = NpcStateStatusEnum.Failed;
          return;
        }
      }
      this.enemy.DesiredWalkSpeed = 0.0f;
      this.animatorState.ResetAllTriggers();
      this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
      this.animatorState.SetTrigger("Fight.Triggers/CancelStagger");
      this.animatorState.SetTrigger("Fight.Triggers/TakeMyMoney");
      this.SetSurrenderValue(true);
      if ((UnityEngine.Object) this.enemy.Enemy != (UnityEngine.Object) null)
        this.enemy.RotationTarget = this.enemy.Enemy.transform;
      this.enemy.RotateByPath = false;
      this.enemy.RetreatAngle = new float?();
    }
  }

  private void State_WeaponChangeEvent(WeaponEnum weapon) => this.weaponService.Weapon = weapon;

  public void Shutdown()
  {
    if (this.failed)
      return;
    this.animatorState.ResetAllTriggers();
    this.animatorState.SetTrigger("Fight.Triggers/CancelWalk");
    this.SetSurrenderValue(false);
    this.npcState.WeaponChangeEvent -= new Action<WeaponEnum>(this.State_WeaponChangeEvent);
    this.agent.areaMask = this.prevAreaMask;
    this.agent.enabled = this.agentWasEnabled;
  }

  public void OnAnimatorMove()
  {
    if (this.failed)
      return;
    this.enemy?.OnExternalAnimatorMove();
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (this.failed || InstanceByRequest<EngineApplication>.Instance.IsPaused || this.Status != 0)
      return;
    if ((UnityEngine.Object) this.enemy == (UnityEngine.Object) null || (UnityEngine.Object) this.enemy.Enemy == (UnityEngine.Object) null)
    {
      this.Status = NpcStateStatusEnum.Failed;
    }
    else
    {
      if ((double) this.lootTime != 0.0)
      {
        this.lootTimeLeft -= Time.deltaTime;
        if ((double) this.lootTimeLeft < 0.0)
        {
          this.animator.SetTrigger("Fight.Triggers/CancelWalk");
          this.Status = NpcStateStatusEnum.Success;
          return;
        }
      }
      this.agent.nextPosition = this.animator.rootPosition;
      this.Status = NpcStateStatusEnum.Running;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
