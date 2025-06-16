// Decompiled with JetBrains decompiler
// Type: TriangleNet.Mesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TriangleNet.Algorithm;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.IO;
using TriangleNet.Log;
using TriangleNet.Smoothing;
using TriangleNet.Tools;

#nullable disable
namespace TriangleNet
{
  public class Mesh
  {
    internal static Triangle dummytri;
    internal static Segment dummysub;
    internal Behavior behavior;
    internal BoundingBox bounds;
    internal bool checkquality;
    internal bool checksegments;
    internal int edges;
    private Stack<Otri> flipstack;
    internal int hash_seg;
    internal int hash_tri;
    internal int hash_vtx;
    internal List<Point> holes;
    internal int hullsize;
    internal int inelements;
    internal Vertex infvertex1;
    internal Vertex infvertex2;
    internal Vertex infvertex3;
    internal int insegments;
    internal int invertices;
    internal TriangleLocator locator;
    private ILog<SimpleLogItem> logger;
    internal int mesh_dim;
    internal int nextras;
    internal NodeNumbering numbering;
    private Quality quality;
    internal List<RegionPointer> regions;
    internal int steinerleft;
    internal Dictionary<int, Segment> subsegs;
    internal Dictionary<int, Triangle> triangles;
    internal int undeads;
    internal Dictionary<int, Vertex> vertices;

    public Behavior Behavior => this.behavior;

    public BoundingBox Bounds => this.bounds;

    public ICollection<Vertex> Vertices => (ICollection<Vertex>) this.vertices.Values;

    public IList<Point> Holes => (IList<Point>) this.holes;

    public ICollection<Triangle> Triangles => (ICollection<Triangle>) this.triangles.Values;

    public ICollection<Segment> Segments => (ICollection<Segment>) this.subsegs.Values;

    public IEnumerable<Edge> Edges
    {
      get
      {
        EdgeEnumerator e = new EdgeEnumerator(this);
        while (e.MoveNext())
          yield return e.Current;
      }
    }

    public int NumberOfInputPoints => this.invertices;

    public int NumberOfEdges => this.edges;

    public bool IsPolygon => this.insegments > 0;

    public NodeNumbering CurrentNumbering => this.numbering;

    public Mesh()
      : this(new Behavior())
    {
    }

    public Mesh(Behavior behavior)
    {
      this.behavior = behavior;
      this.logger = SimpleLog.Instance;
      behavior = new Behavior();
      this.vertices = new Dictionary<int, Vertex>();
      this.triangles = new Dictionary<int, Triangle>();
      this.subsegs = new Dictionary<int, Segment>();
      this.flipstack = new Stack<Otri>();
      this.holes = new List<Point>();
      this.regions = new List<RegionPointer>();
      this.quality = new Quality(this);
      this.locator = new TriangleLocator(this);
      Primitives.ExactInit();
      if (Mesh.dummytri != null)
        return;
      this.DummyInit();
    }

    public void Load(string filename)
    {
      InputGeometry geometry;
      List<ITriangle> triangles;
      FileReader.Read(filename, out geometry, out triangles);
      if (geometry == null || triangles == null)
        return;
      this.Load(geometry, triangles);
    }

    public void Load(InputGeometry input, List<ITriangle> triangles)
    {
      if (input == null || triangles == null)
        throw new ArgumentException("Invalid input (argument is null).");
      this.ResetData();
      if (input.HasSegments)
      {
        this.behavior.Poly = true;
        this.holes.AddRange((IEnumerable<Point>) input.Holes);
      }
      if (!this.behavior.Poly)
      {
        this.behavior.VarArea = false;
        this.behavior.useRegions = false;
      }
      this.behavior.useRegions = input.Regions.Count > 0;
      this.TransferNodes(input);
      this.hullsize = DataReader.Reconstruct(this, input, triangles.ToArray());
      this.edges = (3 * triangles.Count + this.hullsize) / 2;
    }

    public void Triangulate(string inputFile) => this.Triangulate(FileReader.Read(inputFile));

    public void Triangulate(InputGeometry input)
    {
      this.ResetData();
      this.behavior.Poly = input.HasSegments;
      if (!this.behavior.Poly)
      {
        this.behavior.VarArea = false;
        this.behavior.useRegions = false;
      }
      this.behavior.useRegions = input.Regions.Count > 0;
      this.steinerleft = this.behavior.SteinerPoints;
      this.TransferNodes(input);
      this.hullsize = this.Delaunay();
      this.infvertex1 = (Vertex) null;
      this.infvertex2 = (Vertex) null;
      this.infvertex3 = (Vertex) null;
      if (this.behavior.useSegments)
      {
        this.checksegments = true;
        this.FormSkeleton(input);
      }
      if (this.behavior.Poly && this.triangles.Count > 0)
      {
        foreach (Point hole in input.holes)
          this.holes.Add(hole);
        foreach (RegionPointer region in input.regions)
          this.regions.Add(region);
        new Carver(this).CarveHoles();
      }
      else
      {
        this.holes.Clear();
        this.regions.Clear();
      }
      if (this.behavior.Quality && this.triangles.Count > 0)
        this.quality.EnforceQuality();
      this.edges = (3 * this.triangles.Count + this.hullsize) / 2;
    }

    public void Refine(bool halfArea)
    {
      if (halfArea)
      {
        double num1 = 0.0;
        foreach (Triangle triangle in this.triangles.Values)
        {
          double num2 = Math.Abs((triangle.vertices[2].x - triangle.vertices[0].x) * (triangle.vertices[1].y - triangle.vertices[0].y) - (triangle.vertices[1].x - triangle.vertices[0].x) * (triangle.vertices[2].y - triangle.vertices[0].y)) / 2.0;
          if (num2 > num1)
            num1 = num2;
        }
        this.Refine(num1 / 2.0);
      }
      else
        this.Refine();
    }

    public void Refine(double areaConstraint)
    {
      this.behavior.fixedArea = true;
      this.behavior.MaxArea = areaConstraint;
      this.Refine();
      this.behavior.fixedArea = false;
      this.behavior.MaxArea = -1.0;
    }

