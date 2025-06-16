using System;
using System.Collections.Generic;
using TriangleNet.Data;

namespace TriangleNet.Tools
{
  public class RegionIterator
  {
    private Mesh mesh;
    private List<Triangle> viri;

    public RegionIterator(Mesh mesh)
    {
      this.mesh = mesh;
      viri = new List<Triangle>();
    }

    private void ProcessRegion(Action<Triangle> func)
    {
      Otri otri = new Otri();
      Otri o2 = new Otri();
      Osub os = new Osub();
      Behavior behavior = mesh.behavior;
      for (int index = 0; index < viri.Count; ++index)
      {
        otri.triangle = viri[index];
        otri.Uninfect();
        func(otri.triangle);
        for (otri.orient = 0; otri.orient < 3; ++otri.orient)
        {
          otri.Sym(ref o2);
          otri.SegPivot(ref os);
          if (o2.triangle != Mesh.dummytri && !o2.IsInfected() && os.seg == Mesh.dummysub)
          {
            o2.Infect();
            viri.Add(o2.triangle);
          }
        }
        otri.Infect();
      }
      foreach (Triangle triangle in viri)
        triangle.infected = false;
      viri.Clear();
    }

    public void Process(Triangle triangle)
    {
      Process(triangle, tri => tri.region = triangle.region);
    }

    public void Process(Triangle triangle, Action<Triangle> func)
    {
      if (triangle != Mesh.dummytri && !Otri.IsDead(triangle))
      {
        triangle.infected = true;
        viri.Add(triangle);
        ProcessRegion(func);
      }
      viri.Clear();
    }
  }
}
