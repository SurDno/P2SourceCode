using UnityEngine;

namespace Engine.Source.Utility
{
  public static class FunctionUtility
  {
    public static float DefaultFunction(float progress, float total) => 1f;

    public static float EyeFunction(float progress, float total)
    {
      float num = total * 0.5f;
      return (float) ((double) Mathf.Clamp01(1f - Mathf.Pow((float) ((double) progress / (double) num - 1.0), 2f)) * 0.5 + 0.5);
    }
  }
}
