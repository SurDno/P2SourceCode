using TriangleNet.Geometry;

namespace TriangleNet.Data;

public class Triangle : ITriangle {
	internal double area;
	internal int hash;
	internal int id;
	internal bool infected;
	internal Otri[] neighbors;
	internal int region;
	internal Osub[] subsegs;
	internal Vertex[] vertices;

	public Triangle() {
		neighbors = new Otri[3];
		neighbors[0].triangle = Mesh.dummytri;
		neighbors[1].triangle = Mesh.dummytri;
		neighbors[2].triangle = Mesh.dummytri;
		vertices = new Vertex[3];
		subsegs = new Osub[3];
		subsegs[0].seg = Mesh.dummysub;
		subsegs[1].seg = Mesh.dummysub;
		subsegs[2].seg = Mesh.dummysub;
	}

	public int ID => id;

	public int P0 => vertices[0] == null ? -1 : vertices[0].id;

	public int P1 => vertices[1] == null ? -1 : vertices[1].id;

	public Vertex GetVertex(int index) {
		return vertices[index];
	}

	public int P2 => vertices[2] == null ? -1 : vertices[2].id;

	public bool SupportsNeighbors => true;

	public int N0 => neighbors[0].triangle.id;

	public int N1 => neighbors[1].triangle.id;

	public int N2 => neighbors[2].triangle.id;

	public ITriangle GetNeighbor(int index) {
		return neighbors[index].triangle == Mesh.dummytri ? null : (ITriangle)neighbors[index].triangle;
	}

	public ISegment GetSegment(int index) {
		return subsegs[index].seg == Mesh.dummysub ? null : (ISegment)subsegs[index].seg;
	}

	public double Area {
		get => area;
		set => area = value;
	}

	public int Region => region;

	public override int GetHashCode() {
		return hash;
	}

	public override string ToString() {
		return string.Format("TID {0}", hash);
	}
}