using System;

namespace TriangleNet.Geometry;

public class BoundingBox {
	private double xmin;
	private double ymin;
	private double xmax;
	private double ymax;

	public double Xmin => xmin;

	public double Ymin => ymin;

	public double Xmax => xmax;

	public double Ymax => ymax;

	public double Width => xmax - xmin;

	public double Height => ymax - ymin;

	public BoundingBox() {
		xmin = double.MaxValue;
		ymin = double.MaxValue;
		xmax = double.MinValue;
		ymax = double.MinValue;
	}

	public BoundingBox(double xmin, double ymin, double xmax, double ymax) {
		this.xmin = xmin;
		this.ymin = ymin;
		this.xmax = xmax;
		this.ymax = ymax;
	}

	public void Update(double x, double y) {
		xmin = Math.Min(xmin, x);
		ymin = Math.Min(ymin, y);
		xmax = Math.Max(xmax, x);
		ymax = Math.Max(ymax, y);
	}

	public void Scale(double dx, double dy) {
		xmin -= dx;
		xmax += dx;
		ymin -= dy;
		ymax += dy;
	}

	public bool Contains(Point pt) {
		return pt.x >= xmin && pt.x <= xmax && pt.y >= ymin && pt.y <= ymax;
	}
}