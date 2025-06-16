using System;
using System.Collections.Generic;
using TriangleNet.Algorithm;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.IO;
using TriangleNet.Log;
using TriangleNet.Smoothing;
using TriangleNet.Tools;

namespace TriangleNet;

public class Mesh {
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

	public Behavior Behavior => behavior;

	public BoundingBox Bounds => bounds;

	public ICollection<Vertex> Vertices => vertices.Values;

	public IList<Point> Holes => holes;

	public ICollection<Triangle> Triangles => triangles.Values;

	public ICollection<Segment> Segments => subsegs.Values;

	public IEnumerable<Edge> Edges {
		get {
			var e = new EdgeEnumerator(this);
			while (e.MoveNext())
				yield return e.Current;
		}
	}

	public int NumberOfInputPoints => invertices;

	public int NumberOfEdges => edges;

	public bool IsPolygon => insegments > 0;

	public NodeNumbering CurrentNumbering => numbering;

	public Mesh()
		: this(new Behavior()) { }

	public Mesh(Behavior behavior) {
		this.behavior = behavior;
		logger = SimpleLog.Instance;
		behavior = new Behavior();
		vertices = new Dictionary<int, Vertex>();
		triangles = new Dictionary<int, Triangle>();
		subsegs = new Dictionary<int, Segment>();
		flipstack = new Stack<Otri>();
		holes = new List<Point>();
		regions = new List<RegionPointer>();
		quality = new Quality(this);
		locator = new TriangleLocator(this);
		Primitives.ExactInit();
		if (dummytri != null)
			return;
		DummyInit();
	}

	public void Load(string filename) {
		InputGeometry geometry;
		List<ITriangle> triangles;
		FileReader.Read(filename, out geometry, out triangles);
		if (geometry == null || triangles == null)
			return;
		Load(geometry, triangles);
	}

	public void Load(InputGeometry input, List<ITriangle> triangles) {
		if (input == null || triangles == null)
			throw new ArgumentException("Invalid input (argument is null).");
		ResetData();
		if (input.HasSegments) {
			behavior.Poly = true;
			holes.AddRange(input.Holes);
		}

		if (!behavior.Poly) {
			behavior.VarArea = false;
			behavior.useRegions = false;
		}

		behavior.useRegions = input.Regions.Count > 0;
		TransferNodes(input);
		hullsize = DataReader.Reconstruct(this, input, triangles.ToArray());
		edges = (3 * triangles.Count + hullsize) / 2;
	}

	public void Triangulate(string inputFile) {
		Triangulate(FileReader.Read(inputFile));
	}

	public void Triangulate(InputGeometry input) {
		ResetData();
		behavior.Poly = input.HasSegments;
		if (!behavior.Poly) {
			behavior.VarArea = false;
			behavior.useRegions = false;
		}

		behavior.useRegions = input.Regions.Count > 0;
		steinerleft = behavior.SteinerPoints;
		TransferNodes(input);
		hullsize = Delaunay();
		infvertex1 = null;
		infvertex2 = null;
		infvertex3 = null;
		if (behavior.useSegments) {
			checksegments = true;
			FormSkeleton(input);
		}

		if (behavior.Poly && triangles.Count > 0) {
			foreach (var hole in input.holes)
				holes.Add(hole);
			foreach (var region in input.regions)
				regions.Add(region);
			new Carver(this).CarveHoles();
		} else {
			holes.Clear();
			regions.Clear();
		}

		if (behavior.Quality && triangles.Count > 0)
			quality.EnforceQuality();
		edges = (3 * triangles.Count + hullsize) / 2;
	}

	public void Refine(bool halfArea) {
		if (halfArea) {
			var num1 = 0.0;
			foreach (var triangle in triangles.Values) {
				var num2 = Math.Abs(
					(triangle.vertices[2].x - triangle.vertices[0].x) *
					(triangle.vertices[1].y - triangle.vertices[0].y) -
					(triangle.vertices[1].x - triangle.vertices[0].x) *
					(triangle.vertices[2].y - triangle.vertices[0].y)) / 2.0;
				if (num2 > num1)
					num1 = num2;
			}

			Refine(num1 / 2.0);
		} else
			Refine();
	}

	public void Refine(double areaConstraint) {
		behavior.fixedArea = true;
		behavior.MaxArea = areaConstraint;
		Refine();
		behavior.fixedArea = false;
		behavior.MaxArea = -1.0;
	}

	public void Refine() {
		inelements = triangles.Count;
		invertices = vertices.Count;
		if (behavior.Poly)
			insegments = !behavior.useSegments ? hullsize : subsegs.Count;
		Reset();
		steinerleft = behavior.SteinerPoints;
		infvertex1 = null;
		infvertex2 = null;
		infvertex3 = null;
		if (behavior.useSegments)
			checksegments = true;
		if (triangles.Count > 0)
			quality.EnforceQuality();
		edges = (3 * triangles.Count + hullsize) / 2;
	}

	public void Smooth() {
		numbering = NodeNumbering.None;
		new SimpleSmoother(this).Smooth();
	}

	public void Renumber() {
		Renumber(NodeNumbering.Linear);
	}

