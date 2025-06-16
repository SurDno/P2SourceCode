using UnityEngine;

namespace Scripts.Utility
{
  public static class EasyUtility
  {
    private const float PI = 3.14159274f;
    private const float HALFPI = 1.57079637f;

    public static float Interpolate(float p, EasyUtility.Functions function)
    {
      switch (function)
      {
        case EasyUtility.Functions.QuadraticEaseIn:
          return EasyUtility.QuadraticEaseIn(p);
        case EasyUtility.Functions.QuadraticEaseOut:
          return EasyUtility.QuadraticEaseOut(p);
        case EasyUtility.Functions.QuadraticEaseInOut:
          return EasyUtility.QuadraticEaseInOut(p);
        case EasyUtility.Functions.CubicEaseIn:
          return EasyUtility.CubicEaseIn(p);
        case EasyUtility.Functions.CubicEaseOut:
          return EasyUtility.CubicEaseOut(p);
        case EasyUtility.Functions.CubicEaseInOut:
          return EasyUtility.CubicEaseInOut(p);
        case EasyUtility.Functions.QuarticEaseIn:
          return EasyUtility.QuarticEaseIn(p);
        case EasyUtility.Functions.QuarticEaseOut:
          return EasyUtility.QuarticEaseOut(p);
        case EasyUtility.Functions.QuarticEaseInOut:
          return EasyUtility.QuarticEaseInOut(p);
        case EasyUtility.Functions.QuinticEaseIn:
          return EasyUtility.QuinticEaseIn(p);
        case EasyUtility.Functions.QuinticEaseOut:
          return EasyUtility.QuinticEaseOut(p);
        case EasyUtility.Functions.QuinticEaseInOut:
          return EasyUtility.QuinticEaseInOut(p);
        case EasyUtility.Functions.SineEaseIn:
          return EasyUtility.SineEaseIn(p);
        case EasyUtility.Functions.SineEaseOut:
          return EasyUtility.SineEaseOut(p);
        case EasyUtility.Functions.SineEaseInOut:
          return EasyUtility.SineEaseInOut(p);
        case EasyUtility.Functions.CircularEaseIn:
          return EasyUtility.CircularEaseIn(p);
        case EasyUtility.Functions.CircularEaseOut:
          return EasyUtility.CircularEaseOut(p);
        case EasyUtility.Functions.CircularEaseInOut:
          return EasyUtility.CircularEaseInOut(p);
        case EasyUtility.Functions.ExponentialEaseIn:
          return EasyUtility.ExponentialEaseIn(p);
        case EasyUtility.Functions.ExponentialEaseOut:
          return EasyUtility.ExponentialEaseOut(p);
        case EasyUtility.Functions.ExponentialEaseInOut:
          return EasyUtility.ExponentialEaseInOut(p);
        case EasyUtility.Functions.ElasticEaseIn:
          return EasyUtility.ElasticEaseIn(p);
        case EasyUtility.Functions.ElasticEaseOut:
          return EasyUtility.ElasticEaseOut(p);
        case EasyUtility.Functions.ElasticEaseInOut:
          return EasyUtility.ElasticEaseInOut(p);
        case EasyUtility.Functions.BackEaseIn:
          return EasyUtility.BackEaseIn(p);
        case EasyUtility.Functions.BackEaseOut:
          return EasyUtility.BackEaseOut(p);
        case EasyUtility.Functions.BackEaseInOut:
          return EasyUtility.BackEaseInOut(p);
        case EasyUtility.Functions.BounceEaseIn:
          return EasyUtility.BounceEaseIn(p);
        case EasyUtility.Functions.BounceEaseOut:
          return EasyUtility.BounceEaseOut(p);
        case EasyUtility.Functions.BounceEaseInOut:
          return EasyUtility.BounceEaseInOut(p);
        default:
          return EasyUtility.Linear(p);
      }
    }

    public static float Linear(float p) => p;