    public void Refine()
    {
      this.inelements = this.triangles.Count;
      this.invertices = this.vertices.Count;
      if (this.behavior.Poly)
        this.insegments = !this.behavior.useSegments ? this.hullsize : this.subsegs.Count;
      this.Reset();
      this.steinerleft = this.behavior.SteinerPoints;
      this.infvertex1 = (Vertex) null;
      this.infvertex2 = (Vertex) null;
      this.infvertex3 = (Vertex) null;
      if (this.behavior.useSegments)
        this.checksegments = true;
      if (this.triangles.Count > 0)
        this.quality.EnforceQuality();
      this.edges = (3 * this.triangles.Count + this.hullsize) / 2;
    }

    public void Smooth()
    {
      this.numbering = NodeNumbering.None;
      new SimpleSmoother(this).Smooth();
    }

    public void Renumber() => this.Renumber(NodeNumbering.Linear);

    public void Renumber(NodeNumbering num)
    {
      if (num == this.numbering)
        return;
      switch (num)
      {
        case NodeNumbering.Linear:
          int num1 = 0;
          using (Dictionary<int, Vertex>.ValueCollection.Enumerator enumerator = this.vertices.Values.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.id = num1++;
            break;
          }
        case NodeNumbering.CuthillMcKee:
          int[] numArray = new CuthillMcKee().Renumber(this);
          using (Dictionary<int, Vertex>.ValueCollection.Enumerator enumerator = this.vertices.Values.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Vertex current = enumerator.Current;
              current.id = numArray[current.id];
            }
            break;
          }
      }
      this.numbering = num;
      int num2 = 0;
      foreach (Triangle triangle in this.triangles.Values)
        triangle.id = num2++;
    }

    public void Check(out bool isConsistent, out bool isDelaunay)
    {
      isConsistent = this.quality.CheckMesh();
      isDelaunay = this.quality.CheckDelaunay();
    }

    private int Delaunay()
    {
      int num = this.behavior.Algorithm != TriangulationAlgorithm.Dwyer ? (this.behavior.Algorithm != TriangulationAlgorithm.SweepLine ? new Incremental().Triangulate(this) : new SweepLine().Triangulate(this)) : new Dwyer().Triangulate(this);
      return this.triangles.Count == 0 ? 0 : num;
    }

    private void ResetData()
    {
      this.vertices.Clear();
      this.triangles.Clear();
      this.subsegs.Clear();
      this.holes.Clear();
      this.regions.Clear();
      this.hash_vtx = 0;
      this.hash_seg = 0;
      this.hash_tri = 0;
      this.flipstack.Clear();
      this.hullsize = 0;
      this.edges = 0;
      this.Reset();
      this.locator.Reset();
    }

    private void Reset()
    {
      this.numbering = NodeNumbering.None;
      this.undeads = 0;
      this.checksegments = false;
      this.checkquality = false;
      Statistic.InCircleCount = 0L;
      Statistic.CounterClockwiseCount = 0L;
      Statistic.InCircleCountDecimal = 0L;
      Statistic.CounterClockwiseCountDecimal = 0L;
      Statistic.Orient3dCount = 0L;
      Statistic.HyperbolaCount = 0L;
      Statistic.CircleTopCount = 0L;
      Statistic.CircumcenterCount = 0L;
    }

    private void DummyInit()
    {
      Mesh.dummytri = new Triangle();
      Mesh.dummytri.hash = -1;
      Mesh.dummytri.id = -1;
      Mesh.dummytri.neighbors[0].triangle = Mesh.dummytri;
      Mesh.dummytri.neighbors[1].triangle = Mesh.dummytri;
      Mesh.dummytri.neighbors[2].triangle = Mesh.dummytri;
      if (!this.behavior.useSegments)
        return;
      Mesh.dummysub = new Segment();
      Mesh.dummysub.hash = -1;
      Mesh.dummysub.subsegs[0].seg = Mesh.dummysub;
      Mesh.dummysub.subsegs[1].seg = Mesh.dummysub;
      Mesh.dummytri.subsegs[0].seg = Mesh.dummysub;
      Mesh.dummytri.subsegs[1].seg = Mesh.dummysub;
      Mesh.dummytri.subsegs[2].seg = Mesh.dummysub;
    }

    private void TransferNodes(InputGeometry data)
    {
      List<Vertex> points = data.points;
      this.invertices = points.Count;
      this.mesh_dim = 2;
      if (this.invertices < 3)
      {
        this.logger.Error("Input must have at least three input vertices.", "MeshReader.TransferNodes()");
        throw new Exception("Input must have at least three input vertices.");
      }
      this.nextras = points[0].attributes == null ? 0 : points[0].attributes.Length;
      foreach (Vertex vertex in points)
      {
        vertex.hash = this.hash_vtx++;
        vertex.id = vertex.hash;
        this.vertices.Add(vertex.hash, vertex);
      }
      this.bounds = data.Bounds;
    }

    internal void MakeVertexMap()
    {
      Otri otri = new Otri();
      foreach (Triangle triangle in this.triangles.Values)
      {
        otri.triangle = triangle;
        for (otri.orient = 0; otri.orient < 3; ++otri.orient)
          otri.Org().tri = otri;
      }
    }

    internal void MakeTriangle(ref Otri newotri)
    {
      Triangle triangle = new Triangle()
      {
        hash = this.hash_tri++
      };
      triangle.id = triangle.hash;
      newotri.triangle = triangle;
      newotri.orient = 0;
      this.triangles.Add(triangle.hash, triangle);
    }

    internal void MakeSegment(ref Osub newsubseg)
    {
      Segment segment = new Segment();
      segment.hash = this.hash_seg++;
      newsubseg.seg = segment;
      newsubseg.orient = 0;
      this.subsegs.Add(segment.hash, segment);
    }

