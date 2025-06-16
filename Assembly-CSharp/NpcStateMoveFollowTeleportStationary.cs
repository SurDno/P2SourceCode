// Decompiled with JetBrains decompiler
// Type: NpcStateMoveFollowTeleportStationary
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Components.MessangerStationary;
using Inspectors;
using UnityEngine;

#nullable disable
public class NpcStateMoveFollowTeleportStationary : INpcState
{
  private bool inited;
  private bool failed;
  private Pivot pivot;
  private NpcState npcState;
  private bool wasRestartBehaviourAfterTeleport;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateMoveFollowTeleportStationary(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.npcState = npcState;
    this.pivot = pivot;
  }

  public void Activate(float trialTime, SpawnpointKindEnum spawnpointKind)
  {
    if (!this.TryInit())
      return;
    this.wasRestartBehaviourAfterTeleport = this.npcState.RestartBehaviourAfterTeleport;
    Animator animator = this.pivot.GetAnimator();
    if (!((Object) animator != (Object) null))
      return;
    AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(animator);
    if (animatorState != null)
      animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
  }

  public void Shutdown()
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (!this.failed)
      ;
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

  public void OnLodStateChanged(bool enabled)
  {
  }
}