    public static float QuadraticEaseIn(float p) => p * p;

    public static float QuadraticEaseOut(float p) => (float) -((double) p * ((double) p - 2.0));

    public static float QuadraticEaseInOut(float p)
    {
      return (double) p < 0.5 ? 2f * p * p : (float) (-2.0 * (double) p * (double) p + 4.0 * (double) p - 1.0);
    }

    public static float CubicEaseIn(float p) => p * p * p;

    public static float CubicEaseOut(float p)
    {
      float num = p - 1f;
      return (float) ((double) num * (double) num * (double) num + 1.0);
    }

    public static float CubicEaseInOut(float p)
    {
      if ((double) p < 0.5)
        return 4f * p * p * p;
      float num = (float) (2.0 * (double) p - 2.0);
      return (float) (0.5 * (double) num * (double) num * (double) num + 1.0);
    }

    public static float QuarticEaseIn(float p) => p * p * p * p;

    public static float QuarticEaseOut(float p)
    {
      float num = p - 1f;
      return (float) ((double) num * (double) num * (double) num * (1.0 - (double) p) + 1.0);
    }

    public static float QuarticEaseInOut(float p)
    {
      if ((double) p < 0.5)
        return 8f * p * p * p * p;
      float num = p - 1f;
      return (float) (-8.0 * (double) num * (double) num * (double) num * (double) num + 1.0);
    }

    public static float QuinticEaseIn(float p) => p * p * p * p * p;

    public static float QuinticEaseOut(float p)
    {
      float num = p - 1f;
      return (float) ((double) num * (double) num * (double) num * (double) num * (double) num + 1.0);
    }

    public static float QuinticEaseInOut(float p)
    {
      if ((double) p < 0.5)
        return 16f * p * p * p * p * p;
      float num = (float) (2.0 * (double) p - 2.0);
      return (float) (0.5 * (double) num * (double) num * (double) num * (double) num * (double) num + 1.0);
    }

    public static float SineEaseIn(float p)
    {
      return Mathf.Sin((float) (((double) p - 1.0) * 1.5707963705062866)) + 1f;
    }

    public static float SineEaseOut(float p) => Mathf.Sin(p * 1.57079637f);

    public static float SineEaseInOut(float p)
    {
      return (float) (0.5 * (1.0 - (double) Mathf.Cos(p * 3.14159274f)));
    }

    public static float CircularEaseIn(float p)
    {
      return 1f - Mathf.Sqrt((float) (1.0 - (double) p * (double) p));
    }

    public static float CircularEaseOut(float p) => Mathf.Sqrt((2f - p) * p);

    public static float CircularEaseInOut(float p)
    {
      return (double) p < 0.5 ? (float) (0.5 * (1.0 - (double) Mathf.Sqrt((float) (1.0 - 4.0 * ((double) p * (double) p))))) : (float) (0.5 * ((double) Mathf.Sqrt((float) (-(2.0 * (double) p - 3.0) * (2.0 * (double) p - 1.0))) + 1.0));
    }

    public static float ExponentialEaseIn(float p)
    {
      return (double) p == 0.0 ? p : Mathf.Pow(2f, (float) (10.0 * ((double) p - 1.0)));
    }

    public static float ExponentialEaseOut(float p)
    {
      return (double) p == 1.0 ? p : 1f - Mathf.Pow(2f, -10f * p);
    }

    public static float ExponentialEaseInOut(float p)
    {
      if ((double) p == 0.0 || (double) p == 1.0)
        return p;
      return (double) p < 0.5 ? 0.5f * Mathf.Pow(2f, (float) (20.0 * (double) p - 10.0)) : (float) (-0.5 * (double) Mathf.Pow(2f, (float) (-20.0 * (double) p + 10.0)) + 1.0);
    }

    public static float ElasticEaseIn(float p)
    {
      return Mathf.Sin(20.4203529f * p) * Mathf.Pow(2f, (float) (10.0 * ((double) p - 1.0)));
    }

