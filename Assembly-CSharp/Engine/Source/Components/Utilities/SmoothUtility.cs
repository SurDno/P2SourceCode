// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Utilities.SmoothUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Source.Components.Utilities
{
  public static class SmoothUtility
  {
    public static float Smooth12(float x)
    {
      float num = 1f - x;
      return Mathf.Lerp(x, (float) (1.0 - (double) num * (double) num), x);
    }

    public static float Smooth13(float x)
    {
      float num = 1f - x;
      return Mathf.Lerp(x, (float) (1.0 - (double) num * (double) num * (double) num), x);
    }

    public static float Smooth22(float x)
    {
      float num = 1f - x;
      return Mathf.Lerp(x * x, (float) (1.0 - (double) num * (double) num), x);
    }

    public static float Smooth32(float x)
    {
      float num = 1f - x;
      return Mathf.Lerp(x * x * x, (float) (1.0 - (double) num * (double) num), x);
    }

    public static float Smooth33(float x)
    {
      float num = 1f - x;
      return Mathf.Lerp(x * x * x, (float) (1.0 - (double) num * (double) num * (double) num), x);
    }

    public static float Smooth42(float x)
    {
      float num = 1f - x;
      return Mathf.Lerp(x * x * x * x, (float) (1.0 - (double) num * (double) num), x);
    }

    public static float Smooth43(float x)
    {
      float num = 1f - x;
      return Mathf.Lerp(x * x * x * x, (float) (1.0 - (double) num * (double) num * (double) num), x);
    }

    public static float Smooth44(float x)
    {
      float num = 1f - x;
      return Mathf.Lerp(x * x * x * x, (float) (1.0 - (double) num * (double) num * (double) num * (double) num), x);
    }
  }
}
