using TriangleNet.Data;

namespace TriangleNet.Geometry;

public interface ISegment {
	int P0 { get; }

	int P1 { get; }

	int Boundary { get; }

	Vertex GetVertex(int index);

	ITriangle GetTriangle(int index);
}