using System;
using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.Log;
using TriangleNet.Tools;

namespace TriangleNet.Algorithm
{
  internal class SweepLine
  {
    private static int randomseed = 1;
    private static int SAMPLERATE = 10;
    private Mesh mesh;
    private List<SplayNode> splaynodes;
    private double xminextreme;

    private int randomnation(int choices)
    {
      randomseed = (randomseed * 1366 + 150889) % 714025;
      return randomseed / (714025 / choices + 1);
    }

    private void HeapInsert(
      SweepEvent[] heap,
      int heapsize,
      SweepEvent newevent)
    {
      double xkey = newevent.xkey;
      double ykey = newevent.ykey;
      int index1 = heapsize;
      bool flag = index1 > 0;
      while (flag)
      {
        int index2 = index1 - 1 >> 1;
        if (heap[index2].ykey < ykey || heap[index2].ykey == ykey && heap[index2].xkey <= xkey)
        {
          flag = false;
        }
        else
        {
          heap[index1] = heap[index2];
          heap[index1].heapposition = index1;
          index1 = index2;
          flag = index1 > 0;
        }
      }
      heap[index1] = newevent;
      newevent.heapposition = index1;
    }

    private void Heapify(SweepEvent[] heap, int heapsize, int eventnum)
    {
      SweepEvent sweepEvent = heap[eventnum];
      double xkey = sweepEvent.xkey;
      double ykey = sweepEvent.ykey;
      int index1 = 2 * eventnum + 1;
      bool flag = index1 < heapsize;
      while (flag)
      {
        int index2 = heap[index1].ykey >= ykey && (heap[index1].ykey != ykey || heap[index1].xkey >= xkey) ? eventnum : index1;
        int index3 = index1 + 1;
        if (index3 < heapsize && (heap[index3].ykey < heap[index2].ykey || heap[index3].ykey == heap[index2].ykey && heap[index3].xkey < heap[index2].xkey))
          index2 = index3;
        if (index2 == eventnum)
        {
          flag = false;
        }
        else
        {
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

    private void HeapDelete(SweepEvent[] heap, int heapsize, int eventnum)
    {
      SweepEvent sweepEvent = heap[heapsize - 1];
      if (eventnum > 0)
      {
        double xkey = sweepEvent.xkey;
        double ykey = sweepEvent.ykey;
        bool flag;
        do
        {
          int index = eventnum - 1 >> 1;
          if (heap[index].ykey < ykey || heap[index].ykey == ykey && heap[index].xkey <= xkey)
          {
            flag = false;
          }
          else
          {
            heap[eventnum] = heap[index];
            heap[eventnum].heapposition = eventnum;
            eventnum = index;
            flag = eventnum > 0;
          }
        }
        while (flag);
      }
      heap[eventnum] = sweepEvent;
      sweepEvent.heapposition = eventnum;
      Heapify(heap, heapsize - 1, eventnum);
    }

    private void CreateHeap(out SweepEvent[] eventheap)
    {
      int length = 3 * mesh.invertices / 2;
      eventheap = new SweepEvent[length];
      int num = 0;
      foreach (Vertex vertex in mesh.vertices.Values)
        HeapInsert(eventheap, num++, new SweepEvent {
          vertexEvent = vertex,
          xkey = vertex.x,
          ykey = vertex.y
        });
    }

    private SplayNode Splay(
      SplayNode splaytree,
      Point searchpoint,
      ref Otri searchtri)
    {
      if (splaytree == null)
        return null;
      if (splaytree.keyedge.Dest() == splaytree.keydest)
      {
        bool flag1 = RightOfHyperbola(ref splaytree.keyedge, searchpoint);
        SplayNode splaytree1;
        if (flag1)
        {
          splaytree.keyedge.Copy(ref searchtri);
          splaytree1 = splaytree.rchild;
        }
        else
          splaytree1 = splaytree.lchild;
        if (splaytree1 == null)
          return splaytree;
        if (splaytree1.keyedge.Dest() != splaytree1.keydest)
        {
          splaytree1 = Splay(splaytree1, searchpoint, ref searchtri);
          if (splaytree1 == null)
          {
            if (flag1)
              splaytree.rchild = null;
            else
              splaytree.lchild = null;
            return splaytree;
          }
        }
        bool flag2 = RightOfHyperbola(ref splaytree1.keyedge, searchpoint);
        SplayNode splayNode;
        if (flag2)
        {
          splaytree1.keyedge.Copy(ref searchtri);
          splayNode = Splay(splaytree1.rchild, searchpoint, ref searchtri);
          splaytree1.rchild = splayNode;
        }
        else
        {
          splayNode = Splay(splaytree1.lchild, searchpoint, ref searchtri);
          splaytree1.lchild = splayNode;
        }
        if (splayNode == null)
        {
          if (flag1)
          {
            splaytree.rchild = splaytree1.lchild;
            splaytree1.lchild = splaytree;
          }
          else
          {
            splaytree.lchild = splaytree1.rchild;
            splaytree1.rchild = splaytree;
          }
          return splaytree1;
        }
        if (flag2)
        {
          if (flag1)
          {
            splaytree.rchild = splaytree1.lchild;
            splaytree1.lchild = splaytree;
          }
          else
          {
            splaytree.lchild = splayNode.rchild;
            splayNode.rchild = splaytree;
          }
          splaytree1.rchild = splayNode.lchild;
          splayNode.lchild = splaytree1;
        }
        else
        {
          if (flag1)
          {
            splaytree.rchild = splayNode.lchild;
            splayNode.lchild = splaytree;
          }
          else
          {
            splaytree.lchild = splaytree1.rchild;
            splaytree1.rchild = splaytree;
          }
          splaytree1.lchild = splayNode.rchild;
          splayNode.rchild = splaytree1;
        }
        return splayNode;
      }
      SplayNode splayNode1 = Splay(splaytree.lchild, searchpoint, ref searchtri);
      SplayNode splayNode2 = Splay(splaytree.rchild, searchpoint, ref searchtri);
      splaynodes.Remove(splaytree);
      if (splayNode1 == null)
        return splayNode2;
      if (splayNode2 == null)
        return splayNode1;
      if (splayNode1.rchild == null)
      {
        splayNode1.rchild = splayNode2.lchild;
        splayNode2.lchild = splayNode1;
        return splayNode2;
      }
      if (splayNode2.lchild == null)
      {
        splayNode2.lchild = splayNode1.rchild;
        splayNode1.rchild = splayNode2;
        return splayNode1;
      }
      SplayNode rchild = splayNode1.rchild;
      while (rchild.rchild != null)
        rchild = rchild.rchild;
      rchild.rchild = splayNode2;
      return splayNode1;
    }

    private SplayNode SplayInsert(
      SplayNode splayroot,
      Otri newkey,
      Point searchpoint)
    {
      SplayNode splayNode = new SplayNode();
      splaynodes.Add(splayNode);
      newkey.Copy(ref splayNode.keyedge);
      splayNode.keydest = newkey.Dest();
      if (splayroot == null)
      {
        splayNode.lchild = null;
        splayNode.rchild = null;
      }
      else if (RightOfHyperbola(ref splayroot.keyedge, searchpoint))
      {
        splayNode.lchild = splayroot;
        splayNode.rchild = splayroot.rchild;
        splayroot.rchild = null;
      }
      else
      {
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
      double topy)
    {
      Point searchpoint = new Point();
      Otri searchtri = new Otri();
      double num1 = Primitives.CounterClockwise(pa, pb, pc);
      double num2 = pa.x - pc.x;
      double num3 = pa.y - pc.y;
      double num4 = pb.x - pc.x;
      double num5 = pb.y - pc.y;
      double num6 = num2 * num2 + num3 * num3;
      double num7 = num4 * num4 + num5 * num5;
      searchpoint.x = pc.x - (num3 * num7 - num5 * num6) / (2.0 * num1);
      searchpoint.y = topy;
      return SplayInsert(Splay(splayroot, searchpoint, ref searchtri), newkey, searchpoint);
    }

    private bool RightOfHyperbola(ref Otri fronttri, Point newsite)
    {
      ++Statistic.HyperbolaCount;
      Vertex vertex1 = fronttri.Dest();
      Vertex vertex2 = fronttri.Apex();
      if (vertex1.y < vertex2.y || vertex1.y == vertex2.y && vertex1.x < vertex2.x)
      {
        if (newsite.x >= vertex2.x)
          return true;
      }
      else if (newsite.x <= vertex1.x)
        return false;
      double num1 = vertex1.x - newsite.x;
      double num2 = vertex1.y - newsite.y;
      double num3 = vertex2.x - newsite.x;
      double num4 = vertex2.y - newsite.y;
      return num2 * (num3 * num3 + num4 * num4) > num4 * (num1 * num1 + num2 * num2);
    }

    private double CircleTop(Vertex pa, Vertex pb, Vertex pc, double ccwabc)
    {
      ++Statistic.CircleTopCount;
      double num1 = pa.x - pc.x;
      double num2 = pa.y - pc.y;
      double num3 = pb.x - pc.x;
      double num4 = pb.y - pc.y;
      double num5 = pa.x - pb.x;
      double num6 = pa.y - pb.y;
      double num7 = num1 * num1 + num2 * num2;
      double num8 = num3 * num3 + num4 * num4;
      double num9 = num5 * num5 + num6 * num6;
      return pc.y + (num1 * num8 - num3 * num7 + Math.Sqrt(num7 * num8 * num9)) / (2.0 * ccwabc);
    }

    private void Check4DeadEvent(
      ref Otri checktri,
      SweepEvent[] eventheap,
      ref int heapsize)
    {
      SweepEventVertex sweepEventVertex = checktri.Org() as SweepEventVertex;
      if (!(sweepEventVertex != null))
        return;
      int heapposition = sweepEventVertex.evt.heapposition;
      HeapDelete(eventheap, heapsize, heapposition);
      --heapsize;
      checktri.SetOrg(null);
    }

    private SplayNode FrontLocate(
      SplayNode splayroot,
      Otri bottommost,
      Vertex searchvertex,
      ref Otri searchtri,
      ref bool farright)
    {
      bottommost.Copy(ref searchtri);
      splayroot = Splay(splayroot, searchvertex, ref searchtri);
      bool flag;
      for (flag = false; !flag && RightOfHyperbola(ref searchtri, searchvertex); flag = searchtri.Equal(bottommost))
        searchtri.OnextSelf();
      farright = flag;
      return splayroot;
    }

    private int RemoveGhosts(ref Otri startghost)
    {
      Otri o2_1 = new Otri();
      Otri o2_2 = new Otri();
      Otri o2_3 = new Otri();
      bool flag = !mesh.behavior.Poly;
      startghost.Lprev(ref o2_1);
      o2_1.SymSelf();
      Mesh.dummytri.neighbors[0] = o2_1;
      startghost.Copy(ref o2_2);
      int num = 0;
      do
      {
        ++num;
        o2_2.Lnext(ref o2_3);
        o2_2.LprevSelf();
        o2_2.SymSelf();
        if (flag && o2_2.triangle != Mesh.dummytri)
        {
          Vertex vertex = o2_2.Org();
          if (vertex.mark == 0)
            vertex.mark = 1;
        }
        o2_2.Dissolve();
        o2_3.Sym(ref o2_2);
        mesh.TriangleDealloc(o2_3.triangle);
      }
      while (!o2_2.Equal(startghost));
      return num;
    }

    public int Triangulate(Mesh mesh)
    {
      this.mesh = mesh;
      xminextreme = 10.0 * mesh.bounds.Xmin - 9.0 * mesh.bounds.Xmax;
      Otri otri1 = new Otri();
      Otri otri2 = new Otri();
      Otri newkey = new Otri();
      Otri otri3 = new Otri();
      Otri otri4 = new Otri();
      Otri otri5 = new Otri();
      Otri o2 = new Otri();
      bool farright = false;
      splaynodes = new List<SplayNode>();
      SplayNode splayroot = null;
      SweepEvent[] eventheap;
      CreateHeap(out eventheap);
      int invertices = mesh.invertices;
      mesh.MakeTriangle(ref newkey);
      mesh.MakeTriangle(ref otri3);
      newkey.Bond(ref otri3);
      newkey.LnextSelf();
      otri3.LprevSelf();
      newkey.Bond(ref otri3);
      newkey.LnextSelf();
      otri3.LprevSelf();
      newkey.Bond(ref otri3);
      Vertex vertexEvent1 = eventheap[0].vertexEvent;
      HeapDelete(eventheap, invertices, 0);
      int heapsize = invertices - 1;
      while (heapsize != 0)
      {
        Vertex vertexEvent2 = eventheap[0].vertexEvent;
        HeapDelete(eventheap, heapsize, 0);
        --heapsize;
        if (vertexEvent1.x == vertexEvent2.x && vertexEvent1.y == vertexEvent2.y)
        {
          if (Behavior.Verbose)
            SimpleLog.Instance.Warning("A duplicate vertex appeared and was ignored.", "SweepLine.SweepLineDelaunay().1");
          vertexEvent2.type = VertexType.UndeadVertex;
          ++mesh.undeads;
        }
        if (vertexEvent1.x != vertexEvent2.x || vertexEvent1.y != vertexEvent2.y)
        {
          newkey.SetOrg(vertexEvent1);
          newkey.SetDest(vertexEvent2);
          otri3.SetOrg(vertexEvent2);
          otri3.SetDest(vertexEvent1);
          newkey.Lprev(ref otri1);
          Vertex vertex = vertexEvent2;
          while (heapsize > 0)
          {
            SweepEvent sweepEvent1 = eventheap[0];
            HeapDelete(eventheap, heapsize, 0);
            --heapsize;
            bool flag = true;
            if (sweepEvent1.xkey < mesh.bounds.Xmin)
            {
              Otri otriEvent = sweepEvent1.otriEvent;
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
              if (randomnation(SAMPLERATE) == 0)
              {
                otriEvent.SymSelf();
                Vertex pa = otriEvent.Dest();
                Vertex pb = otriEvent.Apex();
                Vertex pc = otriEvent.Org();
                splayroot = CircleTopInsert(splayroot, newkey, pa, pb, pc, sweepEvent1.ykey);
              }
            }
            else
            {
              Vertex vertexEvent3 = sweepEvent1.vertexEvent;
              if (vertexEvent3.x == vertex.x && vertexEvent3.y == vertex.y)
              {
                if (Behavior.Verbose)
                  SimpleLog.Instance.Warning("A duplicate vertex appeared and was ignored.", "SweepLine.SweepLineDelaunay().2");
                vertexEvent3.type = VertexType.UndeadVertex;
                ++mesh.undeads;
                flag = false;
              }
              else
              {
                vertex = vertexEvent3;
                splayroot = FrontLocate(splayroot, otri1, vertexEvent3, ref otri2, ref farright);
                otri1.Copy(ref otri2);
                for (farright = false; !farright && RightOfHyperbola(ref otri2, vertexEvent3); farright = otri2.Equal(otri1))
                  otri2.OnextSelf();
                Check4DeadEvent(ref otri2, eventheap, ref heapsize);
                otri2.Copy(ref otri5);
                otri2.Sym(ref otri4);
                mesh.MakeTriangle(ref newkey);
                mesh.MakeTriangle(ref otri3);
                Vertex ptr = otri5.Dest();
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
                else if (randomnation(SAMPLERATE) == 0)
                {
                  otri3.Lnext(ref o2);
                  splayroot = SplayInsert(splayroot, o2, vertexEvent3);
                }
              }
            }
            if (flag)
            {
              Vertex pa1 = otri4.Apex();
              Vertex pb1 = newkey.Dest();
              Vertex pc1 = newkey.Apex();
              double ccwabc1 = Primitives.CounterClockwise(pa1, pb1, pc1);
              if (ccwabc1 > 0.0)
              {
                SweepEvent sweepEvent2 = new SweepEvent();
                sweepEvent2.xkey = xminextreme;
                sweepEvent2.ykey = CircleTop(pa1, pb1, pc1, ccwabc1);
                sweepEvent2.otriEvent = newkey;
                HeapInsert(eventheap, heapsize, sweepEvent2);
                ++heapsize;
                newkey.SetOrg(new SweepEventVertex(sweepEvent2));
              }
              Vertex pa2 = otri3.Apex();
              Vertex pb2 = otri3.Org();
              Vertex pc2 = otri5.Apex();
              double ccwabc2 = Primitives.CounterClockwise(pa2, pb2, pc2);
              if (ccwabc2 > 0.0)
              {
                SweepEvent sweepEvent3 = new SweepEvent();
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

    private class SweepEvent
    {
      public int heapposition;
      public Otri otriEvent;
      public Vertex vertexEvent;
      public double xkey;
      public double ykey;
    }

    private class SweepEventVertex : Vertex
    {
      public SweepEvent evt;

      public SweepEventVertex(SweepEvent e) => evt = e;
    }

    private class SplayNode
    {
      public Vertex keydest;
      public Otri keyedge;
      public SplayNode lchild;
      public SplayNode rchild;
    }
  }
}
