// Decompiled with JetBrains decompiler
// Type: TriangleNet.Tools.Voronoi
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;

#nullable disable
namespace TriangleNet.Tools
{
  public class Voronoi : IVoronoi
  {
    private BoundingBox bounds;
    private Mesh mesh;
    private Point[] points;
    private int rayIndex;
    private Dictionary<int, Point> rayPoints;
    private List<VoronoiRegion> regions;

    public Voronoi(Mesh mesh)
    {
      this.mesh = mesh;
      this.Generate();
    }

    public Point[] Points => this.points;

    public List<VoronoiRegion> Regions => this.regions;

    private void Generate()
    {
      this.mesh.Renumber();
      this.mesh.MakeVertexMap();
      this.points = new Point[this.mesh.triangles.Count + this.mesh.hullsize];
      this.regions = new List<VoronoiRegion>(this.mesh.vertices.Count);
      this.rayPoints = new Dictionary<int, Point>();
      this.rayIndex = 0;
      this.bounds = new BoundingBox();
      this.ComputeCircumCenters();
      foreach (Vertex vertex in this.mesh.vertices.Values)
        this.ConstructVoronoiRegion(vertex);
    }

    private void ComputeCircumCenters()
    {
      Otri otri = new Otri();
      double xi = 0.0;
      double eta = 0.0;
      foreach (Triangle triangle in this.mesh.triangles.Values)
      {
        otri.triangle = triangle;
        Point circumcenter = Primitives.FindCircumcenter((Point) otri.Org(), (Point) otri.Dest(), (Point) otri.Apex(), ref xi, ref eta);
        circumcenter.id = triangle.id;
        this.points[triangle.id] = circumcenter;
        this.bounds.Update(circumcenter.x, circumcenter.y);
      }
      double num = Math.Max(this.bounds.Width, this.bounds.Height);
      this.bounds.Scale(num, num);
    }

    private void ConstructVoronoiRegion(Vertex vertex)
    {
      VoronoiRegion voronoiRegion = new VoronoiRegion(vertex);
      this.regions.Add(voronoiRegion);
      List<Point> points = new List<Point>();
      Otri o2_1 = new Otri();
      Otri o2_2 = new Otri();
      Otri o2_3 = new Otri();
      Otri o2_4 = new Otri();
      Osub os = new Osub();
      vertex.tri.Copy(ref o2_2);
      o2_2.Copy(ref o2_1);
      o2_2.Onext(ref o2_3);
      if (o2_3.triangle == Mesh.dummytri)
      {
        o2_2.Oprev(ref o2_4);
        if (o2_4.triangle != Mesh.dummytri)
        {
          o2_2.Copy(ref o2_3);
          o2_2.OprevSelf();
          o2_2.Copy(ref o2_1);
        }
      }
      while (o2_3.triangle != Mesh.dummytri)
      {
        points.Add(this.points[o2_1.triangle.id]);
        if (o2_3.Equal(o2_2))
        {
          voronoiRegion.Add(points);
          return;
        }
        o2_3.Copy(ref o2_1);
        o2_3.OnextSelf();
      }
      voronoiRegion.Bounded = false;
      int count = this.mesh.triangles.Count;
      o2_1.Lprev(ref o2_3);
      o2_3.SegPivot(ref os);
      int hash1 = os.seg.hash;
      points.Add(this.points[o2_1.triangle.id]);
      Vertex intersect;
      if (this.rayPoints.ContainsKey(hash1))
      {
        points.Add(this.rayPoints[hash1]);
      }
      else
      {
        Vertex vertex1 = o2_1.Org();
        Vertex vertex2 = o2_1.Apex();
        this.BoxRayIntersection(this.points[o2_1.triangle.id], vertex1.y - vertex2.y, vertex2.x - vertex1.x, out intersect);
        intersect.id = count + this.rayIndex;
        this.points[count + this.rayIndex] = (Point) intersect;
        ++this.rayIndex;
        points.Add((Point) intersect);
        this.rayPoints.Add(hash1, (Point) intersect);
      }
      points.Reverse();
      o2_2.Copy(ref o2_1);
      o2_1.Oprev(ref o2_4);
      while (o2_4.triangle != Mesh.dummytri)
      {
        points.Add(this.points[o2_4.triangle.id]);
        o2_4.Copy(ref o2_1);
        o2_4.OprevSelf();
      }
      o2_1.SegPivot(ref os);
      int hash2 = os.seg.hash;
      if (this.rayPoints.ContainsKey(hash2))
      {
        points.Add(this.rayPoints[hash2]);
      }
      else
      {
        Vertex vertex3 = o2_1.Org();
        Vertex vertex4 = o2_1.Dest();
        this.BoxRayIntersection(this.points[o2_1.triangle.id], vertex4.y - vertex3.y, vertex3.x - vertex4.x, out intersect);
        intersect.id = count + this.rayIndex;
        this.points[count + this.rayIndex] = (Point) intersect;
        ++this.rayIndex;
        points.Add((Point) intersect);
        this.rayPoints.Add(hash2, (Point) intersect);
      }
      points.Reverse();
      voronoiRegion.Add(points);
    }

    private bool BoxRayIntersection(Point pt, double dx, double dy, out Vertex intersect)
    {
      double x1 = pt.X;
      double y1 = pt.Y;
      double xmin = this.bounds.Xmin;
      double xmax = this.bounds.Xmax;
      double ymin = this.bounds.Ymin;
      double ymax = this.bounds.Ymax;
      if (x1 < xmin || x1 > xmax || y1 < ymin || y1 > ymax)
      {
        intersect = (Vertex) null;
        return false;
      }
      double num1;
      double x2;
      double y2;
      if (dx < 0.0)
      {
        num1 = (xmin - x1) / dx;
        x2 = xmin;
        y2 = y1 + num1 * dy;
      }
      else if (dx > 0.0)
      {
        num1 = (xmax - x1) / dx;
        x2 = xmax;
        y2 = y1 + num1 * dy;
      }
      else
      {
        num1 = double.MaxValue;
        x2 = y2 = 0.0;
      }
      double num2;
      double x3;
      double y3;
      if (dy < 0.0)
      {
        num2 = (ymin - y1) / dy;
        x3 = x1 + num2 * dx;
        y3 = ymin;
      }
      else if (dx > 0.0)
      {
        num2 = (ymax - y1) / dy;
        x3 = x1 + num2 * dx;
        y3 = ymax;
      }
      else
      {
        num2 = double.MaxValue;
        x3 = y3 = 0.0;
      }
      intersect = num1 >= num2 ? new Vertex(x3, y3, -1) : new Vertex(x2, y2, -1);
      return true;
    }
  }
}
