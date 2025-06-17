using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;

namespace TriangleNet.Tools
{
  public class VoronoiRegion(Vertex generator) {
    private bool bounded = true;
    private Point generator = generator;
    private int id = generator.id;
    private List<Point> vertices = [];

    public int ID => id;

    public Point Generator => generator;

    public ICollection<Point> Vertices => vertices;

    public bool Bounded
    {
      get => bounded;
      set => bounded = value;
    }

    public void Add(Point point) => vertices.Add(point);

    public void Add(List<Point> points) => vertices.AddRange(points);

    public override string ToString() => string.Format("R-ID {0}", id);
  }
}
