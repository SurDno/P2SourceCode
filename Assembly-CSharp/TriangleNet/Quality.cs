using System;
using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.Log;

namespace TriangleNet;

internal class Quality {
	private Queue<BadSubseg> badsubsegs;
	private Behavior behavior;
	private ILog<SimpleLogItem> logger;
	private Mesh mesh;
	private NewLocation newLocation;
	private BadTriQueue queue;
	private Func<Point, Point, Point, double, bool> userTest;

	public Quality(Mesh mesh) {
		logger = SimpleLog.Instance;
		badsubsegs = new Queue<BadSubseg>();
		queue = new BadTriQueue();
		this.mesh = mesh;
		behavior = mesh.behavior;
		newLocation = new NewLocation(mesh);
	}

	public void AddBadSubseg(BadSubseg badseg) {
		badsubsegs.Enqueue(badseg);
	}

	public bool CheckMesh() {
		var otri = new Otri();
		var o2_1 = new Otri();
		var o2_2 = new Otri();
		var noExact = Behavior.NoExact;
		Behavior.NoExact = false;
		var num = 0;
		foreach (var triangle in mesh.triangles.Values) {
			otri.triangle = triangle;
			for (otri.orient = 0; otri.orient < 3; ++otri.orient) {
				var pa = otri.Org();
				var pb = otri.Dest();
				if (otri.orient == 0) {
					var pc = otri.Apex();
					if (Primitives.CounterClockwise(pa, pb, pc) <= 0.0) {
						logger.Warning("Triangle is flat or inverted.", "Quality.CheckMesh()");
						++num;
					}
				}

				otri.Sym(ref o2_1);
				if (o2_1.triangle != Mesh.dummytri) {
					o2_1.Sym(ref o2_2);
					if (otri.triangle != o2_2.triangle || otri.orient != o2_2.orient) {
						if (otri.triangle == o2_2.triangle)
							logger.Warning("Asymmetric triangle-triangle bond: (Right triangle, wrong orientation)",
								"Quality.CheckMesh()");
						++num;
					}

					var vertex1 = o2_1.Org();
					var vertex2 = o2_1.Dest();
					if (pa != vertex2 || pb != vertex1) {
						logger.Warning("Mismatched edge coordinates between two triangles.", "Quality.CheckMesh()");
						++num;
					}
				}
			}
		}

		mesh.MakeVertexMap();
		foreach (var vertex in mesh.vertices.Values)
			if (vertex.tri.triangle == null)
				logger.Warning("Vertex (ID " + vertex.id + ") not connected to mesh (duplicate input vertex?)",
					"Quality.CheckMesh()");
		if (num == 0)
			logger.Info("Mesh topology appears to be consistent.");
		Behavior.NoExact = noExact;
		return num == 0;
	}

	public bool CheckDelaunay() {
		var otri = new Otri();
		var o2 = new Otri();
		var os = new Osub();
		var noExact = Behavior.NoExact;
		Behavior.NoExact = false;
		var num = 0;
		foreach (var triangle in mesh.triangles.Values) {
			otri.triangle = triangle;
			for (otri.orient = 0; otri.orient < 3; ++otri.orient) {
				var pa = otri.Org();
				var pb = otri.Dest();
				var pc = otri.Apex();
				otri.Sym(ref o2);
				var pd = o2.Apex();
				var flag = o2.triangle != Mesh.dummytri && !Otri.IsDead(o2.triangle) &&
				           otri.triangle.id < o2.triangle.id && pa != mesh.infvertex1 && pa != mesh.infvertex2 &&
				           pa != mesh.infvertex3 && pb != mesh.infvertex1 && pb != mesh.infvertex2 &&
				           pb != mesh.infvertex3 && pc != mesh.infvertex1 && pc != mesh.infvertex2 &&
				           pc != mesh.infvertex3 && pd != mesh.infvertex1 && pd != mesh.infvertex2 &&
				           pd != mesh.infvertex3;
				if (mesh.checksegments & flag) {
					otri.SegPivot(ref os);
					if (os.seg != Mesh.dummysub)
						flag = false;
				}

				if (flag && Primitives.NonRegular(pa, pb, pc, pd) > 0.0) {
					logger.Warning(
						string.Format("Non-regular pair of triangles found (IDs {0}/{1}).", otri.triangle.id,
							o2.triangle.id), "Quality.CheckDelaunay()");
					++num;
				}
			}
		}

		if (num == 0)
			logger.Info("Mesh is Delaunay.");
		Behavior.NoExact = noExact;
		return num == 0;
	}

