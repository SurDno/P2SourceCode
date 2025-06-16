using UnityEngine;

namespace RootMotion
{
  public class Interp
  {
    public static float Float(float t, InterpolationMode mode)
    {
      float num;
      switch (mode)
      {
        case InterpolationMode.None:
          num = None(t, 0.0f, 1f);
          break;
        case InterpolationMode.InOutCubic:
          num = InOutCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InOutQuintic:
          num = InOutQuintic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InOutSine:
          num = InOutSine(t, 0.0f, 1f);
          break;
        case InterpolationMode.InQuintic:
          num = InQuintic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InQuartic:
          num = InQuartic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InCubic:
          num = InCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InQuadratic:
          num = InQuadratic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InElastic:
          num = OutElastic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InElasticSmall:
          num = InElasticSmall(t, 0.0f, 1f);
          break;
        case InterpolationMode.InElasticBig:
          num = InElasticBig(t, 0.0f, 1f);
          break;
        case InterpolationMode.InSine:
          num = InSine(t, 0.0f, 1f);
          break;
        case InterpolationMode.InBack:
          num = InBack(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutQuintic:
          num = OutQuintic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutQuartic:
          num = OutQuartic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutCubic:
          num = OutCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutInCubic:
          num = OutInCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutInQuartic:
          num = OutInCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutElastic:
          num = OutElastic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutElasticSmall:
          num = OutElasticSmall(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutElasticBig:
          num = OutElasticBig(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutSine:
          num = OutSine(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutBack:
          num = OutBack(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutBackCubic:
          num = OutBackCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutBackQuartic:
          num = OutBackQuartic(t, 0.0f, 1f);
          break;
        case InterpolationMode.BackInCubic:
          num = BackInCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.BackInQuartic:
          num = BackInQuartic(t, 0.0f, 1f);
          break;
        default:
          num = 0.0f;
          break;
      }
      return num;
    }

    public static Vector3 V3(Vector3 v1, Vector3 v2, float t, InterpolationMode mode)
    {
      float num = Float(t, mode);
      return (1f - num) * v1 + num * v2;
    }

    public static float LerpValue(
      float value,
      float target,
      float increaseSpeed,
      float decreaseSpeed)
    {
      if (value == (double) target)
        return target;
      return value < (double) target ? Mathf.Clamp(value + Time.deltaTime * increaseSpeed, float.NegativeInfinity, target) : Mathf.Clamp(value - Time.deltaTime * decreaseSpeed, target, float.PositiveInfinity);
    }

    private static float None(float t, float b, float c) => b + c * t;

    private static float InOutCubic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (-2.0 * num2 + 3.0 * num1);
    }

    private static float InOutQuintic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (6.0 * num2 * num1 + -15.0 * num1 * num1 + 10.0 * num2);
    }

    private static float InQuintic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (num2 * num1);
    }

    private static float InQuartic(float t, float b, float c)
    {
      float num = t * t;
      return b + c * (num * num);
    }

    private static float InCubic(float t, float b, float c)
    {
      float num = t * t * t;
      return b + c * num;
    }

    private static float InQuadratic(float t, float b, float c)
    {
      float num = t * t;
      return b + c * num;
    }

    private static float OutQuintic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (num2 * (double) num1 + -5.0 * num1 * num1 + 10.0 * num2 + -10.0 * num1 + 5.0 * t);
    }

    private static float OutQuartic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (-1.0 * num1 * num1 + 4.0 * num2 + -6.0 * num1 + 4.0 * t);
    }

    private static float OutCubic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (num2 + -3.0 * num1 + 3.0 * t);
    }

    private static float OutInCubic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (4.0 * num2 + -6.0 * num1 + 3.0 * t);
    }

    private static float OutInQuartic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (6.0 * num2 + -9.0 * num1 + 4.0 * t);
    }

    private static float BackInCubic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (4.0 * num2 + -3.0 * num1);
    }

    private static float BackInQuartic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (2.0 * num1 * num1 + 2.0 * num2 + -3.0 * num1);
    }

    private static float OutBackCubic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (4.0 * num2 + -9.0 * num1 + 6.0 * t);
    }

    private static float OutBackQuartic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (-2.0 * num1 * num1 + 10.0 * num2 + -15.0 * num1 + 8.0 * t);
    }

    private static float OutElasticSmall(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (33.0 * num2 * num1 + -106.0 * num1 * num1 + 126.0 * num2 + -67.0 * num1 + 15.0 * t);
    }

    private static float OutElasticBig(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (56.0 * num2 * num1 + -175.0 * num1 * num1 + 200.0 * num2 + -100.0 * num1 + 20.0 * t);
    }

    private static float InElasticSmall(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (33.0 * num2 * num1 + -59.0 * num1 * num1 + 32.0 * num2 + -5.0 * num1);
    }

    private static float InElasticBig(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (56.0 * num2 * num1 + -105.0 * num1 * num1 + 60.0 * num2 + -10.0 * num1);
    }

    private static float InSine(float t, float b, float c)
    {
      c -= b;
      return -c * Mathf.Cos((float) (t / 1.0 * 1.5707963705062866)) + c + b;
    }

    private static float OutSine(float t, float b, float c)
    {
      c -= b;
      return c * Mathf.Sin((float) (t / 1.0 * 1.5707963705062866)) + b;
    }

    private static float InOutSine(float t, float b, float c)
    {
      c -= b;
      return (float) (-(double) c / 2.0 * (Mathf.Cos((float) (3.1415927410125732 * t / 1.0)) - 1.0)) + b;
    }

    private static float InElastic(float t, float b, float c)
    {
      c -= b;
      float num1 = 1f;
      float num2 = num1 * 0.3f;
      float num3 = 0.0f;
      if (t == 0.0)
        return b;
      if ((t /= num1) == 1.0)
        return b + c;
      float num4;
      if (num3 == 0.0 || num3 < (double) Mathf.Abs(c))
      {
        num3 = c;
        num4 = num2 / 4f;
      }
      else
        num4 = num2 / 6.28318548f * Mathf.Asin(c / num3);
      return (float) -(num3 * (double) Mathf.Pow(2f, 10f * --t) * Mathf.Sin((float) ((t * (double) num1 - num4) * 6.2831854820251465) / num2)) + b;
    }

    private static float OutElastic(float t, float b, float c)
    {
      c -= b;
      float num1 = 1f;
      float num2 = num1 * 0.3f;
      float num3 = 0.0f;
      if (t == 0.0)
        return b;
      if ((t /= num1) == 1.0)
        return b + c;
      float num4;
      if (num3 == 0.0 || num3 < (double) Mathf.Abs(c))
      {
        num3 = c;
        num4 = num2 / 4f;
      }
      else
        num4 = num2 / 6.28318548f * Mathf.Asin(c / num3);
      return num3 * Mathf.Pow(2f, -10f * t) * Mathf.Sin((float) ((t * (double) num1 - num4) * 6.2831854820251465) / num2) + c + b;
    }

    private static float InBack(float t, float b, float c)
    {
      c -= b;
      t /= 1f;
      float num = 1.70158f;
      return (float) (c * (double) t * t * ((num + 1.0) * t - num)) + b;
    }

    private static float OutBack(float t, float b, float c)
    {
      float num = 1.70158f;
      c -= b;
      t = (float) (t / 1.0 - 1.0);
      return c * (float) (t * (double) t * ((num + 1.0) * t + num) + 1.0) + b;
    }
  }
}
