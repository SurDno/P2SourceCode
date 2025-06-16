// Decompiled with JetBrains decompiler
// Type: ClipperLib.DoublePoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace ClipperLib
{
  public struct DoublePoint
  {
    public double X;
    public double Y;

    public DoublePoint(double x = 0.0, double y = 0.0)
    {
      this.X = x;
      this.Y = y;
    }

    public DoublePoint(DoublePoint dp)
    {
      this.X = dp.X;
      this.Y = dp.Y;
    }

    public DoublePoint(IntPoint ip)
    {
      this.X = (double) ip.X;
      this.Y = (double) ip.Y;
    }
  }
}
