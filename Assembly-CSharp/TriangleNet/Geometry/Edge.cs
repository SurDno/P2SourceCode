// Decompiled with JetBrains decompiler
// Type: TriangleNet.Geometry.Edge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace TriangleNet.Geometry
{
  public class Edge
  {
    public int P0 { get; private set; }

    public int P1 { get; private set; }

    public int Boundary { get; private set; }

    public Edge(int p0, int p1)
      : this(p0, p1, 0)
    {
    }

    public Edge(int p0, int p1, int boundary)
    {
      this.P0 = p0;
      this.P1 = p1;
      this.Boundary = boundary;
    }
  }
}