	public int CheckSeg4Encroach(ref Osub testsubseg) {
		var ot = new Otri();
		var o2 = new Osub();
		var num1 = 0;
		var num2 = 0;
		var vertex1 = testsubseg.Org();
		var vertex2 = testsubseg.Dest();
		testsubseg.TriPivot(ref ot);
		if (ot.triangle != Mesh.dummytri) {
			++num2;
			var vertex3 = ot.Apex();
			var num3 = (vertex1.x - vertex3.x) * (vertex2.x - vertex3.x) +
			           (vertex1.y - vertex3.y) * (vertex2.y - vertex3.y);
			if (num3 < 0.0 && (behavior.ConformingDelaunay || num3 * num3 >= (2.0 * behavior.goodAngle - 1.0) *
				    (2.0 * behavior.goodAngle - 1.0) *
				    ((vertex1.x - vertex3.x) * (vertex1.x - vertex3.x) +
				     (vertex1.y - vertex3.y) * (vertex1.y - vertex3.y)) *
				    ((vertex2.x - vertex3.x) * (vertex2.x - vertex3.x) +
				     (vertex2.y - vertex3.y) * (vertex2.y - vertex3.y))))
				num1 = 1;
		}

		testsubseg.Sym(ref o2);
		o2.TriPivot(ref ot);
		if (ot.triangle != Mesh.dummytri) {
			++num2;
			var vertex4 = ot.Apex();
			var num4 = (vertex1.x - vertex4.x) * (vertex2.x - vertex4.x) +
			           (vertex1.y - vertex4.y) * (vertex2.y - vertex4.y);
			if (num4 < 0.0 && (behavior.ConformingDelaunay || num4 * num4 >= (2.0 * behavior.goodAngle - 1.0) *
				    (2.0 * behavior.goodAngle - 1.0) *
				    ((vertex1.x - vertex4.x) * (vertex1.x - vertex4.x) +
				     (vertex1.y - vertex4.y) * (vertex1.y - vertex4.y)) *
				    ((vertex2.x - vertex4.x) * (vertex2.x - vertex4.x) +
				     (vertex2.y - vertex4.y) * (vertex2.y - vertex4.y))))
				num1 += 2;
		}

		if (num1 > 0 && (behavior.NoBisect == 0 || (behavior.NoBisect == 1 && num2 == 2))) {
			var badSubseg = new BadSubseg();
			if (num1 == 1) {
				badSubseg.encsubseg = testsubseg;
				badSubseg.subsegorg = vertex1;
				badSubseg.subsegdest = vertex2;
			} else {
				badSubseg.encsubseg = o2;
				badSubseg.subsegorg = vertex2;
				badSubseg.subsegdest = vertex1;
			}

			badsubsegs.Enqueue(badSubseg);
		}

		return num1;
	}

