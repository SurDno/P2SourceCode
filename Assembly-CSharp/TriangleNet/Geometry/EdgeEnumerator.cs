using System;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Data;

namespace TriangleNet.Geometry;

public class EdgeEnumerator : IEnumerator<Edge>, IEnumerator, IDisposable {
	private Edge current;
	private Otri neighbor;
	private Vertex p1;
	private Vertex p2;
	private Osub sub;
	private Otri tri;
	private IEnumerator<Triangle> triangles;

	public EdgeEnumerator(Mesh mesh) {
		triangles = mesh.triangles.Values.GetEnumerator();
		triangles.MoveNext();
		tri.triangle = triangles.Current;
		tri.orient = 0;
	}

	public Edge Current => current;

	public void Dispose() {
		triangles.Dispose();
	}

	object IEnumerator.Current => current;

	public bool MoveNext() {
		if (tri.triangle == null)
			return false;
		current = null;
		while (current == null) {
			if (tri.orient == 3) {
				if (!triangles.MoveNext())
					return false;
				tri.triangle = triangles.Current;
				tri.orient = 0;
			}

			tri.Sym(ref neighbor);
			if (tri.triangle.id < neighbor.triangle.id || neighbor.triangle == Mesh.dummytri) {
				p1 = tri.Org();
				p2 = tri.Dest();
				tri.SegPivot(ref sub);
				current = new Edge(p1.id, p2.id, sub.seg.boundary);
			}

			++tri.orient;
		}

		return true;
	}

	public void Reset() {
		triangles.Reset();
	}
}