using System;
using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;

namespace TriangleNet.Tools;

public class BoundedVoronoi : IVoronoi {
	private bool includeBoundary = true;
	private Mesh mesh;
	private Point[] points;
	private List<VoronoiRegion> regions;
	private int segIndex;
	private Dictionary<int, Segment> subsegMap;

	public BoundedVoronoi(Mesh mesh)
		: this(mesh, true) { }

	public BoundedVoronoi(Mesh mesh, bool includeBoundary) {
		this.mesh = mesh;
		this.includeBoundary = includeBoundary;
		Generate();
	}

	public Point[] Points => points;

	public List<VoronoiRegion> Regions => regions;

	private void Generate() {
		mesh.Renumber();
		mesh.MakeVertexMap();
		points = new Point[mesh.triangles.Count + mesh.subsegs.Count * 5];
		regions = new List<VoronoiRegion>(mesh.vertices.Count);
		ComputeCircumCenters();
		TagBlindTriangles();
		foreach (var vertex in mesh.vertices.Values)
			if (vertex.type == VertexType.FreeVertex || vertex.Boundary == 0)
				ConstructBvdCell(vertex);
			else if (includeBoundary)
				ConstructBoundaryBvdCell(vertex);
	}

	private void ComputeCircumCenters() {
		var otri = new Otri();
		var xi = 0.0;
		var eta = 0.0;
		foreach (var triangle in mesh.triangles.Values) {
			otri.triangle = triangle;
			var circumcenter = Primitives.FindCircumcenter(otri.Org(), otri.Dest(), otri.Apex(), ref xi, ref eta);
			circumcenter.id = triangle.id;
			points[triangle.id] = circumcenter;
		}
	}

	private void TagBlindTriangles() {
		var num = 0;
		subsegMap = new Dictionary<int, Segment>();
		var otri = new Otri();
		var o2 = new Otri();
		var seg = new Osub();
		var os = new Osub();
		foreach (var triangle in mesh.triangles.Values)
			triangle.infected = false;
		foreach (var segment in mesh.subsegs.Values) {
			var triangleStack = new Stack<Triangle>();
			seg.seg = segment;
			seg.orient = 0;
			seg.TriPivot(ref otri);
			if (otri.triangle != Mesh.dummytri && !otri.triangle.infected)
				triangleStack.Push(otri.triangle);
			seg.SymSelf();
			seg.TriPivot(ref otri);
			if (otri.triangle != Mesh.dummytri && !otri.triangle.infected)
				triangleStack.Push(otri.triangle);
			while (triangleStack.Count > 0) {
				otri.triangle = triangleStack.Pop();
				otri.orient = 0;
				if (TriangleIsBlinded(ref otri, ref seg)) {
					otri.triangle.infected = true;
					++num;
					subsegMap.Add(otri.triangle.hash, seg.seg);
					for (otri.orient = 0; otri.orient < 3; ++otri.orient) {
						otri.Sym(ref o2);
						o2.SegPivot(ref os);
						if (o2.triangle != Mesh.dummytri && !o2.triangle.infected && os.seg == Mesh.dummysub)
							triangleStack.Push(o2.triangle);
					}
				}
			}
		}
	}

	private bool TriangleIsBlinded(ref Otri tri, ref Osub seg) {
		var p4_1 = tri.Org();
		var p4_2 = tri.Dest();
		var p4_3 = tri.Apex();
		var p1 = seg.Org();
		var p2 = seg.Dest();
		var point = points[tri.triangle.id];
		Point p;
		return SegmentsIntersect(p1, p2, point, p4_1, out p, true) ||
		       SegmentsIntersect(p1, p2, point, p4_2, out p, true) ||
		       SegmentsIntersect(p1, p2, point, p4_3, out p, true);
	}

