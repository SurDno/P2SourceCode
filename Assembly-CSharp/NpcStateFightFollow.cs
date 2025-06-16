// Decompiled with JetBrains decompiler
// Type: NpcStateFightFollow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class NpcStateFightFollow : INpcState
{
  private Pivot pivot;
  private Animator animator;
  private NpcState npcState;
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

  public GameObject GameObject { get; protected set; }

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
    this.ikController = this.GameObject.GetComponent<IKController>();
    this.failed = false;
    this.inited = true;
    return true;
  }

  private bool IsEnemyRunningAway()
  {
    return (double) this.enemy.Enemy.Velocity.magnitude >= 0.5 && (double) Vector3.Dot(this.enemy.transform.forward, (this.enemy.Enemy.transform.position - this.enemy.transform.position).normalized) > 0.25;
  }

  public NpcStateFightFollow(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(float stopDistance, float runDistance, bool aim, float timeAfterHitToRun = 7f)
  {
    if (!this.TryInit())
      return;
    this.prevAreaMask = this.agent.areaMask;
    this.agentWasEnabled = this.agent.enabled;
    this.stopDistance = stopDistance;
    this.runDistance = runDistance;
    this.timeAfterHitToRun = timeAfterHitToRun;
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
          this.Status = NpcStateStatusEnum.Failed;
          return;
        }
      }
      if (FightAnimatorBehavior.GetAnimatorState(this.animator).IsReaction)
        this.weaponService.SwitchWeaponOnImmediate();
      if (!((UnityEngine.Object) this.ikController != (UnityEngine.Object) null & aim) || !((UnityEngine.Object) this.enemy != (UnityEngine.Object) null) || !((UnityEngine.Object) this.enemy.Enemy != (UnityEngine.Object) null))
        return;
      this.ikController.WeaponTarget = this.enemy.Enemy.transform;
    }
  }

  private void State_WeaponChangeEvent(WeaponEnum weapon) => this.weaponService.Weapon = weapon;

  public void Shutdown()
  {
    if (this.failed)
      return;
    this.npcState.WeaponChangeEvent -= new Action<WeaponEnum>(this.State_WeaponChangeEvent);
    this.agent.areaMask = this.prevAreaMask;
    this.agent.enabled = this.agentWasEnabled;
    if (!((UnityEngine.Object) this.ikController != (UnityEngine.Object) null))
      return;
    this.ikController.WeaponTarget = (Transform) null;
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
    if (this.failed)
      return;
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
    {
      this.agent.nextPosition = this.GameObject.transform.position;
    }
    else
    {
      this.UpdatePath();
      this.enemy.RotationTarget = (Transform) null;
      this.enemy.RotateByPath = false;
      this.enemy.RetreatAngle = new float?();
      if (this.enemy.IsReacting || this.enemy.IsQuickBlock)
      {
        if (this.enemy.IsContrReacting && (UnityEngine.Object) this.enemy.CounterReactionEnemy != (UnityEngine.Object) null)
          this.enemy.RotationTarget = this.enemy.CounterReactionEnemy.transform;
        else if (this.enemy.IsQuickBlock && (UnityEngine.Object) this.enemy.PrePunchEnemy != (UnityEngine.Object) null)
          this.enemy.RotationTarget = this.enemy.PrePunchEnemy.transform;
        this.enemy.DesiredWalkSpeed = 0.0f;
        this.Status = NpcStateStatusEnum.Running;
      }
      else
      {
        if ((UnityEngine.Object) this.enemy.Enemy == (UnityEngine.Object) null)
          return;
        Vector3 vector3 = this.enemy.Enemy.transform.position - this.enemy.transform.position;
        float magnitude = vector3.magnitude;
        vector3.Normalize();
        if (this.enemy.IsAttacking || this.enemy.IsContrReacting)
        {
          this.enemy.RotationTarget = this.enemy.Enemy.transform;
          this.Status = NpcStateStatusEnum.Running;
        }
        else
        {
          float num = 0.0f;
          bool flag = (double) this.enemy.TimeFromLastHit > (double) this.timeAfterHitToRun;
          if (NavMesh.Raycast(this.enemy.transform.position, this.enemy.Enemy.transform.position, out NavMeshHit _, -1))
          {
            if (!this.agent.hasPath || !this.agent.isActiveAndEnabled || !this.agent.isOnNavMesh)
            {
              this.Status = NpcStateStatusEnum.Running;
              return;
            }
            if ((double) this.agent.remainingDistance > (double) this.stopDistance)
            {
              num = !this.IsEnemyRunningAway() ? ((double) this.agent.remainingDistance > (double) this.runDistance | flag ? 2f : 1f) : 2f;
              this.enemy.RotationTarget = this.enemy.Enemy.transform;
              this.enemy.RotateByPath = true;
              this.enemy.RetreatAngle = new float?();
            }
          }
          else if ((double) magnitude > (double) this.stopDistance)
          {
            num = !this.IsEnemyRunningAway() ? (((!this.agent.hasPath || !this.agent.isActiveAndEnabled || !this.agent.isOnNavMesh ? 0 : ((double) this.agent.remainingDistance > (double) this.runDistance ? 1 : 0)) | (flag ? 1 : 0)) != 0 ? 2f : 1f) : 2f;
            this.enemy.RotationTarget = this.enemy.Enemy.transform;
          }
          else
          {
            num = 0.0f;
            if (this.enemy.IsContrReacting || !this.enemy.IsReacting)
              this.enemy.RotationTarget = this.enemy.Enemy.transform;
            if (this.enemy.IsAttacking)
              this.enemy.RotationTarget = this.enemy.Enemy.transform;
          }
          this.enemy.DesiredWalkSpeed = num;
          this.agent.nextPosition = this.animator.rootPosition;
          this.Status = NpcStateStatusEnum.Running;
        }
      }
    }
  }

  private void UpdatePath()
  {
    if ((UnityEngine.Object) this.enemy.Enemy == (UnityEngine.Object) null)
      return;
    if ((double) UnityEngine.Random.value < (double) Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(this.agent))
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo((object) this.npcState.Owner), (UnityEngine.Object) this.GameObject);
      Vector3 destination = this.agent.destination;
      this.agent.ResetPath();
      this.agent.SetDestination(destination);
    }
    else
    {
      if ((double) (this.lastPlayerPosition - this.enemy.Enemy.transform.position).magnitude <= 0.33000001311302185)
        return;
      if (!this.agent.isOnNavMesh)
        this.agent.Warp(this.enemy.transform.position);
      if (this.agent.isOnNavMesh)
        this.agent.destination = this.enemy.Enemy.transform.position;
      NavMeshUtility.DrawPath(this.agent);
      this.lastPlayerPosition = this.enemy.Enemy.transform.position;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
