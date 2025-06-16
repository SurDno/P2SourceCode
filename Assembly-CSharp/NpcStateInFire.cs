// Decompiled with JetBrains decompiler
// Type: NpcStateInFire
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class NpcStateInFire : INpcState
{
  private NpcState npcState;
  private Pivot pivot;
  private EngineBehavior behavior;
  private NavMeshAgent agent;
  private Animator animator;
  private AnimatorState45 animatorState;
  private int fireLayerIndex;
  private float fireLayerWeight;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.behavior = this.pivot.GetBehavior();
    this.agent = this.pivot.GetAgent();
    this.animator = this.pivot.GetAnimator();
    if ((Object) this.animator == (Object) null)
    {
      Debug.LogError((object) ("Null animator " + this.GameObject.name), (Object) this.GameObject);
      Debug.LogError((object) ("Null animator " + this.GameObject.GetFullName()));
      this.failed = true;
      return false;
    }
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateInFire(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate()
  {
    if (!this.TryInit())
      return;
    this.animatorState.ResetAllTriggers();
    this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    this.fireLayerIndex = this.animator.GetLayerIndex("Fight Fire Layer");
    this.fireLayerWeight = this.animator.GetLayerWeight(this.fireLayerIndex);
    this.agent.enabled = true;
    Pivot component = this.GameObject.GetComponent<Pivot>();
    if (!((Object) component != (Object) null) || !((Object) component.HidingOuterWeapon != (Object) null))
      return;
    component.HidingOuterWeapon.SetActive(false);
  }

  public void Shutdown()
  {
    if (!this.failed)
      ;
  }

  public void OnAnimatorMove()
  {
    if (this.failed)
      return;
    this.behavior.OnExternalAnimatorMove();
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
    this.fireLayerWeight = Mathf.MoveTowards(this.fireLayerWeight, 1f, Time.deltaTime / 0.5f);
    this.animator.SetLayerWeight(this.fireLayerIndex, this.fireLayerWeight);
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
