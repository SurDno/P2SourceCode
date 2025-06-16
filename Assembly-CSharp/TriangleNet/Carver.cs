using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.Tools;

namespace TriangleNet
{
  internal class Carver
  {
    private Mesh mesh;
    private List<Triangle> viri;

    public Carver(Mesh mesh)
    {
      this.mesh = mesh;
      viri = new List<Triangle>();
    }

    private void InfectHull()
    {
      Otri o2_1 = new Otri();
      Otri o2_2 = new Otri();
      Otri o2_3 = new Otri();
      Osub os = new Osub();
      o2_1.triangle = Mesh.dummytri;
      o2_1.orient = 0;
      o2_1.SymSelf();
      o2_1.Copy(ref o2_3);
      do
      {
        if (!o2_1.IsInfected())
        {
          o2_1.SegPivot(ref os);
          if (os.seg == Mesh.dummysub)
          {
            if (!o2_1.IsInfected())
            {
              o2_1.Infect();
              viri.Add(o2_1.triangle);
            }
          }
          else if (os.seg.boundary == 0)
          {
            os.seg.boundary = 1;
            Vertex vertex1 = o2_1.Org();
            Vertex vertex2 = o2_1.Dest();
            if (vertex1.mark == 0)
              vertex1.mark = 1;
            if (vertex2.mark == 0)
              vertex2.mark = 1;
          }
        }
        o2_1.LnextSelf();
        o2_1.Oprev(ref o2_2);
        while (o2_2.triangle != Mesh.dummytri)
        {
          o2_2.Copy(ref o2_1);
          o2_1.Oprev(ref o2_2);
        }
      }
      while (!o2_1.Equal(o2_3));
    }

    private void Plague()
    {
      Otri o2_1 = new Otri();
      Otri o2_2 = new Otri();
      Osub os = new Osub();
      for (int index = 0; index < viri.Count; ++index)
      {
        o2_1.triangle = viri[index];
        o2_1.Uninfect();
        for (o2_1.orient = 0; o2_1.orient < 3; ++o2_1.orient)
        {
          o2_1.Sym(ref o2_2);
          o2_1.SegPivot(ref os);
          if (o2_2.triangle == Mesh.dummytri || o2_2.IsInfected())
          {
            if (os.seg != Mesh.dummysub)
            {
              mesh.SubsegDealloc(os.seg);
              if (o2_2.triangle != Mesh.dummytri)
              {
                o2_2.Uninfect();
                o2_2.SegDissolve();
                o2_2.Infect();
              }
            }
          }
          else if (os.seg == Mesh.dummysub)
          {
            o2_2.Infect();
            viri.Add(o2_2.triangle);
          }
          else
          {
            os.TriDissolve();
            if (os.seg.boundary == 0)
              os.seg.boundary = 1;
            Vertex vertex1 = o2_2.Org();
            Vertex vertex2 = o2_2.Dest();
            if (vertex1.mark == 0)
              vertex1.mark = 1;
            if (vertex2.mark == 0)
              vertex2.mark = 1;
          }
        }
        o2_1.Infect();
      }
      foreach (Triangle triangle in viri)
      {
        o2_1.triangle = triangle;
        for (o2_1.orient = 0; o2_1.orient < 3; ++o2_1.orient)
        {
          Vertex vertex = o2_1.Org();
          if (vertex != null)
          {
            bool flag = true;
            o2_1.SetOrg(null);
            o2_1.Onext(ref o2_2);
            while (o2_2.triangle != Mesh.dummytri && !o2_2.Equal(o2_1))
            {
              if (o2_2.IsInfected())
                o2_2.SetOrg(null);
              else
                flag = false;
              o2_2.OnextSelf();
            }
            if (o2_2.triangle == Mesh.dummytri)
            {
              o2_1.Oprev(ref o2_2);
              while (o2_2.triangle != Mesh.dummytri)
              {
                if (o2_2.IsInfected())
                  o2_2.SetOrg(null);
                else
                  flag = false;
                o2_2.OprevSelf();
              }
            }
            if (flag)
            {
              vertex.type = VertexType.UndeadVertex;
              ++mesh.undeads;
            }
          }
        }
        for (o2_1.orient = 0; o2_1.orient < 3; ++o2_1.orient)
        {
          o2_1.Sym(ref o2_2);
          if (o2_2.triangle == Mesh.dummytri)
          {
            --mesh.hullsize;
          }
          else
          {
            o2_2.Dissolve();
            ++mesh.hullsize;
          }
        }
        mesh.TriangleDealloc(o2_1.triangle);
      }
      viri.Clear();
    }

    public void CarveHoles()
    {
      Otri searchtri = new Otri();
      Triangle[] triangleArray = null;
      if (!mesh.behavior.Convex)
        InfectHull();
      if (!mesh.behavior.NoHoles)
      {
        foreach (Point hole in mesh.holes)
        {
          if (mesh.bounds.Contains(hole))
          {
            searchtri.triangle = Mesh.dummytri;
            searchtri.orient = 0;
            searchtri.SymSelf();
            if (Primitives.CounterClockwise(searchtri.Org(), searchtri.Dest(), hole) > 0.0 && mesh.locator.Locate(hole, ref searchtri) != LocateResult.Outside && !searchtri.IsInfected())
            {
              searchtri.Infect();
              viri.Add(searchtri.triangle);
            }
          }
        }
      }
      if (mesh.regions.Count > 0)
      {
        int index = 0;
        triangleArray = new Triangle[mesh.regions.Count];
        foreach (RegionPointer region in mesh.regions)
        {
          triangleArray[index] = Mesh.dummytri;
          if (mesh.bounds.Contains(region.point))
          {
            searchtri.triangle = Mesh.dummytri;
            searchtri.orient = 0;
            searchtri.SymSelf();
            if (Primitives.CounterClockwise(searchtri.Org(), searchtri.Dest(), region.point) > 0.0 && mesh.locator.Locate(region.point, ref searchtri) != LocateResult.Outside && !searchtri.IsInfected())
            {
              triangleArray[index] = searchtri.triangle;
              triangleArray[index].region = region.id;
            }
          }
          ++index;
        }
      }
      if (viri.Count > 0)
        Plague();
      if (triangleArray != null)
      {
        RegionIterator regionIterator = new RegionIterator(mesh);
        for (int index = 0; index < triangleArray.Length; ++index)
        {
          if (triangleArray[index] != Mesh.dummytri && !Otri.IsDead(triangleArray[index]))
            regionIterator.Process(triangleArray[index]);
        }
      }
      viri.Clear();
    }
  }
}
