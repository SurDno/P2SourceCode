using System;
using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;

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
      Generate();
    }

    public Point[] Points => points;

    public List<VoronoiRegion> Regions => regions;

    private void Generate()
    {
      mesh.Renumber();
      mesh.MakeVertexMap();
      points = new Point[mesh.triangles.Count + mesh.hullsize];
      regions = new List<VoronoiRegion>(mesh.vertices.Count);
      rayPoints = new Dictionary<int, Point>();
      rayIndex = 0;
      bounds = new BoundingBox();
      ComputeCircumCenters();
      foreach (Vertex vertex in mesh.vertices.Values)
        ConstructVoronoiRegion(vertex);
    }

    private void ComputeCircumCenters()
    {
      Otri otri = new Otri();
      double xi = 0.0;
      double eta = 0.0;
      foreach (Triangle triangle in mesh.triangles.Values)
      {
        otri.triangle = triangle;
        Point circumcenter = Primitives.FindCircumcenter(otri.Org(), otri.Dest(), otri.Apex(), ref xi, ref eta);
        circumcenter.id = triangle.id;
        points[triangle.id] = circumcenter;
        bounds.Update(circumcenter.x, circumcenter.y);
      }
      double num = Math.Max(bounds.Width, bounds.Height);
      bounds.Scale(num, num);
    }

    private void ConstructVoronoiRegion(Vertex vertex)
    {
      VoronoiRegion voronoiRegion = new VoronoiRegion(vertex);
      regions.Add(voronoiRegion);
      List<Point> points = [];
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
      int count = mesh.triangles.Count;
      o2_1.Lprev(ref o2_3);
      o2_3.SegPivot(ref os);
      int hash1 = os.seg.hash;
      points.Add(this.points[o2_1.triangle.id]);
      Vertex intersect;
      if (rayPoints.ContainsKey(hash1))
      {
        points.Add(rayPoints[hash1]);
      }
      else
      {
        Vertex vertex1 = o2_1.Org();
        Vertex vertex2 = o2_1.Apex();
        BoxRayIntersection(this.points[o2_1.triangle.id], vertex1.y - vertex2.y, vertex2.x - vertex1.x, out intersect);
        intersect.id = count + rayIndex;
        this.points[count + rayIndex] = intersect;
        ++rayIndex;
        points.Add(intersect);
        rayPoints.Add(hash1, intersect);
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
      if (rayPoints.ContainsKey(hash2))
      {
        points.Add(rayPoints[hash2]);
      }
      else
      {
        Vertex vertex3 = o2_1.Org();
        Vertex vertex4 = o2_1.Dest();
        BoxRayIntersection(this.points[o2_1.triangle.id], vertex4.y - vertex3.y, vertex3.x - vertex4.x, out intersect);
        intersect.id = count + rayIndex;
        this.points[count + rayIndex] = intersect;
        ++rayIndex;
        points.Add(intersect);
        rayPoints.Add(hash2, intersect);
      }
      points.Reverse();
      voronoiRegion.Add(points);
    }

    private bool BoxRayIntersection(Point pt, double dx, double dy, out Vertex intersect)
    {
      double x1 = pt.X;
      double y1 = pt.Y;
      double xmin = bounds.Xmin;
      double xmax = bounds.Xmax;
      double ymin = bounds.Ymin;
      double ymax = bounds.Ymax;
      if (x1 < xmin || x1 > xmax || y1 < ymin || y1 > ymax)
      {
        intersect = null;
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
