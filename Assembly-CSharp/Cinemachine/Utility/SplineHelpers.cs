namespace Cinemachine.Utility
{
  internal static class SplineHelpers
  {
    public static Vector3 Bezier3(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
      t = Mathf.Clamp01(t);
      float num = 1f - t;
      return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
    }

    public static Vector3 BezierTangent3(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
      t = Mathf.Clamp01(t);
      return (-3f * p0 + 9f * p1 - 9f * p2 + 3f * p3) * t * t + (6f * p0 - 12f * p1 + 6f * p2) * t - 3f * p0 + 3f * p1;
    }

    public static float Bezier1(float t, float p0, float p1, float p2, float p3)
    {
      t = Mathf.Clamp01(t);
      float num = 1f - t;
      return (float) (num * (double) num * num * p0 + 3.0 * num * num * t * p1 + 3.0 * num * t * t * p2 + t * (double) t * t * p3);
    }

    public static float BezierTangent1(float t, float p0, float p1, float p2, float p3)
    {
      t = Mathf.Clamp01(t);
      return (float) ((-3.0 * p0 + 9.0 * p1 - 9.0 * p2 + 3.0 * p3) * t * t + (6.0 * p0 - 12.0 * p1 + 6.0 * p2) * t - 3.0 * p0 + 3.0 * p1);
    }

    public static void ComputeSmoothControlPoints(
      ref Vector4[] knot,
      ref Vector4[] ctrl1,
      ref Vector4[] ctrl2)
    {
      int length = knot.Length;
      if (length <= 2)
      {
        if (length == 2)
        {
          ctrl1[0] = Vector4.Lerp(knot[0], knot[1], 0.33333f);
          ctrl2[0] = Vector4.Lerp(knot[0], knot[1], 0.66666f);
        }
        else
        {
          if (length != 1)
            return;
          ctrl1[0] = ctrl2[0] = knot[0];
        }
      }
      else
      {
        float[] numArray1 = new float[length];
        float[] numArray2 = new float[length];
        float[] numArray3 = new float[length];
        float[] numArray4 = new float[length];
        for (int index1 = 0; index1 < 4; ++index1)
        {
          int index2 = length - 1;
          numArray1[0] = 0.0f;
          numArray2[0] = 2f;
          numArray3[0] = 1f;
          numArray4[0] = knot[0][index1] + 2f * knot[1][index1];
          for (int index3 = 1; index3 < index2 - 1; ++index3)
          {
            numArray1[index3] = 1f;
            numArray2[index3] = 4f;
            numArray3[index3] = 1f;
            numArray4[index3] = (float) (4.0 * (double) knot[index3][index1] + 2.0 * (double) knot[index3 + 1][index1]);
          }
          numArray1[index2 - 1] = 2f;
          numArray2[index2 - 1] = 7f;
          numArray3[index2 - 1] = 0.0f;
          numArray4[index2 - 1] = 8f * knot[index2 - 1][index1] + knot[index2][index1];
          for (int index4 = 1; index4 < index2; ++index4)
          {
            float num = numArray1[index4] / numArray2[index4 - 1];
            numArray2[index4] = numArray2[index4] - num * numArray3[index4 - 1];
            numArray4[index4] = numArray4[index4] - num * numArray4[index4 - 1];
          }
          ctrl1[index2 - 1][index1] = numArray4[index2 - 1] / numArray2[index2 - 1];
          for (int index5 = index2 - 2; index5 >= 0; --index5)
            ctrl1[index5][index1] = (numArray4[index5] - numArray3[index5] * ctrl1[index5 + 1][index1]) / numArray2[index5];
          for (int index6 = 0; index6 < index2; ++index6)
            ctrl2[index6][index1] = 2f * knot[index6 + 1][index1] - ctrl1[index6 + 1][index1];
          ctrl2[index2 - 1][index1] = (float) (0.5 * ((double) knot[index2][index1] + (double) ctrl1[index2 - 1][index1]));
        }
      }
    }

    public static void ComputeSmoothControlPointsLooped(
      ref Vector4[] knot,
      ref Vector4[] ctrl1,
      ref Vector4[] ctrl2)
    {
      int length = knot.Length;
      if (length < 2)
      {
        if (length != 1)
          return;
        ctrl1[0] = ctrl2[0] = knot[0];
      }
      else
      {
        int num = Mathf.Min(4, length - 1);
        Vector4[] knot1 = new Vector4[length + 2 * num];
        Vector4[] ctrl1_1 = new Vector4[length + 2 * num];
        Vector4[] ctrl2_1 = new Vector4[length + 2 * num];
        for (int index = 0; index < num; ++index)
        {
          knot1[index] = knot[length - (num - index)];
          knot1[length + num + index] = knot[index];
        }
        for (int index = 0; index < length; ++index)
          knot1[index + num] = knot[index];
        ComputeSmoothControlPoints(ref knot1, ref ctrl1_1, ref ctrl2_1);
        for (int index = 0; index < length; ++index)
        {
          ctrl1[index] = ctrl1_1[index + num];
          ctrl2[index] = ctrl2_1[index + num];
        }
      }
    }
  }
}
