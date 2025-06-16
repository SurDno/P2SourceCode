// Decompiled with JetBrains decompiler
// Type: Pathologic.Prototype.BullDemoBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Pathologic.Prototype
{
  public class BullDemoBehaviour : StateMachineBehaviour
  {
    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      float num = Random.value;
      if ((double) num < 0.60000002384185791)
        animator.SetInteger("Next", 0);
      else if ((double) num < 0.8)
        animator.SetInteger("Next", 1);
      else
        animator.SetInteger("Next", 2);
    }
  }
}
