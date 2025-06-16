using UnityEngine;

namespace ParadoxNotion
{
  public static class RectUtils
  {
    public static Rect GetBoundRect(params Rect[] rects)
    {
      float num1 = float.PositiveInfinity;
      float num2 = float.NegativeInfinity;
      float num3 = float.PositiveInfinity;
      float num4 = float.NegativeInfinity;
      for (int index = 0; index < rects.Length; ++index)
      {
        num1 = Mathf.Min(num1, rects[index].xMin);
        num2 = Mathf.Max(num2, rects[index].xMax);
        num3 = Mathf.Min(num3, rects[index].yMin);
        num4 = Mathf.Max(num4, rects[index].yMax);
      }
      return Rect.MinMaxRect(num1, num3, num2, num4);
    }

    public static Rect GetBoundRect(params Vector2[] positions)
    {
      float num1 = float.PositiveInfinity;
      float num2 = float.NegativeInfinity;
      float num3 = float.PositiveInfinity;
      float num4 = float.NegativeInfinity;
      for (int index = 0; index < positions.Length; ++index)
      {
        num1 = Mathf.Min(num1, positions[index].x);
        num2 = Mathf.Max(num2, positions[index].x);
        num3 = Mathf.Min(num3, positions[index].y);
        num4 = Mathf.Max(num4, positions[index].y);
      }
      return Rect.MinMaxRect(num1, num3, num2, num4);
    }

    public static bool Encapsulates(this Rect a, Rect b)
    {
      return a.x < (double) b.x && a.xMax > (double) b.xMax && a.y < (double) b.y && a.yMax > (double) b.yMax;
    }
  }
}
