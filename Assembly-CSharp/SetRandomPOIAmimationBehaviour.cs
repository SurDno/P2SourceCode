// Decompiled with JetBrains decompiler
// Type: SetRandomPOIAmimationBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SetRandomPOIAmimationBehaviour : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    int num = Random.Range(0, animator.GetInteger("Movable.POI.MiddleAnimationsCount"));
    animator.SetInteger("Movable.POI.AnimationIndex2", num);
  }
}
