// Decompiled with JetBrains decompiler
// Type: TriangleNet.Smoothing.SimpleSmoother
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.Tools;

#nullable disable
namespace TriangleNet.Smoothing
{
  public class SimpleSmoother : ISmoother
  {
    private Mesh mesh;

    public SimpleSmoother(Mesh mesh) => this.mesh = mesh;

    public void Smooth()
    {
      this.mesh.behavior.Quality = false;
      for (int index = 0; index < 5; ++index)
      {
        this.Step();
        this.mesh.Triangulate(this.Rebuild());
      }
    }

    private void Step()
    {
      foreach (VoronoiRegion region in new BoundedVoronoi(this.mesh, false).Regions)
      {
        int num1 = 0;
        double num2;
        double num3 = num2 = 0.0;
        foreach (Point vertex in (IEnumerable<Point>) region.Vertices)
        {
          ++num1;
          num3 += vertex.x;
          num2 += vertex.y;
        }
        region.Generator.x = num3 / (double) num1;
        region.Generator.y = num2 / (double) num1;
      }
    }

    private InputGeometry Rebuild()
    {
      InputGeometry inputGeometry = new InputGeometry(this.mesh.vertices.Count);
      foreach (Vertex vertex in this.mesh.vertices.Values)
        inputGeometry.AddPoint(vertex.x, vertex.y, vertex.mark);
      foreach (Segment segment in this.mesh.subsegs.Values)
        inputGeometry.AddSegment(segment.P0, segment.P1, segment.Boundary);
      foreach (Point hole in this.mesh.holes)
        inputGeometry.AddHole(hole.x, hole.y);
      foreach (RegionPointer region in this.mesh.regions)
        inputGeometry.AddRegion(region.point.x, region.point.y, region.id);
      return inputGeometry;
    }
  }
}
