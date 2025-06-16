using System;
using TriangleNet.Data;
using TriangleNet.Log;

namespace TriangleNet.Algorithm
{
  internal class Dwyer
  {
    private static Random rand = new Random(DateTime.Now.Millisecond);
    private Mesh mesh;
    private Vertex[] sortarray;
    private bool useDwyer = true;

    private void VertexSort(int left, int right)
    {
      int left1 = left;
      int right1 = right;
      if (right - left + 1 < 32)
      {
        for (int index1 = left + 1; index1 <= right; ++index1)
        {
          Vertex vertex = sortarray[index1];
          int index2;
          for (index2 = index1 - 1; index2 >= left && (sortarray[index2].x > vertex.x || sortarray[index2].x == vertex.x && sortarray[index2].y > vertex.y); --index2)
            sortarray[index2 + 1] = sortarray[index2];
          sortarray[index2 + 1] = vertex;
        }
      }
      else
      {
        int index = rand.Next(left, right);
        double x = sortarray[index].x;
        double y = sortarray[index].y;
        --left;
        ++right;
        while (left < right)
        {
          do
          {
            ++left;
          }
          while (left <= right && (sortarray[left].x < x || sortarray[left].x == x && sortarray[left].y < y));
          do
          {
            --right;
          }
          while (left <= right && (sortarray[right].x > x || sortarray[right].x == x && sortarray[right].y > y));
          if (left < right)
          {
            Vertex vertex = sortarray[left];
            sortarray[left] = sortarray[right];
            sortarray[right] = vertex;
          }
        }
        if (left > left1)
          VertexSort(left1, left);
        if (right1 <= right + 1)
          return;
        VertexSort(right + 1, right1);
      }
    }

    private void VertexMedian(int left, int right, int median, int axis)
    {
      int num1 = right - left + 1;
      int left1 = left;
      int right1 = right;
      if (num1 == 2)
      {
        if (sortarray[left][axis] <= sortarray[right][axis] && (sortarray[left][axis] != sortarray[right][axis] || sortarray[left][1 - axis] <= sortarray[right][1 - axis]))
          return;
        Vertex vertex = sortarray[right];
        sortarray[right] = sortarray[left];
        sortarray[left] = vertex;
      }
      else
      {
        int index = rand.Next(left, right);
        double num2 = sortarray[index][axis];
        double num3 = sortarray[index][1 - axis];
        --left;
        ++right;
        while (left < right)
        {
          do
          {
            ++left;
          }
          while (left <= right && (sortarray[left][axis] < num2 || sortarray[left][axis] == num2 && sortarray[left][1 - axis] < num3));
          do
          {
            --right;
          }
          while (left <= right && (sortarray[right][axis] > num2 || sortarray[right][axis] == num2 && sortarray[right][1 - axis] > num3));
          if (left < right)
          {
            Vertex vertex = sortarray[left];
            sortarray[left] = sortarray[right];
            sortarray[right] = vertex;
          }
        }
        if (left > median)
          VertexMedian(left1, left - 1, median, axis);
        if (right >= median - 1)
          return;
        VertexMedian(right + 1, right1, median, axis);
      }
    }

    private void AlternateAxes(int left, int right, int axis)
    {
      int num1 = right - left + 1;
      int num2 = num1 >> 1;
      if (num1 <= 3)
        axis = 0;
      VertexMedian(left, right, left + num2, axis);
      if (num1 - num2 < 2)
        return;
      if (num2 >= 2)
        AlternateAxes(left, left + num2 - 1, 1 - axis);
      AlternateAxes(left + num2, right, 1 - axis);
    }

