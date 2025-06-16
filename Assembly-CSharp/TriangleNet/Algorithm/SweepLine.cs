using System;
using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.Log;
using TriangleNet.Tools;

namespace TriangleNet.Algorithm;

internal class SweepLine {
	private static int randomseed = 1;
	private static int SAMPLERATE = 10;
	private Mesh mesh;
	private List<SplayNode> splaynodes;
	private double xminextreme;

	private int randomnation(int choices) {
		randomseed = (randomseed * 1366 + 150889) % 714025;
		return randomseed / (714025 / choices + 1);
	}

	private void HeapInsert(
		SweepEvent[] heap,
		int heapsize,
		SweepEvent newevent) {
		var xkey = newevent.xkey;
		var ykey = newevent.ykey;
		var index1 = heapsize;
		var flag = index1 > 0;
		while (flag) {
			var index2 = (index1 - 1) >> 1;
			if (heap[index2].ykey < ykey || (heap[index2].ykey == ykey && heap[index2].xkey <= xkey))
				flag = false;
			else {
				heap[index1] = heap[index2];
				heap[index1].heapposition = index1;
				index1 = index2;
				flag = index1 > 0;
			}
		}

		heap[index1] = newevent;
		newevent.heapposition = index1;
	}

	private void Heapify(SweepEvent[] heap, int heapsize, int eventnum) {
		var sweepEvent = heap[eventnum];
		var xkey = sweepEvent.xkey;
		var ykey = sweepEvent.ykey;
		var index1 = 2 * eventnum + 1;
		var flag = index1 < heapsize;
		while (flag) {
			var index2 = heap[index1].ykey >= ykey && (heap[index1].ykey != ykey || heap[index1].xkey >= xkey)
				? eventnum
				: index1;
			var index3 = index1 + 1;
			if (index3 < heapsize && (heap[index3].ykey < heap[index2].ykey ||
			                          (heap[index3].ykey == heap[index2].ykey &&
			                           heap[index3].xkey < heap[index2].xkey)))
				index2 = index3;
			if (index2 == eventnum)
				flag = false;
			else {
				heap[eventnum] = heap[index2];
				heap[eventnum].heapposition = eventnum;
				heap[index2] = sweepEvent;
				sweepEvent.heapposition = index2;
				eventnum = index2;
				index1 = 2 * eventnum + 1;
				flag = index1 < heapsize;
			}
		}
	}

	private void HeapDelete(SweepEvent[] heap, int heapsize, int eventnum) {
		var sweepEvent = heap[heapsize - 1];
		if (eventnum > 0) {
			var xkey = sweepEvent.xkey;
			var ykey = sweepEvent.ykey;
			bool flag;
			do {
				var index = (eventnum - 1) >> 1;
				if (heap[index].ykey < ykey || (heap[index].ykey == ykey && heap[index].xkey <= xkey))
					flag = false;
				else {
					heap[eventnum] = heap[index];
					heap[eventnum].heapposition = eventnum;
					eventnum = index;
					flag = eventnum > 0;
				}
			} while (flag);
		}

		heap[eventnum] = sweepEvent;
		sweepEvent.heapposition = eventnum;
		Heapify(heap, heapsize - 1, eventnum);
	}

	private void CreateHeap(out SweepEvent[] eventheap) {
		var length = 3 * mesh.invertices / 2;
		eventheap = new SweepEvent[length];
		var num = 0;
		foreach (var vertex in mesh.vertices.Values)
			HeapInsert(eventheap, num++, new SweepEvent {
				vertexEvent = vertex,
				xkey = vertex.x,
				ykey = vertex.y
			});
	}