	public void TestTriangle(ref Otri testtri) {
		var o2_1 = new Otri();
		var o2_2 = new Otri();
		var os = new Osub();
		var enqorg = testtri.Org();
		var enqdest = testtri.Dest();
		var enqapex = testtri.Apex();
		var num1 = enqorg.x - enqdest.x;
		var num2 = enqorg.y - enqdest.y;
		var num3 = enqdest.x - enqapex.x;
		var num4 = enqdest.y - enqapex.y;
		var num5 = enqapex.x - enqorg.x;
		var num6 = enqapex.y - enqorg.y;
		var num7 = num1 * num1;
		var num8 = num2 * num2;
		var num9 = num3 * num3;
		var num10 = num4 * num4;
		var num11 = num5 * num5;
		var num12 = num6 * num6;
		var num13 = num7 + num8;
		var num14 = num9 + num10;
		var num15 = num11 + num12;
		double minedge;
		double num16;
		Vertex vertex1;
		Vertex vertex2;
		if (num13 < num14 && num13 < num15) {
			minedge = num13;
			var num17 = num3 * num5 + num4 * num6;
			num16 = num17 * num17 / (num14 * num15);
			vertex1 = enqorg;
			vertex2 = enqdest;
			testtri.Copy(ref o2_1);
		} else if (num14 < num15) {
			minedge = num14;
			var num18 = num1 * num5 + num2 * num6;
			num16 = num18 * num18 / (num13 * num15);
			vertex1 = enqdest;
			vertex2 = enqapex;
			testtri.Lnext(ref o2_1);
		} else {
			minedge = num15;
			var num19 = num1 * num3 + num2 * num4;
			num16 = num19 * num19 / (num13 * num14);
			vertex1 = enqapex;
			vertex2 = enqorg;
			testtri.Lprev(ref o2_1);
		}

		if (behavior.VarArea || behavior.fixedArea || behavior.Usertest) {
			var num20 = 0.5 * (num1 * num4 - num2 * num3);
			if (behavior.fixedArea && num20 > behavior.MaxArea) {
				queue.Enqueue(ref testtri, minedge, enqapex, enqorg, enqdest);
				return;
			}

			if (behavior.VarArea && num20 > testtri.triangle.area && testtri.triangle.area > 0.0) {
				queue.Enqueue(ref testtri, minedge, enqapex, enqorg, enqdest);
				return;
			}

			if (behavior.Usertest && userTest != null && userTest(enqorg, enqdest, enqapex, num20)) {
				queue.Enqueue(ref testtri, minedge, enqapex, enqorg, enqdest);
				return;
			}
		}

		var num21 = num13 <= num14 || num13 <= num15
			? num14 <= num15
				? (num13 + num14 - num15) / (2.0 * Math.Sqrt(num13 * num14))
				: (num13 + num15 - num14) / (2.0 * Math.Sqrt(num13 * num15))
			: (num14 + num15 - num13) / (2.0 * Math.Sqrt(num14 * num15));
		if (num16 <= behavior.goodAngle && (num21 >= behavior.maxGoodAngle || behavior.MaxAngle == 0.0))
			return;
		if (vertex1.type == VertexType.SegmentVertex && vertex2.type == VertexType.SegmentVertex) {
			o2_1.SegPivot(ref os);
			if (os.seg == Mesh.dummysub) {
				o2_1.Copy(ref o2_2);
				do {
					o2_1.OprevSelf();
					o2_1.SegPivot(ref os);
				} while (os.seg == Mesh.dummysub);

				var vertex3 = os.SegOrg();
				var vertex4 = os.SegDest();
				do {
					o2_2.DnextSelf();
					o2_2.SegPivot(ref os);
				} while (os.seg == Mesh.dummysub);

				var vertex5 = os.SegOrg();
				var vertex6 = os.SegDest();
				Vertex vertex7 = null;
				if (vertex4.x == vertex5.x && vertex4.y == vertex5.y)
					vertex7 = vertex4;
				else if (vertex3.x == vertex6.x && vertex3.y == vertex6.y)
					vertex7 = vertex3;
				if (vertex7 != null) {
					var num22 = (vertex1.x - vertex7.x) * (vertex1.x - vertex7.x) +
					            (vertex1.y - vertex7.y) * (vertex1.y - vertex7.y);
					var num23 = (vertex2.x - vertex7.x) * (vertex2.x - vertex7.x) +
					            (vertex2.y - vertex7.y) * (vertex2.y - vertex7.y);
					if (num22 < 1001.0 / 1000.0 * num23 && num22 > 0.999 * num23)
						return;
				}
			}
		}

		queue.Enqueue(ref testtri, minedge, enqapex, enqorg, enqdest);
	}

	private void TallyEncs() {
		var testsubseg = new Osub();
		testsubseg.orient = 0;
		foreach (var segment in mesh.subsegs.Values) {
			testsubseg.seg = segment;
			CheckSeg4Encroach(ref testsubseg);
		}
	}

