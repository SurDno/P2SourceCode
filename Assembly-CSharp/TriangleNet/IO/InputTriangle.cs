using TriangleNet.Data;
using TriangleNet.Geometry;

namespace TriangleNet.IO
{
  public class InputTriangle : ITriangle
  {
    internal double area;
    internal int region;
    internal int[] vertices;

    public InputTriangle(int p0, int p1, int p2)
    {
      vertices = new int[3]{ p0, p1, p2 };
    }

    public int ID => 0;

    public int P0 => vertices[0];

    public int P1 => vertices[1];

    public int P2 => vertices[2];

    public Vertex GetVertex(int index) => null;

    public bool SupportsNeighbors => false;

    public int N0 => -1;

    public int N1 => -1;

    public int N2 => -1;

    public ITriangle GetNeighbor(int index) => null;

    public ISegment GetSegment(int index) => null;

    public double Area
    {
      get => area;
      set => area = value;
    }

    public int Region
    {
      get => region;
      set => region = value;
    }
  }
}
