// Decompiled with JetBrains decompiler
// Type: MovementControllerUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class MovementControllerUtility
{
  public static void SetRandomAnimation(
    Animator animator,
    int secondaryIdleAnimationCount,
    int secondaryLowIdleAnimationCount)
  {
    int num1 = 5;
    int num2 = 3;
    int num3 = Random.Range(0, secondaryIdleAnimationCount * num2 + secondaryLowIdleAnimationCount);
    int num4 = num3 >= secondaryIdleAnimationCount * num2 ? num1 + (num3 - secondaryIdleAnimationCount * num2) : num3 / num2;
    animator.SetInteger("Movable.Idle.AnimationControl", num4);
  }
}
