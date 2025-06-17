namespace TriangleNet.Data
{
  internal struct Otri
  {
    public Triangle triangle;
    public int orient;
    private static readonly int[] plus1Mod3 = [
      1,
      2,
      0
    ];
    private static readonly int[] minus1Mod3 = [
      2,
      0,
      1
    ];

    public override string ToString()
    {
      return triangle == null ? "O-TID [null]" : string.Format("O-TID {0}", triangle.hash);
    }

    public void Sym(ref Otri o2)
    {
      o2.triangle = triangle.neighbors[orient].triangle;
      o2.orient = triangle.neighbors[orient].orient;
    }

    public void SymSelf()
    {
      int orient = this.orient;
      this.orient = triangle.neighbors[orient].orient;
      triangle = triangle.neighbors[orient].triangle;
    }

    public void Lnext(ref Otri o2)
    {
      o2.triangle = triangle;
      o2.orient = plus1Mod3[orient];
    }

    public void LnextSelf() => orient = plus1Mod3[orient];

    public void Lprev(ref Otri o2)
    {
      o2.triangle = triangle;
      o2.orient = minus1Mod3[orient];
    }

    public void LprevSelf() => orient = minus1Mod3[orient];

    public void Onext(ref Otri o2)
    {
      o2.triangle = triangle;
      o2.orient = minus1Mod3[this.orient];
      int orient = o2.orient;
      o2.orient = o2.triangle.neighbors[orient].orient;
      o2.triangle = o2.triangle.neighbors[orient].triangle;
    }

    public void OnextSelf()
    {
      this.orient = minus1Mod3[this.orient];
      int orient = this.orient;
      this.orient = triangle.neighbors[orient].orient;
      triangle = triangle.neighbors[orient].triangle;
    }

    public void Oprev(ref Otri o2)
    {
      o2.triangle = triangle.neighbors[orient].triangle;
      o2.orient = triangle.neighbors[orient].orient;
      o2.orient = plus1Mod3[o2.orient];
    }

    public void OprevSelf()
    {
      int orient = this.orient;
      this.orient = triangle.neighbors[orient].orient;
      triangle = triangle.neighbors[orient].triangle;
      this.orient = plus1Mod3[this.orient];
    }

    public void Dnext(ref Otri o2)
    {
      o2.triangle = triangle.neighbors[orient].triangle;
      o2.orient = triangle.neighbors[orient].orient;
      o2.orient = minus1Mod3[o2.orient];
    }

    public void DnextSelf()
    {
      int orient = this.orient;
      this.orient = triangle.neighbors[orient].orient;
      triangle = triangle.neighbors[orient].triangle;
      this.orient = minus1Mod3[this.orient];
    }

    public void Dprev(ref Otri o2)
    {
      o2.triangle = triangle;
      o2.orient = plus1Mod3[this.orient];
      int orient = o2.orient;
      o2.orient = o2.triangle.neighbors[orient].orient;
      o2.triangle = o2.triangle.neighbors[orient].triangle;
    }

    public void DprevSelf()
    {
      this.orient = plus1Mod3[this.orient];
      int orient = this.orient;
      this.orient = triangle.neighbors[orient].orient;
      triangle = triangle.neighbors[orient].triangle;
    }

    public void Rnext(ref Otri o2)
    {
      o2.triangle = triangle.neighbors[this.orient].triangle;
      o2.orient = triangle.neighbors[this.orient].orient;
      o2.orient = plus1Mod3[o2.orient];
      int orient = o2.orient;
      o2.orient = o2.triangle.neighbors[orient].orient;
      o2.triangle = o2.triangle.neighbors[orient].triangle;
    }

    public void RnextSelf()
    {
      int orient1 = orient;
      orient = triangle.neighbors[orient1].orient;
      triangle = triangle.neighbors[orient1].triangle;
      orient = plus1Mod3[orient];
      int orient2 = orient;
      orient = triangle.neighbors[orient2].orient;
      triangle = triangle.neighbors[orient2].triangle;
    }

    public void Rprev(ref Otri o2)
    {
      o2.triangle = triangle.neighbors[this.orient].triangle;
      o2.orient = triangle.neighbors[this.orient].orient;
      o2.orient = minus1Mod3[o2.orient];
      int orient = o2.orient;
      o2.orient = o2.triangle.neighbors[orient].orient;
      o2.triangle = o2.triangle.neighbors[orient].triangle;
    }

    public void RprevSelf()
    {
      int orient1 = orient;
      orient = triangle.neighbors[orient1].orient;
      triangle = triangle.neighbors[orient1].triangle;
      orient = minus1Mod3[orient];
      int orient2 = orient;
      orient = triangle.neighbors[orient2].orient;
      triangle = triangle.neighbors[orient2].triangle;
    }

    public Vertex Org() => triangle.vertices[plus1Mod3[orient]];

    public Vertex Dest() => triangle.vertices[minus1Mod3[orient]];

    public Vertex Apex() => triangle.vertices[orient];

    public void SetOrg(Vertex ptr) => triangle.vertices[plus1Mod3[orient]] = ptr;

    public void SetDest(Vertex ptr) => triangle.vertices[minus1Mod3[orient]] = ptr;

    public void SetApex(Vertex ptr) => triangle.vertices[orient] = ptr;

    public void Bond(ref Otri o2)
    {
      triangle.neighbors[orient].triangle = o2.triangle;
      triangle.neighbors[orient].orient = o2.orient;
      o2.triangle.neighbors[o2.orient].triangle = triangle;
      o2.triangle.neighbors[o2.orient].orient = orient;
    }

    public void Dissolve()
    {
      triangle.neighbors[orient].triangle = Mesh.dummytri;
      triangle.neighbors[orient].orient = 0;
    }

    public void Copy(ref Otri o2)
    {
      o2.triangle = triangle;
      o2.orient = orient;
    }

    public bool Equal(Otri o2) => triangle == o2.triangle && orient == o2.orient;

    public void Infect() => triangle.infected = true;

    public void Uninfect() => triangle.infected = false;

    public bool IsInfected() => triangle.infected;

    public static bool IsDead(Triangle tria) => tria.neighbors[0].triangle == null;

    public static void Kill(Triangle tria)
    {
      tria.neighbors[0].triangle = null;
      tria.neighbors[2].triangle = null;
    }

    public void SegPivot(ref Osub os) => os = triangle.subsegs[orient];

    public void SegBond(ref Osub os)
    {
      triangle.subsegs[orient] = os;
      os.seg.triangles[os.orient] = this;
    }

    public void SegDissolve() => triangle.subsegs[orient].seg = Mesh.dummysub;
  }
}
