using TriangleNet.Geometry;

namespace TriangleNet.Data
{
  public class Triangle : ITriangle
  {
    internal double area;
    internal int hash;
    internal int id;
    internal bool infected;
    internal Otri[] neighbors;
    internal int region;
    internal Osub[] subsegs;
    internal Vertex[] vertices;

    public Triangle()
    {
      this.neighbors = new Otri[3];
      this.neighbors[0].triangle = Mesh.dummytri;
      this.neighbors[1].triangle = Mesh.dummytri;
      this.neighbors[2].triangle = Mesh.dummytri;
      this.vertices = new Vertex[3];
      this.subsegs = new Osub[3];
      this.subsegs[0].seg = Mesh.dummysub;
      this.subsegs[1].seg = Mesh.dummysub;
      this.subsegs[2].seg = Mesh.dummysub;
    }

    public int ID => this.id;

    public int P0 => (Point) this.vertices[0] == (Point) null ? -1 : this.vertices[0].id;

    public int P1 => (Point) this.vertices[1] == (Point) null ? -1 : this.vertices[1].id;

    public Vertex GetVertex(int index) => this.vertices[index];

    public int P2 => (Point) this.vertices[2] == (Point) null ? -1 : this.vertices[2].id;

    public bool SupportsNeighbors => true;

    public int N0 => this.neighbors[0].triangle.id;

    public int N1 => this.neighbors[1].triangle.id;

    public int N2 => this.neighbors[2].triangle.id;

    public ITriangle GetNeighbor(int index)
    {
      return this.neighbors[index].triangle == Mesh.dummytri ? (ITriangle) null : (ITriangle) this.neighbors[index].triangle;
    }

    public ISegment GetSegment(int index)
    {
      return this.subsegs[index].seg == Mesh.dummysub ? (ISegment) null : (ISegment) this.subsegs[index].seg;
    }

    public double Area
    {
      get => this.area;
      set => this.area = value;
    }

    public int Region => this.region;

    public override int GetHashCode() => this.hash;

    public override string ToString() => string.Format("TID {0}", (object) this.hash);
  }
}