    internal InsertVertexResult InsertVertex(
      Vertex newvertex,
      ref Otri searchtri,
      ref Osub splitseg,
      bool segmentflaws,
      bool triflaws)
    {
      Otri otri1 = new Otri();
      Otri o2_1 = new Otri();
      Otri o2_2 = new Otri();
      Otri o2_3 = new Otri();
      Otri o2_4 = new Otri();
      Otri o2_5 = new Otri();
      Otri otri2 = new Otri();
      Otri otri3 = new Otri();
      Otri newotri = new Otri();
      Otri o2_6 = new Otri();
      Otri o2_7 = new Otri();
      Otri o2_8 = new Otri();
      Otri o2_9 = new Otri();
      Otri o2_10 = new Otri();
      Osub os1 = new Osub();
      Osub os2 = new Osub();
      Osub os3 = new Osub();
      Osub os4 = new Osub();
      Osub os5 = new Osub();
      Osub osub1 = new Osub();
      Osub o2_11 = new Osub();
      Osub osub2 = new Osub();
      LocateResult locateResult;
      if (splitseg.seg == null)
      {
        if (searchtri.triangle == Mesh.dummytri)
        {
          otri1.triangle = Mesh.dummytri;
          otri1.orient = 0;
          otri1.SymSelf();
          locateResult = this.locator.Locate((Point) newvertex, ref otri1);
        }
        else
        {
          searchtri.Copy(ref otri1);
          locateResult = this.locator.PreciseLocate((Point) newvertex, ref otri1, true);
        }
      }
      else
      {
        searchtri.Copy(ref otri1);
        locateResult = LocateResult.OnEdge;
      }
      int num1;
      switch (locateResult)
      {
        case LocateResult.OnEdge:
          num1 = 1;
          break;
        case LocateResult.OnVertex:
          otri1.Copy(ref searchtri);
          this.locator.Update(ref otri1);
          return InsertVertexResult.Duplicate;
        default:
          num1 = locateResult == LocateResult.Outside ? 1 : 0;
          break;
      }
      if (num1 != 0)
      {
        if (this.checksegments && splitseg.seg == null)
        {
          otri1.SegPivot(ref os5);
          if (os5.seg != Mesh.dummysub)
          {
            if (segmentflaws)
            {
              bool flag = this.behavior.NoBisect != 2;
              if (flag && this.behavior.NoBisect == 1)
              {
                otri1.Sym(ref o2_10);
                flag = o2_10.triangle != Mesh.dummytri;
              }
              if (flag)
                this.quality.AddBadSubseg(new BadSubseg()
                {
                  encsubseg = os5,
                  subsegorg = os5.Org(),
                  subsegdest = os5.Dest()
                });
            }
            otri1.Copy(ref searchtri);
            this.locator.Update(ref otri1);
            return InsertVertexResult.Violating;
          }
        }
        otri1.Lprev(ref o2_3);
        o2_3.Sym(ref o2_7);
        otri1.Sym(ref o2_5);
        bool flag1 = o2_5.triangle != Mesh.dummytri;
        if (flag1)
        {
          o2_5.LnextSelf();
          o2_5.Sym(ref o2_9);
          this.MakeTriangle(ref newotri);
        }
        else
          ++this.hullsize;
        this.MakeTriangle(ref otri3);
        Vertex ptr1 = otri1.Org();
        otri1.Dest();
        Vertex ptr2 = otri1.Apex();
        otri3.SetOrg(ptr2);
        otri3.SetDest(ptr1);
        otri3.SetApex(newvertex);
        otri1.SetOrg(newvertex);
        otri3.triangle.region = o2_3.triangle.region;
        if (this.behavior.VarArea)
          otri3.triangle.area = o2_3.triangle.area;
        if (flag1)
        {
          Vertex ptr3 = o2_5.Dest();
          newotri.SetOrg(ptr1);
          newotri.SetDest(ptr3);
          newotri.SetApex(newvertex);
          o2_5.SetOrg(newvertex);
          newotri.triangle.region = o2_5.triangle.region;
          if (this.behavior.VarArea)
            newotri.triangle.area = o2_5.triangle.area;
        }
        if (this.checksegments)
        {
          o2_3.SegPivot(ref os2);
          if (os2.seg != Mesh.dummysub)
          {
            o2_3.SegDissolve();
            otri3.SegBond(ref os2);
          }
          if (flag1)
          {
            o2_5.SegPivot(ref os4);
            if (os4.seg != Mesh.dummysub)
            {
              o2_5.SegDissolve();
              newotri.SegBond(ref os4);
            }
          }
        }
        otri3.Bond(ref o2_7);
        otri3.LprevSelf();
        otri3.Bond(ref o2_3);
        otri3.LprevSelf();
        if (flag1)
        {
          newotri.Bond(ref o2_9);
          newotri.LnextSelf();
          newotri.Bond(ref o2_5);
          newotri.LnextSelf();
          newotri.Bond(ref otri3);
        }
        if (splitseg.seg != null)
        {
          splitseg.SetDest(newvertex);
          Vertex ptr4 = splitseg.SegOrg();
          Vertex ptr5 = splitseg.SegDest();
          splitseg.SymSelf();
          splitseg.Pivot(ref o2_11);
          this.InsertSubseg(ref otri3, splitseg.seg.boundary);
          otri3.SegPivot(ref osub2);
          osub2.SetSegOrg(ptr4);
          osub2.SetSegDest(ptr5);
          splitseg.Bond(ref osub2);
          osub2.SymSelf();
          osub2.Bond(ref o2_11);
          splitseg.SymSelf();
          if (newvertex.mark == 0)
            newvertex.mark = splitseg.seg.boundary;
        }
        if (this.checkquality)
        {
          this.flipstack.Clear();
          this.flipstack.Push(new Otri());
          this.flipstack.Push(otri1);
        }
        otri1.LnextSelf();
      }
      else
      {
        otri1.Lnext(ref o2_2);
        otri1.Lprev(ref o2_3);
        o2_2.Sym(ref o2_6);
        o2_3.Sym(ref o2_7);
        this.MakeTriangle(ref otri2);
        this.MakeTriangle(ref otri3);
        Vertex ptr6 = otri1.Org();
        Vertex ptr7 = otri1.Dest();
        Vertex ptr8 = otri1.Apex();
        otri2.SetOrg(ptr7);
        otri2.SetDest(ptr8);
        otri2.SetApex(newvertex);
        otri3.SetOrg(ptr8);
        otri3.SetDest(ptr6);
        otri3.SetApex(newvertex);
        otri1.SetApex(newvertex);
        otri2.triangle.region = otri1.triangle.region;
        otri3.triangle.region = otri1.triangle.region;
        if (this.behavior.VarArea)
        {
          double area = otri1.triangle.area;
          otri2.triangle.area = area;
          otri3.triangle.area = area;
        }
        if (this.checksegments)
        {
          o2_2.SegPivot(ref os1);
          if (os1.seg != Mesh.dummysub)
          {
            o2_2.SegDissolve();
            otri2.SegBond(ref os1);
          }
          o2_3.SegPivot(ref os2);
          if (os2.seg != Mesh.dummysub)
          {
            o2_3.SegDissolve();
            otri3.SegBond(ref os2);
          }
        }
        otri2.Bond(ref o2_6);
        otri3.Bond(ref o2_7);
        otri2.LnextSelf();
        otri3.LprevSelf();
        otri2.Bond(ref otri3);
        otri2.LnextSelf();
        o2_2.Bond(ref otri2);
        otri3.LprevSelf();
        o2_3.Bond(ref otri3);
        if (this.checkquality)
        {
          this.flipstack.Clear();
          this.flipstack.Push(otri1);
        }
      }
      InsertVertexResult insertVertexResult = InsertVertexResult.Successful;
      Vertex vertex1 = otri1.Org();
      Vertex vertex2 = vertex1;
      Vertex vertex3 = otri1.Dest();
      while (true)
      {
        bool flag = true;
        if (this.checksegments)
        {
          otri1.SegPivot(ref osub1);
          if (osub1.seg != Mesh.dummysub)
          {
            flag = false;
            if (segmentflaws && this.quality.CheckSeg4Encroach(ref osub1) > 0)
              insertVertexResult = InsertVertexResult.Encroaching;
          }
        }
        if (flag)
        {
          otri1.Sym(ref o2_1);
          if (o2_1.triangle == Mesh.dummytri)
          {
            flag = false;
          }
          else
          {
            Vertex vertex4 = o2_1.Apex();
            flag = !((Point) vertex3 == (Point) this.infvertex1) && !((Point) vertex3 == (Point) this.infvertex2) && !((Point) vertex3 == (Point) this.infvertex3) ? (!((Point) vertex2 == (Point) this.infvertex1) && !((Point) vertex2 == (Point) this.infvertex2) && !((Point) vertex2 == (Point) this.infvertex3) ? !((Point) vertex4 == (Point) this.infvertex1) && !((Point) vertex4 == (Point) this.infvertex2) && !((Point) vertex4 == (Point) this.infvertex3) && Primitives.InCircle((Point) vertex3, (Point) newvertex, (Point) vertex2, (Point) vertex4) > 0.0 : Primitives.CounterClockwise((Point) vertex4, (Point) vertex3, (Point) newvertex) > 0.0) : Primitives.CounterClockwise((Point) newvertex, (Point) vertex2, (Point) vertex4) > 0.0;
            if (flag)
            {
              o2_1.Lprev(ref o2_4);
              o2_4.Sym(ref o2_8);
              o2_1.Lnext(ref o2_5);
              o2_5.Sym(ref o2_9);
              otri1.Lnext(ref o2_2);
              o2_2.Sym(ref o2_6);
              otri1.Lprev(ref o2_3);
              o2_3.Sym(ref o2_7);
              o2_4.Bond(ref o2_6);
              o2_2.Bond(ref o2_7);
              o2_3.Bond(ref o2_9);
              o2_5.Bond(ref o2_8);
              if (this.checksegments)
              {
                o2_4.SegPivot(ref os3);
                o2_2.SegPivot(ref os1);
                o2_3.SegPivot(ref os2);
                o2_5.SegPivot(ref os4);
                if (os3.seg == Mesh.dummysub)
                  o2_5.SegDissolve();
                else
                  o2_5.SegBond(ref os3);
                if (os1.seg == Mesh.dummysub)
                  o2_4.SegDissolve();
                else
                  o2_4.SegBond(ref os1);
                if (os2.seg == Mesh.dummysub)
                  o2_2.SegDissolve();
                else
                  o2_2.SegBond(ref os2);
                if (os4.seg == Mesh.dummysub)
                  o2_3.SegDissolve();
                else
                  o2_3.SegBond(ref os4);
              }
              otri1.SetOrg(vertex4);
              otri1.SetDest(newvertex);
              otri1.SetApex(vertex2);
              o2_1.SetOrg(newvertex);
              o2_1.SetDest(vertex4);
              o2_1.SetApex(vertex3);
              int num2 = Math.Min(o2_1.triangle.region, otri1.triangle.region);
              o2_1.triangle.region = num2;
              otri1.triangle.region = num2;
              if (this.behavior.VarArea)
              {
                double num3 = o2_1.triangle.area > 0.0 && otri1.triangle.area > 0.0 ? 0.5 * (o2_1.triangle.area + otri1.triangle.area) : -1.0;
                o2_1.triangle.area = num3;
                otri1.triangle.area = num3;
              }
              if (this.checkquality)
                this.flipstack.Push(otri1);
              otri1.LprevSelf();
              vertex3 = vertex4;
            }
          }
        }
        if (!flag)
        {
          if (triflaws)
            this.quality.TestTriangle(ref otri1);
          otri1.LnextSelf();
          otri1.Sym(ref o2_10);
          if (!((Point) vertex3 == (Point) vertex1) && o2_10.triangle != Mesh.dummytri)
          {
            o2_10.Lnext(ref otri1);
            vertex2 = vertex3;
            vertex3 = otri1.Dest();
          }
          else
            break;
        }
      }
      otri1.Lnext(ref searchtri);
      Otri otri4 = new Otri();
      otri1.Lnext(ref otri4);
      this.locator.Update(ref otri4);
      return insertVertexResult;
    }

