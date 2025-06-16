using UnityEngine;

namespace Cinemachine.Utility
{
  public static class Damper
  {
    private const float Epsilon = 0.0001f;
    public const float kNegligibleResidual = 0.01f;

    private static float DecayConstant(float time, float residual)
    {
      return Mathf.Log(1f / residual) / time;
    }

    private static float Decay(float initial, float decayConstant, float deltaTime)
    {
      return initial / Mathf.Exp(decayConstant * deltaTime);
    }

    public static float Damp(float initial, float dampTime, float deltaTime)
    {
      if ((double) dampTime < 9.9999997473787516E-05 || (double) Mathf.Abs(initial) < 9.9999997473787516E-05)
        return initial;
      return (double) deltaTime < 9.9999997473787516E-05 ? 0.0f : initial - Damper.Decay(initial, Damper.DecayConstant(dampTime, 0.01f), deltaTime);
    }

    public static Vector3 Damp(Vector3 initial, Vector3 dampTime, float deltaTime)
    {
      for (int index = 0; index < 3; ++index)
        initial[index] = Damper.Damp(initial[index], dampTime[index], deltaTime);
      return initial;
    }

    public static Vector3 Damp(Vector3 initial, float dampTime, float deltaTime)
    {
      for (int index = 0; index < 3; ++index)
        initial[index] = Damper.Damp(initial[index], dampTime, deltaTime);
      return initial;
    }
  }
}
