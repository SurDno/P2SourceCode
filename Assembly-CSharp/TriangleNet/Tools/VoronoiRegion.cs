using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;

namespace TriangleNet.Tools;

public class VoronoiRegion {
	private bool bounded;
	private Point generator;
	private int id;
	private List<Point> vertices;

	public int ID => id;

	public Point Generator => generator;

	public ICollection<Point> Vertices => vertices;

	public bool Bounded {
		get => bounded;
		set => bounded = value;
	}

	public VoronoiRegion(Vertex generator) {
		id = generator.id;
		this.generator = generator;
		vertices = new List<Point>();
		bounded = true;
	}

	public void Add(Point point) {
		vertices.Add(point);
	}

	public void Add(List<Point> points) {
		vertices.AddRange(points);
	}

	public override string ToString() {
		return string.Format("R-ID {0}", id);
	}
}