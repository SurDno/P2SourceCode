// Decompiled with JetBrains decompiler
// Type: NpcStateFightSurrender
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
public class NpcStateFightSurrender : INpcState
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
  private float moveTime;
  private float moveTimeLeft;
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

  public NpcStateFightSurrender(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(float moveTime)
  {
    if (!this.TryInit())
      return;
    this.prevAreaMask = this.agent.areaMask;
    this.agentWasEnabled = this.agent.enabled;
    this.moveTime = moveTime;
    this.moveTimeLeft = moveTime;
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
      this.animatorState.ResetAllTriggers();
      this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
      this.animatorState.SetTrigger("Fight.Triggers/CancelStagger");
      this.animatorState.SetTrigger("Fight.Triggers/Surrender");
    }
  }

  private void State_WeaponChangeEvent(WeaponEnum weapon) => this.weaponService.Weapon = weapon;

  public void Shutdown()
  {
    if (this.failed)
      return;
    this.animatorState.SetTrigger("Fight.Triggers/CancelWalk");
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
      this.enemy.RotationTarget = this.enemy.Enemy.transform;
      this.enemy.RotateByPath = false;
      this.enemy.RetreatAngle = new float?();
      this.enemy.DesiredWalkSpeed = -Mathf.Clamp01((float) (((double) PathfindingHelper.GetSurrenderRange(this.enemy.transform.position, -this.enemy.transform.forward) - 0.5) / 1.0));
      if ((double) this.moveTime != 0.0)
      {
        this.moveTimeLeft -= Time.deltaTime;
        if ((double) this.moveTimeLeft < 0.0)
        {
          this.Status = NpcStateStatusEnum.Success;
          return;
        }
      }
      this.agent.nextPosition = this.animator.rootPosition;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
