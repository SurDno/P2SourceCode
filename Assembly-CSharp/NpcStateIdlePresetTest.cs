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
    if (!this.TryInit())
      return;
    this.preset = target;
    this.rigidbody.isKinematic = true;
    this.rigidbody.useGravity = false;
    this.animator.updateMode = AnimatorUpdateMode.Normal;
    this.SetIdle();
  }
}