    internal void InsertSubseg(ref Otri tri, int subsegmark)
    {
      Otri o2 = new Otri();
      Osub osub = new Osub();
      Vertex ptr1 = tri.Org();
      Vertex ptr2 = tri.Dest();
      if (ptr1.mark == 0)
        ptr1.mark = subsegmark;
      if (ptr2.mark == 0)
        ptr2.mark = subsegmark;
      tri.SegPivot(ref osub);
      if (osub.seg == Mesh.dummysub)
      {
        this.MakeSegment(ref osub);
        osub.SetOrg(ptr2);
        osub.SetDest(ptr1);
        osub.SetSegOrg(ptr2);
        osub.SetSegDest(ptr1);
        tri.SegBond(ref osub);
        tri.Sym(ref o2);
        osub.SymSelf();
        o2.SegBond(ref osub);
        osub.seg.boundary = subsegmark;
      }
      else if (osub.seg.boundary == 0)
        osub.seg.boundary = subsegmark;
    }

    internal void Flip(ref Otri flipedge)
    {
      Otri o2_1 = new Otri();
      Otri o2_2 = new Otri();
      Otri o2_3 = new Otri();
      Otri o2_4 = new Otri();
      Otri o2_5 = new Otri();
      Otri o2_6 = new Otri();
      Otri o2_7 = new Otri();
      Otri o2_8 = new Otri();
      Otri o2_9 = new Otri();
      Osub os1 = new Osub();
      Osub os2 = new Osub();
      Osub os3 = new Osub();
      Osub os4 = new Osub();
      Vertex ptr1 = flipedge.Org();
      Vertex ptr2 = flipedge.Dest();
      Vertex ptr3 = flipedge.Apex();
      flipedge.Sym(ref o2_5);
      Vertex ptr4 = o2_5.Apex();
      o2_5.Lprev(ref o2_3);
      o2_3.Sym(ref o2_8);
      o2_5.Lnext(ref o2_4);
      o2_4.Sym(ref o2_9);
      flipedge.Lnext(ref o2_1);
      o2_1.Sym(ref o2_6);
      flipedge.Lprev(ref o2_2);
      o2_2.Sym(ref o2_7);
      o2_3.Bond(ref o2_6);
      o2_1.Bond(ref o2_7);
      o2_2.Bond(ref o2_9);
      o2_4.Bond(ref o2_8);
      if (this.checksegments)
      {
        o2_3.SegPivot(ref os3);
        o2_1.SegPivot(ref os1);
        o2_2.SegPivot(ref os2);
        o2_4.SegPivot(ref os4);
        if (os3.seg == Mesh.dummysub)
          o2_4.SegDissolve();
        else
          o2_4.SegBond(ref os3);
        if (os1.seg == Mesh.dummysub)
          o2_3.SegDissolve();
        else
          o2_3.SegBond(ref os1);
        if (os2.seg == Mesh.dummysub)
          o2_1.SegDissolve();
        else
          o2_1.SegBond(ref os2);
        if (os4.seg == Mesh.dummysub)
          o2_2.SegDissolve();
        else
          o2_2.SegBond(ref os4);
      }
      flipedge.SetOrg(ptr4);
      flipedge.SetDest(ptr3);
      flipedge.SetApex(ptr1);
      o2_5.SetOrg(ptr3);
      o2_5.SetDest(ptr4);
      o2_5.SetApex(ptr2);
    }

