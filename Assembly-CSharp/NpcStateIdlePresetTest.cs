// Decompiled with JetBrains decompiler
// Type: NpcStateIdlePresetTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using UnityEngine;

#nullable disable
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
