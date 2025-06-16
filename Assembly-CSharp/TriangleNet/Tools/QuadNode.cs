using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Tools;

internal class QuadNode {
	private const int SW = 0;
	private const int SE = 1;
	private const int NW = 2;
	private const int NE = 3;
	private const double EPS = 1E-06;

	private static readonly byte[] BITVECTOR = new byte[4] {
		1,
		2,
		4,
		8
	};

	private byte bitRegions;
	private BoundingBox bounds;
	private Point pivot;
	private QuadNode[] regions;
	private QuadTree tree;
	private List<int> triangles;

	public QuadNode(BoundingBox box, QuadTree tree)
		: this(box, tree, false) { }

	public QuadNode(BoundingBox box, QuadTree tree, bool init) {
		this.tree = tree;
		bounds = new BoundingBox(box.Xmin, box.Ymin, box.Xmax, box.Ymax);
		pivot = new Point((box.Xmin + box.Xmax) / 2.0, (box.Ymin + box.Ymax) / 2.0);
		bitRegions = 0;
		regions = new QuadNode[4];
		triangles = new List<int>();
		if (!init)
			return;
		triangles.Capacity = tree.triangles.Length;
		foreach (var triangle in tree.triangles)
			triangles.Add(triangle.ID);
	}

	public List<int> FindTriangles(Point searchPoint) {
		var region = FindRegion(searchPoint);
		return regions[region] == null ? triangles : regions[region].FindTriangles(searchPoint);
	}

	public void CreateSubRegion(int currentDepth) {
		regions[0] = new QuadNode(new BoundingBox(bounds.Xmin, bounds.Ymin, pivot.X, pivot.Y), tree);
		regions[1] = new QuadNode(new BoundingBox(pivot.X, bounds.Ymin, bounds.Xmax, pivot.Y), tree);
		regions[2] = new QuadNode(new BoundingBox(bounds.Xmin, pivot.Y, pivot.X, bounds.Ymax), tree);
		regions[3] = new QuadNode(new BoundingBox(pivot.X, pivot.Y, bounds.Xmax, bounds.Ymax), tree);
		var triangle1 = new Point[3];
		foreach (var triangle2 in triangles) {
			var triangle3 = tree.triangles[triangle2];
			triangle1[0] = triangle3.GetVertex(0);
			triangle1[1] = triangle3.GetVertex(1);
			triangle1[2] = triangle3.GetVertex(2);
			AddTriangleToRegion(triangle1, triangle3.ID);
		}

		for (var index = 0; index < 4; ++index)
			if (regions[index].triangles.Count > tree.sizeBound && currentDepth < tree.maxDepth)
				regions[index].CreateSubRegion(currentDepth + 1);
	}

	private void AddTriangleToRegion(Point[] triangle, int index) {
		bitRegions = 0;
		if (QuadTree.IsPointInTriangle(pivot, triangle[0], triangle[1], triangle[2])) {
			AddToRegion(index, 0);
			AddToRegion(index, 1);
			AddToRegion(index, 2);
			AddToRegion(index, 3);
		} else {
			FindTriangleIntersections(triangle, index);
			if (bitRegions != 0)
				return;
			regions[FindRegion(triangle[0])].triangles.Add(index);
		}
	}

	private void FindTriangleIntersections(Point[] triangle, int index) {
		var k = 2;
		for (var index1 = 0; index1 < 3; k = index1++) {
			var dx = triangle[index1].X - triangle[k].X;
			var dy = triangle[index1].Y - triangle[k].Y;
			if (dx != 0.0)
				FindIntersectionsWithX(dx, dy, triangle, index, k);
			if (dy != 0.0)
				FindIntersectionsWithY(dx, dy, triangle, index, k);
		}
	}

	private void FindIntersectionsWithX(double dx, double dy, Point[] triangle, int index, int k) {
		var num1 = (pivot.X - triangle[k].X) / dx;
		if (num1 < 1.000001 && num1 > -1E-06) {
			var num2 = triangle[k].Y + num1 * dy;
			if (num2 < pivot.Y) {
				if (num2 >= bounds.Ymin) {
					AddToRegion(index, 0);
					AddToRegion(index, 1);
				}
			} else if (num2 <= bounds.Ymax) {
				AddToRegion(index, 2);
				AddToRegion(index, 3);
			}
		}

		var num3 = (bounds.Xmin - triangle[k].X) / dx;
		if (num3 < 1.000001 && num3 > -1E-06) {
			var num4 = triangle[k].Y + num3 * dy;
			if (num4 <= pivot.Y && num4 >= bounds.Ymin)
				AddToRegion(index, 0);
			else if (num4 >= pivot.Y && num4 <= bounds.Ymax)
				AddToRegion(index, 2);
		}

		var num5 = (bounds.Xmax - triangle[k].X) / dx;
		if (num5 >= 1.000001 || num5 <= -1E-06)
			return;
		var num6 = triangle[k].Y + num5 * dy;
		if (num6 <= pivot.Y && num6 >= bounds.Ymin)
			AddToRegion(index, 1);
		else if (num6 >= pivot.Y && num6 <= bounds.Ymax)
			AddToRegion(index, 3);
	}

	private void FindIntersectionsWithY(double dx, double dy, Point[] triangle, int index, int k) {
		var num1 = (pivot.Y - triangle[k].Y) / dy;
		if (num1 < 1.000001 && num1 > -1E-06) {
			var num2 = triangle[k].X + num1 * dy;
			if (num2 > pivot.X) {
				if (num2 <= bounds.Xmax) {
					AddToRegion(index, 1);
					AddToRegion(index, 3);
				}
			} else if (num2 >= bounds.Xmin) {
				AddToRegion(index, 0);
				AddToRegion(index, 2);
			}
		}

		var num3 = (bounds.Ymin - triangle[k].Y) / dy;
		if (num3 < 1.000001 && num3 > -1E-06) {
			var num4 = triangle[k].X + num3 * dx;
			if (num4 <= pivot.X && num4 >= bounds.Xmin)
				AddToRegion(index, 0);
			else if (num4 >= pivot.X && num4 <= bounds.Xmax)
				AddToRegion(index, 1);
		}

		var num5 = (bounds.Ymax - triangle[k].Y) / dy;
		if (num5 >= 1.000001 || num5 <= -1E-06)
			return;
		var num6 = triangle[k].X + num5 * dx;
		if (num6 <= pivot.X && num6 >= bounds.Xmin)
			AddToRegion(index, 2);
		else if (num6 >= pivot.X && num6 <= bounds.Xmax)
			AddToRegion(index, 3);
	}

	private int FindRegion(Point point) {
		var region = 2;
		if (point.Y < pivot.Y)
			region = 0;
		if (point.X > pivot.X)
			++region;
		return region;
	}

	private void AddToRegion(int index, int region) {
		if ((bitRegions & BITVECTOR[region]) != 0)
			return;
		regions[region].triangles.Add(index);
		bitRegions |= BITVECTOR[region];
	}
}