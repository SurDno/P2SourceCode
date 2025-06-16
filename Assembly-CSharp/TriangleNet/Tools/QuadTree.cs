// Decompiled with JetBrains decompiler
// Type: TriangleNet.Tools.QuadTree
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Linq;
using TriangleNet.Data;
using TriangleNet.Geometry;

#nullable disable
namespace TriangleNet.Tools
{
  public class QuadTree
  {
    internal int maxDepth;
    private QuadNode root;
    internal int sizeBound;
    internal ITriangle[] triangles;

    public QuadTree(Mesh mesh, int maxDepth = 10, int sizeBound = 10)
    {
      this.maxDepth = maxDepth;
      this.sizeBound = sizeBound;
      this.triangles = (ITriangle[]) mesh.Triangles.ToArray<Triangle>();
      int num1 = 0;
      this.root = new QuadNode(mesh.Bounds, this, true);
      int num2;
      this.root.CreateSubRegion(num2 = num1 + 1);
    }

    public ITriangle Query(double x, double y)
    {
      Point point = new Point(x, y);
      List<int> triangles = this.root.FindTriangles(point);
      List<ITriangle> source = new List<ITriangle>();
      foreach (int index in triangles)
      {
        ITriangle triangle = this.triangles[index];
        if (QuadTree.IsPointInTriangle(point, (Point) triangle.GetVertex(0), (Point) triangle.GetVertex(1), (Point) triangle.GetVertex(2)))
          source.Add(triangle);
      }
      return source.FirstOrDefault<ITriangle>();
    }

    internal static bool IsPointInTriangle(Point p, Point t0, Point t1, Point t2)
    {
      Point p1 = new Point(t1.X - t0.X, t1.Y - t0.Y);
      Point p2 = new Point(t2.X - t0.X, t2.Y - t0.Y);
      Point p3 = new Point(p.X - t0.X, p.Y - t0.Y);
      Point q1 = new Point(-p1.Y, p1.X);
      Point q2 = new Point(-p2.Y, p2.X);
      double num1 = QuadTree.DotProduct(p3, q2) / QuadTree.DotProduct(p1, q2);
      double num2 = QuadTree.DotProduct(p3, q1) / QuadTree.DotProduct(p2, q1);
      return num1 >= 0.0 && num2 >= 0.0 && num1 + num2 <= 1.0;
    }

    internal static double DotProduct(Point p, Point q) => p.X * q.X + p.Y * q.Y;
  }
}