	private void SplitEncSegs(bool triflaws) {
		var otri1 = new Otri();
		var otri2 = new Otri();
		var os = new Osub();
		var osub = new Osub();
		while (badsubsegs.Count > 0 && mesh.steinerleft != 0) {
			var badSubseg = badsubsegs.Dequeue();
			var encsubseg = badSubseg.encsubseg;
			var pa = encsubseg.Org();
			var pb = encsubseg.Dest();
			if (!Osub.IsDead(encsubseg.seg) && pa == badSubseg.subsegorg && pb == badSubseg.subsegdest) {
				encsubseg.TriPivot(ref otri1);
				otri1.Lnext(ref otri2);
				otri2.SegPivot(ref os);
				var flag1 = os.seg != Mesh.dummysub;
				otri2.LnextSelf();
				otri2.SegPivot(ref os);
				var flag2 = os.seg != Mesh.dummysub;
				if (!behavior.ConformingDelaunay && !flag1 && !flag2) {
					var vertex = otri1.Apex();
					while (vertex.type == VertexType.FreeVertex &&
					       (pa.x - vertex.x) * (pb.x - vertex.x) + (pa.y - vertex.y) * (pb.y - vertex.y) < 0.0) {
						mesh.DeleteVertex(ref otri2);
						encsubseg.TriPivot(ref otri1);
						vertex = otri1.Apex();
						otri1.Lprev(ref otri2);
					}
				}

				otri1.Sym(ref otri2);
				if (otri2.triangle != Mesh.dummytri) {
					otri2.LnextSelf();
					otri2.SegPivot(ref os);
					var flag3 = os.seg != Mesh.dummysub;
					flag2 |= flag3;
					otri2.LnextSelf();
					otri2.SegPivot(ref os);
					var flag4 = os.seg != Mesh.dummysub;
					flag1 |= flag4;
					if (!behavior.ConformingDelaunay && !flag4 && !flag3) {
						var vertex = otri2.Org();
						while (vertex.type == VertexType.FreeVertex &&
						       (pa.x - vertex.x) * (pb.x - vertex.x) + (pa.y - vertex.y) * (pb.y - vertex.y) < 0.0) {
							mesh.DeleteVertex(ref otri2);
							otri1.Sym(ref otri2);
							vertex = otri2.Apex();
							otri2.LprevSelf();
						}
					}
				}

				double num1;
				if (flag1 | flag2) {
					var num2 = Math.Sqrt((pb.x - pa.x) * (pb.x - pa.x) + (pb.y - pa.y) * (pb.y - pa.y));
					var num3 = 1.0;
					while (num2 > 3.0 * num3)
						num3 *= 2.0;
					while (num2 < 1.5 * num3)
						num3 *= 0.5;
					num1 = num3 / num2;
					if (flag2)
						num1 = 1.0 - num1;
				} else
					num1 = 0.5;

				var vertex1 = new Vertex(pa.x + num1 * (pb.x - pa.x), pa.y + num1 * (pb.y - pa.y), encsubseg.Mark(),
					mesh.nextras);
				vertex1.type = VertexType.SegmentVertex;
				vertex1.hash = mesh.hash_vtx++;
				vertex1.id = vertex1.hash;
				mesh.vertices.Add(vertex1.hash, vertex1);
				for (var index = 0; index < mesh.nextras; ++index)
					vertex1.attributes[index] =
						pa.attributes[index] + num1 * (pb.attributes[index] - pa.attributes[index]);
				if (!Behavior.NoExact) {
					var num4 = Primitives.CounterClockwise(pa, pb, vertex1);
					var num5 = (pa.x - pb.x) * (pa.x - pb.x) + (pa.y - pb.y) * (pa.y - pb.y);
					if (num4 != 0.0 && num5 != 0.0) {
						var d = num4 / num5;
						if (!double.IsNaN(d)) {
							vertex1.x += d * (pb.y - pa.y);
							vertex1.y += d * (pa.x - pb.x);
						}
					}
				}

				if ((vertex1.x == pa.x && vertex1.y == pa.y) || (vertex1.x == pb.x && vertex1.y == pb.y)) {
					logger.Error(
						"Ran out of precision: I attempted to split a segment to a smaller size than can be accommodated by the finite precision of floating point arithmetic.",
						"Quality.SplitEncSegs()");
					throw new Exception("Ran out of precision");
				}

				var insertVertexResult = mesh.InsertVertex(vertex1, ref otri1, ref encsubseg, true, triflaws);
				if (insertVertexResult != InsertVertexResult.Successful &&
				    insertVertexResult != InsertVertexResult.Encroaching) {
					logger.Error("Failure to split a segment.", "Quality.SplitEncSegs()");
					throw new Exception("Failure to split a segment.");
				}

				if (mesh.steinerleft > 0)
					--mesh.steinerleft;
				CheckSeg4Encroach(ref encsubseg);
				encsubseg.NextSelf();
				CheckSeg4Encroach(ref encsubseg);
			}

			badSubseg.subsegorg = null;
		}
	}

