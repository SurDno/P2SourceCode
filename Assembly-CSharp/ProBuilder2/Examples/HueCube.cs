using ProBuilder2.Common;
using UnityEngine;

namespace ProBuilder2.Examples
{
  public class HueCube : MonoBehaviour
  {
    private pb_Object pb;

    private void Start()
    {
      pb = pb_ShapeGenerator.CubeGenerator(Vector3.one);
      int length = pb.sharedIndices.Length;
      Color[] colorArray = new Color[length];
      for (int index = 0; index < length; ++index)
        colorArray[index] = HSVtoRGB((float) (index / (double) length * 360.0), 1f, 1f);
      Color[] colors = pb.colors;
      for (int index1 = 0; index1 < pb.sharedIndices.Length; ++index1)
      {
        foreach (int index2 in pb.sharedIndices[index1].array)
          colors[index2] = colorArray[index1];
      }
      pb.SetColors(colors);
      pb.Refresh();
    }

    private static Color HSVtoRGB(float h, float s, float v)
    {
      if (s == 0.0)
        return new Color(v, v, v, 1f);
      h /= 60f;
      int num1 = (int) Mathf.Floor(h);
      float num2 = h - num1;
      float num3 = v * (1f - s);
      float num4 = v * (float) (1.0 - s * (double) num2);
      float num5 = v * (float) (1.0 - s * (1.0 - num2));
      float r;
      float g;
      float b;
      switch (num1)
      {
        case 0:
          r = v;
          g = num5;
          b = num3;
          break;
        case 1:
          r = num4;
          g = v;
          b = num3;
          break;
        case 2:
          r = num3;
          g = v;
          b = num5;
          break;
        case 3:
          r = num3;
          g = num4;
          b = v;
          break;
        case 4:
          r = num5;
          g = num3;
          b = v;
          break;
        default:
          r = v;
          g = num3;
          b = num4;
          break;
      }
      return new Color(r, g, b, 1f);
    }
  }
}