    private void MergeHulls(
      ref Otri farleft,
      ref Otri innerleft,
      ref Otri innerright,
      ref Otri farright,
      int axis)
    {
      Otri o2_1 = new Otri();
      Otri o2_2 = new Otri();
      Otri otri1 = new Otri();
      Otri o2_3 = new Otri();
      Otri o2_4 = new Otri();
      Otri o2_5 = new Otri();
      Otri o2_6 = new Otri();
      Otri otri2 = new Otri();
      Vertex vertex1 = innerleft.Dest();
      Vertex pb = innerleft.Apex();
      Vertex vertex2 = innerright.Org();
      Vertex pa = innerright.Apex();
      if (useDwyer && axis == 1)
      {
        Vertex vertex3 = farleft.Org();
        Vertex vertex4 = farleft.Apex();
        Vertex vertex5 = farright.Dest();
        Vertex vertex6 = farright.Apex();
        for (; vertex4.y < vertex3.y; vertex4 = farleft.Apex())
        {
          farleft.LnextSelf();
          farleft.SymSelf();
          vertex3 = vertex4;
        }
        innerleft.Sym(ref o2_6);
        for (Vertex vertex7 = o2_6.Apex(); vertex7.y > vertex1.y; vertex7 = o2_6.Apex())
        {
          o2_6.Lnext(ref innerleft);
          pb = vertex1;
          vertex1 = vertex7;
          innerleft.Sym(ref o2_6);
        }
        for (; pa.y < vertex2.y; pa = innerright.Apex())
        {
          innerright.LnextSelf();
          innerright.SymSelf();
          vertex2 = pa;
        }
        farright.Sym(ref o2_6);
        for (Vertex vertex8 = o2_6.Apex(); vertex8.y > vertex5.y; vertex8 = o2_6.Apex())
        {
          o2_6.Lnext(ref farright);
          vertex6 = vertex5;
          vertex5 = vertex8;
          farright.Sym(ref o2_6);
        }
      }
      bool flag1;
      do
      {
        flag1 = false;
        if (Primitives.CounterClockwise(vertex1, pb, vertex2) > 0.0)
        {
          innerleft.LprevSelf();
          innerleft.SymSelf();
          vertex1 = pb;
          pb = innerleft.Apex();
          flag1 = true;
        }
        if (Primitives.CounterClockwise(pa, vertex2, vertex1) > 0.0)
        {
          innerright.LnextSelf();
          innerright.SymSelf();
          vertex2 = pa;
          pa = innerright.Apex();
          flag1 = true;
        }
      }
      while (flag1);
      innerleft.Sym(ref o2_1);
      innerright.Sym(ref o2_2);
      mesh.MakeTriangle(ref otri2);
      otri2.Bond(ref innerleft);
      otri2.LnextSelf();
      otri2.Bond(ref innerright);
      otri2.LnextSelf();
      otri2.SetOrg(vertex2);
      otri2.SetDest(vertex1);
      Vertex vertex9 = farleft.Org();
      if (vertex1 == vertex9)
        otri2.Lnext(ref farleft);
      Vertex vertex10 = farright.Dest();
      if (vertex2 == vertex10)
        otri2.Lprev(ref farright);
      Vertex vertex11 = vertex1;
      Vertex vertex12 = vertex2;
      Vertex vertex13 = o2_1.Apex();
      Vertex vertex14 = o2_2.Apex();
      while (true)
      {
        bool flag2 = Primitives.CounterClockwise(vertex13, vertex11, vertex12) <= 0.0;
        bool flag3 = Primitives.CounterClockwise(vertex14, vertex11, vertex12) <= 0.0;
        if (!(flag2 & flag3))
        {
          if (!flag2)
          {
            o2_1.Lprev(ref otri1);
            otri1.SymSelf();
            Vertex vertex15 = otri1.Apex();
            if (vertex15 != null)
            {
              for (bool flag4 = Primitives.InCircle(vertex11, vertex12, vertex13, vertex15) > 0.0; flag4; flag4 = vertex15 != null && Primitives.InCircle(vertex11, vertex12, vertex13, vertex15) > 0.0)
              {
                otri1.LnextSelf();
                otri1.Sym(ref o2_4);
                otri1.LnextSelf();
                otri1.Sym(ref o2_3);
                otri1.Bond(ref o2_4);
                o2_1.Bond(ref o2_3);
                o2_1.LnextSelf();
                o2_1.Sym(ref o2_5);
                otri1.LprevSelf();
                otri1.Bond(ref o2_5);
                o2_1.SetOrg(vertex11);
                o2_1.SetDest(null);
                o2_1.SetApex(vertex15);
                otri1.SetOrg(null);
                otri1.SetDest(vertex13);
                otri1.SetApex(vertex15);
                vertex13 = vertex15;
                o2_3.Copy(ref otri1);
                vertex15 = otri1.Apex();
              }
            }
          }
          if (!flag3)
          {
            o2_2.Lnext(ref otri1);
            otri1.SymSelf();
            Vertex vertex16 = otri1.Apex();
            if (vertex16 != null)
            {
              for (bool flag5 = Primitives.InCircle(vertex11, vertex12, vertex14, vertex16) > 0.0; flag5; flag5 = vertex16 != null && Primitives.InCircle(vertex11, vertex12, vertex14, vertex16) > 0.0)
              {
                otri1.LprevSelf();
                otri1.Sym(ref o2_4);
                otri1.LprevSelf();
                otri1.Sym(ref o2_3);
                otri1.Bond(ref o2_4);
                o2_2.Bond(ref o2_3);
                o2_2.LprevSelf();
                o2_2.Sym(ref o2_5);
                otri1.LnextSelf();
                otri1.Bond(ref o2_5);
                o2_2.SetOrg(null);
                o2_2.SetDest(vertex12);
                o2_2.SetApex(vertex16);
                otri1.SetOrg(vertex14);
                otri1.SetDest(null);
                otri1.SetApex(vertex16);
                vertex14 = vertex16;
                o2_3.Copy(ref otri1);
                vertex16 = otri1.Apex();
              }
            }
          }
          if (flag2 || !flag3 && Primitives.InCircle(vertex13, vertex11, vertex12, vertex14) > 0.0)
          {
            otri2.Bond(ref o2_2);
            o2_2.Lprev(ref otri2);
            otri2.SetDest(vertex11);
            vertex12 = vertex14;
            otri2.Sym(ref o2_2);
            vertex14 = o2_2.Apex();
          }
          else
          {
            otri2.Bond(ref o2_1);
            o2_1.Lnext(ref otri2);
            otri2.SetOrg(vertex12);
            vertex11 = vertex13;
            otri2.Sym(ref o2_1);
            vertex13 = o2_1.Apex();
          }
        }
        else
          break;
      }
      mesh.MakeTriangle(ref otri1);
      otri1.SetOrg(vertex11);
      otri1.SetDest(vertex12);
      otri1.Bond(ref otri2);
      otri1.LnextSelf();
      otri1.Bond(ref o2_2);
      otri1.LnextSelf();
      otri1.Bond(ref o2_1);
      if (!useDwyer || axis != 1)
        return;
      Vertex vertex17 = farleft.Org();
      Vertex vertex18 = farleft.Apex();
      Vertex vertex19 = farright.Dest();
      Vertex vertex20 = farright.Apex();
      farleft.Sym(ref o2_6);
      for (Vertex vertex21 = o2_6.Apex(); vertex21.x < vertex17.x; vertex21 = o2_6.Apex())
      {
        o2_6.Lprev(ref farleft);
        vertex18 = vertex17;
        vertex17 = vertex21;
        farleft.Sym(ref o2_6);
      }
      for (; vertex20.x > vertex19.x; vertex20 = farright.Apex())
      {
        farright.LprevSelf();
        farright.SymSelf();
        vertex19 = vertex20;
      }
    }

