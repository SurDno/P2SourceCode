// Decompiled with JetBrains decompiler
// Type: Engine.Source.Utility.FunctionUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
