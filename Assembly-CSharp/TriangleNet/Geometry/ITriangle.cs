using TriangleNet.Data;

namespace TriangleNet.Geometry;

public interface ITriangle {
	int ID { get; }

	int P0 { get; }

	int P1 { get; }

	int P2 { get; }

	bool SupportsNeighbors { get; }

	int N0 { get; }

	int N1 { get; }

	int N2 { get; }

	double Area { get; set; }

	int Region { get; }

	Vertex GetVertex(int index);

	ITriangle GetNeighbor(int index);

	ISegment GetSegment(int index);
}