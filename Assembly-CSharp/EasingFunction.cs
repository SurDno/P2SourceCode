// Decompiled with JetBrains decompiler
// Type: EasingFunction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class EasingFunction
{
  private const float NATURAL_LOG_OF_2 = 0.6931472f;

  public static float Linear(float start, float end, float value) => Mathf.Lerp(start, end, value);

  public static float Spring(float start, float end, float value)
  {
    value = Mathf.Clamp01(value);
    value = (float) (((double) Mathf.Sin((float) ((double) value * 3.1415927410125732 * (0.20000000298023224 + 2.5 * (double) value * (double) value * (double) value))) * (double) Mathf.Pow(1f - value, 2.2f) + (double) value) * (1.0 + 1.2000000476837158 * (1.0 - (double) value)));
    return start + (end - start) * value;
  }

  public static float EaseInQuad(float start, float end, float value)
  {
    end -= start;
    return end * value * value + start;
  }

  public static float EaseOutQuad(float start, float end, float value)
  {
    end -= start;
    return (float) (-(double) end * (double) value * ((double) value - 2.0)) + start;
  }

  public static float EaseInOutQuad(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return end * 0.5f * value * value + start;
    --value;
    return (float) (-(double) end * 0.5 * ((double) value * ((double) value - 2.0) - 1.0)) + start;
  }

  public static float EaseInCubic(float start, float end, float value)
  {
    end -= start;
    return end * value * value * value + start;
  }

  public static float EaseOutCubic(float start, float end, float value)
  {
    --value;
    end -= start;
    return end * (float) ((double) value * (double) value * (double) value + 1.0) + start;
  }

  public static float EaseInOutCubic(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return end * 0.5f * value * value * value + start;
    value -= 2f;
    return (float) ((double) end * 0.5 * ((double) value * (double) value * (double) value + 2.0)) + start;
  }

  public static float EaseInQuart(float start, float end, float value)
  {
    end -= start;
    return end * value * value * value * value + start;
  }

  public static float EaseOutQuart(float start, float end, float value)
  {
    --value;
    end -= start;
    return (float) (-(double) end * ((double) value * (double) value * (double) value * (double) value - 1.0)) + start;
  }

  public static float EaseInOutQuart(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return end * 0.5f * value * value * value * value + start;
    value -= 2f;
    return (float) (-(double) end * 0.5 * ((double) value * (double) value * (double) value * (double) value - 2.0)) + start;
  }

  public static float EaseInQuint(float start, float end, float value)
  {
    end -= start;
    return end * value * value * value * value * value + start;
  }

  public static float EaseOutQuint(float start, float end, float value)
  {
    --value;
    end -= start;
    return end * (float) ((double) value * (double) value * (double) value * (double) value * (double) value + 1.0) + start;
  }

  public static float EaseInOutQuint(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return end * 0.5f * value * value * value * value * value + start;
    value -= 2f;
    return (float) ((double) end * 0.5 * ((double) value * (double) value * (double) value * (double) value * (double) value + 2.0)) + start;
  }

  public static float EaseInSine(float start, float end, float value)
  {
    end -= start;
    return -end * Mathf.Cos(value * 1.57079637f) + end + start;
  }

  public static float EaseOutSine(float start, float end, float value)
  {
    end -= start;
    return end * Mathf.Sin(value * 1.57079637f) + start;
  }

  public static float EaseInOutSine(float start, float end, float value)
  {
    end -= start;
    return (float) (-(double) end * 0.5 * ((double) Mathf.Cos(3.14159274f * value) - 1.0)) + start;
  }

  public static float EaseInExpo(float start, float end, float value)
  {
    end -= start;
    return end * Mathf.Pow(2f, (float) (10.0 * ((double) value - 1.0))) + start;
  }

  public static float EaseOutExpo(float start, float end, float value)
  {
    end -= start;
    return end * (float) (-(double) Mathf.Pow(2f, -10f * value) + 1.0) + start;
  }

  public static float EaseInOutExpo(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return end * 0.5f * Mathf.Pow(2f, (float) (10.0 * ((double) value - 1.0))) + start;
    --value;
    return (float) ((double) end * 0.5 * (-(double) Mathf.Pow(2f, -10f * value) + 2.0)) + start;
  }

  public static float EaseInCirc(float start, float end, float value)
  {
    end -= start;
    return (float) (-(double) end * ((double) Mathf.Sqrt((float) (1.0 - (double) value * (double) value)) - 1.0)) + start;
  }

  public static float EaseOutCirc(float start, float end, float value)
  {
    --value;
    end -= start;
    return end * Mathf.Sqrt((float) (1.0 - (double) value * (double) value)) + start;
  }

  public static float EaseInOutCirc(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return (float) (-(double) end * 0.5 * ((double) Mathf.Sqrt((float) (1.0 - (double) value * (double) value)) - 1.0)) + start;
    value -= 2f;
    return (float) ((double) end * 0.5 * ((double) Mathf.Sqrt((float) (1.0 - (double) value * (double) value)) + 1.0)) + start;
  }

  public static float EaseInBounce(float start, float end, float value)
  {
    end -= start;
    float num = 1f;
    return end - EasingFunction.EaseOutBounce(0.0f, end, num - value) + start;
  }

  public static float EaseOutBounce(float start, float end, float value)
  {
    value /= 1f;
    end -= start;
    if ((double) value < 0.36363637447357178)
      return end * (121f / 16f * value * value) + start;
    if ((double) value < 0.72727274894714355)
    {
      value -= 0.545454562f;
      return end * (float) (121.0 / 16.0 * (double) value * (double) value + 0.75) + start;
    }
    if ((double) value < 10.0 / 11.0)
    {
      value -= 0.8181818f;
      return end * (float) (121.0 / 16.0 * (double) value * (double) value + 15.0 / 16.0) + start;
    }
    value -= 0.954545438f;
    return end * (float) (121.0 / 16.0 * (double) value * (double) value + 63.0 / 64.0) + start;
  }

  public static float EaseInOutBounce(float start, float end, float value)
  {
    end -= start;
    float num = 1f;
    return (double) value < (double) num * 0.5 ? EasingFunction.EaseInBounce(0.0f, end, value * 2f) * 0.5f + start : (float) ((double) EasingFunction.EaseOutBounce(0.0f, end, value * 2f - num) * 0.5 + (double) end * 0.5) + start;
  }

  public static float EaseInBack(float start, float end, float value)
  {
    end -= start;
    value /= 1f;
    float num = 1.70158f;
    return (float) ((double) end * (double) value * (double) value * (((double) num + 1.0) * (double) value - (double) num)) + start;
  }

  public static float EaseOutBack(float start, float end, float value)
  {
    float num = 1.70158f;
    end -= start;
    --value;
    return end * (float) ((double) value * (double) value * (((double) num + 1.0) * (double) value + (double) num) + 1.0) + start;
  }

  public static float EaseInOutBack(float start, float end, float value)
  {
    float num1 = 1.70158f;
    end -= start;
    value /= 0.5f;
    if ((double) value < 1.0)
    {
      float num2 = num1 * 1.525f;
      return (float) ((double) end * 0.5 * ((double) value * (double) value * (((double) num2 + 1.0) * (double) value - (double) num2))) + start;
    }
    value -= 2f;
    float num3 = num1 * 1.525f;
    return (float) ((double) end * 0.5 * ((double) value * (double) value * (((double) num3 + 1.0) * (double) value + (double) num3) + 2.0)) + start;
  }

  public static float EaseInElastic(float start, float end, float value)
  {
    end -= start;
    float num1 = 1f;
    float num2 = num1 * 0.3f;
    float num3 = 0.0f;
    if ((double) value == 0.0)
      return start;
    if ((double) (value /= num1) == 1.0)
      return start + end;
    float num4;
    if ((double) num3 == 0.0 || (double) num3 < (double) Mathf.Abs(end))
    {
      num3 = end;
      num4 = num2 / 4f;
    }
    else
      num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);
    return (float) -((double) num3 * (double) Mathf.Pow(2f, 10f * --value) * (double) Mathf.Sin((float) (((double) value * (double) num1 - (double) num4) * 6.2831854820251465) / num2)) + start;
  }

  public static float EaseOutElastic(float start, float end, float value)
  {
    end -= start;
    float num1 = 1f;
    float num2 = num1 * 0.3f;
    float num3 = 0.0f;
    if ((double) value == 0.0)
      return start;
    if ((double) (value /= num1) == 1.0)
      return start + end;
    float num4;
    if ((double) num3 == 0.0 || (double) num3 < (double) Mathf.Abs(end))
    {
      num3 = end;
      num4 = num2 * 0.25f;
    }
    else
      num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);
    return num3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((float) (((double) value * (double) num1 - (double) num4) * 6.2831854820251465) / num2) + end + start;
  }

  public static float EaseInOutElastic(float start, float end, float value)
  {
    end -= start;
    float num1 = 1f;
    float num2 = num1 * 0.3f;
    float num3 = 0.0f;
    if ((double) value == 0.0)
      return start;
    if ((double) (value /= num1 * 0.5f) == 2.0)
      return start + end;
    float num4;
    if ((double) num3 == 0.0 || (double) num3 < (double) Mathf.Abs(end))
    {
      num3 = end;
      num4 = num2 / 4f;
    }
    else
      num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);
    return (double) value < 1.0 ? (float) (-0.5 * ((double) num3 * (double) Mathf.Pow(2f, 10f * --value) * (double) Mathf.Sin((float) (((double) value * (double) num1 - (double) num4) * 6.2831854820251465) / num2))) + start : (float) ((double) num3 * (double) Mathf.Pow(2f, -10f * --value) * (double) Mathf.Sin((float) (((double) value * (double) num1 - (double) num4) * 6.2831854820251465) / num2) * 0.5) + end + start;
  }

  public static float LinearD(float start, float end, float value) => end - start;

  public static float EaseInQuadD(float start, float end, float value)
  {
    return (float) (2.0 * ((double) end - (double) start)) * value;
  }

  public static float EaseOutQuadD(float start, float end, float value)
  {
    end -= start;
    return (float) (-(double) end * (double) value - (double) end * ((double) value - 2.0));
  }

  public static float EaseInOutQuadD(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return end * value;
    --value;
    return end * (1f - value);
  }

  public static float EaseInCubicD(float start, float end, float value)
  {
    return (float) (3.0 * ((double) end - (double) start)) * value * value;
  }

  public static float EaseOutCubicD(float start, float end, float value)
  {
    --value;
    end -= start;
    return 3f * end * value * value;
  }

  public static float EaseInOutCubicD(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return 1.5f * end * value * value;
    value -= 2f;
    return 1.5f * end * value * value;
  }

  public static float EaseInQuartD(float start, float end, float value)
  {
    return (float) (4.0 * ((double) end - (double) start)) * value * value * value;
  }

  public static float EaseOutQuartD(float start, float end, float value)
  {
    --value;
    end -= start;
    return -4f * end * value * value * value;
  }

  public static float EaseInOutQuartD(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return 2f * end * value * value * value;
    value -= 2f;
    return -2f * end * value * value * value;
  }

  public static float EaseInQuintD(float start, float end, float value)
  {
    return (float) (5.0 * ((double) end - (double) start)) * value * value * value * value;
  }

  public static float EaseOutQuintD(float start, float end, float value)
  {
    --value;
    end -= start;
    return 5f * end * value * value * value * value;
  }

  public static float EaseInOutQuintD(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return 2.5f * end * value * value * value * value;
    value -= 2f;
    return 2.5f * end * value * value * value * value;
  }

  public static float EaseInSineD(float start, float end, float value)
  {
    return (float) (((double) end - (double) start) * 0.5 * 3.1415927410125732) * Mathf.Sin(1.57079637f * value);
  }

  public static float EaseOutSineD(float start, float end, float value)
  {
    end -= start;
    return 1.57079637f * end * Mathf.Cos(value * 1.57079637f);
  }

  public static float EaseInOutSineD(float start, float end, float value)
  {
    end -= start;
    return (float) ((double) end * 0.5 * 3.1415927410125732) * Mathf.Sin(3.14159274f * value);
  }

  public static float EaseInExpoD(float start, float end, float value)
  {
    return (float) (6.9314718246459961 * ((double) end - (double) start)) * Mathf.Pow(2f, (float) (10.0 * ((double) value - 1.0)));
  }

  public static float EaseOutExpoD(float start, float end, float value)
  {
    end -= start;
    return 3.465736f * end * Mathf.Pow(2f, (float) (1.0 - 10.0 * (double) value));
  }

  public static float EaseInOutExpoD(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return 3.465736f * end * Mathf.Pow(2f, (float) (10.0 * ((double) value - 1.0)));
    --value;
    return 3.465736f * end / Mathf.Pow(2f, 10f * value);
  }

  public static float EaseInCircD(float start, float end, float value)
  {
    return (end - start) * value / Mathf.Sqrt((float) (1.0 - (double) value * (double) value));
  }

  public static float EaseOutCircD(float start, float end, float value)
  {
    --value;
    end -= start;
    return -end * value / Mathf.Sqrt((float) (1.0 - (double) value * (double) value));
  }

  public static float EaseInOutCircD(float start, float end, float value)
  {
    value /= 0.5f;
    end -= start;
    if ((double) value < 1.0)
      return (float) ((double) end * (double) value / (2.0 * (double) Mathf.Sqrt((float) (1.0 - (double) value * (double) value))));
    value -= 2f;
    return (float) (-(double) end * (double) value / (2.0 * (double) Mathf.Sqrt((float) (1.0 - (double) value * (double) value))));
  }

  public static float EaseInBounceD(float start, float end, float value)
  {
    end -= start;
    float num = 1f;
    return EasingFunction.EaseOutBounceD(0.0f, end, num - value);
  }

  public static float EaseOutBounceD(float start, float end, float value)
  {
    value /= 1f;
    end -= start;
    if ((double) value < 0.36363637447357178)
      return (float) (2.0 * (double) end * (121.0 / 16.0)) * value;
    if ((double) value < 0.72727274894714355)
    {
      value -= 0.545454562f;
      return (float) (2.0 * (double) end * (121.0 / 16.0)) * value;
    }
    if ((double) value < 10.0 / 11.0)
    {
      value -= 0.8181818f;
      return (float) (2.0 * (double) end * (121.0 / 16.0)) * value;
    }
    value -= 0.954545438f;
    return (float) (2.0 * (double) end * (121.0 / 16.0)) * value;
  }

  public static float EaseInOutBounceD(float start, float end, float value)
  {
    end -= start;
    float num = 1f;
    return (double) value < (double) num * 0.5 ? EasingFunction.EaseInBounceD(0.0f, end, value * 2f) * 0.5f : EasingFunction.EaseOutBounceD(0.0f, end, value * 2f - num) * 0.5f;
  }

  public static float EaseInBackD(float start, float end, float value)
  {
    float num = 1.70158f;
    return (float) (3.0 * ((double) num + 1.0) * ((double) end - (double) start) * (double) value * (double) value - 2.0 * (double) num * ((double) end - (double) start) * (double) value);
  }

  public static float EaseOutBackD(float start, float end, float value)
  {
    float num = 1.70158f;
    end -= start;
    --value;
    return end * (float) (((double) num + 1.0) * (double) value * (double) value + 2.0 * (double) value * (((double) num + 1.0) * (double) value + (double) num));
  }

  public static float EaseInOutBackD(float start, float end, float value)
  {
    float num1 = 1.70158f;
    end -= start;
    value /= 0.5f;
    if ((double) value < 1.0)
    {
      float num2 = num1 * 1.525f;
      return (float) (0.5 * (double) end * ((double) num2 + 1.0) * (double) value * (double) value + (double) end * (double) value * (((double) num2 + 1.0) * (double) value - (double) num2));
    }
    value -= 2f;
    float num3 = num1 * 1.525f;
    return (float) (0.5 * (double) end * (((double) num3 + 1.0) * (double) value * (double) value + 2.0 * (double) value * (((double) num3 + 1.0) * (double) value + (double) num3)));
  }

  public static float EaseInElasticD(float start, float end, float value)
  {
    return EasingFunction.EaseOutElasticD(start, end, 1f - value);
  }

  public static float EaseOutElasticD(float start, float end, float value)
  {
    end -= start;
    float num1 = 1f;
    float num2 = num1 * 0.3f;
    float num3 = 0.0f;
    float num4;
    if ((double) num3 == 0.0 || (double) num3 < (double) Mathf.Abs(end))
    {
      num3 = end;
      num4 = num2 * 0.25f;
    }
    else
      num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);
    return (float) ((double) num3 * 3.1415927410125732 * (double) num1 * (double) Mathf.Pow(2f, (float) (1.0 - 10.0 * (double) value)) * (double) Mathf.Cos((float) (6.2831854820251465 * ((double) num1 * (double) value - (double) num4)) / num2) / (double) num2 - 3.465735912322998 * (double) num3 * (double) Mathf.Pow(2f, (float) (1.0 - 10.0 * (double) value)) * (double) Mathf.Sin((float) (6.2831854820251465 * ((double) num1 * (double) value - (double) num4)) / num2));
  }

  public static float EaseInOutElasticD(float start, float end, float value)
  {
    end -= start;
    float num1 = 1f;
    float num2 = num1 * 0.3f;
    float num3 = 0.0f;
    float num4;
    if ((double) num3 == 0.0 || (double) num3 < (double) Mathf.Abs(end))
    {
      num3 = end;
      num4 = num2 / 4f;
    }
    else
      num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);
    if ((double) value < 1.0)
    {
      --value;
      return (float) (-3.465735912322998 * (double) num3 * (double) Mathf.Pow(2f, 10f * value) * (double) Mathf.Sin((float) (6.2831854820251465 * ((double) num1 * (double) value - 2.0)) / num2) - (double) num3 * 3.1415927410125732 * (double) num1 * (double) Mathf.Pow(2f, 10f * value) * (double) Mathf.Cos((float) (6.2831854820251465 * ((double) num1 * (double) value - (double) num4)) / num2) / (double) num2);
    }
    --value;
    return (float) ((double) num3 * 3.1415927410125732 * (double) num1 * (double) Mathf.Cos((float) (6.2831854820251465 * ((double) num1 * (double) value - (double) num4)) / num2) / ((double) num2 * (double) Mathf.Pow(2f, 10f * value)) - 3.465735912322998 * (double) num3 * (double) Mathf.Sin((float) (6.2831854820251465 * ((double) num1 * (double) value - (double) num4)) / num2) / (double) Mathf.Pow(2f, 10f * value));
  }

  public static float SpringD(float start, float end, float value)
  {
    value = Mathf.Clamp01(value);
    end -= start;
    return (float) ((double) end * (6.0 * (1.0 - (double) value) / 5.0 + 1.0) * (-2.2000000476837158 * (double) Mathf.Pow(1f - value, 1.2f) * (double) Mathf.Sin((float) (3.1415927410125732 * (double) value * (2.5 * (double) value * (double) value * (double) value + 0.20000000298023224))) + (double) Mathf.Pow(1f - value, 2.2f) * (3.1415927410125732 * (2.5 * (double) value * (double) value * (double) value + 0.20000000298023224) + 23.561944961547852 * (double) value * (double) value * (double) value) * (double) Mathf.Cos((float) (3.1415927410125732 * (double) value * (2.5 * (double) value * (double) value * (double) value + 0.20000000298023224))) + 1.0) - 6.0 * (double) end * ((double) Mathf.Pow(1f - value, 2.2f) * (double) Mathf.Sin((float) (3.1415927410125732 * (double) value * (2.5 * (double) value * (double) value * (double) value + 0.20000000298023224))) + (double) value / 5.0));
  }

  public static EasingFunction.Function GetEasingFunction(EasingFunction.Ease easingFunction)
  {
    switch (easingFunction)
    {
      case EasingFunction.Ease.EaseInQuad:
        return new EasingFunction.Function(EasingFunction.EaseInQuad);
      case EasingFunction.Ease.EaseOutQuad:
        return new EasingFunction.Function(EasingFunction.EaseOutQuad);
      case EasingFunction.Ease.EaseInOutQuad:
        return new EasingFunction.Function(EasingFunction.EaseInOutQuad);
      case EasingFunction.Ease.EaseInCubic:
        return new EasingFunction.Function(EasingFunction.EaseInCubic);
      case EasingFunction.Ease.EaseOutCubic:
        return new EasingFunction.Function(EasingFunction.EaseOutCubic);
      case EasingFunction.Ease.EaseInOutCubic:
        return new EasingFunction.Function(EasingFunction.EaseInOutCubic);
      case EasingFunction.Ease.EaseInQuart:
        return new EasingFunction.Function(EasingFunction.EaseInQuart);
      case EasingFunction.Ease.EaseOutQuart:
        return new EasingFunction.Function(EasingFunction.EaseOutQuart);
      case EasingFunction.Ease.EaseInOutQuart:
        return new EasingFunction.Function(EasingFunction.EaseInOutQuart);
      case EasingFunction.Ease.EaseInQuint:
        return new EasingFunction.Function(EasingFunction.EaseInQuint);
      case EasingFunction.Ease.EaseOutQuint:
        return new EasingFunction.Function(EasingFunction.EaseOutQuint);
      case EasingFunction.Ease.EaseInOutQuint:
        return new EasingFunction.Function(EasingFunction.EaseInOutQuint);
      case EasingFunction.Ease.EaseInSine:
        return new EasingFunction.Function(EasingFunction.EaseInSine);
      case EasingFunction.Ease.EaseOutSine:
        return new EasingFunction.Function(EasingFunction.EaseOutSine);
      case EasingFunction.Ease.EaseInOutSine:
        return new EasingFunction.Function(EasingFunction.EaseInOutSine);
      case EasingFunction.Ease.EaseInExpo:
        return new EasingFunction.Function(EasingFunction.EaseInExpo);
      case EasingFunction.Ease.EaseOutExpo:
        return new EasingFunction.Function(EasingFunction.EaseOutExpo);
      case EasingFunction.Ease.EaseInOutExpo:
        return new EasingFunction.Function(EasingFunction.EaseInOutExpo);
      case EasingFunction.Ease.EaseInCirc:
        return new EasingFunction.Function(EasingFunction.EaseInCirc);
      case EasingFunction.Ease.EaseOutCirc:
        return new EasingFunction.Function(EasingFunction.EaseOutCirc);
      case EasingFunction.Ease.EaseInOutCirc:
        return new EasingFunction.Function(EasingFunction.EaseInOutCirc);
      case EasingFunction.Ease.Linear:
        return new EasingFunction.Function(EasingFunction.Linear);
      case EasingFunction.Ease.Spring:
        return new EasingFunction.Function(EasingFunction.Spring);
      case EasingFunction.Ease.EaseInBounce:
        return new EasingFunction.Function(EasingFunction.EaseInBounce);
      case EasingFunction.Ease.EaseOutBounce:
        return new EasingFunction.Function(EasingFunction.EaseOutBounce);
      case EasingFunction.Ease.EaseInOutBounce:
        return new EasingFunction.Function(EasingFunction.EaseInOutBounce);
      case EasingFunction.Ease.EaseInBack:
        return new EasingFunction.Function(EasingFunction.EaseInBack);
      case EasingFunction.Ease.EaseOutBack:
        return new EasingFunction.Function(EasingFunction.EaseOutBack);
      case EasingFunction.Ease.EaseInOutBack:
        return new EasingFunction.Function(EasingFunction.EaseInOutBack);
      case EasingFunction.Ease.EaseInElastic:
        return new EasingFunction.Function(EasingFunction.EaseInElastic);
      case EasingFunction.Ease.EaseOutElastic:
        return new EasingFunction.Function(EasingFunction.EaseOutElastic);
      case EasingFunction.Ease.EaseInOutElastic:
        return new EasingFunction.Function(EasingFunction.EaseInOutElastic);
      default:
        return (EasingFunction.Function) null;
    }
  }

  public static EasingFunction.Function GetEasingFunctionDerivative(
    EasingFunction.Ease easingFunction)
  {
    switch (easingFunction)
    {
      case EasingFunction.Ease.EaseInQuad:
        return new EasingFunction.Function(EasingFunction.EaseInQuadD);
      case EasingFunction.Ease.EaseOutQuad:
        return new EasingFunction.Function(EasingFunction.EaseOutQuadD);
      case EasingFunction.Ease.EaseInOutQuad:
        return new EasingFunction.Function(EasingFunction.EaseInOutQuadD);
      case EasingFunction.Ease.EaseInCubic:
        return new EasingFunction.Function(EasingFunction.EaseInCubicD);
      case EasingFunction.Ease.EaseOutCubic:
        return new EasingFunction.Function(EasingFunction.EaseOutCubicD);
      case EasingFunction.Ease.EaseInOutCubic:
        return new EasingFunction.Function(EasingFunction.EaseInOutCubicD);
      case EasingFunction.Ease.EaseInQuart:
        return new EasingFunction.Function(EasingFunction.EaseInQuartD);
      case EasingFunction.Ease.EaseOutQuart:
        return new EasingFunction.Function(EasingFunction.EaseOutQuartD);
      case EasingFunction.Ease.EaseInOutQuart:
        return new EasingFunction.Function(EasingFunction.EaseInOutQuartD);
      case EasingFunction.Ease.EaseInQuint:
        return new EasingFunction.Function(EasingFunction.EaseInQuintD);
      case EasingFunction.Ease.EaseOutQuint:
        return new EasingFunction.Function(EasingFunction.EaseOutQuintD);
      case EasingFunction.Ease.EaseInOutQuint:
        return new EasingFunction.Function(EasingFunction.EaseInOutQuintD);
      case EasingFunction.Ease.EaseInSine:
        return new EasingFunction.Function(EasingFunction.EaseInSineD);
      case EasingFunction.Ease.EaseOutSine:
        return new EasingFunction.Function(EasingFunction.EaseOutSineD);
      case EasingFunction.Ease.EaseInOutSine:
        return new EasingFunction.Function(EasingFunction.EaseInOutSineD);
      case EasingFunction.Ease.EaseInExpo:
        return new EasingFunction.Function(EasingFunction.EaseInExpoD);
      case EasingFunction.Ease.EaseOutExpo:
        return new EasingFunction.Function(EasingFunction.EaseOutExpoD);
      case EasingFunction.Ease.EaseInOutExpo:
        return new EasingFunction.Function(EasingFunction.EaseInOutExpoD);
      case EasingFunction.Ease.EaseInCirc:
        return new EasingFunction.Function(EasingFunction.EaseInCircD);
      case EasingFunction.Ease.EaseOutCirc:
        return new EasingFunction.Function(EasingFunction.EaseOutCircD);
      case EasingFunction.Ease.EaseInOutCirc:
        return new EasingFunction.Function(EasingFunction.EaseInOutCircD);
      case EasingFunction.Ease.Linear:
        return new EasingFunction.Function(EasingFunction.LinearD);
      case EasingFunction.Ease.Spring:
        return new EasingFunction.Function(EasingFunction.SpringD);
      case EasingFunction.Ease.EaseInBounce:
        return new EasingFunction.Function(EasingFunction.EaseInBounceD);
      case EasingFunction.Ease.EaseOutBounce:
        return new EasingFunction.Function(EasingFunction.EaseOutBounceD);
      case EasingFunction.Ease.EaseInOutBounce:
        return new EasingFunction.Function(EasingFunction.EaseInOutBounceD);
      case EasingFunction.Ease.EaseInBack:
        return new EasingFunction.Function(EasingFunction.EaseInBackD);
      case EasingFunction.Ease.EaseOutBack:
        return new EasingFunction.Function(EasingFunction.EaseOutBackD);
      case EasingFunction.Ease.EaseInOutBack:
        return new EasingFunction.Function(EasingFunction.EaseInOutBackD);
      case EasingFunction.Ease.EaseInElastic:
        return new EasingFunction.Function(EasingFunction.EaseInElasticD);
      case EasingFunction.Ease.EaseOutElastic:
        return new EasingFunction.Function(EasingFunction.EaseOutElasticD);
      case EasingFunction.Ease.EaseInOutElastic:
        return new EasingFunction.Function(EasingFunction.EaseInOutElasticD);
      default:
        return (EasingFunction.Function) null;
    }
  }

  public enum Ease
  {
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
    EaseInSine,
    EaseOutSine,
    EaseInOutSine,
    EaseInExpo,
    EaseOutExpo,
    EaseInOutExpo,
    EaseInCirc,
    EaseOutCirc,
    EaseInOutCirc,
    Linear,
    Spring,
    EaseInBounce,
    EaseOutBounce,
    EaseInOutBounce,
    EaseInBack,
    EaseOutBack,
    EaseInOutBack,
    EaseInElastic,
    EaseOutElastic,
    EaseInOutElastic,
  }

  public delegate float Function(float s, float e, float v);
}
