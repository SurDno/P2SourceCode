using System;

namespace TriangleNet.Geometry
{
  public class BoundingBox(double xmin, double ymin, double xmax, double ymax) {
    private double xmin = xmin;
    private double ymin = ymin;
    private double xmax = xmax;
    private double ymax = ymax;

    public double Xmin => xmin;

    public double Ymin => ymin;

    public double Xmax => xmax;

    public double Ymax => ymax;

    public double Width => xmax - xmin;

    public double Height => ymax - ymin;

    public BoundingBox() : this(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue) { }

    public void Update(double x, double y)
    {
      xmin = Math.Min(xmin, x);
      ymin = Math.Min(ymin, y);
      xmax = Math.Max(xmax, x);
      ymax = Math.Max(ymax, y);
    }

    public void Scale(double dx, double dy)
    {
      xmin -= dx;
      xmax += dx;
      ymin -= dy;
      ymax += dy;
    }

    public bool Contains(Point pt)
    {
      return pt.x >= xmin && pt.x <= xmax && pt.y >= ymin && pt.y <= ymax;
    }
  }
}
