// Decompiled with JetBrains decompiler
// Type: NpcStateFightIdle
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
public class NpcStateFightIdle : INpcState
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

  public NpcStateFightIdle(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(bool aim)
  {
    if (!this.TryInit())
      return;
    this.prevAreaMask = this.agent.areaMask;
    this.agentWasEnabled = this.agent.enabled;
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
    if (this.failed || InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    this.enemy.DesiredWalkSpeed = 0.0f;
    this.agent.nextPosition = this.animator.rootPosition;
    this.Status = NpcStateStatusEnum.Running;
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
