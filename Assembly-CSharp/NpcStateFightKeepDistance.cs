// Decompiled with JetBrains decompiler
// Type: NpcStateFightKeepDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class NpcStateFightKeepDistance : INpcState
{
  private NpcState npcState;
  private Pivot pivot;
  private NPCEnemy enemy;
  private NavMeshAgent agent;
  private Animator animator;
  private AnimatorState45 animatorState;
  private NPCWeaponService weaponService;
  private IKController ikController;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private float keepDistance;
  private bool strafe;
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

  private bool IsEnemyRunningAway()
  {
    return (double) this.enemy.Enemy.Velocity.magnitude >= 0.5 && (double) Vector3.Dot(this.enemy.transform.forward, (this.enemy.Enemy.transform.position - this.enemy.transform.position).normalized) > 0.25;
  }

  public NpcStateFightKeepDistance(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(float keepDistance, bool strafe, bool aim)
  {
    if (!this.TryInit())
      return;
    this.prevAreaMask = this.agent.areaMask;
    this.agentWasEnabled = this.agent.enabled;
    this.keepDistance = keepDistance;
    this.strafe = strafe;
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
      this.enemy.RotationTarget = (Transform) null;
      this.enemy.RotateByPath = false;
      this.enemy.RetreatAngle = new float?();
      if ((UnityEngine.Object) this.enemy.Enemy == (UnityEngine.Object) null)
        return;
      Vector3 forward = this.enemy.Enemy.transform.position - this.enemy.transform.position;
      float magnitude = forward.magnitude;
      forward.Normalize();
      if (this.enemy.IsReacting)
      {
        if (this.enemy.IsAttacking || this.enemy.IsContrReacting)
          this.enemy.transform.rotation = Quaternion.RotateTowards(this.enemy.transform.rotation, Quaternion.AngleAxis(0.0f, Vector3.up) * Quaternion.LookRotation(forward), 270f * Time.deltaTime);
        this.Status = NpcStateStatusEnum.Running;
      }
      else
      {
        float num1;
        if ((double) magnitude < (double) this.keepDistance)
        {
          if (false)
          {
            float? retreatDirection2 = PathfindingHelper.FindBestRetreatDirection2(this.enemy.transform, this.enemy.Enemy.transform);
            if (!retreatDirection2.HasValue)
            {
              num1 = 0.0f;
              this.enemy.RotationTarget = this.enemy.Enemy.transform;
            }
            else
            {
              Quaternion quaternion = Quaternion.LookRotation(forward) * Quaternion.AngleAxis(retreatDirection2.Value, Vector3.up);
              num1 = PathfindingHelper.IsFreeSpace(this.enemy.transform.position, this.enemy.transform.position - this.enemy.transform.forward * 1f) ? -2f : 0.0f;
              this.enemy.RotationTarget = this.enemy.Enemy.transform;
            }
          }
          else
          {
            bool flag = this.strafe && (double) magnitude < 2.0;
            float distance = PathfindingHelper.FindDistance(this.enemy.transform.position, -this.enemy.transform.forward, 10f);
            float num2 = flag ? PathfindingHelper.FindDistance(this.enemy.transform.position, -this.enemy.transform.right, 2.5f) : 0.0f;
            float num3 = flag ? PathfindingHelper.FindDistance(this.enemy.transform.position, this.enemy.transform.right, 2.5f) : 0.0f;
            if ((double) num2 > (double) distance && (double) num2 > (double) num3)
            {
              num1 = 0.0f;
              this.enemy.RotationTarget = this.enemy.Enemy.transform;
              this.animatorState.SetTrigger("Fight.Triggers/StepLeft");
            }
            else if ((double) num3 > (double) distance && (double) num3 > (double) num2)
            {
              num1 = 0.0f;
              this.enemy.RotationTarget = this.enemy.Enemy.transform;
              this.animatorState.SetTrigger("Fight.Triggers/StepRight");
            }
            else
            {
              Quaternion.LookRotation(forward);
              num1 = (double) distance > 1.0 ? -2f : 0.0f;
              this.enemy.RotationTarget = this.enemy.Enemy.transform;
            }
          }
        }
        else if ((double) magnitude < (double) this.keepDistance + 1.0)
        {
          num1 = 0.0f;
          this.enemy.RotationTarget = this.enemy.Enemy.transform;
        }
        else
        {
          num1 = 1f;
          this.enemy.RotationTarget = this.enemy.Enemy.transform;
        }
        this.enemy.DesiredWalkSpeed = num1;
        this.agent.nextPosition = this.animator.rootPosition;
        this.Status = NpcStateStatusEnum.Running;
      }
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