	private void ConstructBvdCell(Vertex vertex) {
		var voronoiRegion = new VoronoiRegion(vertex);
		regions.Add(voronoiRegion);
		var o2_1 = new Otri();
		var o2_2 = new Otri();
		var o2_3 = new Otri();
		var osub = new Osub();
		var o2_4 = new Osub();
		var count = mesh.triangles.Count;
		var points = new List<Point>();
		vertex.tri.Copy(ref o2_2);
		if (o2_2.Org() != vertex)
			throw new Exception("ConstructBvdCell: inconsistent topology.");
		o2_2.Copy(ref o2_1);
		o2_2.Onext(ref o2_3);
		do {
			var point1 = this.points[o2_1.triangle.id];
			var point2 = this.points[o2_3.triangle.id];
			Point p;
			if (!o2_1.triangle.infected) {
				points.Add(point1);
				if (o2_3.triangle.infected) {
					o2_4.seg = subsegMap[o2_3.triangle.hash];
					if (SegmentsIntersect(o2_4.SegOrg(), o2_4.SegDest(), point1, point2, out p, true)) {
						p.id = count + segIndex;
						this.points[count + segIndex] = p;
						++segIndex;
						points.Add(p);
					}
				}
			} else {
				osub.seg = subsegMap[o2_1.triangle.hash];
				if (!o2_3.triangle.infected) {
					if (SegmentsIntersect(osub.SegOrg(), osub.SegDest(), point1, point2, out p, true)) {
						p.id = count + segIndex;
						this.points[count + segIndex] = p;
						++segIndex;
						points.Add(p);
					}
				} else {
					o2_4.seg = subsegMap[o2_3.triangle.hash];
					if (!osub.Equal(o2_4)) {
						if (SegmentsIntersect(osub.SegOrg(), osub.SegDest(), point1, point2, out p, true)) {
							p.id = count + segIndex;
							this.points[count + segIndex] = p;
							++segIndex;
							points.Add(p);
						}

						if (SegmentsIntersect(o2_4.SegOrg(), o2_4.SegDest(), point1, point2, out p, true)) {
							p.id = count + segIndex;
							this.points[count + segIndex] = p;
							++segIndex;
							points.Add(p);
						}
					}
				}
			}

			o2_3.Copy(ref o2_1);
			o2_3.OnextSelf();
		} while (!o2_1.Equal(o2_2));

		voronoiRegion.Add(points);
	}

