using System;
using System.Collections.Generic;
using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;
using Inspectors;
using UnityEngine;

namespace Engine.Drawing.Gradient
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ColorGradient : IBlendable<ColorGradient>
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    protected List<GradientAlphaKey> alphaKeys = new(512);
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    protected List<GradientColorKey> colorKeys = new(512);

    public List<GradientColorKey> ColorKeys
    {
      get => colorKeys;
      set => colorKeys = value;
    }

    public List<GradientAlphaKey> AlphaKeys
    {
      get => alphaKeys;
      set => alphaKeys = value;
    }

    public void Blend(ColorGradient a, ColorGradient b, IPureBlendOperation opp)
    {
      IBlendOperation op = (IBlendOperation) opp;
      BlendColor(a, b, op);
      BlendAlpha(a, b, op);
    }

    private void BlendColor(ColorGradient a, ColorGradient b, IBlendOperation op)
    {
      ColorKeys.Clear();
      if (a.ColorKeys.Count == 0)
      {
        GradientColorKey gradientColorKey = new GradientColorKey(Color.clear, 0.0f);
        a.ColorKeys.Add(gradientColorKey);
      }
      if (b.ColorKeys.Count == 0)
      {
        GradientColorKey gradientColorKey = new GradientColorKey(Color.clear, 0.0f);
        b.ColorKeys.Add(gradientColorKey);
      }
      int index1 = 0;
      int index2 = 0;
      while (index1 < a.ColorKeys.Count || index2 < b.ColorKeys.Count)
      {
        bool flag1 = index1 < a.ColorKeys.Count;
        bool flag2 = index2 < b.ColorKeys.Count;
        float time1 = flag1 ? a.ColorKeys[index1].time : 0.0f;
        float time2 = flag2 ? b.ColorKeys[index2].time : 0.0f;
        GradientColorKey gradientColorKey;
        if (flag1 && !flag2 || flag1 & flag2 && time1 < (double) time2)
        {
          gradientColorKey = new GradientColorKey();
          gradientColorKey.color = op.Blend(a.ColorKeys[index1].color, GetColor(b.ColorKeys, index2, time1));
          gradientColorKey.time = time1;
          ColorKeys.Add(gradientColorKey);
          ++index1;
        }
        else if (!flag1 & flag2 || flag1 & flag2 && time1 > (double) time2)
        {
          gradientColorKey = new GradientColorKey();
          gradientColorKey.color = op.Blend(GetColor(a.ColorKeys, index1, time2), b.ColorKeys[index2].color);
          gradientColorKey.time = time2;
          ColorKeys.Add(gradientColorKey);
          ++index2;
        }
        else
        {
          if (!(flag1 & flag2) || time1 != (double) time2)
            throw new Exception();
          gradientColorKey = new GradientColorKey();
          gradientColorKey.color = op.Blend(a.ColorKeys[index1].color, b.ColorKeys[index2].color);
          gradientColorKey.time = time1;
          ColorKeys.Add(gradientColorKey);
          ++index1;
          ++index2;
        }
      }
    }

    private void BlendAlpha(ColorGradient a, ColorGradient b, IBlendOperation op)
    {
      AlphaKeys.Clear();
      if (a.AlphaKeys.Count == 0)
      {
        GradientAlphaKey gradientAlphaKey = new GradientAlphaKey(0.0f, 0.0f);
        a.AlphaKeys.Add(gradientAlphaKey);
      }
      if (b.AlphaKeys.Count == 0)
      {
        GradientAlphaKey gradientAlphaKey = new GradientAlphaKey(0.0f, 0.0f);
        b.AlphaKeys.Add(gradientAlphaKey);
      }
      int index1 = 0;
      int index2 = 0;
      while (index1 < a.AlphaKeys.Count || index2 < b.AlphaKeys.Count)
      {
        bool flag1 = index1 < a.AlphaKeys.Count;
        bool flag2 = index2 < b.AlphaKeys.Count;
        float time1 = flag1 ? a.AlphaKeys[index1].time : 0.0f;
        float time2 = flag2 ? b.AlphaKeys[index2].time : 0.0f;
        GradientAlphaKey gradientAlphaKey;
        if (flag1 && !flag2 || flag1 & flag2 && time1 < (double) time2)
        {
          gradientAlphaKey = new GradientAlphaKey();
          gradientAlphaKey.alpha = op.Blend(a.AlphaKeys[index1].alpha, GetAlpha(b.AlphaKeys, index2, time1));
          gradientAlphaKey.time = time1;
          AlphaKeys.Add(gradientAlphaKey);
          ++index1;
        }
        else if (!flag1 & flag2 || flag1 & flag2 && time1 > (double) time2)
        {
          gradientAlphaKey = new GradientAlphaKey();
          gradientAlphaKey.alpha = op.Blend(GetAlpha(a.AlphaKeys, index1, time2), b.AlphaKeys[index2].alpha);
          gradientAlphaKey.time = time2;
          AlphaKeys.Add(gradientAlphaKey);
          ++index2;
        }
        else
        {
          if (!(flag1 & flag2) || time1 != (double) time2)
            throw new Exception();
          gradientAlphaKey = new GradientAlphaKey();
          gradientAlphaKey.alpha = op.Blend(a.AlphaKeys[index1].alpha, b.AlphaKeys[index2].alpha);
          gradientAlphaKey.time = time1;
          AlphaKeys.Add(gradientAlphaKey);
          ++index1;
          ++index2;
        }
      }
    }

    private static Color GetColor(List<GradientColorKey> items, int index, float time)
    {
      if (index >= items.Count)
      {
        --index;
        return index < 0 ? Color.clear : items[index].color;
      }
      if (index == 0)
        return items[index].color;
      GradientColorKey gradientColorKey1 = items[index - 1];
      GradientColorKey gradientColorKey2 = items[index];
      float t = Mathf.Clamp01(time - gradientColorKey1.time);
      return Color.Lerp(gradientColorKey1.color, gradientColorKey2.color, t);
    }

    private static float GetAlpha(List<GradientAlphaKey> items, int index, float time)
    {
      if (index >= items.Count)
      {
        --index;
        return index < 0 ? 0.0f : items[index].alpha;
      }
      if (index == 0)
        return items[index].alpha;
      GradientAlphaKey gradientAlphaKey1 = items[index - 1];
      GradientAlphaKey gradientAlphaKey2 = items[index];
      float t = Mathf.Clamp01(time - gradientAlphaKey1.time);
      return Mathf.Lerp(gradientAlphaKey1.alpha, gradientAlphaKey2.alpha, t);
    }
  }
}
