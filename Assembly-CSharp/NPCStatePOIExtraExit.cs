// Decompiled with JetBrains decompiler
// Type: NPCStatePOIExtraExit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using UnityEngine;

#nullable disable
public class NPCStatePOIExtraExit : INpcState
{
  private Animator animator;
  private NpcState npcState;
  private Pivot pivot;
  private Rigidbody rigidbody;
  private AnimatorState45 animatorState;
  private bool initiallyKinematic;
  private bool failed;
  private bool inited = false;
  private bool complete;
  private float exitTime;

  public GameObject GameObject { get; private set; }

  public NPCStatePOIExtraExit(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.animator = this.pivot.GetAnimator();
    if ((Object) this.animator == (Object) null)
    {
      Debug.LogError((object) ("Null animator " + this.GameObject.name), (Object) this.GameObject);
      Debug.LogError((object) ("Null animator " + this.GameObject.GetFullName()));
      this.failed = true;
      return false;
    }
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    this.rigidbody = this.pivot.GetRigidbody();
    this.failed = false;
    this.inited = true;
    return true;
  }

  public void Activate(INpcState previousState, float time)
  {
    if (!this.TryInit())
      return;
    if (!(previousState is INpcStateNeedSyncBack))
    {
      this.complete = true;
    }
    else
    {
      this.exitTime = time;
      bool flag = this.pivot.HasFastPOIExit((previousState as INpcStateNeedSyncBack).GetPoiType());
      this.animatorState.ResetAllTriggers();
      this.animatorState.SetTrigger(flag ? "Triggers/POIExtraExit" : "Triggers/POIExtraExitNoAnimation");
      this.complete = false;
      if (!(bool) (Object) this.rigidbody)
        return;
      this.initiallyKinematic = this.rigidbody.isKinematic;
      this.rigidbody.isKinematic = true;
    }
  }

  public NpcStateStatusEnum Status
  {
    get => this.complete ? NpcStateStatusEnum.Success : NpcStateStatusEnum.Running;
  }

  public void Shutdown()
  {
    if (!(bool) (Object) this.rigidbody)
      return;
    this.rigidbody.isKinematic = this.initiallyKinematic;
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
  }

  public void OnAnimatorMove()
  {
    if (this.failed || this.complete)
      return;
    float num = this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
    if (!this.animatorState.IsPOI && !this.animatorState.IsPOIExit)
      this.animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
    this.exitTime -= num;
    if ((double) this.exitTime <= 0.0 && !this.animatorState.IsPOI && !this.animatorState.IsPOIExit)
    {
      this.complete = true;
    }
    else
    {
      this.GameObject.transform.position += this.animator.deltaPosition;
      this.GameObject.transform.rotation *= this.animator.deltaRotation;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
