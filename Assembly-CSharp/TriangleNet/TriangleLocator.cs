using TriangleNet.Data;
using TriangleNet.Geometry;

namespace TriangleNet
{
  internal class TriangleLocator
  {
    private Mesh mesh;
    internal Otri recenttri;
    private Sampler sampler;

    public TriangleLocator(Mesh mesh)
    {
      this.mesh = mesh;
      this.sampler = new Sampler();
    }

    public void Update(ref Otri otri) => otri.Copy(ref this.recenttri);

    public void Reset() => this.recenttri.triangle = (Triangle) null;

    public LocateResult PreciseLocate(Point searchpoint, ref Otri searchtri, bool stopatsubsegment)
    {
      Otri o2 = new Otri();
      Osub os = new Osub();
      Vertex pa = searchtri.Org();
      Vertex pb = searchtri.Dest();
      Vertex vertex = searchtri.Apex();
      double num1;
      double num2;
      while (true)
      {
        if (vertex.x != searchpoint.X || vertex.y != searchpoint.Y)
        {
          num1 = Primitives.CounterClockwise((Point) pa, (Point) vertex, searchpoint);
          num2 = Primitives.CounterClockwise((Point) vertex, (Point) pb, searchpoint);
          bool flag;
          if (num1 > 0.0)
            flag = num2 <= 0.0 || (vertex.x - searchpoint.X) * (pb.x - pa.x) + (vertex.y - searchpoint.Y) * (pb.y - pa.y) > 0.0;
          else if (num2 > 0.0)
            flag = false;
          else
            goto label_6;
          if (flag)
          {
            searchtri.Lprev(ref o2);
            pb = vertex;
          }
          else
          {
            searchtri.Lnext(ref o2);
            pa = vertex;
          }
          o2.Sym(ref searchtri);
          if (this.mesh.checksegments & stopatsubsegment)
          {
            o2.SegPivot(ref os);
            if (os.seg != Mesh.dummysub)
              goto label_16;
          }
          if (searchtri.triangle != Mesh.dummytri)
            vertex = searchtri.Apex();
          else
            goto label_18;
        }
        else
          break;
      }
      searchtri.LprevSelf();
      return LocateResult.OnVertex;
label_6:
      if (num1 == 0.0)
      {
        searchtri.LprevSelf();
        return LocateResult.OnEdge;
      }
      if (num2 != 0.0)
        return LocateResult.InTriangle;
      searchtri.LnextSelf();
      return LocateResult.OnEdge;
label_16:
      o2.Copy(ref searchtri);
      return LocateResult.Outside;
label_18:
      o2.Copy(ref searchtri);
      return LocateResult.Outside;
    }

    public LocateResult Locate(Point searchpoint, ref Otri searchtri)
    {
      Otri otri = new Otri();
      Vertex vertex1 = searchtri.Org();
      double num1 = (searchpoint.X - vertex1.x) * (searchpoint.X - vertex1.x) + (searchpoint.Y - vertex1.y) * (searchpoint.Y - vertex1.y);
      if (this.recenttri.triangle != null && !Otri.IsDead(this.recenttri.triangle))
      {
        Vertex vertex2 = this.recenttri.Org();
        if (vertex2.x == searchpoint.X && vertex2.y == searchpoint.Y)
        {
          this.recenttri.Copy(ref searchtri);
          return LocateResult.OnVertex;
        }
        double num2 = (searchpoint.X - vertex2.x) * (searchpoint.X - vertex2.x) + (searchpoint.Y - vertex2.y) * (searchpoint.Y - vertex2.y);
        if (num2 < num1)
        {
          this.recenttri.Copy(ref searchtri);
          num1 = num2;
        }
      }
      this.sampler.Update(this.mesh);
      foreach (int sample in this.sampler.GetSamples(this.mesh))
      {
        otri.triangle = this.mesh.triangles[sample];
        if (!Otri.IsDead(otri.triangle))
        {
          Vertex vertex3 = otri.Org();
          double num3 = (searchpoint.X - vertex3.x) * (searchpoint.X - vertex3.x) + (searchpoint.Y - vertex3.y) * (searchpoint.Y - vertex3.y);
          if (num3 < num1)
          {
            otri.Copy(ref searchtri);
            num1 = num3;
          }
        }
      }
      Vertex pa = searchtri.Org();
      Vertex pb = searchtri.Dest();
      if (pa.x == searchpoint.X && pa.y == searchpoint.Y)
        return LocateResult.OnVertex;
      if (pb.x == searchpoint.X && pb.y == searchpoint.Y)
      {
        searchtri.LnextSelf();
        return LocateResult.OnVertex;
      }
      double num4 = Primitives.CounterClockwise((Point) pa, (Point) pb, searchpoint);
      if (num4 < 0.0)
        searchtri.SymSelf();
      else if (num4 == 0.0 && pa.x < searchpoint.X == searchpoint.X < pb.x && pa.y < searchpoint.Y == searchpoint.Y < pb.y)
        return LocateResult.OnEdge;
      return this.PreciseLocate(searchpoint, ref searchtri, false);
    }
  }
}
