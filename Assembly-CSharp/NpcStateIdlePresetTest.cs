using Engine.Behaviours.Components;
using UnityEngine;

public class NpcStateIdlePresetTest : NpcStateIdlePreset
{
  public NpcStateIdlePresetTest(NpcState npcState, Pivot pivot)
    : base(npcState, pivot)
  {
  }

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
