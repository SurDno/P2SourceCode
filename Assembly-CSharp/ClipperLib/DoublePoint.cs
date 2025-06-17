namespace ClipperLib
{
  public struct DoublePoint(double x = 0.0, double y = 0.0) {
    public double X = x;
    public double Y = y;

    public DoublePoint(DoublePoint dp) : this(dp.X, dp.Y) { }

    public DoublePoint(IntPoint ip) : this(ip.X, ip.Y) { }
  }
}
