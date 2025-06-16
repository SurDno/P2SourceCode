// Decompiled with JetBrains decompiler
// Type: TriangleNet.Geometry.ISegment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using TriangleNet.Data;

#nullable disable
namespace TriangleNet.Geometry
{
  public interface ISegment
  {
    int P0 { get; }

    int P1 { get; }

    int Boundary { get; }

    Vertex GetVertex(int index);

    ITriangle GetTriangle(int index);
  }
}
