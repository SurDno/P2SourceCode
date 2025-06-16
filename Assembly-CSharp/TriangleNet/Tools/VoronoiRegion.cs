using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;

namespace TriangleNet.Tools
{
  public class VoronoiRegion
  {
    private bool bounded;
    private Point generator;
    private int id;
    private List<Point> vertices;

    public int ID => this.id;

    public Point Generator => this.generator;

    public ICollection<Point> Vertices => (ICollection<Point>) this.vertices;

    public bool Bounded
    {
      get => this.bounded;
      set => this.bounded = value;
    }

    public VoronoiRegion(Vertex generator)
    {
      this.id = generator.id;
      this.generator = (Point) generator;
      this.vertices = new List<Point>();
      this.bounded = true;
    }

    public void Add(Point point) => this.vertices.Add(point);

    public void Add(List<Point> points) => this.vertices.AddRange((IEnumerable<Point>) points);

    public override string ToString() => string.Format("R-ID {0}", (object) this.id);
  }
}