	private SplayNode Splay(
		SplayNode splaytree,
		Point searchpoint,
		ref Otri searchtri) {
		if (splaytree == null)
			return null;
		if (splaytree.keyedge.Dest() == splaytree.keydest) {
			var flag1 = RightOfHyperbola(ref splaytree.keyedge, searchpoint);
			SplayNode splaytree1;
			if (flag1) {
				splaytree.keyedge.Copy(ref searchtri);
				splaytree1 = splaytree.rchild;
			} else
				splaytree1 = splaytree.lchild;

			if (splaytree1 == null)
				return splaytree;
			if (splaytree1.keyedge.Dest() != splaytree1.keydest) {
				splaytree1 = Splay(splaytree1, searchpoint, ref searchtri);
				if (splaytree1 == null) {
					if (flag1)
						splaytree.rchild = null;
					else
						splaytree.lchild = null;
					return splaytree;
				}
			}

			var flag2 = RightOfHyperbola(ref splaytree1.keyedge, searchpoint);
			SplayNode splayNode;
			if (flag2) {
				splaytree1.keyedge.Copy(ref searchtri);
				splayNode = Splay(splaytree1.rchild, searchpoint, ref searchtri);
				splaytree1.rchild = splayNode;
			} else {
				splayNode = Splay(splaytree1.lchild, searchpoint, ref searchtri);
				splaytree1.lchild = splayNode;
			}

			if (splayNode == null) {
				if (flag1) {
					splaytree.rchild = splaytree1.lchild;
					splaytree1.lchild = splaytree;
				} else {
					splaytree.lchild = splaytree1.rchild;
					splaytree1.rchild = splaytree;
				}

				return splaytree1;
			}

			if (flag2) {
				if (flag1) {
					splaytree.rchild = splaytree1.lchild;
					splaytree1.lchild = splaytree;
				} else {
					splaytree.lchild = splayNode.rchild;
					splayNode.rchild = splaytree;
				}

				splaytree1.rchild = splayNode.lchild;
				splayNode.lchild = splaytree1;
			} else {
				if (flag1) {
					splaytree.rchild = splayNode.lchild;
					splayNode.lchild = splaytree;
				} else {
					splaytree.lchild = splaytree1.rchild;
					splaytree1.rchild = splaytree;
				}

				splaytree1.lchild = splayNode.rchild;
				splayNode.rchild = splaytree1;
			}

			return splayNode;
		}

		var splayNode1 = Splay(splaytree.lchild, searchpoint, ref searchtri);
		var splayNode2 = Splay(splaytree.rchild, searchpoint, ref searchtri);
		splaynodes.Remove(splaytree);
		if (splayNode1 == null)
			return splayNode2;
		if (splayNode2 == null)
			return splayNode1;
		if (splayNode1.rchild == null) {
			splayNode1.rchild = splayNode2.lchild;
			splayNode2.lchild = splayNode1;
			return splayNode2;
		}

		if (splayNode2.lchild == null) {
			splayNode2.lchild = splayNode1.rchild;
			splayNode1.rchild = splayNode2;
			return splayNode1;
		}

		var rchild = splayNode1.rchild;
		while (rchild.rchild != null)
			rchild = rchild.rchild;
		rchild.rchild = splayNode2;
		return splayNode1;
	}

	private SplayNode SplayInsert(
		SplayNode splayroot,
		Otri newkey,
		Point searchpoint) {
		var splayNode = new SplayNode();
		splaynodes.Add(splayNode);
		newkey.Copy(ref splayNode.keyedge);
		splayNode.keydest = newkey.Dest();
		if (splayroot == null) {
			splayNode.lchild = null;
			splayNode.rchild = null;
		} else if (RightOfHyperbola(ref splayroot.keyedge, searchpoint)) {
			splayNode.lchild = splayroot;
			splayNode.rchild = splayroot.rchild;
			splayroot.rchild = null;
		} else {
			splayNode.lchild = splayroot.lchild;
			splayNode.rchild = splayroot;
			splayroot.lchild = null;
		}

		return splayNode;
	}

