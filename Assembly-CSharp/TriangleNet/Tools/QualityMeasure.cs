using System;
using TriangleNet.Data;
using TriangleNet.Geometry;

namespace TriangleNet.Tools
{
  public class QualityMeasure
  {
    private AlphaMeasure alphaMeasure = new();
    private AreaMeasure areaMeasure = new();
    private Mesh mesh;
    private Q_Measure qMeasure = new();

    public double AreaMinimum => areaMeasure.area_min;

    public double AreaMaximum => areaMeasure.area_max;

    public double AreaRatio => areaMeasure.area_max / areaMeasure.area_min;

    public double AlphaMinimum => alphaMeasure.alpha_min;

    public double AlphaMaximum => alphaMeasure.alpha_max;

    public double AlphaAverage => alphaMeasure.alpha_ave;

    public double AlphaArea => alphaMeasure.alpha_area;

    public double Q_Minimum => qMeasure.q_min;

    public double Q_Maximum => qMeasure.q_max;

    public double Q_Average => qMeasure.q_ave;

    public double Q_Area => qMeasure.q_area;

    public void Update(Mesh mesh)
    {
      this.mesh = mesh;
      areaMeasure.Reset();
      alphaMeasure.Reset();
      qMeasure.Reset();
      Compute();
    }

    private void Compute()
    {
      int n = 0;
      foreach (Triangle triangle in mesh.triangles.Values)
      {
        ++n;
        Point vertex1 = triangle.vertices[0];
        Point vertex2 = triangle.vertices[1];
        Point vertex3 = triangle.vertices[2];
        double num1 = vertex1.x - vertex2.x;
        double num2 = vertex1.y - vertex2.y;
        double ab = Math.Sqrt(num1 * num1 + num2 * num2);
        double num3 = vertex2.x - vertex3.x;
        double num4 = vertex2.y - vertex3.y;
        double bc = Math.Sqrt(num3 * num3 + num4 * num4);
        double num5 = vertex3.x - vertex1.x;
        double num6 = vertex3.y - vertex1.y;
        double ca = Math.Sqrt(num5 * num5 + num6 * num6);
        double area = areaMeasure.Measure(vertex1, vertex2, vertex3);
        alphaMeasure.Measure(ab, bc, ca, area);
        qMeasure.Measure(ab, bc, ca, area);
      }
      alphaMeasure.Normalize(n, areaMeasure.area_total);
      qMeasure.Normalize(n, areaMeasure.area_total);
    }

    public int Bandwidth()
    {
      if (mesh == null)
        return 0;
      int val1_1 = 0;
      int val1_2 = 0;
      foreach (Triangle triangle in mesh.triangles.Values)
      {
        for (int index1 = 0; index1 < 3; ++index1)
        {
          int id1 = triangle.GetVertex(index1).id;
          for (int index2 = 0; index2 < 3; ++index2)
          {
            int id2 = triangle.GetVertex(index2).id;
            val1_2 = Math.Max(val1_2, id2 - id1);
            val1_1 = Math.Max(val1_1, id1 - id2);
          }
        }
      }
      return val1_1 + 1 + val1_2;
    }

    private class AreaMeasure
    {
      public double area_max = double.MinValue;
      public double area_min = double.MaxValue;
      public double area_total;
      public int area_zero;

      public void Reset()
      {
        area_min = double.MaxValue;
        area_max = double.MinValue;
        area_total = 0.0;
        area_zero = 0;
      }

      public double Measure(Point a, Point b, Point c)
      {
        double val2 = 0.5 * Math.Abs(a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));
        area_min = Math.Min(area_min, val2);
        area_max = Math.Max(area_max, val2);
        area_total += val2;
        if (val2 == 0.0)
          ++area_zero;
        return val2;
      }
    }

    private class AlphaMeasure
    {
      public double alpha_area;
      public double alpha_ave;
      public double alpha_max;
      public double alpha_min;

      public void Reset()
      {
        alpha_min = double.MaxValue;
        alpha_max = double.MinValue;
        alpha_ave = 0.0;
        alpha_area = 0.0;
      }

      private double acos(double c)
      {
        if (c <= -1.0)
          return Math.PI;
        return 1.0 <= c ? 0.0 : Math.Acos(c);
      }

      public double Measure(double ab, double bc, double ca, double area)
      {
        double maxValue = double.MaxValue;
        double num1 = ab * ab;
        double num2 = bc * bc;
        double num3 = ca * ca;
        double val2_1;
        double val2_2;
        double val2_3;
        if (ab == 0.0 && bc == 0.0 && ca == 0.0)
        {
          val2_1 = 2.0 * Math.PI / 3.0;
          val2_2 = 2.0 * Math.PI / 3.0;
          val2_3 = 2.0 * Math.PI / 3.0;
        }
        else
        {
          val2_1 = ca != 0.0 && ab != 0.0 ? acos((num3 + num1 - num2) / (2.0 * ca * ab)) : Math.PI;
          val2_2 = ab != 0.0 && bc != 0.0 ? acos((num1 + num2 - num3) / (2.0 * ab * bc)) : Math.PI;
          val2_3 = bc != 0.0 && ca != 0.0 ? acos((num2 + num3 - num1) / (2.0 * bc * ca)) : Math.PI;
        }
        double val1 = Math.Min(Math.Min(Math.Min(maxValue, val2_1), val2_2), val2_3) * 3.0 / Math.PI;
        alpha_ave += val1;
        alpha_area += area * val1;
        alpha_min = Math.Min(val1, alpha_min);
        alpha_max = Math.Max(val1, alpha_max);
        return val1;
      }

      public void Normalize(int n, double area_total)
      {
        if (n > 0)
          alpha_ave /= n;
        else
          alpha_ave = 0.0;
        if (0.0 < area_total)
          alpha_area /= area_total;
        else
          alpha_area = 0.0;
      }
    }

    private class Q_Measure
    {
      public double q_area;
      public double q_ave;
      public double q_max;
      public double q_min;

      public void Reset()
      {
        q_min = double.MaxValue;
        q_max = double.MinValue;
        q_ave = 0.0;
        q_area = 0.0;
      }

      public double Measure(double ab, double bc, double ca, double area)
      {
        double val2 = (bc + ca - ab) * (ca + ab - bc) * (ab + bc - ca) / (ab * bc * ca);
        q_min = Math.Min(q_min, val2);
        q_max = Math.Max(q_max, val2);
        q_ave += val2;
        q_area += val2 * area;
        return val2;
      }

      public void Normalize(int n, double area_total)
      {
        if (n > 0)
          q_ave /= n;
        else
          q_ave = 0.0;
        if (area_total > 0.0)
          q_area /= area_total;
        else
          q_area = 0.0;
      }
    }
  }
}
