using Engine.Behaviours.Components;
using UnityEngine;

public class NpcStateIdlePresetTest(NpcState npcState, Pivot pivot) : NpcStateIdlePreset(npcState, pivot) {
  public void Activate(IdlePresetObject target)
  {
    if (!TryInit())
      return;
    preset = target;
    rigidbody.isKinematic = true;
    rigidbody.useGravity = false;
    animator.updateMode = AnimatorUpdateMode.Normal;
    SetIdle();
  }
}