	private SplayNode CircleTopInsert(
		SplayNode splayroot,
		Otri newkey,
		Vertex pa,
		Vertex pb,
		Vertex pc,
		double topy) {
		var searchpoint = new Point();
		var searchtri = new Otri();
		var num1 = Primitives.CounterClockwise(pa, pb, pc);
		var num2 = pa.x - pc.x;
		var num3 = pa.y - pc.y;
		var num4 = pb.x - pc.x;
		var num5 = pb.y - pc.y;
		var num6 = num2 * num2 + num3 * num3;
		var num7 = num4 * num4 + num5 * num5;
		searchpoint.x = pc.x - (num3 * num7 - num5 * num6) / (2.0 * num1);
		searchpoint.y = topy;
		return SplayInsert(Splay(splayroot, searchpoint, ref searchtri), newkey, searchpoint);
	}

	private bool RightOfHyperbola(ref Otri fronttri, Point newsite) {
		++Statistic.HyperbolaCount;
		var vertex1 = fronttri.Dest();
		var vertex2 = fronttri.Apex();
		if (vertex1.y < vertex2.y || (vertex1.y == vertex2.y && vertex1.x < vertex2.x)) {
			if (newsite.x >= vertex2.x)
				return true;
		} else if (newsite.x <= vertex1.x)
			return false;

		var num1 = vertex1.x - newsite.x;
		var num2 = vertex1.y - newsite.y;
		var num3 = vertex2.x - newsite.x;
		var num4 = vertex2.y - newsite.y;
		return num2 * (num3 * num3 + num4 * num4) > num4 * (num1 * num1 + num2 * num2);
	}

	private double CircleTop(Vertex pa, Vertex pb, Vertex pc, double ccwabc) {
		++Statistic.CircleTopCount;
		var num1 = pa.x - pc.x;
		var num2 = pa.y - pc.y;
		var num3 = pb.x - pc.x;
		var num4 = pb.y - pc.y;
		var num5 = pa.x - pb.x;
		var num6 = pa.y - pb.y;
		var num7 = num1 * num1 + num2 * num2;
		var num8 = num3 * num3 + num4 * num4;
		var num9 = num5 * num5 + num6 * num6;
		return pc.y + (num1 * num8 - num3 * num7 + Math.Sqrt(num7 * num8 * num9)) / (2.0 * ccwabc);
	}

	private void Check4DeadEvent(
		ref Otri checktri,
		SweepEvent[] eventheap,
		ref int heapsize) {
		var sweepEventVertex = checktri.Org() as SweepEventVertex;
		if (!(sweepEventVertex != null))
			return;
		var heapposition = sweepEventVertex.evt.heapposition;
		HeapDelete(eventheap, heapsize, heapposition);
		--heapsize;
		checktri.SetOrg(null);
	}

	private SplayNode FrontLocate(
		SplayNode splayroot,
		Otri bottommost,
		Vertex searchvertex,
		ref Otri searchtri,
		ref bool farright) {
		bottommost.Copy(ref searchtri);
		splayroot = Splay(splayroot, searchvertex, ref searchtri);
		bool flag;
		for (flag = false; !flag && RightOfHyperbola(ref searchtri, searchvertex); flag = searchtri.Equal(bottommost))
			searchtri.OnextSelf();
		farright = flag;
		return splayroot;
	}

	private int RemoveGhosts(ref Otri startghost) {
		var o2_1 = new Otri();
		var o2_2 = new Otri();
		var o2_3 = new Otri();
		var flag = !mesh.behavior.Poly;
		startghost.Lprev(ref o2_1);
		o2_1.SymSelf();
		Mesh.dummytri.neighbors[0] = o2_1;
		startghost.Copy(ref o2_2);
		var num = 0;
		do {
			++num;
			o2_2.Lnext(ref o2_3);
			o2_2.LprevSelf();
			o2_2.SymSelf();
			if (flag && o2_2.triangle != Mesh.dummytri) {
				var vertex = o2_2.Org();
				if (vertex.mark == 0)
					vertex.mark = 1;
			}

			o2_2.Dissolve();
			o2_3.Sym(ref o2_2);
			mesh.TriangleDealloc(o2_3.triangle);
		} while (!o2_2.Equal(startghost));

		return num;
	}