    internal void Unflip(ref Otri flipedge)
    {
      Otri o2_1 = new Otri();
      Otri o2_2 = new Otri();
      Otri o2_3 = new Otri();
      Otri o2_4 = new Otri();
      Otri o2_5 = new Otri();
      Otri o2_6 = new Otri();
      Otri o2_7 = new Otri();
      Otri o2_8 = new Otri();
      Otri o2_9 = new Otri();
      Osub os1 = new Osub();
      Osub os2 = new Osub();
      Osub os3 = new Osub();
      Osub os4 = new Osub();
      Vertex ptr1 = flipedge.Org();
      Vertex ptr2 = flipedge.Dest();
      Vertex ptr3 = flipedge.Apex();
      flipedge.Sym(ref o2_5);
      Vertex ptr4 = o2_5.Apex();
      o2_5.Lprev(ref o2_3);
      o2_3.Sym(ref o2_8);
      o2_5.Lnext(ref o2_4);
      o2_4.Sym(ref o2_9);
      flipedge.Lnext(ref o2_1);
      o2_1.Sym(ref o2_6);
      flipedge.Lprev(ref o2_2);
      o2_2.Sym(ref o2_7);
      o2_3.Bond(ref o2_9);
      o2_1.Bond(ref o2_8);
      o2_2.Bond(ref o2_6);
      o2_4.Bond(ref o2_7);
      if (this.checksegments)
      {
        o2_3.SegPivot(ref os3);
        o2_1.SegPivot(ref os1);
        o2_2.SegPivot(ref os2);
        o2_4.SegPivot(ref os4);
        if (os3.seg == Mesh.dummysub)
          o2_1.SegDissolve();
        else
          o2_1.SegBond(ref os3);
        if (os1.seg == Mesh.dummysub)
          o2_2.SegDissolve();
        else
          o2_2.SegBond(ref os1);
        if (os2.seg == Mesh.dummysub)
          o2_4.SegDissolve();
        else
          o2_4.SegBond(ref os2);
        if (os4.seg == Mesh.dummysub)
          o2_3.SegDissolve();
        else
          o2_3.SegBond(ref os4);
      }
      flipedge.SetOrg(ptr3);
      flipedge.SetDest(ptr4);
      flipedge.SetApex(ptr2);
      o2_5.SetOrg(ptr4);
      o2_5.SetDest(ptr3);
      o2_5.SetApex(ptr1);
    }

    private void TriangulatePolygon(
      Otri firstedge,
      Otri lastedge,
      int edgecount,
      bool doflip,
      bool triflaws)
    {
      Otri otri = new Otri();
      Otri firstedge1 = new Otri();
      Otri o2 = new Otri();
      int num = 1;
      Vertex pa = lastedge.Apex();
      Vertex pb = firstedge.Dest();
      firstedge.Onext(ref firstedge1);
      Vertex pc = firstedge1.Dest();
      firstedge1.Copy(ref otri);
      for (int index = 2; index <= edgecount - 2; ++index)
      {
        otri.OnextSelf();
        Vertex pd = otri.Dest();
        if (Primitives.InCircle((Point) pa, (Point) pb, (Point) pc, (Point) pd) > 0.0)
        {
          otri.Copy(ref firstedge1);
          pc = pd;
          num = index;
        }
      }
      if (num > 1)
      {
        firstedge1.Oprev(ref o2);
        this.TriangulatePolygon(firstedge, o2, num + 1, true, triflaws);
      }
      if (num < edgecount - 2)
      {
        firstedge1.Sym(ref o2);
        this.TriangulatePolygon(firstedge1, lastedge, edgecount - num, true, triflaws);
        o2.Sym(ref firstedge1);
      }
      if (doflip)
      {
        this.Flip(ref firstedge1);
        if (triflaws)
        {
          firstedge1.Sym(ref otri);
          this.quality.TestTriangle(ref otri);
        }
      }
      firstedge1.Copy(ref lastedge);
    }