    public static float ElasticEaseOut(float p)
    {
      return (float) ((double) Mathf.Sin((float) (-20.420352935791016 * ((double) p + 1.0))) * (double) Mathf.Pow(2f, -10f * p) + 1.0);
    }

    public static float ElasticEaseInOut(float p)
    {
      return (double) p < 0.5 ? 0.5f * Mathf.Sin((float) (20.420352935791016 * (2.0 * (double) p))) * Mathf.Pow(2f, (float) (10.0 * (2.0 * (double) p - 1.0))) : (float) (0.5 * ((double) Mathf.Sin((float) (-20.420352935791016 * (2.0 * (double) p - 1.0 + 1.0))) * (double) Mathf.Pow(2f, (float) (-10.0 * (2.0 * (double) p - 1.0))) + 2.0));
    }

    public static float BackEaseIn(float p)
    {
      return (float) ((double) p * (double) p * (double) p - (double) p * (double) Mathf.Sin(p * 3.14159274f));
    }

    public static float BackEaseOut(float p)
    {
      float num = 1f - p;
      return (float) (1.0 - ((double) num * (double) num * (double) num - (double) num * (double) Mathf.Sin(num * 3.14159274f)));
    }

    public static float BackEaseInOut(float p)
    {
      if ((double) p < 0.5)
      {
        float num = 2f * p;
        return (float) (0.5 * ((double) num * (double) num * (double) num - (double) num * (double) Mathf.Sin(num * 3.14159274f)));
      }
      float num1 = (float) (1.0 - (2.0 * (double) p - 1.0));
      return (float) (0.5 * (1.0 - ((double) num1 * (double) num1 * (double) num1 - (double) num1 * (double) Mathf.Sin(num1 * 3.14159274f))) + 0.5);
    }

    public static float BounceEaseIn(float p) => 1f - EasyUtility.BounceEaseOut(1f - p);

    public static float BounceEaseOut(float p)
    {
      if ((double) p < 0.36363637447357178)
        return (float) (121.0 * (double) p * (double) p / 16.0);
      if ((double) p < 0.72727274894714355)
        return (float) (9.0749998092651367 * (double) p * (double) p - 9.8999996185302734 * (double) p + 3.4000000953674316);
      return (double) p < 0.89999997615814209 ? (float) (12.066481590270996 * (double) p * (double) p - 19.635457992553711 * (double) p + 8.89806079864502) : (float) (10.800000190734863 * (double) p * (double) p - 20.520000457763672 * (double) p + 10.720000267028809);
    }

    public static float BounceEaseInOut(float p)
    {
      return (double) p < 0.5 ? 0.5f * EasyUtility.BounceEaseIn(p * 2f) : (float) (0.5 * (double) EasyUtility.BounceEaseOut((float) ((double) p * 2.0 - 1.0)) + 0.5);
    }

    public enum Functions
    {
      Linear,
      QuadraticEaseIn,
      QuadraticEaseOut,
      QuadraticEaseInOut,
      CubicEaseIn,
      CubicEaseOut,
      CubicEaseInOut,
      QuarticEaseIn,
      QuarticEaseOut,
      QuarticEaseInOut,
      QuinticEaseIn,
      QuinticEaseOut,
      QuinticEaseInOut,
      SineEaseIn,
      SineEaseOut,
      SineEaseInOut,
      CircularEaseIn,
      CircularEaseOut,
      CircularEaseInOut,
      ExponentialEaseIn,
      ExponentialEaseOut,
      ExponentialEaseInOut,
      ElasticEaseIn,
      ElasticEaseOut,
      ElasticEaseInOut,
      BackEaseIn,
      BackEaseOut,
      BackEaseInOut,
      BounceEaseIn,
      BounceEaseOut,
      BounceEaseInOut,
    }
  }
}
