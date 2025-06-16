using System;
using UnityEngine;

namespace Engine.Source.Services.Gizmos
{
  public static class GizmoUtility
  {
    private const float minSegmentCount = 32f;
    private const float maxSegmentCount = 100f;
    private const float segmentLen = 0.5f;

    public static void DrawBox(Vector3 min, Vector3 max)
    {
      GL.Vertex(new Vector3(min.x, min.y, min.z));
      GL.Vertex(new Vector3(max.x, min.y, min.z));
      GL.Vertex(new Vector3(max.x, min.y, min.z));
      GL.Vertex(new Vector3(max.x, min.y, max.z));
      GL.Vertex(new Vector3(max.x, min.y, max.z));
      GL.Vertex(new Vector3(min.x, min.y, max.z));
      GL.Vertex(new Vector3(min.x, min.y, max.z));
      GL.Vertex(new Vector3(min.x, min.y, min.z));
      GL.Vertex(new Vector3(min.x, max.y, min.z));
      GL.Vertex(new Vector3(max.x, max.y, min.z));
      GL.Vertex(new Vector3(max.x, max.y, min.z));
      GL.Vertex(new Vector3(max.x, max.y, max.z));
      GL.Vertex(new Vector3(max.x, max.y, max.z));
      GL.Vertex(new Vector3(min.x, max.y, max.z));
      GL.Vertex(new Vector3(min.x, max.y, max.z));
      GL.Vertex(new Vector3(min.x, max.y, min.z));
      GL.Vertex(new Vector3(min.x, min.y, min.z));
      GL.Vertex(new Vector3(min.x, max.y, min.z));
      GL.Vertex(new Vector3(max.x, min.y, min.z));
      GL.Vertex(new Vector3(max.x, max.y, min.z));
      GL.Vertex(new Vector3(max.x, min.y, max.z));
      GL.Vertex(new Vector3(max.x, max.y, max.z));
      GL.Vertex(new Vector3(min.x, min.y, max.z));
      GL.Vertex(new Vector3(min.x, max.y, max.z));
    }

    public static void DrawCircle(Vector3 position, float radius, bool solid)
    {
      int num = (int) Mathf.Clamp((float) (int) ((double) (6.28318548f * radius) / 0.5), 32f, 100f) / 2 * 2;
      Vector3 v1 = new Vector3(Mathf.Cos(0.0f) * radius + position.x, position.y, Mathf.Sin(0.0f) * radius + position.z);
      for (int index = 0; index < num; ++index)
      {
        float f = Mathf.Lerp(0.0f, 6.28318548f, (float) (index + 1) / (float) num);
        Vector3 v2 = new Vector3(Mathf.Cos(f) * radius + position.x, position.y, Mathf.Sin(f) * radius + position.z);
        if (solid || index % 2 != 1)
        {
          GL.Vertex(v1);
          GL.Vertex(v2);
        }
        v1 = v2;
      }
    }

    public static void DrawSector(
      Vector3 position,
      float radius,
      float startAngle,
      float endAngle,
      bool solid,
      Func<float, float, float> func)
    {
      startAngle %= 360f;
      if ((double) startAngle < 0.0)
        startAngle += 360f;
      endAngle %= 360f;
      if ((double) endAngle < 0.0)
        endAngle += 360f;
      float num1 = startAngle * ((float) Math.PI / 180f);
      float num2 = endAngle * ((float) Math.PI / 180f);
      if ((double) num1 > (double) num2)
        num2 += 6.28318548f;
      int num3 = (int) Mathf.Clamp((float) (int) ((double) ((num2 - num1) * radius) / 0.5), 32f, 100f) / 2 * 2 + 1;
      float num4 = radius * func(0.0f, num2 - num1);
      Vector3 v1 = new Vector3(Mathf.Cos(num1) * num4 + position.x, position.y, Mathf.Sin(num1) * num4 + position.z);
      GL.Vertex(v1);
      GL.Vertex(position);
      float num5 = radius * func(num2 - num1, num2 - num1);
      Vector3 v2 = new Vector3(Mathf.Cos(num2) * num5 + position.x, position.y, Mathf.Sin(num2) * num5 + position.z);
      GL.Vertex(v2);
      GL.Vertex(position);
      Vector3 v3 = v1;
      for (int index = 0; index < num3; ++index)
      {
        float f = Mathf.Lerp(num1, num2, (float) (index + 1) / (float) num3);
        float num6 = radius * func(f - num1, num2 - num1);
        Vector3 v4 = new Vector3(Mathf.Cos(f) * num6 + position.x, position.y, Mathf.Sin(f) * num6 + position.z);
        if (solid || index % 2 != 1)
        {
          GL.Vertex(v3);
          GL.Vertex(v4);
        }
        v3 = v4;
      }
      GL.Vertex(v3);
      GL.Vertex(v2);
    }
  }
}
