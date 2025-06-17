using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Components.MessangerStationary;
using Inspectors;
using UnityEngine;

public class NpcStateMoveFollowTeleportStationary(NpcState npcState, Pivot pivot) : INpcState 
  {
  private bool inited;
  private bool failed;
  private bool wasRestartBehaviourAfterTeleport;

  public GameObject GameObject { get; private set; } = npcState.gameObject;

  [Inspected]
  public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

  private bool TryInit()
  {
    if (inited)
      return true;
    failed = false;
    inited = true;
    return true;
  }

  public void Activate(float trialTime, SpawnpointKindEnum spawnpointKind)
  {
    if (!TryInit())
      return;
    wasRestartBehaviourAfterTeleport = npcState.RestartBehaviourAfterTeleport;
    Animator animator = pivot.GetAnimator();
    if (!(animator != null))
      return;
    AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(animator);
    if (animatorState != null)
      animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
  }

  public void Shutdown()
  {
    if (!failed)
      ;
  }

  public void Update()
  {
    if (!failed)
      ;
  }

  public void OnAnimatorMove()
  {
    if (!failed)
      ;
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!failed)
      ;
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
