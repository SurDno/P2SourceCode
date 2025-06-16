namespace Engine.Behaviours.Unity.Mecanim
{
  public static class AnimatorBehaviorBase45Utility
  {
    public static float LeftLegStopDistance(Rootmotion45 pivot, float speed)
    {
      float num = speed - 1f;
      return pivot.RetargetLegScale * (float) (pivot.WalkLeftLegStopLength * (1.0 - num) + pivot.RunLeftLegStopLength * (double) num);
    }

    public static float RightLegStopDistance(Rootmotion45 pivot, float speed)
    {
      float num = speed - 1f;
      return pivot.RetargetLegScale * (float) (pivot.WalkRightLegStopLength * (1.0 - num) + pivot.RunRightLegStopLength * (double) num);
    }

    public static bool NeedToStopEnterLeftLeg(
      Rootmotion45 pivot,
      float speed,
      float remainingDistance,
      out float scale)
    {
      float num1 = speed - 1f;
      float num2 = pivot.RetargetLegScale * (float) (pivot.WalkCycleLength * (1.0 - num1) + pivot.RunCycleLength * (double) num1);
      float num3 = RightLegStopDistance(pivot, speed);
      float num4 = LeftLegStopDistance(pivot, speed);
      float f = remainingDistance - (num2 / 2f + num4);
      if ((double) Mathf.Abs((float) (remainingDistance - num2 / 2.0 - (num2 / 2.0 + num3))) > (double) Mathf.Abs(f))
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
      float num2 = pivot.RetargetLegScale * (float) (pivot.WalkCycleLength * (1.0 - num1) + pivot.RunCycleLength * (double) num1);
      float num3 = RightLegStopDistance(pivot, speed);
      float num4 = LeftLegStopDistance(pivot, speed);
      float f = remainingDistance - (num2 / 2f + num3);
      if ((double) Mathf.Abs((float) (remainingDistance - num2 / 2.0 - (num2 / 2.0 + num4))) > (double) Mathf.Abs(f))
      {
        scale = remainingDistance / (num2 / 2f + num3);
        return true;
      }
      scale = 1f;
      return false;
    }
  }
}