    internal void DeleteVertex(ref Otri deltri)
    {
      Otri o2_1 = new Otri();
      Otri o2_2 = new Otri();
      Otri o2_3 = new Otri();
      Otri o2_4 = new Otri();
      Otri o2_5 = new Otri();
      Otri o2_6 = new Otri();
      Otri o2_7 = new Otri();
      Otri o2_8 = new Otri();
      Osub os1 = new Osub();
      Osub os2 = new Osub();
      this.VertexDealloc(deltri.Org());
      deltri.Onext(ref o2_1);
      int edgecount = 1;
      while (!deltri.Equal(o2_1))
      {
        ++edgecount;
        o2_1.OnextSelf();
      }
      if (edgecount > 3)
      {
        deltri.Onext(ref o2_2);
        deltri.Oprev(ref o2_3);
        this.TriangulatePolygon(o2_2, o2_3, edgecount, false, this.behavior.NoBisect == 0);
      }
      deltri.Lprev(ref o2_4);
      deltri.Dnext(ref o2_5);
      o2_5.Sym(ref o2_7);
      o2_4.Oprev(ref o2_6);
      o2_6.Sym(ref o2_8);
      deltri.Bond(ref o2_7);
      o2_4.Bond(ref o2_8);
      o2_5.SegPivot(ref os1);
      if (os1.seg != Mesh.dummysub)
        deltri.SegBond(ref os1);
      o2_6.SegPivot(ref os2);
      if (os2.seg != Mesh.dummysub)
        o2_4.SegBond(ref os2);
      Vertex ptr = o2_5.Org();
      deltri.SetOrg(ptr);
      if (this.behavior.NoBisect == 0)
        this.quality.TestTriangle(ref deltri);
      this.TriangleDealloc(o2_5.triangle);
      this.TriangleDealloc(o2_6.triangle);
    }

    internal void UndoVertex()
    {
      Otri o2_1 = new Otri();
      Otri o2_2 = new Otri();
      Otri o2_3 = new Otri();
      Otri o2_4 = new Otri();
      Otri o2_5 = new Otri();
      Otri o2_6 = new Otri();
      Otri o2_7 = new Otri();
      Osub os1 = new Osub();
      Osub os2 = new Osub();
      Osub os3 = new Osub();
      while (this.flipstack.Count > 0)
      {
        Otri flipedge = this.flipstack.Pop();
        if (this.flipstack.Count == 0)
        {
          flipedge.Dprev(ref o2_1);
          o2_1.LnextSelf();
          flipedge.Onext(ref o2_2);
          o2_2.LprevSelf();
          o2_1.Sym(ref o2_4);
          o2_2.Sym(ref o2_5);
          Vertex ptr = o2_1.Dest();
          flipedge.SetApex(ptr);
          flipedge.LnextSelf();
          flipedge.Bond(ref o2_4);
          o2_1.SegPivot(ref os1);
          flipedge.SegBond(ref os1);
          flipedge.LnextSelf();
          flipedge.Bond(ref o2_5);
          o2_2.SegPivot(ref os2);
          flipedge.SegBond(ref os2);
          this.TriangleDealloc(o2_1.triangle);
          this.TriangleDealloc(o2_2.triangle);
        }
        else if (this.flipstack.Peek().triangle == null)
        {
          flipedge.Lprev(ref o2_7);
          o2_7.Sym(ref o2_2);
          o2_2.LnextSelf();
          o2_2.Sym(ref o2_5);
          Vertex ptr = o2_2.Dest();
          flipedge.SetOrg(ptr);
          o2_7.Bond(ref o2_5);
          o2_2.SegPivot(ref os2);
          o2_7.SegBond(ref os2);
          this.TriangleDealloc(o2_2.triangle);
          flipedge.Sym(ref o2_7);
          if (o2_7.triangle != Mesh.dummytri)
          {
            o2_7.LnextSelf();
            o2_7.Dnext(ref o2_3);
            o2_3.Sym(ref o2_6);
            o2_7.SetOrg(ptr);
            o2_7.Bond(ref o2_6);
            o2_3.SegPivot(ref os3);
            o2_7.SegBond(ref os3);
            this.TriangleDealloc(o2_3.triangle);
          }
          this.flipstack.Clear();
        }
        else
          this.Unflip(ref flipedge);
      }
    }

    private FindDirectionResult FindDirection(ref Otri searchtri, Vertex searchpoint)
    {
      Otri o2 = new Otri();
      Vertex vertex = searchtri.Org();
      Vertex pc1 = searchtri.Dest();
      Vertex pc2 = searchtri.Apex();
      double num1 = Primitives.CounterClockwise((Point) searchpoint, (Point) vertex, (Point) pc2);
      bool flag1 = num1 > 0.0;
      double num2 = Primitives.CounterClockwise((Point) vertex, (Point) searchpoint, (Point) pc1);
      bool flag2 = num2 > 0.0;
      if (flag1 & flag2)
      {
        searchtri.Onext(ref o2);
        if (o2.triangle == Mesh.dummytri)
          flag1 = false;
        else
          flag2 = false;
      }
      for (; flag1; flag1 = num1 > 0.0)
      {
        searchtri.OnextSelf();
        if (searchtri.triangle == Mesh.dummytri)
        {
          this.logger.Error("Unable to find a triangle on path.", "Mesh.FindDirection().1");
          throw new Exception("Unable to find a triangle on path.");
        }
        Vertex pc3 = searchtri.Apex();
        num2 = num1;
        num1 = Primitives.CounterClockwise((Point) searchpoint, (Point) vertex, (Point) pc3);
      }
      for (; flag2; flag2 = num2 > 0.0)
      {
        searchtri.OprevSelf();
        if (searchtri.triangle == Mesh.dummytri)
        {
          this.logger.Error("Unable to find a triangle on path.", "Mesh.FindDirection().2");
          throw new Exception("Unable to find a triangle on path.");
        }
        Vertex pc4 = searchtri.Dest();
        num1 = num2;
        num2 = Primitives.CounterClockwise((Point) vertex, (Point) searchpoint, (Point) pc4);
      }
      if (num1 == 0.0)
        return FindDirectionResult.Leftcollinear;
      return num2 == 0.0 ? FindDirectionResult.Rightcollinear : FindDirectionResult.Within;
    }

