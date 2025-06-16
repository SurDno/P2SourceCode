namespace ClipperLib
{
  public struct IntPoint
  {
    public long X;
    public long Y;

    public IntPoint(long X, long Y)
    {
      this.X = X;
      this.Y = Y;
    }

    public IntPoint(double x, double y)
    {
      this.X = (long) x;
      this.Y = (long) y;
    }

    public IntPoint(IntPoint pt)
    {
      this.X = pt.X;
      this.Y = pt.Y;
    }

    public static bool operator ==(IntPoint a, IntPoint b) => a.X == b.X && a.Y == b.Y;

    public static bool operator !=(IntPoint a, IntPoint b) => a.X != b.X || a.Y != b.Y;

    public override bool Equals(object obj)
    {
      return obj != null && obj is IntPoint intPoint && this.X == intPoint.X && this.Y == intPoint.Y;
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
