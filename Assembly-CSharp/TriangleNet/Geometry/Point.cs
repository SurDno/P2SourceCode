using System;

namespace TriangleNet.Geometry
{
  public class Point : IComparable<Point>, IEquatable<Point>
  {
    internal double[] attributes;
    internal int id;
    internal int mark;
    internal double x;
    internal double y;

    public int ID => this.id;

    public double X => this.x;

    public double Y => this.y;

    public int Boundary => this.mark;

    public double[] Attributes => this.attributes;

    public Point()
      : this(0.0, 0.0, 0)
    {
    }

    public Point(double x, double y)
      : this(x, y, 0)
    {
    }

    public Point(double x, double y, int mark)
    {
      this.x = x;
      this.y = y;
      this.mark = mark;
    }

    public int CompareTo(Point other)
    {
      return this.x == other.x && this.y == other.y ? 0 : (this.x < other.x || this.x == other.x && this.y < other.y ? -1 : 1);
    }

    public bool Equals(Point p) => (object) p != null && this.x == p.x && this.y == p.y;

    public static bool operator ==(Point a, Point b)
    {
      if ((object) a == (object) b)
        return true;
      return (object) a != null && (object) b != null && a.Equals(b);
    }

    public static bool operator !=(Point a, Point b) => !(a == b);

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      Point point = obj as Point;
      return (object) point != null && this.x == point.x && this.y == point.y;
    }

    public override int GetHashCode() => this.x.GetHashCode() ^ this.y.GetHashCode();

    public override string ToString()
    {
      return string.Format("[{0},{1}]", (object) this.x, (object) this.y);
    }
  }
}
