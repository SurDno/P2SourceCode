using System;
using TriangleNet.Geometry;

namespace TriangleNet.Data
{
  public class Vertex : Point
  {
    internal int hash;
    internal Otri tri;
    internal VertexType type;

    public VertexType Type => type;

    public double this[int i]
    {
      get
      {
        if (i == 0)
          return x;
        if (i == 1)
          return y;
        throw new ArgumentOutOfRangeException("Index must be 0 or 1.");
      }
    }

    public Vertex()
      : this(0.0, 0.0, 0, 0)
    {
    }

    public Vertex(double x, double y)
      : this(x, y, 0, 0)
    {
    }

    public Vertex(double x, double y, int mark)
      : this(x, y, mark, 0)
    {
    }

    public Vertex(double x, double y, int mark, int attribs)
      : base(x, y, mark)
    {
      type = VertexType.InputVertex;
      if (attribs <= 0)
        return;
      attributes = new double[attribs];
    }

    public override int GetHashCode() => hash;
  }
}
