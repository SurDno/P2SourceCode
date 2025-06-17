using System;
using System.Collections.Generic;
using TriangleNet.Data;

namespace TriangleNet.Geometry
{
  public class InputGeometry
  {
    private BoundingBox bounds;
    internal List<Point> holes;
    private int pointAttributes = -1;
    internal List<Vertex> points;
    internal List<RegionPointer> regions;
    internal List<Edge> segments;

    public BoundingBox Bounds => bounds;

    public bool HasSegments => segments.Count > 0;

    public int Count => points.Count;

    public IEnumerable<Point> Points => points;

    public ICollection<Edge> Segments => segments;

    public ICollection<Point> Holes => holes;

    public ICollection<RegionPointer> Regions => regions;

    public InputGeometry()
      : this(3)
    {
    }

    public InputGeometry(int capacity)
    {
      points = new List<Vertex>(capacity);
      segments = [];
      holes = [];
      regions = [];
      bounds = new BoundingBox();
      pointAttributes = -1;
    }

    public void Clear()
    {
      points.Clear();
      segments.Clear();
      holes.Clear();
      regions.Clear();
      pointAttributes = -1;
    }

    public void AddPoint(double x, double y) => AddPoint(x, y, 0);

    public void AddPoint(double x, double y, int boundary)
    {
      points.Add(new Vertex(x, y, boundary));
      bounds.Update(x, y);
    }

    public void AddPoint(double x, double y, int boundary, double attribute)
    {
      AddPoint(x, y, 0, [attribute]);
    }

    public void AddPoint(double x, double y, int boundary, double[] attribs)
    {
      if (pointAttributes < 0)
      {
        pointAttributes = attribs == null ? 0 : attribs.Length;
      }
      else
      {
        if (attribs == null && pointAttributes > 0)
          throw new ArgumentException("Inconsitent use of point attributes.");
        if (attribs != null && pointAttributes != attribs.Length)
          throw new ArgumentException("Inconsitent use of point attributes.");
      }
      List<Vertex> points = this.points;
      Vertex vertex = new Vertex(x, y, boundary);
      vertex.attributes = attribs;
      points.Add(vertex);
      bounds.Update(x, y);
    }

    public void AddHole(double x, double y) => holes.Add(new Point(x, y));

    public void AddRegion(double x, double y, int id)
    {
      regions.Add(new RegionPointer(x, y, id));
    }

    public void AddSegment(int p0, int p1) => AddSegment(p0, p1, 0);

    public void AddSegment(int p0, int p1, int boundary)
    {
      if (p0 == p1 || p0 < 0 || p1 < 0)
        throw new NotSupportedException("Invalid endpoints.");
      segments.Add(new Edge(p0, p1, boundary));
    }
  }
}
