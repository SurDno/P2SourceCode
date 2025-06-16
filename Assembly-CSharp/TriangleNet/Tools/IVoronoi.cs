// Decompiled with JetBrains decompiler
// Type: TriangleNet.Tools.IVoronoi
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using TriangleNet.Geometry;

#nullable disable
namespace TriangleNet.Tools
{
  public interface IVoronoi
  {
    Point[] Points { get; }

    List<VoronoiRegion> Regions { get; }
  }
}