	public void Renumber(NodeNumbering num) {
		if (num == numbering)
			return;
		switch (num) {
			case NodeNumbering.Linear:
				var num1 = 0;
				using (var enumerator = vertices.Values.GetEnumerator()) {
					while (enumerator.MoveNext())
						enumerator.Current.id = num1++;
					break;
				}
			case NodeNumbering.CuthillMcKee:
				var numArray = new CuthillMcKee().Renumber(this);
				using (var enumerator = vertices.Values.GetEnumerator()) {
					while (enumerator.MoveNext()) {
						var current = enumerator.Current;
						current.id = numArray[current.id];
					}

					break;
				}
		}

		numbering = num;
		var num2 = 0;
		foreach (var triangle in triangles.Values)
			triangle.id = num2++;
	}

	public void Check(out bool isConsistent, out bool isDelaunay) {
		isConsistent = quality.CheckMesh();
		isDelaunay = quality.CheckDelaunay();
	}

	private int Delaunay() {
		var num = behavior.Algorithm != TriangulationAlgorithm.Dwyer
			? behavior.Algorithm != TriangulationAlgorithm.SweepLine
				? new Incremental().Triangulate(this)
				: new SweepLine().Triangulate(this)
			: new Dwyer().Triangulate(this);
		return triangles.Count == 0 ? 0 : num;
	}

	private void ResetData() {
		vertices.Clear();
		triangles.Clear();
		subsegs.Clear();
		holes.Clear();
		regions.Clear();
		hash_vtx = 0;
		hash_seg = 0;
		hash_tri = 0;
		flipstack.Clear();
		hullsize = 0;
		edges = 0;
		Reset();
		locator.Reset();
	}

	private void Reset() {
		numbering = NodeNumbering.None;
		undeads = 0;
		checksegments = false;
		checkquality = false;
		Statistic.InCircleCount = 0L;
		Statistic.CounterClockwiseCount = 0L;
		Statistic.InCircleCountDecimal = 0L;
		Statistic.CounterClockwiseCountDecimal = 0L;
		Statistic.Orient3dCount = 0L;
		Statistic.HyperbolaCount = 0L;
		Statistic.CircleTopCount = 0L;
		Statistic.CircumcenterCount = 0L;
	}

	private void DummyInit() {
		dummytri = new Triangle();
		dummytri.hash = -1;
		dummytri.id = -1;
		dummytri.neighbors[0].triangle = dummytri;
		dummytri.neighbors[1].triangle = dummytri;
		dummytri.neighbors[2].triangle = dummytri;
		if (!behavior.useSegments)
			return;
		dummysub = new Segment();
		dummysub.hash = -1;
		dummysub.subsegs[0].seg = dummysub;
		dummysub.subsegs[1].seg = dummysub;
		dummytri.subsegs[0].seg = dummysub;
		dummytri.subsegs[1].seg = dummysub;
		dummytri.subsegs[2].seg = dummysub;
	}

	private void TransferNodes(InputGeometry data) {
		var points = data.points;
		invertices = points.Count;
		mesh_dim = 2;
		if (invertices < 3) {
			logger.Error("Input must have at least three input vertices.", "MeshReader.TransferNodes()");
			throw new Exception("Input must have at least three input vertices.");
		}

		nextras = points[0].attributes == null ? 0 : points[0].attributes.Length;
		foreach (var vertex in points) {
			vertex.hash = hash_vtx++;
			vertex.id = vertex.hash;
			vertices.Add(vertex.hash, vertex);
		}

		bounds = data.Bounds;
	}

	internal void MakeVertexMap() {
		var otri = new Otri();
		foreach (var triangle in triangles.Values) {
			otri.triangle = triangle;
			for (otri.orient = 0; otri.orient < 3; ++otri.orient)
				otri.Org().tri = otri;
		}
	}

	internal void MakeTriangle(ref Otri newotri) {
		var triangle = new Triangle {
			hash = hash_tri++
		};
		triangle.id = triangle.hash;
		newotri.triangle = triangle;
		newotri.orient = 0;
		triangles.Add(triangle.hash, triangle);
	}

	internal void MakeSegment(ref Osub newsubseg) {
		var segment = new Segment();
		segment.hash = hash_seg++;
		newsubseg.seg = segment;
		newsubseg.orient = 0;
		subsegs.Add(segment.hash, segment);
	}

