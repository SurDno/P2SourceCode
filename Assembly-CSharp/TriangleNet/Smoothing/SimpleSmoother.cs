using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.Tools;

namespace TriangleNet.Smoothing
{
  public class SimpleSmoother(Mesh mesh) : ISmoother 
  {
    public void Smooth()
    {
      mesh.behavior.Quality = false;
      for (int index = 0; index < 5; ++index)
      {
        Step();
        mesh.Triangulate(Rebuild());
      }
    }

    private void Step()
    {
      foreach (VoronoiRegion region in new BoundedVoronoi(mesh, false).Regions)
      {
        int num1 = 0;
        double num2;
        double num3 = num2 = 0.0;
        foreach (Point vertex in region.Vertices)
        {
          ++num1;
          num3 += vertex.x;
          num2 += vertex.y;
        }
        region.Generator.x = num3 / num1;
        region.Generator.y = num2 / num1;
      }
    }

    private InputGeometry Rebuild()
    {
      InputGeometry inputGeometry = new InputGeometry(mesh.vertices.Count);
      foreach (Vertex vertex in mesh.vertices.Values)
        inputGeometry.AddPoint(vertex.x, vertex.y, vertex.mark);
      foreach (Segment segment in mesh.subsegs.Values)
        inputGeometry.AddSegment(segment.P0, segment.P1, segment.Boundary);
      foreach (Point hole in mesh.holes)
        inputGeometry.AddHole(hole.x, hole.y);
      foreach (RegionPointer region in mesh.regions)
        inputGeometry.AddRegion(region.point.x, region.point.y, region.id);
      return inputGeometry;
    }
  }
}