	public int Triangulate(Mesh mesh) {
		this.mesh = mesh;
		xminextreme = 10.0 * mesh.bounds.Xmin - 9.0 * mesh.bounds.Xmax;
		var otri1 = new Otri();
		var otri2 = new Otri();
		var newkey = new Otri();
		var otri3 = new Otri();
		var otri4 = new Otri();
		var otri5 = new Otri();
		var o2 = new Otri();
		var farright = false;
		splaynodes = new List<SplayNode>();
		SplayNode splayroot = null;
		SweepEvent[] eventheap;
		CreateHeap(out eventheap);
		var invertices = mesh.invertices;
		mesh.MakeTriangle(ref newkey);
		mesh.MakeTriangle(ref otri3);
		newkey.Bond(ref otri3);
		newkey.LnextSelf();
		otri3.LprevSelf();
		newkey.Bond(ref otri3);
		newkey.LnextSelf();
		otri3.LprevSelf();
		newkey.Bond(ref otri3);
		var vertexEvent1 = eventheap[0].vertexEvent;
		HeapDelete(eventheap, invertices, 0);
		var heapsize = invertices - 1;
		while (heapsize != 0) {
			var vertexEvent2 = eventheap[0].vertexEvent;
			HeapDelete(eventheap, heapsize, 0);
			--heapsize;
			if (vertexEvent1.x == vertexEvent2.x && vertexEvent1.y == vertexEvent2.y) {
				if (Behavior.Verbose)
					SimpleLog.Instance.Warning("A duplicate vertex appeared and was ignored.",
						"SweepLine.SweepLineDelaunay().1");
				vertexEvent2.type = VertexType.UndeadVertex;
				++mesh.undeads;
			}

			if (vertexEvent1.x != vertexEvent2.x || vertexEvent1.y != vertexEvent2.y) {
				newkey.SetOrg(vertexEvent1);
				newkey.SetDest(vertexEvent2);
				otri3.SetOrg(vertexEvent2);
				otri3.SetDest(vertexEvent1);
				newkey.Lprev(ref otri1);
				var vertex = vertexEvent2;
				while (heapsize > 0) {
					var sweepEvent1 = eventheap[0];
					HeapDelete(eventheap, heapsize, 0);
					--heapsize;
					var flag = true;
					if (sweepEvent1.xkey < mesh.bounds.Xmin) {
						var otriEvent = sweepEvent1.otriEvent;
						otriEvent.Oprev(ref otri4);
						Check4DeadEvent(ref otri4, eventheap, ref heapsize);
						otriEvent.Onext(ref otri5);
						Check4DeadEvent(ref otri5, eventheap, ref heapsize);
						if (otri4.Equal(otri1))
							otriEvent.Lprev(ref otri1);
						mesh.Flip(ref otriEvent);
						otriEvent.SetApex(null);
						otriEvent.Lprev(ref newkey);
						otriEvent.Lnext(ref otri3);
						newkey.Sym(ref otri4);
						if (randomnation(SAMPLERATE) == 0) {
							otriEvent.SymSelf();
							var pa = otriEvent.Dest();
							var pb = otriEvent.Apex();
							var pc = otriEvent.Org();
							splayroot = CircleTopInsert(splayroot, newkey, pa, pb, pc, sweepEvent1.ykey);
						}
					} else {
						var vertexEvent3 = sweepEvent1.vertexEvent;
						if (vertexEvent3.x == vertex.x && vertexEvent3.y == vertex.y) {
							if (Behavior.Verbose)
								SimpleLog.Instance.Warning("A duplicate vertex appeared and was ignored.",
									"SweepLine.SweepLineDelaunay().2");
							vertexEvent3.type = VertexType.UndeadVertex;
							++mesh.undeads;
							flag = false;
						} else {
							vertex = vertexEvent3;
							splayroot = FrontLocate(splayroot, otri1, vertexEvent3, ref otri2, ref farright);
							otri1.Copy(ref otri2);
							for (farright = false;
							     !farright && RightOfHyperbola(ref otri2, vertexEvent3);
							     farright = otri2.Equal(otri1))
								otri2.OnextSelf();
							Check4DeadEvent(ref otri2, eventheap, ref heapsize);
							otri2.Copy(ref otri5);
							otri2.Sym(ref otri4);
							mesh.MakeTriangle(ref newkey);
							mesh.MakeTriangle(ref otri3);
							var ptr = otri5.Dest();
							newkey.SetOrg(ptr);
							newkey.SetDest(vertexEvent3);
							otri3.SetOrg(vertexEvent3);
							otri3.SetDest(ptr);
							newkey.Bond(ref otri3);
							newkey.LnextSelf();
							otri3.LprevSelf();
							newkey.Bond(ref otri3);
							newkey.LnextSelf();
							otri3.LprevSelf();
							newkey.Bond(ref otri4);
							otri3.Bond(ref otri5);
							if (!farright && otri5.Equal(otri1))
								newkey.Copy(ref otri1);
							if (randomnation(SAMPLERATE) == 0)
								splayroot = SplayInsert(splayroot, newkey, vertexEvent3);
							else if (randomnation(SAMPLERATE) == 0) {
								otri3.Lnext(ref o2);
								splayroot = SplayInsert(splayroot, o2, vertexEvent3);
							}
						}
					}

					if (flag) {
						var pa1 = otri4.Apex();
						var pb1 = newkey.Dest();
						var pc1 = newkey.Apex();
						var ccwabc1 = Primitives.CounterClockwise(pa1, pb1, pc1);
						if (ccwabc1 > 0.0) {
							var sweepEvent2 = new SweepEvent();
							sweepEvent2.xkey = xminextreme;
							sweepEvent2.ykey = CircleTop(pa1, pb1, pc1, ccwabc1);
							sweepEvent2.otriEvent = newkey;
							HeapInsert(eventheap, heapsize, sweepEvent2);
							++heapsize;
							newkey.SetOrg(new SweepEventVertex(sweepEvent2));
						}

						var pa2 = otri3.Apex();
						var pb2 = otri3.Org();
						var pc2 = otri5.Apex();
						var ccwabc2 = Primitives.CounterClockwise(pa2, pb2, pc2);
						if (ccwabc2 > 0.0) {
							var sweepEvent3 = new SweepEvent();
							sweepEvent3.xkey = xminextreme;
							sweepEvent3.ykey = CircleTop(pa2, pb2, pc2, ccwabc2);
							sweepEvent3.otriEvent = otri5;
							HeapInsert(eventheap, heapsize, sweepEvent3);
							++heapsize;
							otri5.SetOrg(new SweepEventVertex(sweepEvent3));
						}
					}
				}

				splaynodes.Clear();
				otri1.LprevSelf();
				return RemoveGhosts(ref otri1);
			}
		}

		SimpleLog.Instance.Error("Input vertices are all identical.", "SweepLine.SweepLineDelaunay()");
		throw new Exception("Input vertices are all identical.");
	}

	private class SweepEvent {
		public int heapposition;
		public Otri otriEvent;
		public Vertex vertexEvent;
		public double xkey;
		public double ykey;
	}

	private class SweepEventVertex : Vertex {
		public SweepEvent evt;

		public SweepEventVertex(SweepEvent e) {
			evt = e;
		}
	}

	private class SplayNode {
		public Vertex keydest;
		public Otri keyedge;
		public SplayNode lchild;
		public SplayNode rchild;
	}
}