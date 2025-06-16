// Decompiled with JetBrains decompiler
// Type: CutsceneAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
public class CutsceneAnimator : MonoBehaviour
{
  [SerializeField]
  private CutsceneAnimator.Item[] items = new CutsceneAnimator.Item[0];

  [Inspected]
  private void Run()
  {
    foreach (CutsceneAnimator.Item obj in this.items)
    {
      if ((UnityEngine.Object) obj.Animator != (UnityEngine.Object) null && !obj.Trigger.IsNullOrEmpty())
        obj.Animator.SetTrigger(obj.Trigger);
    }
  }

  [Serializable]
  public class Item
  {
    public Animator Animator;
    public string Trigger;
  }
}
