// Decompiled with JetBrains decompiler
// Type: RootMotion.Interp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
          num = Interp.None(t, 0.0f, 1f);
          break;
        case InterpolationMode.InOutCubic:
          num = Interp.InOutCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InOutQuintic:
          num = Interp.InOutQuintic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InOutSine:
          num = Interp.InOutSine(t, 0.0f, 1f);
          break;
        case InterpolationMode.InQuintic:
          num = Interp.InQuintic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InQuartic:
          num = Interp.InQuartic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InCubic:
          num = Interp.InCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InQuadratic:
          num = Interp.InQuadratic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InElastic:
          num = Interp.OutElastic(t, 0.0f, 1f);
          break;
        case InterpolationMode.InElasticSmall:
          num = Interp.InElasticSmall(t, 0.0f, 1f);
          break;
        case InterpolationMode.InElasticBig:
          num = Interp.InElasticBig(t, 0.0f, 1f);
          break;
        case InterpolationMode.InSine:
          num = Interp.InSine(t, 0.0f, 1f);
          break;
        case InterpolationMode.InBack:
          num = Interp.InBack(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutQuintic:
          num = Interp.OutQuintic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutQuartic:
          num = Interp.OutQuartic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutCubic:
          num = Interp.OutCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutInCubic:
          num = Interp.OutInCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutInQuartic:
          num = Interp.OutInCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutElastic:
          num = Interp.OutElastic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutElasticSmall:
          num = Interp.OutElasticSmall(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutElasticBig:
          num = Interp.OutElasticBig(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutSine:
          num = Interp.OutSine(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutBack:
          num = Interp.OutBack(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutBackCubic:
          num = Interp.OutBackCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.OutBackQuartic:
          num = Interp.OutBackQuartic(t, 0.0f, 1f);
          break;
        case InterpolationMode.BackInCubic:
          num = Interp.BackInCubic(t, 0.0f, 1f);
          break;
        case InterpolationMode.BackInQuartic:
          num = Interp.BackInQuartic(t, 0.0f, 1f);
          break;
        default:
          num = 0.0f;
          break;
      }
      return num;
    }

    public static Vector3 V3(Vector3 v1, Vector3 v2, float t, InterpolationMode mode)
    {
      float num = Interp.Float(t, mode);
      return (1f - num) * v1 + num * v2;
    }

    public static float LerpValue(
      float value,
      float target,
      float increaseSpeed,
      float decreaseSpeed)
    {
      if ((double) value == (double) target)
        return target;
      return (double) value < (double) target ? Mathf.Clamp(value + Time.deltaTime * increaseSpeed, float.NegativeInfinity, target) : Mathf.Clamp(value - Time.deltaTime * decreaseSpeed, target, float.PositiveInfinity);
    }

    private static float None(float t, float b, float c) => b + c * t;

    private static float InOutCubic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (-2.0 * (double) num2 + 3.0 * (double) num1);
    }

    private static float InOutQuintic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (6.0 * (double) num2 * (double) num1 + -15.0 * (double) num1 * (double) num1 + 10.0 * (double) num2);
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
      return b + c * (float) ((double) num2 * (double) num1 + -5.0 * (double) num1 * (double) num1 + 10.0 * (double) num2 + -10.0 * (double) num1 + 5.0 * (double) t);
    }

    private static float OutQuartic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (-1.0 * (double) num1 * (double) num1 + 4.0 * (double) num2 + -6.0 * (double) num1 + 4.0 * (double) t);
    }

    private static float OutCubic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) ((double) num2 + -3.0 * (double) num1 + 3.0 * (double) t);
    }

    private static float OutInCubic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (4.0 * (double) num2 + -6.0 * (double) num1 + 3.0 * (double) t);
    }

    private static float OutInQuartic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (6.0 * (double) num2 + -9.0 * (double) num1 + 4.0 * (double) t);
    }

    private static float BackInCubic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (4.0 * (double) num2 + -3.0 * (double) num1);
    }

    private static float BackInQuartic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (2.0 * (double) num1 * (double) num1 + 2.0 * (double) num2 + -3.0 * (double) num1);
    }

    private static float OutBackCubic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (4.0 * (double) num2 + -9.0 * (double) num1 + 6.0 * (double) t);
    }

    private static float OutBackQuartic(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (-2.0 * (double) num1 * (double) num1 + 10.0 * (double) num2 + -15.0 * (double) num1 + 8.0 * (double) t);
    }

    private static float OutElasticSmall(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (33.0 * (double) num2 * (double) num1 + -106.0 * (double) num1 * (double) num1 + 126.0 * (double) num2 + -67.0 * (double) num1 + 15.0 * (double) t);
    }

    private static float OutElasticBig(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (56.0 * (double) num2 * (double) num1 + -175.0 * (double) num1 * (double) num1 + 200.0 * (double) num2 + -100.0 * (double) num1 + 20.0 * (double) t);
    }

    private static float InElasticSmall(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (33.0 * (double) num2 * (double) num1 + -59.0 * (double) num1 * (double) num1 + 32.0 * (double) num2 + -5.0 * (double) num1);
    }

    private static float InElasticBig(float t, float b, float c)
    {
      float num1 = t * t;
      float num2 = num1 * t;
      return b + c * (float) (56.0 * (double) num2 * (double) num1 + -105.0 * (double) num1 * (double) num1 + 60.0 * (double) num2 + -10.0 * (double) num1);
    }

    private static float InSine(float t, float b, float c)
    {
      c -= b;
      return -c * Mathf.Cos((float) ((double) t / 1.0 * 1.5707963705062866)) + c + b;
    }

    private static float OutSine(float t, float b, float c)
    {
      c -= b;
      return c * Mathf.Sin((float) ((double) t / 1.0 * 1.5707963705062866)) + b;
    }

    private static float InOutSine(float t, float b, float c)
    {
      c -= b;
      return (float) (-(double) c / 2.0 * ((double) Mathf.Cos((float) (3.1415927410125732 * (double) t / 1.0)) - 1.0)) + b;
    }

    private static float InElastic(float t, float b, float c)
    {
      c -= b;
      float num1 = 1f;
      float num2 = num1 * 0.3f;
      float num3 = 0.0f;
      if ((double) t == 0.0)
        return b;
      if ((double) (t /= num1) == 1.0)
        return b + c;
      float num4;
      if ((double) num3 == 0.0 || (double) num3 < (double) Mathf.Abs(c))
      {
        num3 = c;
        num4 = num2 / 4f;
      }
      else
        num4 = num2 / 6.28318548f * Mathf.Asin(c / num3);
      return (float) -((double) num3 * (double) Mathf.Pow(2f, 10f * --t) * (double) Mathf.Sin((float) (((double) t * (double) num1 - (double) num4) * 6.2831854820251465) / num2)) + b;
    }

    private static float OutElastic(float t, float b, float c)
    {
      c -= b;
      float num1 = 1f;
      float num2 = num1 * 0.3f;
      float num3 = 0.0f;
      if ((double) t == 0.0)
        return b;
      if ((double) (t /= num1) == 1.0)
        return b + c;
      float num4;
      if ((double) num3 == 0.0 || (double) num3 < (double) Mathf.Abs(c))
      {
        num3 = c;
        num4 = num2 / 4f;
      }
      else
        num4 = num2 / 6.28318548f * Mathf.Asin(c / num3);
      return num3 * Mathf.Pow(2f, -10f * t) * Mathf.Sin((float) (((double) t * (double) num1 - (double) num4) * 6.2831854820251465) / num2) + c + b;
    }

    private static float InBack(float t, float b, float c)
    {
      c -= b;
      t /= 1f;
      float num = 1.70158f;
      return (float) ((double) c * (double) t * (double) t * (((double) num + 1.0) * (double) t - (double) num)) + b;
    }

    private static float OutBack(float t, float b, float c)
    {
      float num = 1.70158f;
      c -= b;
      t = (float) ((double) t / 1.0 - 1.0);
      return c * (float) ((double) t * (double) t * (((double) num + 1.0) * (double) t + (double) num) + 1.0) + b;
    }
  }
}