	private void TallyFaces() {
		var testtri = new Otri();
		testtri.orient = 0;
		foreach (var triangle in mesh.triangles.Values) {
			testtri.triangle = triangle;
			TestTriangle(ref testtri);
		}
	}

	private void SplitTriangle(BadTriangle badtri) {
		var otri = new Otri();
		var xi = 0.0;
		var eta = 0.0;
		var poortri = badtri.poortri;
		var torg = poortri.Org();
		var tdest = poortri.Dest();
		var tapex = poortri.Apex();
		if (Otri.IsDead(poortri.triangle) || !(torg == badtri.triangorg) || !(tdest == badtri.triangdest) ||
		    !(tapex == badtri.triangapex))
			return;
		var flag = false;
		var point = !behavior.fixedArea && !behavior.VarArea
			? newLocation.FindLocation(torg, tdest, tapex, ref xi, ref eta, true, poortri)
			: Primitives.FindCircumcenter(torg, tdest, tapex, ref xi, ref eta, behavior.offconstant);
		if ((point.x == torg.x && point.y == torg.y) || (point.x == tdest.x && point.y == tdest.y) ||
		    (point.x == tapex.x && point.y == tapex.y)) {
			if (Behavior.Verbose) {
				logger.Warning("New vertex falls on existing vertex.", "Quality.SplitTriangle()");
				flag = true;
			}
		} else {
			var newvertex = new Vertex(point.x, point.y, 0, mesh.nextras);
			newvertex.type = VertexType.FreeVertex;
			for (var index = 0; index < mesh.nextras; ++index)
				newvertex.attributes[index] = torg.attributes[index] +
				                              xi * (tdest.attributes[index] - torg.attributes[index]) +
				                              eta * (tapex.attributes[index] - torg.attributes[index]);
			if (eta < xi)
				poortri.LprevSelf();
			var splitseg = new Osub();
			switch (mesh.InsertVertex(newvertex, ref poortri, ref splitseg, true, true)) {
				case InsertVertexResult.Successful:
					newvertex.hash = mesh.hash_vtx++;
					newvertex.id = newvertex.hash;
					mesh.vertices.Add(newvertex.hash, newvertex);
					if (mesh.steinerleft > 0) {
						--mesh.steinerleft;
						goto case InsertVertexResult.Violating;
					}

					goto case InsertVertexResult.Violating;
				case InsertVertexResult.Encroaching:
					mesh.UndoVertex();
					goto case InsertVertexResult.Violating;
				case InsertVertexResult.Violating:
					break;
				default:
					if (Behavior.Verbose) {
						logger.Warning("New vertex falls on existing vertex.", "Quality.SplitTriangle()");
						flag = true;
					}

					goto case InsertVertexResult.Violating;
			}
		}

		if (flag) {
			logger.Error(
				"The new vertex is at the circumcenter of triangle: This probably means that I am trying to refine triangles to a smaller size than can be accommodated by the finite precision of floating point arithmetic.",
				"Quality.SplitTriangle()");
			throw new Exception("The new vertex is at the circumcenter of triangle.");
		}
	}

	public void EnforceQuality() {
		TallyEncs();
		SplitEncSegs(false);
		if (behavior.MinAngle > 0.0 || behavior.VarArea || behavior.fixedArea || behavior.Usertest) {
			TallyFaces();
			mesh.checkquality = true;
			while (queue.Count > 0 && mesh.steinerleft != 0) {
				var badtri = queue.Dequeue();
				SplitTriangle(badtri);
				if (badsubsegs.Count > 0) {
					queue.Enqueue(badtri);
					SplitEncSegs(true);
				}
			}
		}

		if (!Behavior.Verbose || !behavior.ConformingDelaunay || badsubsegs.Count <= 0 || mesh.steinerleft != 0)
			return;
		logger.Warning(
			"I ran out of Steiner points, but the mesh has encroached subsegments, and therefore might not be truly Delaunay. If the Delaunay property is important to you, try increasing the number of Steiner points.",
			"Quality.EnforceQuality()");
	}
}