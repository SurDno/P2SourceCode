using TriangleNet.Geometry;

namespace TriangleNet.Data;

public class Segment : ISegment {
	internal int boundary;
	internal int hash;
	internal Osub[] subsegs;
	internal Otri[] triangles;
	internal Vertex[] vertices;

	public Segment() {
		subsegs = new Osub[2];
		subsegs[0].seg = Mesh.dummysub;
		subsegs[1].seg = Mesh.dummysub;
		vertices = new Vertex[4];
		triangles = new Otri[2];
		triangles[0].triangle = Mesh.dummytri;
		triangles[1].triangle = Mesh.dummytri;
		boundary = 0;
	}

	public int P0 => vertices[0].id;

	public int P1 => vertices[1].id;

	public int Boundary => boundary;

	public Vertex GetVertex(int index) {
		return vertices[index];
	}

	public ITriangle GetTriangle(int index) {
		return triangles[index].triangle == Mesh.dummytri ? null : (ITriangle)triangles[index].triangle;
	}

	public override int GetHashCode() {
		return hash;
	}

	public override string ToString() {
		return string.Format("SID {0}", hash);
	}
}