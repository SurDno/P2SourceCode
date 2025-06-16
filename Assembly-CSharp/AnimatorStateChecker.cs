// Decompiled with JetBrains decompiler
// Type: AnimatorStateChecker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Inspectors;
using UnityEngine;

#nullable disable
public class AnimatorStateChecker : MonoBehaviour
{
  [Inspected]
  private AnimatorState45 animatorState;

  private void Awake()
  {
    this.animatorState = AnimatorState45.GetAnimatorState(this.GetComponent<Pivot>().GetAnimator());
  }
}
