namespace TriangleNet.Geometry
{
  public class RegionPointer(double x, double y, int id) {
    internal int id = id;
    internal Point point = new(x, y);
  }
}
