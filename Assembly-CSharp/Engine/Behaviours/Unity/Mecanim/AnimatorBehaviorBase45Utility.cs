// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Unity.Mecanim.AnimatorBehaviorBase45Utility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Unity.Mecanim
{
  public static class AnimatorBehaviorBase45Utility
  {
    public static float LeftLegStopDistance(Rootmotion45 pivot, float speed)
    {
      float num = speed - 1f;
      return pivot.RetargetLegScale * (float) ((double) pivot.WalkLeftLegStopLength * (1.0 - (double) num) + (double) pivot.RunLeftLegStopLength * (double) num);
    }

    public static float RightLegStopDistance(Rootmotion45 pivot, float speed)
    {
      float num = speed - 1f;
      return pivot.RetargetLegScale * (float) ((double) pivot.WalkRightLegStopLength * (1.0 - (double) num) + (double) pivot.RunRightLegStopLength * (double) num);
    }

    public static bool NeedToStopEnterLeftLeg(
      Rootmotion45 pivot,
      float speed,
      float remainingDistance,
      out float scale)
    {
      float num1 = speed - 1f;
      float num2 = pivot.RetargetLegScale * (float) ((double) pivot.WalkCycleLength * (1.0 - (double) num1) + (double) pivot.RunCycleLength * (double) num1);
      float num3 = AnimatorBehaviorBase45Utility.RightLegStopDistance(pivot, speed);
      float num4 = AnimatorBehaviorBase45Utility.LeftLegStopDistance(pivot, speed);
      float f = remainingDistance - (num2 / 2f + num4);
      if ((double) Mathf.Abs((float) ((double) remainingDistance - (double) num2 / 2.0 - ((double) num2 / 2.0 + (double) num3))) > (double) Mathf.Abs(f))
      {
        scale = remainingDistance / (num2 / 2f + num4);
        return true;
      }
      scale = 1f;
      return false;
    }

    public static bool NeedToStopEnterRightLeg(
      Rootmotion45 pivot,
      float speed,
      float remainingDistance,
      out float scale)
    {
      float num1 = speed - 1f;
      float num2 = pivot.RetargetLegScale * (float) ((double) pivot.WalkCycleLength * (1.0 - (double) num1) + (double) pivot.RunCycleLength * (double) num1);
      float num3 = AnimatorBehaviorBase45Utility.RightLegStopDistance(pivot, speed);
      float num4 = AnimatorBehaviorBase45Utility.LeftLegStopDistance(pivot, speed);
      float f = remainingDistance - (num2 / 2f + num3);
      if ((double) Mathf.Abs((float) ((double) remainingDistance - (double) num2 / 2.0 - ((double) num2 / 2.0 + (double) num4))) > (double) Mathf.Abs(f))
      {
        scale = remainingDistance / (num2 / 2f + num3);
        return true;
      }
      scale = 1f;
      return false;
    }
  }
}
