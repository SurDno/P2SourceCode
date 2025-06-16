// Decompiled with JetBrains decompiler
// Type: TriangleNet.Geometry.ITriangle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using TriangleNet.Data;

#nullable disable
namespace TriangleNet.Geometry
{
  public interface ITriangle
  {
    int ID { get; }

    int P0 { get; }

    int P1 { get; }

    int P2 { get; }

    bool SupportsNeighbors { get; }

    int N0 { get; }

    int N1 { get; }

    int N2 { get; }

    double Area { get; set; }

    int Region { get; }

    Vertex GetVertex(int index);

    ITriangle GetNeighbor(int index);

    ISegment GetSegment(int index);
  }
}
