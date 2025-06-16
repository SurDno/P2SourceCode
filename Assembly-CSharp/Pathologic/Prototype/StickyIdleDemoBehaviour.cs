// Decompiled with JetBrains decompiler
// Type: Pathologic.Prototype.StickyIdleDemoBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Pathologic.Prototype
{
  public class StickyIdleDemoBehaviour : StateMachineBehaviour
  {
    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      if ((double) Random.value < 0.5)
      {
        animator.SetInteger("Next", 0);
      }
      else
      {
        int num = Random.Range(1, 7);
        animator.SetInteger("Next", num);
      }
    }
  }
}
