using System;

namespace TriangleNet.Geometry
{
  public class Point(double x, double y, int mark) : IComparable<Point>, IEquatable<Point> {
    internal double[] attributes;
    internal int id;
    internal int mark = mark;
    internal double x = x;
    internal double y = y;

    public int ID => id;

    public double X => x;

    public double Y => y;

    public int Boundary => mark;

    public double[] Attributes => attributes;

    public Point()
      : this(0.0, 0.0, 0)
    {
    }

    public Point(double x, double y)
      : this(x, y, 0)
    {
    }

    public int CompareTo(Point other)
    {
      return x == other.x && y == other.y ? 0 : (x < other.x || x == other.x && y < other.y ? -1 : 1);
    }

    public bool Equals(Point p) => (object) p != null && x == p.x && y == p.y;

    public static bool operator ==(Point a, Point b)
    {
      if (a == (object) b)
        return true;
      return (object) a != null && (object) b != null && a.Equals(b);
    }

    public static bool operator !=(Point a, Point b) => !(a == b);

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      Point point = obj as Point;
      return (object) point != null && x == point.x && y == point.y;
    }

    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();

    public override string ToString()
    {
      return string.Format("[{0},{1}]", x, y);
    }
  }
}
