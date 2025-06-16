// Decompiled with JetBrains decompiler
// Type: TriangleNet.Geometry.RegionPointer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace TriangleNet.Geometry
{
  public class RegionPointer
  {
    internal int id;
    internal Point point;

    public RegionPointer(double x, double y, int id)
    {
      this.point = new Point(x, y);
      this.id = id;
    }
  }
}