	private void ConstructBoundaryBvdCell(Vertex vertex) {
		var voronoiRegion = new VoronoiRegion(vertex);
		regions.Add(voronoiRegion);
		var o2_1 = new Otri();
		var o2_2 = new Otri();
		var o2_3 = new Otri();
		var o2_4 = new Otri();
		var osub = new Osub();
		var o2_5 = new Osub();
		var count = mesh.triangles.Count;
		var points = new List<Point>();
		vertex.tri.Copy(ref o2_2);
		if (o2_2.Org() != vertex)
			throw new Exception("ConstructBoundaryBvdCell: inconsistent topology.");
		o2_2.Copy(ref o2_1);
		o2_2.Onext(ref o2_3);
		o2_2.Oprev(ref o2_4);
		if (o2_4.triangle != Mesh.dummytri) {
			while (o2_4.triangle != Mesh.dummytri && !o2_4.Equal(o2_2)) {
				o2_4.Copy(ref o2_1);
				o2_4.OprevSelf();
			}

			o2_1.Copy(ref o2_2);
			o2_1.Onext(ref o2_3);
		}

		if (o2_4.triangle == Mesh.dummytri) {
			var point = new Point(vertex.x, vertex.y);
			point.id = count + segIndex;
			this.points[count + segIndex] = point;
			++segIndex;
			points.Add(point);
		}

		var vertex1 = o2_1.Org();
		var vertex2 = o2_1.Dest();
		var p = new Point((vertex1.X + vertex2.X) / 2.0, (vertex1.Y + vertex2.Y) / 2.0);
		p.id = count + segIndex;
		this.points[count + segIndex] = p;
		++segIndex;
		points.Add(p);
		do {
			var point1 = this.points[o2_1.triangle.id];
			if (o2_3.triangle == Mesh.dummytri) {
				if (!o2_1.triangle.infected)
					points.Add(point1);
				var vertex3 = o2_1.Org();
				var vertex4 = o2_1.Apex();
				p = new Point((vertex3.X + vertex4.X) / 2.0, (vertex3.Y + vertex4.Y) / 2.0);
				p.id = count + segIndex;
				this.points[count + segIndex] = p;
				++segIndex;
				points.Add(p);
				break;
			}

			var point2 = this.points[o2_3.triangle.id];
			if (!o2_1.triangle.infected) {
				points.Add(point1);
				if (o2_3.triangle.infected) {
					o2_5.seg = subsegMap[o2_3.triangle.hash];
					if (SegmentsIntersect(o2_5.SegOrg(), o2_5.SegDest(), point1, point2, out p, true)) {
						p.id = count + segIndex;
						this.points[count + segIndex] = p;
						++segIndex;
						points.Add(p);
					}
				}
			} else {
				osub.seg = subsegMap[o2_1.triangle.hash];
				var p1 = osub.SegOrg();
				var p2 = osub.SegDest();
				if (!o2_3.triangle.infected) {
					vertex2 = o2_1.Dest();
					var vertex5 = o2_1.Apex();
					var p3 = new Point((vertex2.X + vertex5.X) / 2.0, (vertex2.Y + vertex5.Y) / 2.0);
					if (SegmentsIntersect(p1, p2, p3, point1, out p, false)) {
						p.id = count + segIndex;
						this.points[count + segIndex] = p;
						++segIndex;
						points.Add(p);
					}

					if (SegmentsIntersect(p1, p2, point1, point2, out p, true)) {
						p.id = count + segIndex;
						this.points[count + segIndex] = p;
						++segIndex;
						points.Add(p);
					}
				} else {
					o2_5.seg = subsegMap[o2_3.triangle.hash];
					if (!osub.Equal(o2_5)) {
						if (SegmentsIntersect(p1, p2, point1, point2, out p, true)) {
							p.id = count + segIndex;
							this.points[count + segIndex] = p;
							++segIndex;
							points.Add(p);
						}

						if (SegmentsIntersect(o2_5.SegOrg(), o2_5.SegDest(), point1, point2, out p, true)) {
							p.id = count + segIndex;
							this.points[count + segIndex] = p;
							++segIndex;
							points.Add(p);
						}
					} else {
						var p3 = new Point((vertex1.X + vertex2.X) / 2.0, (vertex1.Y + vertex2.Y) / 2.0);
						if (SegmentsIntersect(p1, p2, p3, point2, out p, false)) {
							p.id = count + segIndex;
							this.points[count + segIndex] = p;
							++segIndex;
							points.Add(p);
						}
					}
				}
			}

			o2_3.Copy(ref o2_1);
			o2_3.OnextSelf();
		} while (!o2_1.Equal(o2_2));

		voronoiRegion.Add(points);
	}

	private bool SegmentsIntersect(
		Point p1,
		Point p2,
		Point p3,
		Point p4,
		out Point p,
		bool strictIntersect) {
		p = null;
		var x1 = p1.X;
		var y1 = p1.Y;
		var x2 = p2.X;
		var y2 = p2.Y;
		var x3 = p3.X;
		var y3 = p3.Y;
		var x4 = p4.X;
		var y4 = p4.Y;
		if ((x1 == x2 && y1 == y2) || (x3 == x4 && y3 == y4) || (x1 == x3 && y1 == y3) || (x2 == x3 && y2 == y3) ||
		    (x1 == x4 && y1 == y4) || (x2 == x4 && y2 == y4))
			return false;
		var num1 = x2 - x1;
		var num2 = y2 - y1;
		var num3 = x3 - x1;
		var num4 = y3 - y1;
		var num5 = x4 - x1;
		var num6 = y4 - y1;
		var num7 = Math.Sqrt(num1 * num1 + num2 * num2);
		var num8 = num1 / num7;
		var num9 = num2 / num7;
		var num10 = num3 * num8 + num4 * num9;
		var num11 = num4 * num8 - num3 * num9;
		var num12 = num10;
		var num13 = num5 * num8 + num6 * num9;
		var num14 = num6 * num8 - num5 * num9;
		var num15 = num13;
		if ((num11 < 0.0 && num14 < 0.0) || ((num11 < 0.0 ? 0 : num14 >= 0.0 ? 1 : 0) & (strictIntersect ? 1 : 0)) != 0)
			return false;
		var num16 = num15 + (num12 - num15) * num14 / (num14 - num11);
		if (num16 < 0.0 || (num16 > num7) & strictIntersect)
			return false;
		p = new Point(x1 + num16 * num8, y1 + num16 * num9);
		return true;
	}
}