    private void DivconqRecurse(
      int left,
      int right,
      int axis,
      ref Otri farleft,
      ref Otri farright)
    {
      Otri newotri = new Otri();
      Otri otri1 = new Otri();
      Otri otri2 = new Otri();
      Otri otri3 = new Otri();
      Otri otri4 = new Otri();
      Otri otri5 = new Otri();
      int num1 = right - left + 1;
      switch (num1)
      {
        case 2:
          mesh.MakeTriangle(ref farleft);
          farleft.SetOrg(sortarray[left]);
          farleft.SetDest(sortarray[left + 1]);
          mesh.MakeTriangle(ref farright);
          farright.SetOrg(sortarray[left + 1]);
          farright.SetDest(sortarray[left]);
          farleft.Bond(ref farright);
          farleft.LprevSelf();
          farright.LnextSelf();
          farleft.Bond(ref farright);
          farleft.LprevSelf();
          farright.LnextSelf();
          farleft.Bond(ref farright);
          farright.Lprev(ref farleft);
          break;
        case 3:
          mesh.MakeTriangle(ref newotri);
          mesh.MakeTriangle(ref otri1);
          mesh.MakeTriangle(ref otri2);
          mesh.MakeTriangle(ref otri3);
          double num2 = Primitives.CounterClockwise(sortarray[left], sortarray[left + 1], sortarray[left + 2]);
          if (num2 == 0.0)
          {
            newotri.SetOrg(sortarray[left]);
            newotri.SetDest(sortarray[left + 1]);
            otri1.SetOrg(sortarray[left + 1]);
            otri1.SetDest(sortarray[left]);
            otri2.SetOrg(sortarray[left + 2]);
            otri2.SetDest(sortarray[left + 1]);
            otri3.SetOrg(sortarray[left + 1]);
            otri3.SetDest(sortarray[left + 2]);
            newotri.Bond(ref otri1);
            otri2.Bond(ref otri3);
            newotri.LnextSelf();
            otri1.LprevSelf();
            otri2.LnextSelf();
            otri3.LprevSelf();
            newotri.Bond(ref otri3);
            otri1.Bond(ref otri2);
            newotri.LnextSelf();
            otri1.LprevSelf();
            otri2.LnextSelf();
            otri3.LprevSelf();
            newotri.Bond(ref otri1);
            otri2.Bond(ref otri3);
            otri1.Copy(ref farleft);
            otri2.Copy(ref farright);
            break;
          }
          newotri.SetOrg(sortarray[left]);
          otri1.SetDest(sortarray[left]);
          otri3.SetOrg(sortarray[left]);
          if (num2 > 0.0)
          {
            newotri.SetDest(sortarray[left + 1]);
            otri1.SetOrg(sortarray[left + 1]);
            otri2.SetDest(sortarray[left + 1]);
            newotri.SetApex(sortarray[left + 2]);
            otri2.SetOrg(sortarray[left + 2]);
            otri3.SetDest(sortarray[left + 2]);
          }
          else
          {
            newotri.SetDest(sortarray[left + 2]);
            otri1.SetOrg(sortarray[left + 2]);
            otri2.SetDest(sortarray[left + 2]);
            newotri.SetApex(sortarray[left + 1]);
            otri2.SetOrg(sortarray[left + 1]);
            otri3.SetDest(sortarray[left + 1]);
          }
          newotri.Bond(ref otri1);
          newotri.LnextSelf();
          newotri.Bond(ref otri2);
          newotri.LnextSelf();
          newotri.Bond(ref otri3);
          otri1.LprevSelf();
          otri2.LnextSelf();
          otri1.Bond(ref otri2);
          otri1.LprevSelf();
          otri3.LprevSelf();
          otri1.Bond(ref otri3);
          otri2.LnextSelf();
          otri3.LprevSelf();
          otri2.Bond(ref otri3);
          otri1.Copy(ref farleft);
          if (num2 > 0.0)
            otri2.Copy(ref farright);
          else
            farleft.Lnext(ref farright);
          break;
        default:
          int num3 = num1 >> 1;
          DivconqRecurse(left, left + num3 - 1, 1 - axis, ref farleft, ref otri4);
          DivconqRecurse(left + num3, right, 1 - axis, ref otri5, ref farright);
          MergeHulls(ref farleft, ref otri4, ref otri5, ref farright, axis);
          break;
      }
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

    public int Triangulate(Mesh m)
    {
      Otri otri = new Otri();
      Otri farright = new Otri();
      mesh = m;
      sortarray = new Vertex[m.invertices];
      int num1 = 0;
      foreach (Vertex vertex in m.vertices.Values)
        sortarray[num1++] = vertex;
      VertexSort(0, m.invertices - 1);
      int index1 = 0;
      for (int index2 = 1; index2 < m.invertices; ++index2)
      {
        if (sortarray[index1].x == sortarray[index2].x && sortarray[index1].y == sortarray[index2].y)
        {
          if (Behavior.Verbose)
            SimpleLog.Instance.Warning(string.Format("A duplicate vertex appeared and was ignored (ID {0}).", sortarray[index2].hash), "DivConquer.DivconqDelaunay()");
          sortarray[index2].type = VertexType.UndeadVertex;
          ++m.undeads;
        }
        else
        {
          ++index1;
          sortarray[index1] = sortarray[index2];
        }
      }
      int num2 = index1 + 1;
      if (useDwyer)
      {
        int left = num2 >> 1;
        if (num2 - left >= 2)
        {
          if (left >= 2)
            AlternateAxes(0, left - 1, 1);
          AlternateAxes(left, num2 - 1, 1);
        }
      }
      DivconqRecurse(0, num2 - 1, 0, ref otri, ref farright);
      return RemoveGhosts(ref otri);
    }
  }
}
