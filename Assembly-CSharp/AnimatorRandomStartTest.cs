// Decompiled with JetBrains decompiler
// Type: AnimatorRandomStartTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using UnityEngine;

#nullable disable
public class AnimatorRandomStartTest : MonoBehaviour
{
  private void Start()
  {
    Pivot component = this.GetComponent<Pivot>();
    if (!(bool) (Object) component)
      return;
    Animator animator = component.GetAnimator();
    if ((bool) (Object) animator)
      animator.Play(0, 0, (float) Random.Range(0, 1));
  }

  private void Update()
  {
  }
}
