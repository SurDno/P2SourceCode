namespace ClipperLib
{
  public struct IntPoint(long x, long y) {
    public long X = x;
    public long Y = y;

    public IntPoint(double x, double y) : this((long) x, (long) y) { }

    public IntPoint(IntPoint pt) : this(pt.X, pt.Y) { }

    public static bool operator ==(IntPoint a, IntPoint b) => a.X == b.X && a.Y == b.Y;

    public static bool operator !=(IntPoint a, IntPoint b) => a.X != b.X || a.Y != b.Y;

    public override bool Equals(object obj)
    {
      return obj != null && obj is IntPoint intPoint && X == intPoint.X && Y == intPoint.Y;
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
