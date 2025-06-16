// Decompiled with JetBrains decompiler
// Type: TriangleNet.Geometry.EdgeEnumerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Data;

#nullable disable
namespace TriangleNet.Geometry
{
  public class EdgeEnumerator : IEnumerator<Edge>, IEnumerator, IDisposable
  {
    private Edge current;
    private Otri neighbor = new Otri();
    private Vertex p1;
    private Vertex p2;
    private Osub sub = new Osub();
    private Otri tri = new Otri();
    private IEnumerator<Triangle> triangles;

    public EdgeEnumerator(Mesh mesh)
    {
      this.triangles = (IEnumerator<Triangle>) mesh.triangles.Values.GetEnumerator();
      this.triangles.MoveNext();
      this.tri.triangle = this.triangles.Current;
      this.tri.orient = 0;
    }

    public Edge Current => this.current;

    public void Dispose() => this.triangles.Dispose();

    object IEnumerator.Current => (object) this.current;

    public bool MoveNext()
    {
      if (this.tri.triangle == null)
        return false;
      this.current = (Edge) null;
      while (this.current == null)
      {
        if (this.tri.orient == 3)
        {
          if (!this.triangles.MoveNext())
            return false;
          this.tri.triangle = this.triangles.Current;
          this.tri.orient = 0;
        }
        this.tri.Sym(ref this.neighbor);
        if (this.tri.triangle.id < this.neighbor.triangle.id || this.neighbor.triangle == Mesh.dummytri)
        {
          this.p1 = this.tri.Org();
          this.p2 = this.tri.Dest();
          this.tri.SegPivot(ref this.sub);
          this.current = new Edge(this.p1.id, this.p2.id, this.sub.seg.boundary);
        }
        ++this.tri.orient;
      }
      return true;
    }

    public void Reset() => this.triangles.Reset();
  }
}