    private void SegmentIntersection(ref Otri splittri, ref Osub splitsubseg, Vertex endpoint2)
    {
      Osub o2 = new Osub();
      Vertex searchpoint = splittri.Apex();
      Vertex vertex1 = splittri.Org();
      Vertex vertex2 = splittri.Dest();
      double num1 = vertex2.x - vertex1.x;
      double num2 = vertex2.y - vertex1.y;
      double num3 = endpoint2.x - searchpoint.x;
      double num4 = endpoint2.y - searchpoint.y;
      double num5 = vertex1.x - endpoint2.x;
      double num6 = vertex1.y - endpoint2.y;
      double num7 = num2 * num3 - num1 * num4;
      if (num7 == 0.0)
      {
        this.logger.Error("Attempt to find intersection of parallel segments.", "Mesh.SegmentIntersection()");
        throw new Exception("Attempt to find intersection of parallel segments.");
      }
      double num8 = (num4 * num5 - num3 * num6) / num7;
      Vertex vertex3 = new Vertex(vertex1.x + num8 * (vertex2.x - vertex1.x), vertex1.y + num8 * (vertex2.y - vertex1.y), splitsubseg.seg.boundary, this.nextras);
      vertex3.hash = this.hash_vtx++;
      vertex3.id = vertex3.hash;
      for (int index = 0; index < this.nextras; ++index)
        vertex3.attributes[index] = vertex1.attributes[index] + num8 * (vertex2.attributes[index] - vertex1.attributes[index]);
      this.vertices.Add(vertex3.hash, vertex3);
      if (this.InsertVertex(vertex3, ref splittri, ref splitsubseg, false, false) != 0)
      {
        this.logger.Error("Failure to split a segment.", "Mesh.SegmentIntersection()");
        throw new Exception("Failure to split a segment.");
      }
      vertex3.tri = splittri;
      if (this.steinerleft > 0)
        --this.steinerleft;
      splitsubseg.SymSelf();
      splitsubseg.Pivot(ref o2);
      splitsubseg.Dissolve();
      o2.Dissolve();
      do
      {
        splitsubseg.SetSegOrg(vertex3);
        splitsubseg.NextSelf();
      }
      while (splitsubseg.seg != Mesh.dummysub);
      do
      {
        o2.SetSegOrg(vertex3);
        o2.NextSelf();
      }
      while (o2.seg != Mesh.dummysub);
      int direction = (int) this.FindDirection(ref splittri, searchpoint);
      Vertex vertex4 = splittri.Dest();
      Vertex vertex5 = splittri.Apex();
      if (vertex5.x == searchpoint.x && vertex5.y == searchpoint.y)
        splittri.OnextSelf();
      else if (vertex4.x != searchpoint.x || vertex4.y != searchpoint.y)
      {
        this.logger.Error("Topological inconsistency after splitting a segment.", "Mesh.SegmentIntersection()");
        throw new Exception("Topological inconsistency after splitting a segment.");
      }
    }

    private bool ScoutSegment(ref Otri searchtri, Vertex endpoint2, int newmark)
    {
      Otri otri = new Otri();
      Osub osub = new Osub();
      FindDirectionResult direction = this.FindDirection(ref searchtri, endpoint2);
      Vertex vertex1 = searchtri.Dest();
      Vertex vertex2 = searchtri.Apex();
      if (vertex2.x == endpoint2.x && vertex2.y == endpoint2.y || vertex1.x == endpoint2.x && vertex1.y == endpoint2.y)
      {
        if (vertex2.x == endpoint2.x && vertex2.y == endpoint2.y)
          searchtri.LprevSelf();
        this.InsertSubseg(ref searchtri, newmark);
        return true;
      }
      if (direction == FindDirectionResult.Leftcollinear)
      {
        searchtri.LprevSelf();
        this.InsertSubseg(ref searchtri, newmark);
        return this.ScoutSegment(ref searchtri, endpoint2, newmark);
      }
      if (direction == FindDirectionResult.Rightcollinear)
      {
        this.InsertSubseg(ref searchtri, newmark);
        searchtri.LnextSelf();
        return this.ScoutSegment(ref searchtri, endpoint2, newmark);
      }
      searchtri.Lnext(ref otri);
      otri.SegPivot(ref osub);
      if (osub.seg == Mesh.dummysub)
        return false;
      this.SegmentIntersection(ref otri, ref osub, endpoint2);
      otri.Copy(ref searchtri);
      this.InsertSubseg(ref searchtri, newmark);
      return this.ScoutSegment(ref searchtri, endpoint2, newmark);
    }

    private void DelaunayFixup(ref Otri fixuptri, bool leftside)
    {
      Otri otri1 = new Otri();
      Otri otri2 = new Otri();
      Osub os = new Osub();
      fixuptri.Lnext(ref otri1);
      otri1.Sym(ref otri2);
      if (otri2.triangle == Mesh.dummytri)
        return;
      otri1.SegPivot(ref os);
      if (os.seg != Mesh.dummysub)
        return;
      Vertex vertex1 = otri1.Apex();
      Vertex vertex2 = otri1.Org();
      Vertex vertex3 = otri1.Dest();
      Vertex vertex4 = otri2.Apex();
      if (leftside)
      {
        if (Primitives.CounterClockwise((Point) vertex1, (Point) vertex2, (Point) vertex4) <= 0.0)
          return;
      }
      else if (Primitives.CounterClockwise((Point) vertex4, (Point) vertex3, (Point) vertex1) <= 0.0)
        return;
      if (Primitives.CounterClockwise((Point) vertex3, (Point) vertex2, (Point) vertex4) > 0.0 && Primitives.InCircle((Point) vertex2, (Point) vertex4, (Point) vertex3, (Point) vertex1) <= 0.0)
        return;
      this.Flip(ref otri1);
      fixuptri.LprevSelf();
      this.DelaunayFixup(ref fixuptri, leftside);
      this.DelaunayFixup(ref otri2, leftside);
    }

