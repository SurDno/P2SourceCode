// Decompiled with JetBrains decompiler
// Type: AnimatorEventProxy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
public class AnimatorEventProxy : MonoBehaviour
{
  public event Action AnimatorMoveEvent;

  public event Action<string> AnimatorEventEvent;

  private void OnAnimatorMove()
  {
    Action animatorMoveEvent = this.AnimatorMoveEvent;
    if (animatorMoveEvent == null)
      return;
    animatorMoveEvent();
  }

  private void AnimatorEvent(string name)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(name);
    Action<string> animatorEventEvent = this.AnimatorEventEvent;
    if (animatorEventEvent != null)
      animatorEventEvent(name);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }
}
