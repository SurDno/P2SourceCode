namespace TriangleNet.Geometry
{
  public class Edge(int p0, int p1, int boundary) {
    public int P0 { get; private set; } = p0;

    public int P1 { get; private set; } = p1;

    public int Boundary { get; private set; } = boundary;

    public Edge(int p0, int p1)
      : this(p0, p1, 0)
    {
    }
  }
}