    private void ConstrainedEdge(ref Otri starttri, Vertex endpoint2, int newmark)
    {
      Otri otri1 = new Otri();
      Otri otri2 = new Otri();
      Osub osub = new Osub();
      Vertex pa = starttri.Org();
      starttri.Lnext(ref otri1);
      this.Flip(ref otri1);
      bool flag1 = false;
      bool flag2 = false;
      do
      {
        Vertex pc = otri1.Org();
        if (pc.x == endpoint2.x && pc.y == endpoint2.y)
        {
          otri1.Oprev(ref otri2);
          this.DelaunayFixup(ref otri1, false);
          this.DelaunayFixup(ref otri2, true);
          flag2 = true;
        }
        else
        {
          double num = Primitives.CounterClockwise((Point) pa, (Point) endpoint2, (Point) pc);
          if (num == 0.0)
          {
            flag1 = true;
            otri1.Oprev(ref otri2);
            this.DelaunayFixup(ref otri1, false);
            this.DelaunayFixup(ref otri2, true);
            flag2 = true;
          }
          else
          {
            if (num > 0.0)
            {
              otri1.Oprev(ref otri2);
              this.DelaunayFixup(ref otri2, true);
              otri1.LprevSelf();
            }
            else
            {
              this.DelaunayFixup(ref otri1, false);
              otri1.OprevSelf();
            }
            otri1.SegPivot(ref osub);
            if (osub.seg == Mesh.dummysub)
            {
              this.Flip(ref otri1);
            }
            else
            {
              flag1 = true;
              this.SegmentIntersection(ref otri1, ref osub, endpoint2);
              flag2 = true;
            }
          }
        }
      }
      while (!flag2);
      this.InsertSubseg(ref otri1, newmark);
      if (!flag1 || this.ScoutSegment(ref otri1, endpoint2, newmark))
        return;
      this.ConstrainedEdge(ref otri1, endpoint2, newmark);
    }

    private void InsertSegment(Vertex endpoint1, Vertex endpoint2, int newmark)
    {
      Otri otri1 = new Otri();
      Otri otri2 = new Otri();
      Vertex vertex1 = (Vertex) null;
      Otri tri1 = endpoint1.tri;
      if (tri1.triangle != null)
        vertex1 = tri1.Org();
      if ((Point) vertex1 != (Point) endpoint1)
      {
        tri1.triangle = Mesh.dummytri;
        tri1.orient = 0;
        tri1.SymSelf();
        if (this.locator.Locate((Point) endpoint1, ref tri1) != LocateResult.OnVertex)
        {
          this.logger.Error("Unable to locate PSLG vertex in triangulation.", "Mesh.InsertSegment().1");
          throw new Exception("Unable to locate PSLG vertex in triangulation.");
        }
      }
      this.locator.Update(ref tri1);
      if (this.ScoutSegment(ref tri1, endpoint2, newmark))
        return;
      endpoint1 = tri1.Org();
      Vertex vertex2 = (Vertex) null;
      Otri tri2 = endpoint2.tri;
      if (tri2.triangle != null)
        vertex2 = tri2.Org();
      if ((Point) vertex2 != (Point) endpoint2)
      {
        tri2.triangle = Mesh.dummytri;
        tri2.orient = 0;
        tri2.SymSelf();
        if (this.locator.Locate((Point) endpoint2, ref tri2) != LocateResult.OnVertex)
        {
          this.logger.Error("Unable to locate PSLG vertex in triangulation.", "Mesh.InsertSegment().2");
          throw new Exception("Unable to locate PSLG vertex in triangulation.");
        }
      }
      this.locator.Update(ref tri2);
      if (this.ScoutSegment(ref tri2, endpoint1, newmark))
        return;
      endpoint2 = tri2.Org();
      this.ConstrainedEdge(ref tri1, endpoint2, newmark);
    }

    private void MarkHull()
    {
      Otri otri = new Otri();
      Otri o2_1 = new Otri();
      Otri o2_2 = new Otri();
      otri.triangle = Mesh.dummytri;
      otri.orient = 0;
      otri.SymSelf();
      otri.Copy(ref o2_2);
      do
      {
        this.InsertSubseg(ref otri, 1);
        otri.LnextSelf();
        otri.Oprev(ref o2_1);
        while (o2_1.triangle != Mesh.dummytri)
        {
          o2_1.Copy(ref otri);
          otri.Oprev(ref o2_1);
        }
      }
      while (!otri.Equal(o2_2));
    }

    private void FormSkeleton(InputGeometry input)
    {
      this.insegments = 0;
      if (this.behavior.Poly)
      {
        if (this.triangles.Count == 0)
          return;
        if (input.HasSegments)
          this.MakeVertexMap();
        foreach (Edge segment in input.segments)
        {
          ++this.insegments;
          int p0 = segment.P0;
          int p1 = segment.P1;
          int boundary = segment.Boundary;
          if (p0 < 0 || p0 >= this.invertices)
          {
            if (Behavior.Verbose)
              this.logger.Warning("Invalid first endpoint of segment.", "Mesh.FormSkeleton().1");
          }
          else if (p1 < 0 || p1 >= this.invertices)
          {
            if (Behavior.Verbose)
              this.logger.Warning("Invalid second endpoint of segment.", "Mesh.FormSkeleton().2");
          }
          else
          {
            Vertex vertex1 = this.vertices[p0];
            Vertex vertex2 = this.vertices[p1];
            if (vertex1.x == vertex2.x && vertex1.y == vertex2.y)
            {
              if (Behavior.Verbose)
                this.logger.Warning("Endpoints of segments are coincident.", "Mesh.FormSkeleton()");
            }
            else
              this.InsertSegment(vertex1, vertex2, boundary);
          }
        }
      }
      if (!this.behavior.Convex && this.behavior.Poly)
        return;
      this.MarkHull();
    }

    internal void TriangleDealloc(Triangle dyingtriangle)
    {
      Otri.Kill(dyingtriangle);
      this.triangles.Remove(dyingtriangle.hash);
    }

    internal void VertexDealloc(Vertex dyingvertex)
    {
      dyingvertex.type = VertexType.DeadVertex;
      this.vertices.Remove(dyingvertex.hash);
    }

    internal void SubsegDealloc(Segment dyingsubseg)
    {
      Osub.Kill(dyingsubseg);
      this.subsegs.Remove(dyingsubseg.hash);
    }
  }
}