	internal InsertVertexResult InsertVertex(
		Vertex newvertex,
		ref Otri searchtri,
		ref Osub splitseg,
		bool segmentflaws,
		bool triflaws) {
		var otri1 = new Otri();
		var o2_1 = new Otri();
		var o2_2 = new Otri();
		var o2_3 = new Otri();
		var o2_4 = new Otri();
		var o2_5 = new Otri();
		var otri2 = new Otri();
		var otri3 = new Otri();
		var newotri = new Otri();
		var o2_6 = new Otri();
		var o2_7 = new Otri();
		var o2_8 = new Otri();
		var o2_9 = new Otri();
		var o2_10 = new Otri();
		var os1 = new Osub();
		var os2 = new Osub();
		var os3 = new Osub();
		var os4 = new Osub();
		var os5 = new Osub();
		var osub1 = new Osub();
		var o2_11 = new Osub();
		var osub2 = new Osub();
		LocateResult locateResult;
		if (splitseg.seg == null) {
			if (searchtri.triangle == dummytri) {
				otri1.triangle = dummytri;
				otri1.orient = 0;
				otri1.SymSelf();
				locateResult = locator.Locate(newvertex, ref otri1);
			} else {
				searchtri.Copy(ref otri1);
				locateResult = locator.PreciseLocate(newvertex, ref otri1, true);
			}
		} else {
			searchtri.Copy(ref otri1);
			locateResult = LocateResult.OnEdge;
		}

		int num1;
		switch (locateResult) {
			case LocateResult.OnEdge:
				num1 = 1;
				break;
			case LocateResult.OnVertex:
				otri1.Copy(ref searchtri);
				locator.Update(ref otri1);
				return InsertVertexResult.Duplicate;
			default:
				num1 = locateResult == LocateResult.Outside ? 1 : 0;
				break;
		}

		if (num1 != 0) {
			if (checksegments && splitseg.seg == null) {
				otri1.SegPivot(ref os5);
				if (os5.seg != dummysub) {
					if (segmentflaws) {
						var flag = behavior.NoBisect != 2;
						if (flag && behavior.NoBisect == 1) {
							otri1.Sym(ref o2_10);
							flag = o2_10.triangle != dummytri;
						}

						if (flag)
							quality.AddBadSubseg(new BadSubseg {
								encsubseg = os5,
								subsegorg = os5.Org(),
								subsegdest = os5.Dest()
							});
					}

					otri1.Copy(ref searchtri);
					locator.Update(ref otri1);
					return InsertVertexResult.Violating;
				}
			}

			otri1.Lprev(ref o2_3);
			o2_3.Sym(ref o2_7);
			otri1.Sym(ref o2_5);
			var flag1 = o2_5.triangle != dummytri;
			if (flag1) {
				o2_5.LnextSelf();
				o2_5.Sym(ref o2_9);
				MakeTriangle(ref newotri);
			} else
				++hullsize;

			MakeTriangle(ref otri3);
			var ptr1 = otri1.Org();
			otri1.Dest();
			var ptr2 = otri1.Apex();
			otri3.SetOrg(ptr2);
			otri3.SetDest(ptr1);
			otri3.SetApex(newvertex);
			otri1.SetOrg(newvertex);
			otri3.triangle.region = o2_3.triangle.region;
			if (behavior.VarArea)
				otri3.triangle.area = o2_3.triangle.area;
			if (flag1) {
				var ptr3 = o2_5.Dest();
				newotri.SetOrg(ptr1);
				newotri.SetDest(ptr3);
				newotri.SetApex(newvertex);
				o2_5.SetOrg(newvertex);
				newotri.triangle.region = o2_5.triangle.region;
				if (behavior.VarArea)
					newotri.triangle.area = o2_5.triangle.area;
			}

			if (checksegments) {
				o2_3.SegPivot(ref os2);
				if (os2.seg != dummysub) {
					o2_3.SegDissolve();
					otri3.SegBond(ref os2);
				}

				if (flag1) {
					o2_5.SegPivot(ref os4);
					if (os4.seg != dummysub) {
						o2_5.SegDissolve();
						newotri.SegBond(ref os4);
					}
				}
			}

			otri3.Bond(ref o2_7);
			otri3.LprevSelf();
			otri3.Bond(ref o2_3);
			otri3.LprevSelf();
			if (flag1) {
				newotri.Bond(ref o2_9);
				newotri.LnextSelf();
				newotri.Bond(ref o2_5);
				newotri.LnextSelf();
				newotri.Bond(ref otri3);
			}

			if (splitseg.seg != null) {
				splitseg.SetDest(newvertex);
				var ptr4 = splitseg.SegOrg();
				var ptr5 = splitseg.SegDest();
				splitseg.SymSelf();
				splitseg.Pivot(ref o2_11);
				InsertSubseg(ref otri3, splitseg.seg.boundary);
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

			if (checkquality) {
				flipstack.Clear();
				flipstack.Push(new Otri());
				flipstack.Push(otri1);
			}

			otri1.LnextSelf();
		} else {
			otri1.Lnext(ref o2_2);
			otri1.Lprev(ref o2_3);
			o2_2.Sym(ref o2_6);
			o2_3.Sym(ref o2_7);
			MakeTriangle(ref otri2);
			MakeTriangle(ref otri3);
			var ptr6 = otri1.Org();
			var ptr7 = otri1.Dest();
			var ptr8 = otri1.Apex();
			otri2.SetOrg(ptr7);
			otri2.SetDest(ptr8);
			otri2.SetApex(newvertex);
			otri3.SetOrg(ptr8);
			otri3.SetDest(ptr6);
			otri3.SetApex(newvertex);
			otri1.SetApex(newvertex);
			otri2.triangle.region = otri1.triangle.region;
			otri3.triangle.region = otri1.triangle.region;
			if (behavior.VarArea) {
				var area = otri1.triangle.area;
				otri2.triangle.area = area;
				otri3.triangle.area = area;
			}

			if (checksegments) {
				o2_2.SegPivot(ref os1);
				if (os1.seg != dummysub) {
					o2_2.SegDissolve();
					otri2.SegBond(ref os1);
				}

				o2_3.SegPivot(ref os2);
				if (os2.seg != dummysub) {
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
			if (checkquality) {
				flipstack.Clear();
				flipstack.Push(otri1);
			}
		}

		var insertVertexResult = InsertVertexResult.Successful;
		var vertex1 = otri1.Org();
		var vertex2 = vertex1;
		var vertex3 = otri1.Dest();
		while (true) {
			var flag = true;
			if (checksegments) {
				otri1.SegPivot(ref osub1);
				if (osub1.seg != dummysub) {
					flag = false;
					if (segmentflaws && quality.CheckSeg4Encroach(ref osub1) > 0)
						insertVertexResult = InsertVertexResult.Encroaching;
				}
			}

			if (flag) {
				otri1.Sym(ref o2_1);
				if (o2_1.triangle == dummytri)
					flag = false;
				else {
					var vertex4 = o2_1.Apex();
					flag = !(vertex3 == infvertex1) && !(vertex3 == infvertex2) && !(vertex3 == infvertex3)
						? !(vertex2 == infvertex1) && !(vertex2 == infvertex2) && !(vertex2 == infvertex3)
							? !(vertex4 == infvertex1) && !(vertex4 == infvertex2) && !(vertex4 == infvertex3) &&
							  Primitives.InCircle(vertex3, newvertex, vertex2, vertex4) > 0.0
							: Primitives.CounterClockwise(vertex4, vertex3, newvertex) > 0.0
						: Primitives.CounterClockwise(newvertex, vertex2, vertex4) > 0.0;
					if (flag) {
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
						if (checksegments) {
							o2_4.SegPivot(ref os3);
							o2_2.SegPivot(ref os1);
							o2_3.SegPivot(ref os2);
							o2_5.SegPivot(ref os4);
							if (os3.seg == dummysub)
								o2_5.SegDissolve();
							else
								o2_5.SegBond(ref os3);
							if (os1.seg == dummysub)
								o2_4.SegDissolve();
							else
								o2_4.SegBond(ref os1);
							if (os2.seg == dummysub)
								o2_2.SegDissolve();
							else
								o2_2.SegBond(ref os2);
							if (os4.seg == dummysub)
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
						var num2 = Math.Min(o2_1.triangle.region, otri1.triangle.region);
						o2_1.triangle.region = num2;
						otri1.triangle.region = num2;
						if (behavior.VarArea) {
							var num3 = o2_1.triangle.area > 0.0 && otri1.triangle.area > 0.0
								? 0.5 * (o2_1.triangle.area + otri1.triangle.area)
								: -1.0;
							o2_1.triangle.area = num3;
							otri1.triangle.area = num3;
						}

						if (checkquality)
							flipstack.Push(otri1);
						otri1.LprevSelf();
						vertex3 = vertex4;
					}
				}
			}

			if (!flag) {
				if (triflaws)
					quality.TestTriangle(ref otri1);
				otri1.LnextSelf();
				otri1.Sym(ref o2_10);
				if (!(vertex3 == vertex1) && o2_10.triangle != dummytri) {
					o2_10.Lnext(ref otri1);
					vertex2 = vertex3;
					vertex3 = otri1.Dest();
				} else
					break;
			}
		}

		otri1.Lnext(ref searchtri);
		var otri4 = new Otri();
		otri1.Lnext(ref otri4);
		locator.Update(ref otri4);
		return insertVertexResult;
	}

	internal void InsertSubseg(ref Otri tri, int subsegmark) {
		var o2 = new Otri();
		var osub = new Osub();
		var ptr1 = tri.Org();
		var ptr2 = tri.Dest();
		if (ptr1.mark == 0)
			ptr1.mark = subsegmark;
		if (ptr2.mark == 0)
			ptr2.mark = subsegmark;
		tri.SegPivot(ref osub);
		if (osub.seg == dummysub) {
			MakeSegment(ref osub);
			osub.SetOrg(ptr2);
			osub.SetDest(ptr1);
			osub.SetSegOrg(ptr2);
			osub.SetSegDest(ptr1);
			tri.SegBond(ref osub);
			tri.Sym(ref o2);
			osub.SymSelf();
			o2.SegBond(ref osub);
			osub.seg.boundary = subsegmark;
		} else if (osub.seg.boundary == 0)
			osub.seg.boundary = subsegmark;
	}

	internal void Flip(ref Otri flipedge) {
		var o2_1 = new Otri();
		var o2_2 = new Otri();
		var o2_3 = new Otri();
		var o2_4 = new Otri();
		var o2_5 = new Otri();
		var o2_6 = new Otri();
		var o2_7 = new Otri();
		var o2_8 = new Otri();
		var o2_9 = new Otri();
		var os1 = new Osub();
		var os2 = new Osub();
		var os3 = new Osub();
		var os4 = new Osub();
		var ptr1 = flipedge.Org();
		var ptr2 = flipedge.Dest();
		var ptr3 = flipedge.Apex();
		flipedge.Sym(ref o2_5);
		var ptr4 = o2_5.Apex();
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
		if (checksegments) {
			o2_3.SegPivot(ref os3);
			o2_1.SegPivot(ref os1);
			o2_2.SegPivot(ref os2);
			o2_4.SegPivot(ref os4);
			if (os3.seg == dummysub)
				o2_4.SegDissolve();
			else
				o2_4.SegBond(ref os3);
			if (os1.seg == dummysub)
				o2_3.SegDissolve();
			else
				o2_3.SegBond(ref os1);
			if (os2.seg == dummysub)
				o2_1.SegDissolve();
			else
				o2_1.SegBond(ref os2);
			if (os4.seg == dummysub)
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

	internal void Unflip(ref Otri flipedge) {
		var o2_1 = new Otri();
		var o2_2 = new Otri();
		var o2_3 = new Otri();
		var o2_4 = new Otri();
		var o2_5 = new Otri();
		var o2_6 = new Otri();
		var o2_7 = new Otri();
		var o2_8 = new Otri();
		var o2_9 = new Otri();
		var os1 = new Osub();
		var os2 = new Osub();
		var os3 = new Osub();
		var os4 = new Osub();
		var ptr1 = flipedge.Org();
		var ptr2 = flipedge.Dest();
		var ptr3 = flipedge.Apex();
		flipedge.Sym(ref o2_5);
		var ptr4 = o2_5.Apex();
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
		if (checksegments) {
			o2_3.SegPivot(ref os3);
			o2_1.SegPivot(ref os1);
			o2_2.SegPivot(ref os2);
			o2_4.SegPivot(ref os4);
			if (os3.seg == dummysub)
				o2_1.SegDissolve();
			else
				o2_1.SegBond(ref os3);
			if (os1.seg == dummysub)
				o2_2.SegDissolve();
			else
				o2_2.SegBond(ref os1);
			if (os2.seg == dummysub)
				o2_4.SegDissolve();
			else
				o2_4.SegBond(ref os2);
			if (os4.seg == dummysub)
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
		bool triflaws) {
		var otri = new Otri();
		var firstedge1 = new Otri();
		var o2 = new Otri();
		var num = 1;
		var pa = lastedge.Apex();
		var pb = firstedge.Dest();
		firstedge.Onext(ref firstedge1);
		var pc = firstedge1.Dest();
		firstedge1.Copy(ref otri);
		for (var index = 2; index <= edgecount - 2; ++index) {
			otri.OnextSelf();
			var pd = otri.Dest();
			if (Primitives.InCircle(pa, pb, pc, pd) > 0.0) {
				otri.Copy(ref firstedge1);
				pc = pd;
				num = index;
			}
		}

		if (num > 1) {
			firstedge1.Oprev(ref o2);
			TriangulatePolygon(firstedge, o2, num + 1, true, triflaws);
		}

		if (num < edgecount - 2) {
			firstedge1.Sym(ref o2);
			TriangulatePolygon(firstedge1, lastedge, edgecount - num, true, triflaws);
			o2.Sym(ref firstedge1);
		}

		if (doflip) {
			Flip(ref firstedge1);
			if (triflaws) {
				firstedge1.Sym(ref otri);
				quality.TestTriangle(ref otri);
			}
		}

		firstedge1.Copy(ref lastedge);
	}

	internal void DeleteVertex(ref Otri deltri) {
		var o2_1 = new Otri();
		var o2_2 = new Otri();
		var o2_3 = new Otri();
		var o2_4 = new Otri();
		var o2_5 = new Otri();
		var o2_6 = new Otri();
		var o2_7 = new Otri();
		var o2_8 = new Otri();
		var os1 = new Osub();
		var os2 = new Osub();
		VertexDealloc(deltri.Org());
		deltri.Onext(ref o2_1);
		var edgecount = 1;
		while (!deltri.Equal(o2_1)) {
			++edgecount;
			o2_1.OnextSelf();
		}

		if (edgecount > 3) {
			deltri.Onext(ref o2_2);
			deltri.Oprev(ref o2_3);
			TriangulatePolygon(o2_2, o2_3, edgecount, false, behavior.NoBisect == 0);
		}

		deltri.Lprev(ref o2_4);
		deltri.Dnext(ref o2_5);
		o2_5.Sym(ref o2_7);
		o2_4.Oprev(ref o2_6);
		o2_6.Sym(ref o2_8);
		deltri.Bond(ref o2_7);
		o2_4.Bond(ref o2_8);
		o2_5.SegPivot(ref os1);
		if (os1.seg != dummysub)
			deltri.SegBond(ref os1);
		o2_6.SegPivot(ref os2);
		if (os2.seg != dummysub)
			o2_4.SegBond(ref os2);
		var ptr = o2_5.Org();
		deltri.SetOrg(ptr);
		if (behavior.NoBisect == 0)
			quality.TestTriangle(ref deltri);
		TriangleDealloc(o2_5.triangle);
		TriangleDealloc(o2_6.triangle);
	}

	internal void UndoVertex() {
		var o2_1 = new Otri();
		var o2_2 = new Otri();
		var o2_3 = new Otri();
		var o2_4 = new Otri();
		var o2_5 = new Otri();
		var o2_6 = new Otri();
		var o2_7 = new Otri();
		var os1 = new Osub();
		var os2 = new Osub();
		var os3 = new Osub();
		while (flipstack.Count > 0) {
			var flipedge = flipstack.Pop();
			if (flipstack.Count == 0) {
				flipedge.Dprev(ref o2_1);
				o2_1.LnextSelf();
				flipedge.Onext(ref o2_2);
				o2_2.LprevSelf();
				o2_1.Sym(ref o2_4);
				o2_2.Sym(ref o2_5);
				var ptr = o2_1.Dest();
				flipedge.SetApex(ptr);
				flipedge.LnextSelf();
				flipedge.Bond(ref o2_4);
				o2_1.SegPivot(ref os1);
				flipedge.SegBond(ref os1);
				flipedge.LnextSelf();
				flipedge.Bond(ref o2_5);
				o2_2.SegPivot(ref os2);
				flipedge.SegBond(ref os2);
				TriangleDealloc(o2_1.triangle);
				TriangleDealloc(o2_2.triangle);
			} else if (flipstack.Peek().triangle == null) {
				flipedge.Lprev(ref o2_7);
				o2_7.Sym(ref o2_2);
				o2_2.LnextSelf();
				o2_2.Sym(ref o2_5);
				var ptr = o2_2.Dest();
				flipedge.SetOrg(ptr);
				o2_7.Bond(ref o2_5);
				o2_2.SegPivot(ref os2);
				o2_7.SegBond(ref os2);
				TriangleDealloc(o2_2.triangle);
				flipedge.Sym(ref o2_7);
				if (o2_7.triangle != dummytri) {
					o2_7.LnextSelf();
					o2_7.Dnext(ref o2_3);
					o2_3.Sym(ref o2_6);
					o2_7.SetOrg(ptr);
					o2_7.Bond(ref o2_6);
					o2_3.SegPivot(ref os3);
					o2_7.SegBond(ref os3);
					TriangleDealloc(o2_3.triangle);
				}

				flipstack.Clear();
			} else
				Unflip(ref flipedge);
		}
	}

	private FindDirectionResult FindDirection(ref Otri searchtri, Vertex searchpoint) {
		var o2 = new Otri();
		var vertex = searchtri.Org();
		var pc1 = searchtri.Dest();
		var pc2 = searchtri.Apex();
		var num1 = Primitives.CounterClockwise(searchpoint, vertex, pc2);
		var flag1 = num1 > 0.0;
		var num2 = Primitives.CounterClockwise(vertex, searchpoint, pc1);
		var flag2 = num2 > 0.0;
		if (flag1 & flag2) {
			searchtri.Onext(ref o2);
			if (o2.triangle == dummytri)
				flag1 = false;
			else
				flag2 = false;
		}

		for (; flag1; flag1 = num1 > 0.0) {
			searchtri.OnextSelf();
			if (searchtri.triangle == dummytri) {
				logger.Error("Unable to find a triangle on path.", "Mesh.FindDirection().1");
				throw new Exception("Unable to find a triangle on path.");
			}

			var pc3 = searchtri.Apex();
			num2 = num1;
			num1 = Primitives.CounterClockwise(searchpoint, vertex, pc3);
		}

		for (; flag2; flag2 = num2 > 0.0) {
			searchtri.OprevSelf();
			if (searchtri.triangle == dummytri) {
				logger.Error("Unable to find a triangle on path.", "Mesh.FindDirection().2");
				throw new Exception("Unable to find a triangle on path.");
			}

			var pc4 = searchtri.Dest();
			num1 = num2;
			num2 = Primitives.CounterClockwise(vertex, searchpoint, pc4);
		}

		if (num1 == 0.0)
			return FindDirectionResult.Leftcollinear;
		return num2 == 0.0 ? FindDirectionResult.Rightcollinear : FindDirectionResult.Within;
	}

	private void SegmentIntersection(ref Otri splittri, ref Osub splitsubseg, Vertex endpoint2) {
		var o2 = new Osub();
		var searchpoint = splittri.Apex();
		var vertex1 = splittri.Org();
		var vertex2 = splittri.Dest();
		var num1 = vertex2.x - vertex1.x;
		var num2 = vertex2.y - vertex1.y;
		var num3 = endpoint2.x - searchpoint.x;
		var num4 = endpoint2.y - searchpoint.y;
		var num5 = vertex1.x - endpoint2.x;
		var num6 = vertex1.y - endpoint2.y;
		var num7 = num2 * num3 - num1 * num4;
		if (num7 == 0.0) {
			logger.Error("Attempt to find intersection of parallel segments.", "Mesh.SegmentIntersection()");
			throw new Exception("Attempt to find intersection of parallel segments.");
		}

		var num8 = (num4 * num5 - num3 * num6) / num7;
		var vertex3 = new Vertex(vertex1.x + num8 * (vertex2.x - vertex1.x), vertex1.y + num8 * (vertex2.y - vertex1.y),
			splitsubseg.seg.boundary, nextras);
		vertex3.hash = hash_vtx++;
		vertex3.id = vertex3.hash;
		for (var index = 0; index < nextras; ++index)
			vertex3.attributes[index] = vertex1.attributes[index] +
			                            num8 * (vertex2.attributes[index] - vertex1.attributes[index]);
		vertices.Add(vertex3.hash, vertex3);
		if (InsertVertex(vertex3, ref splittri, ref splitsubseg, false, false) != 0) {
			logger.Error("Failure to split a segment.", "Mesh.SegmentIntersection()");
			throw new Exception("Failure to split a segment.");
		}

		vertex3.tri = splittri;
		if (steinerleft > 0)
			--steinerleft;
		splitsubseg.SymSelf();
		splitsubseg.Pivot(ref o2);
		splitsubseg.Dissolve();
		o2.Dissolve();
		do {
			splitsubseg.SetSegOrg(vertex3);
			splitsubseg.NextSelf();
		} while (splitsubseg.seg != dummysub);

		do {
			o2.SetSegOrg(vertex3);
			o2.NextSelf();
		} while (o2.seg != dummysub);

		var direction = (int)FindDirection(ref splittri, searchpoint);
		var vertex4 = splittri.Dest();
		var vertex5 = splittri.Apex();
		if (vertex5.x == searchpoint.x && vertex5.y == searchpoint.y)
			splittri.OnextSelf();
		else if (vertex4.x != searchpoint.x || vertex4.y != searchpoint.y) {
			logger.Error("Topological inconsistency after splitting a segment.", "Mesh.SegmentIntersection()");
			throw new Exception("Topological inconsistency after splitting a segment.");
		}
	}

	private bool ScoutSegment(ref Otri searchtri, Vertex endpoint2, int newmark) {
		var otri = new Otri();
		var osub = new Osub();
		var direction = FindDirection(ref searchtri, endpoint2);
		var vertex1 = searchtri.Dest();
		var vertex2 = searchtri.Apex();
		if ((vertex2.x == endpoint2.x && vertex2.y == endpoint2.y) ||
		    (vertex1.x == endpoint2.x && vertex1.y == endpoint2.y)) {
			if (vertex2.x == endpoint2.x && vertex2.y == endpoint2.y)
				searchtri.LprevSelf();
			InsertSubseg(ref searchtri, newmark);
			return true;
		}

		if (direction == FindDirectionResult.Leftcollinear) {
			searchtri.LprevSelf();
			InsertSubseg(ref searchtri, newmark);
			return ScoutSegment(ref searchtri, endpoint2, newmark);
		}

		if (direction == FindDirectionResult.Rightcollinear) {
			InsertSubseg(ref searchtri, newmark);
			searchtri.LnextSelf();
			return ScoutSegment(ref searchtri, endpoint2, newmark);
		}

		searchtri.Lnext(ref otri);
		otri.SegPivot(ref osub);
		if (osub.seg == dummysub)
			return false;
		SegmentIntersection(ref otri, ref osub, endpoint2);
		otri.Copy(ref searchtri);
		InsertSubseg(ref searchtri, newmark);
		return ScoutSegment(ref searchtri, endpoint2, newmark);
	}

	private void DelaunayFixup(ref Otri fixuptri, bool leftside) {
		var otri1 = new Otri();
		var otri2 = new Otri();
		var os = new Osub();
		fixuptri.Lnext(ref otri1);
		otri1.Sym(ref otri2);
		if (otri2.triangle == dummytri)
			return;
		otri1.SegPivot(ref os);
		if (os.seg != dummysub)
			return;
		var vertex1 = otri1.Apex();
		var vertex2 = otri1.Org();
		var vertex3 = otri1.Dest();
		var vertex4 = otri2.Apex();
		if (leftside) {
			if (Primitives.CounterClockwise(vertex1, vertex2, vertex4) <= 0.0)
				return;
		} else if (Primitives.CounterClockwise(vertex4, vertex3, vertex1) <= 0.0)
			return;

		if (Primitives.CounterClockwise(vertex3, vertex2, vertex4) > 0.0 &&
		    Primitives.InCircle(vertex2, vertex4, vertex3, vertex1) <= 0.0)
			return;
		Flip(ref otri1);
		fixuptri.LprevSelf();
		DelaunayFixup(ref fixuptri, leftside);
		DelaunayFixup(ref otri2, leftside);
	}

	private void ConstrainedEdge(ref Otri starttri, Vertex endpoint2, int newmark) {
		var otri1 = new Otri();
		var otri2 = new Otri();
		var osub = new Osub();
		var pa = starttri.Org();
		starttri.Lnext(ref otri1);
		Flip(ref otri1);
		var flag1 = false;
		var flag2 = false;
		do {
			var pc = otri1.Org();
			if (pc.x == endpoint2.x && pc.y == endpoint2.y) {
				otri1.Oprev(ref otri2);
				DelaunayFixup(ref otri1, false);
				DelaunayFixup(ref otri2, true);
				flag2 = true;
			} else {
				var num = Primitives.CounterClockwise(pa, endpoint2, pc);
				if (num == 0.0) {
					flag1 = true;
					otri1.Oprev(ref otri2);
					DelaunayFixup(ref otri1, false);
					DelaunayFixup(ref otri2, true);
					flag2 = true;
				} else {
					if (num > 0.0) {
						otri1.Oprev(ref otri2);
						DelaunayFixup(ref otri2, true);
						otri1.LprevSelf();
					} else {
						DelaunayFixup(ref otri1, false);
						otri1.OprevSelf();
					}

					otri1.SegPivot(ref osub);
					if (osub.seg == dummysub)
						Flip(ref otri1);
					else {
						flag1 = true;
						SegmentIntersection(ref otri1, ref osub, endpoint2);
						flag2 = true;
					}
				}
			}
		} while (!flag2);

		InsertSubseg(ref otri1, newmark);
		if (!flag1 || ScoutSegment(ref otri1, endpoint2, newmark))
			return;
		ConstrainedEdge(ref otri1, endpoint2, newmark);
	}

	private void InsertSegment(Vertex endpoint1, Vertex endpoint2, int newmark) {
		var otri1 = new Otri();
		var otri2 = new Otri();
		Vertex vertex1 = null;
		var tri1 = endpoint1.tri;
		if (tri1.triangle != null)
			vertex1 = tri1.Org();
		if (vertex1 != endpoint1) {
			tri1.triangle = dummytri;
			tri1.orient = 0;
			tri1.SymSelf();
			if (locator.Locate(endpoint1, ref tri1) != LocateResult.OnVertex) {
				logger.Error("Unable to locate PSLG vertex in triangulation.", "Mesh.InsertSegment().1");
				throw new Exception("Unable to locate PSLG vertex in triangulation.");
			}
		}

		locator.Update(ref tri1);
		if (ScoutSegment(ref tri1, endpoint2, newmark))
			return;
		endpoint1 = tri1.Org();
		Vertex vertex2 = null;
		var tri2 = endpoint2.tri;
		if (tri2.triangle != null)
			vertex2 = tri2.Org();
		if (vertex2 != endpoint2) {
			tri2.triangle = dummytri;
			tri2.orient = 0;
			tri2.SymSelf();
			if (locator.Locate(endpoint2, ref tri2) != LocateResult.OnVertex) {
				logger.Error("Unable to locate PSLG vertex in triangulation.", "Mesh.InsertSegment().2");
				throw new Exception("Unable to locate PSLG vertex in triangulation.");
			}
		}

		locator.Update(ref tri2);
		if (ScoutSegment(ref tri2, endpoint1, newmark))
			return;
		endpoint2 = tri2.Org();
		ConstrainedEdge(ref tri1, endpoint2, newmark);
	}

	private void MarkHull() {
		var otri = new Otri();
		var o2_1 = new Otri();
		var o2_2 = new Otri();
		otri.triangle = dummytri;
		otri.orient = 0;
		otri.SymSelf();
		otri.Copy(ref o2_2);
		do {
			InsertSubseg(ref otri, 1);
			otri.LnextSelf();
			otri.Oprev(ref o2_1);
			while (o2_1.triangle != dummytri) {
				o2_1.Copy(ref otri);
				otri.Oprev(ref o2_1);
			}
		} while (!otri.Equal(o2_2));
	}

	private void FormSkeleton(InputGeometry input) {
		insegments = 0;
		if (behavior.Poly) {
			if (triangles.Count == 0)
				return;
			if (input.HasSegments)
				MakeVertexMap();
			foreach (var segment in input.segments) {
				++insegments;
				var p0 = segment.P0;
				var p1 = segment.P1;
				var boundary = segment.Boundary;
				if (p0 < 0 || p0 >= invertices) {
					if (Behavior.Verbose)
						logger.Warning("Invalid first endpoint of segment.", "Mesh.FormSkeleton().1");
				} else if (p1 < 0 || p1 >= invertices) {
					if (Behavior.Verbose)
						logger.Warning("Invalid second endpoint of segment.", "Mesh.FormSkeleton().2");
				} else {
					var vertex1 = vertices[p0];
					var vertex2 = vertices[p1];
					if (vertex1.x == vertex2.x && vertex1.y == vertex2.y) {
						if (Behavior.Verbose)
							logger.Warning("Endpoints of segments are coincident.", "Mesh.FormSkeleton()");
					} else
						InsertSegment(vertex1, vertex2, boundary);
				}
			}
		}

		if (!behavior.Convex && behavior.Poly)
			return;
		MarkHull();
	}

	internal void TriangleDealloc(Triangle dyingtriangle) {
		Otri.Kill(dyingtriangle);
		triangles.Remove(dyingtriangle.hash);
	}

	internal void VertexDealloc(Vertex dyingvertex) {
		dyingvertex.type = VertexType.DeadVertex;
		vertices.Remove(dyingvertex.hash);
	}

	internal void SubsegDealloc(Segment dyingsubseg) {
		Osub.Kill(dyingsubseg);
		subsegs.Remove(dyingsubseg.hash);
	}
}