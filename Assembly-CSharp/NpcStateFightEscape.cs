// Decompiled with JetBrains decompiler
// Type: NpcStateFightEscape
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
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
public class NpcStateFightEscape : INpcState
{
  private NpcState npcState;
  private Pivot pivot;
  private NPCEnemy enemy;
  private NavMeshAgent agent;
  private Animator animator;
  private AnimatorState45 animatorState;
  protected FightAnimatorBehavior.AnimatorState fightAnimatorState;
  private NPCWeaponService weaponService;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private float escapeDistance;
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
    this.fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(this.animator);
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateFightEscape(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(float escapeDistance)
  {
    if (!this.TryInit())
      return;
    this.prevAreaMask = this.agent.areaMask;
    this.agentWasEnabled = this.agent.enabled;
    this.escapeDistance = escapeDistance;
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
      this.enemy.DesiredWalkSpeed = 0.0f;
      this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
      this.animatorState.ResetTrigger("Fight.Triggers/CancelEscape");
      if ((UnityEngine.Object) this.enemy == (UnityEngine.Object) null)
        Debug.LogError((object) "enemy == null");
      else if ((UnityEngine.Object) this.enemy.Enemy == (UnityEngine.Object) null)
        Debug.LogError((object) "enemy.Enemy == null");
      else if ((UnityEngine.Object) this.enemy.Enemy.transform == (UnityEngine.Object) null)
        Debug.LogError((object) "enemy.Enemy.transform == null");
      else if ((UnityEngine.Object) this.enemy.transform == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "enemy.transform == null");
      }
      else
      {
        if ((double) Vector3.Angle(this.enemy.Enemy.transform.position - this.enemy.transform.position, this.enemy.transform.forward) < 60.0)
          this.animatorState.SetTrigger("Fight.Triggers/Escape");
        else
          this.animatorState.SetTrigger("Fight.Triggers/EscapeImmediate");
        NavMeshHit hit;
        if (this.agent.isOnNavMesh || !NavMesh.SamplePosition(this.enemy.gameObject.transform.position, out hit, 1f, -1))
          return;
        this.agent.Warp(hit.position);
      }
    }
  }

  private void State_WeaponChangeEvent(WeaponEnum weapon) => this.weaponService.Weapon = weapon;

  public void Shutdown()
  {
    if (this.failed)
      return;
    this.animatorState.ResetTrigger("Fight.Triggers/CancelAttack");
    this.animatorState.ResetTrigger("Fight.Triggers/Escape");
    this.animatorState.ResetTrigger("Fight.Triggers/EscapeImmediate");
    this.animatorState.SetTrigger("Fight.Triggers/CancelEscape");
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
      this.enemy.RotationTarget = (Transform) null;
      this.enemy.RotateByPath = false;
      this.enemy.RetreatAngle = new float?();
      if (this.fightAnimatorState.IsReaction)
        return;
      Vector3 vector3 = this.enemy.Enemy.transform.position - this.enemy.transform.position;
      float magnitude = vector3.magnitude;
      if ((double) this.escapeDistance != 0.0 && (double) magnitude > (double) this.escapeDistance)
      {
        this.animatorState.SetTrigger("Fight.Triggers/CancelEscape");
        this.Status = NpcStateStatusEnum.Success;
      }
      else
      {
        vector3 /= magnitude;
        float? retreatDirection2 = PathfindingHelper.FindBestRetreatDirection2(this.enemy.transform, this.enemy.Enemy.transform);
        this.enemy.RotationTarget = this.enemy.Enemy.transform;
        this.enemy.RetreatAngle = retreatDirection2;
        this.agent.nextPosition = this.animator.rootPosition;
      }
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
