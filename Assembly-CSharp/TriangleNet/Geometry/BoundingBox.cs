// Decompiled with JetBrains decompiler
// Type: TriangleNet.Geometry.BoundingBox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace TriangleNet.Geometry
{
  public class BoundingBox
  {
    private double xmin;
    private double ymin;
    private double xmax;
    private double ymax;

    public double Xmin => this.xmin;

    public double Ymin => this.ymin;

    public double Xmax => this.xmax;

    public double Ymax => this.ymax;

    public double Width => this.xmax - this.xmin;

    public double Height => this.ymax - this.ymin;

    public BoundingBox()
    {
      this.xmin = double.MaxValue;
      this.ymin = double.MaxValue;
      this.xmax = double.MinValue;
      this.ymax = double.MinValue;
    }

    public BoundingBox(double xmin, double ymin, double xmax, double ymax)
    {
      this.xmin = xmin;
      this.ymin = ymin;
      this.xmax = xmax;
      this.ymax = ymax;
    }

    public void Update(double x, double y)
    {
      this.xmin = Math.Min(this.xmin, x);
      this.ymin = Math.Min(this.ymin, y);
      this.xmax = Math.Max(this.xmax, x);
      this.ymax = Math.Max(this.ymax, y);
    }

    public void Scale(double dx, double dy)
    {
      this.xmin -= dx;
      this.xmax += dx;
      this.ymin -= dy;
      this.ymax += dy;
    }

    public bool Contains(Point pt)
    {
      return pt.x >= this.xmin && pt.x <= this.xmax && pt.y >= this.ymin && pt.y <= this.ymax;
    }
  }
}
