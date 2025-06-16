using TriangleNet.Geometry;

namespace TriangleNet.Data
{
  public class Segment : ISegment
  {
    internal int boundary;
    internal int hash;
    internal Osub[] subsegs;
    internal Otri[] triangles;
    internal Vertex[] vertices;

    public Segment()
    {
      this.subsegs = new Osub[2];
      this.subsegs[0].seg = Mesh.dummysub;
      this.subsegs[1].seg = Mesh.dummysub;
      this.vertices = new Vertex[4];
      this.triangles = new Otri[2];
      this.triangles[0].triangle = Mesh.dummytri;
      this.triangles[1].triangle = Mesh.dummytri;
      this.boundary = 0;
    }

    public int P0 => this.vertices[0].id;

    public int P1 => this.vertices[1].id;

    public int Boundary => this.boundary;

    public Vertex GetVertex(int index) => this.vertices[index];

    public ITriangle GetTriangle(int index)
    {
      return this.triangles[index].triangle == Mesh.dummytri ? (ITriangle) null : (ITriangle) this.triangles[index].triangle;
    }

    public override int GetHashCode() => this.hash;

    public override string ToString() => string.Format("SID {0}", (object) this.hash);
  }
}
