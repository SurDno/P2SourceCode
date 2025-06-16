using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.Tools;

namespace TriangleNet.Smoothing;

public class SimpleSmoother : ISmoother {
	private Mesh mesh;

	public SimpleSmoother(Mesh mesh) {
		this.mesh = mesh;
	}

	public void Smooth() {
		mesh.behavior.Quality = false;
		for (var index = 0; index < 5; ++index) {
			Step();
			mesh.Triangulate(Rebuild());
		}
	}

	private void Step() {
		foreach (var region in new BoundedVoronoi(mesh, false).Regions) {
			var num1 = 0;
			double num2;
			var num3 = num2 = 0.0;
			foreach (var vertex in region.Vertices) {
				++num1;
				num3 += vertex.x;
				num2 += vertex.y;
			}

			region.Generator.x = num3 / num1;
			region.Generator.y = num2 / num1;
		}
	}

	private InputGeometry Rebuild() {
		var inputGeometry = new InputGeometry(mesh.vertices.Count);
		foreach (var vertex in mesh.vertices.Values)
			inputGeometry.AddPoint(vertex.x, vertex.y, vertex.mark);
		foreach (var segment in mesh.subsegs.Values)
			inputGeometry.AddSegment(segment.P0, segment.P1, segment.Boundary);
		foreach (var hole in mesh.holes)
			inputGeometry.AddHole(hole.x, hole.y);
		foreach (var region in mesh.regions)
			inputGeometry.AddRegion(region.point.x, region.point.y, region.id);
		return inputGeometry;
	}
}