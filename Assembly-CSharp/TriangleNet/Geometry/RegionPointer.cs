namespace TriangleNet.Geometry;

public class RegionPointer {
	internal int id;
	internal Point point;

	public RegionPointer(double x, double y, int id) {
		point = new Point(x, y);
		this.id = id;
	}
}