namespace TriangleNet.Geometry
{
  public class Edge
  {
    public int P0 { get; private set; }

    public int P1 { get; private set; }

    public int Boundary { get; private set; }

    public Edge(int p0, int p1)
      : this(p0, p1, 0)
    {
    }

    public Edge(int p0, int p1, int boundary)
    {
      P0 = p0;
      P1 = p1;
      Boundary = boundary;
    }
  }
}
