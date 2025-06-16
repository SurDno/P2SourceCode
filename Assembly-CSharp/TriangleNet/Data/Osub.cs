namespace TriangleNet.Data
{
  internal struct Osub
  {
    public Segment seg;
    public int orient;

    public override string ToString()
    {
      return this.seg == null ? "O-TID [null]" : string.Format("O-SID {0}", (object) this.seg.hash);
    }

    public void Sym(ref Osub o2)
    {
      o2.seg = this.seg;
      o2.orient = 1 - this.orient;
    }

    public void SymSelf() => this.orient = 1 - this.orient;

    public void Pivot(ref Osub o2) => o2 = this.seg.subsegs[this.orient];

    public void PivotSelf() => this = this.seg.subsegs[this.orient];

    public void Next(ref Osub o2) => o2 = this.seg.subsegs[1 - this.orient];

    public void NextSelf() => this = this.seg.subsegs[1 - this.orient];

    public Vertex Org() => this.seg.vertices[this.orient];

    public Vertex Dest() => this.seg.vertices[1 - this.orient];

    public void SetOrg(Vertex ptr) => this.seg.vertices[this.orient] = ptr;

    public void SetDest(Vertex ptr) => this.seg.vertices[1 - this.orient] = ptr;

    public Vertex SegOrg() => this.seg.vertices[2 + this.orient];

    public Vertex SegDest() => this.seg.vertices[3 - this.orient];

    public void SetSegOrg(Vertex ptr) => this.seg.vertices[2 + this.orient] = ptr;

    public void SetSegDest(Vertex ptr) => this.seg.vertices[3 - this.orient] = ptr;

    public int Mark() => this.seg.boundary;

    public void SetMark(int value) => this.seg.boundary = value;

    public void Bond(ref Osub o2)
    {
      this.seg.subsegs[this.orient] = o2;
      o2.seg.subsegs[o2.orient] = this;
    }

    public void Dissolve() => this.seg.subsegs[this.orient].seg = Mesh.dummysub;

    public void Copy(ref Osub o2)
    {
      o2.seg = this.seg;
      o2.orient = this.orient;
    }

    public bool Equal(Osub o2) => this.seg == o2.seg && this.orient == o2.orient;

    public static bool IsDead(Segment sub) => sub.subsegs[0].seg == null;

    public static void Kill(Segment sub)
    {
      sub.subsegs[0].seg = (Segment) null;
      sub.subsegs[1].seg = (Segment) null;
    }

    public void TriPivot(ref Otri ot) => ot = this.seg.triangles[this.orient];

    public void TriDissolve() => this.seg.triangles[this.orient].triangle = Mesh.dummytri;
  }
}
