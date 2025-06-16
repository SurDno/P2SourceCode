// Decompiled with JetBrains decompiler
// Type: NpcStateFightFollowTarget
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

  public NpcStateFightFollowTarget(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
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
    if (!this.TryInit())
      return;
    this.prevAreaMask = this.agent.areaMask;
    this.agentWasEnabled = this.agent.enabled;
    this.stopDistance = stopDistance;
    this.runDistance = runDistance;
    this.retreatDistance = retreatDistance;
    this.target = target;
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
      if (!((UnityEngine.Object) this.ikController != (UnityEngine.Object) null & aim))
        return;
      this.ikController.WeaponTarget = target;
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
      Vector3 vector3 = this.target.position - this.enemy.transform.position;
      float magnitude = vector3.magnitude;
      vector3.Normalize();
      if (this.enemy.IsAttacking || this.enemy.IsContrReacting)
      {
        this.enemy.RotationTarget = this.target;
        this.Status = NpcStateStatusEnum.Running;
      }
      else
      {
        float num = 0.0f;
        if (NavMesh.Raycast(this.enemy.transform.position, this.target.position, out NavMeshHit _, -1))
        {
          if (!this.agent.hasPath)
          {
            this.Status = NpcStateStatusEnum.Running;
            return;
          }
          if ((double) this.agent.remainingDistance > (double) this.stopDistance)
          {
            num = (double) this.agent.remainingDistance > (double) this.runDistance ? 2f : 1f;
            this.enemy.RotateByPath = true;
          }
          else if ((double) magnitude < (double) this.retreatDistance)
            num = -1f;
          this.enemy.RotationTarget = this.target;
        }
        else
          num = (double) magnitude <= (double) this.stopDistance ? ((double) magnitude >= (double) this.retreatDistance ? 0.0f : -1f) : ((double) this.agent.remainingDistance > (double) this.runDistance ? 2f : 1f);
        this.enemy.RotationTarget = this.target;
        this.enemy.DesiredWalkSpeed = num;
        this.agent.nextPosition = this.animator.rootPosition;
        this.Status = NpcStateStatusEnum.Running;
      }
    }
  }

  private void UpdatePath()
  {
    if ((double) UnityEngine.Random.value < (double) Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(this.agent))
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo((object) this.npcState.Owner), (UnityEngine.Object) this.GameObject);
      Vector3 destination = this.agent.destination;
      this.agent.ResetPath();
      this.agent.SetDestination(destination);
    }
    else
    {
      if ((double) (this.lastPlayerPosition - this.target.position).magnitude <= 0.33000001311302185)
        return;
      if (!this.agent.isOnNavMesh)
        this.agent.Warp(this.enemy.transform.position);
      if (this.agent.isOnNavMesh)
        this.agent.destination = this.target.position;
      NavMeshUtility.DrawPath(this.agent);
      this.lastPlayerPosition = this.target.position;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
