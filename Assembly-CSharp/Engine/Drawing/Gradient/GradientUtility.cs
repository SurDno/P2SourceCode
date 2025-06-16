// Decompiled with JetBrains decompiler
// Type: Engine.Drawing.Gradient.GradientUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Drawing.Gradient
{
  public static class GradientUtility
  {
    public static void Copy(UnityEngine.Gradient from, ColorGradient to)
    {
      if (from == null || to == null)
        return;
      to.AlphaKeys.Clear();
      to.ColorKeys.Clear();
      GradientColorKey[] colorKeys = from.colorKeys;
      for (int index = 0; index < colorKeys.Length; ++index)
      {
        if (to.ColorKeys.Count <= index)
          to.ColorKeys.Add(colorKeys[index]);
        else
          to.ColorKeys[index] = colorKeys[index];
      }
      GradientAlphaKey[] alphaKeys = from.alphaKeys;
      for (int index = 0; index < alphaKeys.Length; ++index)
      {
        if (to.AlphaKeys.Count <= index)
          to.AlphaKeys.Add(alphaKeys[index]);
        else
          to.AlphaKeys[index] = alphaKeys[index];
      }
    }

    public static void Copy(ColorGradient from, UnityEngine.Gradient to)
    {
      if (from == null || to == null)
        return;
      GradientColorKey[] gradientColorKeyArray = to.colorKeys;
      if (gradientColorKeyArray.Length != from.ColorKeys.Count)
        gradientColorKeyArray = new GradientColorKey[from.ColorKeys.Count];
      for (int index = 0; index < gradientColorKeyArray.Length; ++index)
        gradientColorKeyArray[index] = from.ColorKeys[index];
      to.colorKeys = gradientColorKeyArray;
      GradientAlphaKey[] gradientAlphaKeyArray = to.alphaKeys;
      if (gradientAlphaKeyArray.Length != from.AlphaKeys.Count)
        gradientAlphaKeyArray = new GradientAlphaKey[from.AlphaKeys.Count];
      for (int index = 0; index < gradientAlphaKeyArray.Length; ++index)
        gradientAlphaKeyArray[index] = from.AlphaKeys[index];
      to.alphaKeys = gradientAlphaKeyArray;
    }
  }
}
