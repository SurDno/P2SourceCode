// Decompiled with JetBrains decompiler
// Type: TriangleNet.Data.Vertex
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using TriangleNet.Geometry;

#nullable disable
namespace TriangleNet.Data
{
  public class Vertex : Point
  {
    internal int hash;
    internal Otri tri;
    internal VertexType type;

    public VertexType Type => this.type;

    public double this[int i]
    {
      get
      {
        if (i == 0)
          return this.x;
        if (i == 1)
          return this.y;
        throw new ArgumentOutOfRangeException("Index must be 0 or 1.");
      }
    }

    public Vertex()
      : this(0.0, 0.0, 0, 0)
    {
    }

    public Vertex(double x, double y)
      : this(x, y, 0, 0)
    {
    }

    public Vertex(double x, double y, int mark)
      : this(x, y, mark, 0)
    {
    }

    public Vertex(double x, double y, int mark, int attribs)
      : base(x, y, mark)
    {
      this.type = VertexType.InputVertex;
      if (attribs <= 0)
        return;
      this.attributes = new double[attribs];
    }

    public override int GetHashCode() => this.hash;
  }
}
