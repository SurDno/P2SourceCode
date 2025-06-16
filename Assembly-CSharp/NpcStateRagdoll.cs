// Decompiled with JetBrains decompiler
// Type: NpcStateRagdoll
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Source.Commons;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class NpcStateRagdoll : INpcState, INpcStateRagdoll
{
  private NpcState npcState;
  private NPCEnemy enemy;
  private Behaviour behavior;
  private NavMeshAgent agent;
  private Pivot pivot;
  private Rigidbody rigidbody;
  private Animator animator;
  private AnimatorState45 animatorState;
  private NPCWeaponService weaponService;
  private CapsuleCollider collider;
  private bool initiallyKinematic;
  private bool initiallyColliderEnabled;
  private int ragdollLayerIndex;
  private bool inited;
  private bool failed;
  private bool agentWasEnabled;
  private float initialRagdollLayerWeight;
  private float initialRagdollWeight;
  private float actualRagdollWeight;
  private AnimatorUpdateMode initialAnimatorUpdateMode;
  private AnimatorCullingMode initialAnimatorCullingMode;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status { get; private set; }

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.enemy = this.pivot.GetNpcEnemy();
    this.behavior = (Behaviour) this.pivot.GetBehavior();
    this.rigidbody = this.pivot.GetRigidbody();
    this.collider = this.GameObject.GetComponent<CapsuleCollider>();
    this.animator = this.pivot.GetAnimator();
    this.weaponService = this.pivot.GetNpcWeaponService();
    this.agent = this.pivot.GetAgent();
    if ((UnityEngine.Object) this.agent == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("No navmesh agent " + this.GameObject.name), (UnityEngine.Object) this.GameObject);
      this.failed = true;
      return false;
    }
    if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("Null animator " + this.GameObject.name), (UnityEngine.Object) this.GameObject);
      Debug.LogError((object) ("Null animator " + this.GameObject.GetFullName()));
      this.failed = true;
      return false;
    }
    this.ragdollLayerIndex = this.animator.GetLayerIndex("Ragdoll Layer");
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateRagdoll(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(bool internalCollisions, INpcState previousState)
  {
    if (!this.TryInit())
      return;
    if ((bool) (UnityEngine.Object) this.rigidbody)
      this.initiallyKinematic = this.rigidbody.isKinematic;
    if ((bool) (UnityEngine.Object) this.collider)
      this.initiallyColliderEnabled = this.collider.enabled;
    if ((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null)
      this.weaponService.Weapon = this.npcState.Weapon;
    this.npcState.WeaponChangeEvent += new Action<WeaponEnum>(this.State_WeaponChangeEvent);
    this.Status = NpcStateStatusEnum.Running;
    if ((bool) (UnityEngine.Object) this.rigidbody)
      this.rigidbody.isKinematic = true;
    if ((bool) (UnityEngine.Object) this.collider)
      this.collider.enabled = false;
    if (this.pivot.HierarhyStructure == Pivot.HierarhyStructureEnum.PuppetMaster)
      this.animatorState.SetTrigger("Triggers/Ragdoll");
    this.initialRagdollWeight = this.pivot.RagdollWeight;
    if (previousState is INpcStateRagdoll)
    {
      this.pivot.RagdollWeight = (previousState as INpcStateRagdoll).GetActualRagdollWeight();
      this.actualRagdollWeight = this.pivot.RagdollWeight;
    }
    else
      this.pivot.RagdollWeight = Mathf.Max(this.pivot.RagdollWeight, 0.5f);
    this.pivot.RgdollInternalCollisions = internalCollisions;
    this.initialAnimatorUpdateMode = this.animator.updateMode;
    this.animator.updateMode = AnimatorUpdateMode.Normal;
    this.initialAnimatorCullingMode = this.animator.cullingMode;
    this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    this.agentWasEnabled = this.agent.enabled;
    this.agent.enabled = true;
    this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    this.animatorState.SetTrigger("Fight.Triggers/CancelEscape");
    if (!(bool) (UnityEngine.Object) this.enemy)
      return;
    this.enemy.RotationTarget = (Transform) null;
    this.enemy.RotateByPath = false;
  }

  private void State_WeaponChangeEvent(WeaponEnum weapon)
  {
    if (!((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null))
      return;
    this.weaponService.Weapon = weapon;
  }

  public void Shutdown()
  {
    if (this.failed)
      return;
    if ((bool) (UnityEngine.Object) this.rigidbody)
      this.rigidbody.isKinematic = this.initiallyKinematic;
    if ((bool) (UnityEngine.Object) this.collider)
      this.collider.enabled = this.initiallyColliderEnabled;
    if (this.pivot.HierarhyStructure == Pivot.HierarhyStructureEnum.PuppetMaster)
      this.animator.SetLayerWeight(this.ragdollLayerIndex, this.initialRagdollLayerWeight);
    this.pivot.RagdollWeight = this.initialRagdollWeight;
    this.agent.enabled = this.agentWasEnabled;
    this.animator.updateMode = this.initialAnimatorUpdateMode;
    this.animator.cullingMode = this.initialAnimatorCullingMode;
  }

  public void OnAnimatorMove()
  {
    if (!this.failed)
      ;
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
    this.pivot.RagdollWeight = Mathf.MoveTowards(this.pivot.RagdollWeight, 1f, Time.deltaTime / 0.5f);
    this.actualRagdollWeight = this.pivot.RagdollWeight;
    if (!this.animator.isActiveAndEnabled || !this.agent.isActiveAndEnabled)
      return;
    Vector3 vector3 = this.GameObject.transform.position + this.animator.deltaPosition * (1f - this.pivot.RagdollWeight);
    vector3.y = Mathf.MoveTowards(vector3.y, this.agent.nextPosition.y, Time.deltaTime * 0.1f);
    this.agent.nextPosition = vector3;
    this.GameObject.transform.position = this.agent.nextPosition;
  }

  public float GetActualRagdollWeight() => this.actualRagdollWeight;

  public void OnLodStateChanged(bool enabled)
  {
  }
}
