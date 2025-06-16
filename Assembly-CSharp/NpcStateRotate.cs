// Decompiled with JetBrains decompiler
// Type: NpcStateRotate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class NpcStateRotate : INpcState
{
  private NpcStateRotate.ModeEnum mode;
  private NpcState npcState;
  private Pivot pivot;
  private EngineBehavior behavior;
  private NavMeshAgent agent;
  private NPCWeaponService weaponService;
  private Rigidbody rigidbody;
  private bool rigidbodyWasGravity;
  private bool agentWasEnabled;
  private Transform target;
  private Quaternion rotation;
  private bool done;
  [Inspected]
  private bool inited;
  [Inspected]
  private bool failed;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status
  {
    get => this.done ? NpcStateStatusEnum.Success : NpcStateStatusEnum.Running;
  }

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.behavior = this.pivot.GetBehavior();
    this.weaponService = this.pivot.GetNpcWeaponService();
    this.rigidbody = this.pivot.GetRigidbody();
    this.agent = this.pivot.GetAgent();
    if ((UnityEngine.Object) this.agent == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("No navmesh agent " + this.GameObject.name), (UnityEngine.Object) this.GameObject);
      this.failed = true;
      return false;
    }
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateRotate(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(Transform target)
  {
    if (!this.TryInit())
      return;
    this.target = target;
    this.done = false;
    this.agentWasEnabled = this.agent.enabled;
    bool indoor = true;
    if (this.npcState.Owner != null)
    {
      LocationItemComponent component = (LocationItemComponent) this.npcState.Owner.GetComponent<ILocationItemComponent>();
      if (component == null)
      {
        Debug.LogWarning((object) (this.GameObject.name + ": location component not found"));
        return;
      }
      indoor = component.IsIndoor;
    }
    NPCStateHelper.SetAgentAreaMask(this.agent, indoor);
    this.agent.enabled = true;
    if ((UnityEngine.Object) this.rigidbody != (UnityEngine.Object) null)
    {
      this.rigidbodyWasGravity = this.rigidbody.useGravity;
      this.rigidbody.useGravity = false;
    }
    this.mode = NpcStateRotate.ModeEnum.Transform;
    if (!((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null))
      return;
    this.weaponService.Weapon = WeaponEnum.Unknown;
  }

  public void Activate(Quaternion rotation)
  {
    this.rotation = rotation;
    this.done = false;
    this.mode = NpcStateRotate.ModeEnum.Quaternion;
    if (!((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null))
      return;
    this.weaponService.Weapon = WeaponEnum.Unknown;
  }

  public void Shutdown()
  {
    if (this.failed)
      return;
    this.weaponService.Weapon = this.npcState.Weapon;
    if (!(bool) (UnityEngine.Object) this.rigidbody)
      return;
    this.rigidbody.useGravity = this.rigidbodyWasGravity;
  }

  public void OnAnimatorMove()
  {
    if (this.failed)
      return;
    this.behavior?.OnExternalAnimatorMove();
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (this.failed || InstanceByRequest<EngineApplication>.Instance.IsPaused || this.done)
      return;
    Vector3 direction;
    if (this.mode == NpcStateRotate.ModeEnum.Transform)
    {
      if ((UnityEngine.Object) this.target == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "null target");
        this.done = true;
        return;
      }
      direction = this.target.position - this.GameObject.transform.position;
    }
    else
    {
      if (this.mode != NpcStateRotate.ModeEnum.Quaternion)
        throw new NotSupportedException();
      direction = this.rotation * Vector3.forward;
    }
    if ((UnityEngine.Object) this.behavior == (UnityEngine.Object) null)
      return;
    this.done = this.behavior.Rotate(direction);
  }

  public void OnLodStateChanged(bool enabled)
  {
  }

  private enum ModeEnum
  {
    Unknown,
    Transform,
    Quaternion,
  }
}
