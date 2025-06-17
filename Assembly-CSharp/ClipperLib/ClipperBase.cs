using System.Collections.Generic;

namespace ClipperLib
{
  public class ClipperBase
  {
    protected const double horizontal = -3.4E+38;
    protected const int Skip = -2;
    protected const int Unassigned = -1;
    protected const double tolerance = 1E-20;
    public const long loRange = 1073741823;
    public const long hiRange = 4611686018427387903;
    internal LocalMinima m_MinimaList;
    internal LocalMinima m_CurrentLM;
    internal List<List<TEdge>> m_edges = [];
    internal bool m_UseFullRange;
    internal bool m_HasOpenPaths;

    internal static bool near_zero(double val) => val > -1E-20 && val < 1E-20;

    public bool PreserveCollinear { get; set; }

    public void Swap(ref long val1, ref long val2)
    {
      long num = val1;
      val1 = val2;
      val2 = num;
    }

    internal static bool IsHorizontal(TEdge e) => e.Delta.Y == 0L;

    internal bool PointIsVertex(IntPoint pt, OutPt pp)
    {
      OutPt outPt = pp;
      while (!(outPt.Pt == pt))
      {
        outPt = outPt.Next;
        if (outPt == pp)
          return false;
      }
      return true;
    }

    internal bool PointOnLineSegment(
      IntPoint pt,
      IntPoint linePt1,
      IntPoint linePt2,
      bool UseFullRange)
    {
      return UseFullRange ? pt.X == linePt1.X && pt.Y == linePt1.Y || pt.X == linePt2.X && pt.Y == linePt2.Y || pt.X > linePt1.X == pt.X < linePt2.X && pt.Y > linePt1.Y == pt.Y < linePt2.Y && Int128.Int128Mul(pt.X - linePt1.X, linePt2.Y - linePt1.Y) == Int128.Int128Mul(linePt2.X - linePt1.X, pt.Y - linePt1.Y) : pt.X == linePt1.X && pt.Y == linePt1.Y || pt.X == linePt2.X && pt.Y == linePt2.Y || pt.X > linePt1.X == pt.X < linePt2.X && pt.Y > linePt1.Y == pt.Y < linePt2.Y && (pt.X - linePt1.X) * (linePt2.Y - linePt1.Y) == (linePt2.X - linePt1.X) * (pt.Y - linePt1.Y);
    }

    internal bool PointOnPolygon(IntPoint pt, OutPt pp, bool UseFullRange)
    {
      OutPt outPt = pp;
      do
      {
        if (!PointOnLineSegment(pt, outPt.Pt, outPt.Next.Pt, UseFullRange))
          outPt = outPt.Next;
        else
          goto label_1;
      }
      while (outPt != pp);
      goto label_4;
label_1:
      return true;
label_4:
      return false;
    }

    internal static bool SlopesEqual(TEdge e1, TEdge e2, bool UseFullRange)
    {
      return UseFullRange ? Int128.Int128Mul(e1.Delta.Y, e2.Delta.X) == Int128.Int128Mul(e1.Delta.X, e2.Delta.Y) : e1.Delta.Y * e2.Delta.X == e1.Delta.X * e2.Delta.Y;
    }

    protected static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, bool UseFullRange)
    {
      return UseFullRange ? Int128.Int128Mul(pt1.Y - pt2.Y, pt2.X - pt3.X) == Int128.Int128Mul(pt1.X - pt2.X, pt2.Y - pt3.Y) : (pt1.Y - pt2.Y) * (pt2.X - pt3.X) - (pt1.X - pt2.X) * (pt2.Y - pt3.Y) == 0L;
    }

    protected static bool SlopesEqual(
      IntPoint pt1,
      IntPoint pt2,
      IntPoint pt3,
      IntPoint pt4,
      bool UseFullRange)
    {
      return UseFullRange ? Int128.Int128Mul(pt1.Y - pt2.Y, pt3.X - pt4.X) == Int128.Int128Mul(pt1.X - pt2.X, pt3.Y - pt4.Y) : (pt1.Y - pt2.Y) * (pt3.X - pt4.X) - (pt1.X - pt2.X) * (pt3.Y - pt4.Y) == 0L;
    }

    internal ClipperBase()
    {
      m_MinimaList = null;
      m_CurrentLM = null;
      m_UseFullRange = false;
      m_HasOpenPaths = false;
    }

    public virtual void Clear()
    {
      DisposeLocalMinimaList();
      for (int index1 = 0; index1 < m_edges.Count; ++index1)
      {
        for (int index2 = 0; index2 < m_edges[index1].Count; ++index2)
          m_edges[index1][index2] = null;
        m_edges[index1].Clear();
      }
      m_edges.Clear();
      m_UseFullRange = false;
      m_HasOpenPaths = false;
    }

    private void DisposeLocalMinimaList()
    {
      LocalMinima next;
      for (; m_MinimaList != null; m_MinimaList = next)
      {
        next = m_MinimaList.Next;
        m_MinimaList = null;
      }
      m_CurrentLM = null;
    }

    private void RangeTest(IntPoint Pt, ref bool useFullRange)
    {
      if (useFullRange)
      {
        if (Pt.X > 4611686018427387903L || Pt.Y > 4611686018427387903L || -Pt.X > 4611686018427387903L || -Pt.Y > 4611686018427387903L)
          throw new ClipperException("Coordinate outside allowed range");
      }
      else
      {
        if (Pt.X <= 1073741823L && Pt.Y <= 1073741823L && -Pt.X <= 1073741823L && -Pt.Y <= 1073741823L)
          return;
        useFullRange = true;
        RangeTest(Pt, ref useFullRange);
      }
    }

    private void InitEdge(TEdge e, TEdge eNext, TEdge ePrev, IntPoint pt)
    {
      e.Next = eNext;
      e.Prev = ePrev;
      e.Curr = pt;
      e.OutIdx = -1;
    }

    private void InitEdge2(TEdge e, PolyType polyType)
    {
      if (e.Curr.Y >= e.Next.Curr.Y)
      {
        e.Bot = e.Curr;
        e.Top = e.Next.Curr;
      }
      else
      {
        e.Top = e.Curr;
        e.Bot = e.Next.Curr;
      }
      SetDx(e);
      e.PolyTyp = polyType;
    }

    private TEdge FindNextLocMin(TEdge E)
    {
      TEdge tedge;
      do
      {
        while (E.Bot != E.Prev.Bot || E.Curr == E.Top)
          E = E.Next;
        if (E.Dx == -3.4E+38 || E.Prev.Dx == -3.4E+38)
        {
          while (E.Prev.Dx == -3.4E+38)
            E = E.Prev;
          tedge = E;
          while (E.Dx == -3.4E+38)
            E = E.Next;
        }
        else
          goto label_12;
      }
      while (E.Top.Y == E.Prev.Bot.Y);
      if (tedge.Prev.Bot.X < E.Bot.X)
        E = tedge;
label_12:
      return E;
    }

    private TEdge ProcessBound(TEdge E, bool LeftBoundIsForward)
    {
      TEdge tedge1 = E;
      if (tedge1.OutIdx == -2)
      {
        E = tedge1;
        if (LeftBoundIsForward)
        {
          while (E.Top.Y == E.Next.Bot.Y)
            E = E.Next;
          while (E != tedge1 && E.Dx == -3.4E+38)
            E = E.Prev;
        }
        else
        {
          while (E.Top.Y == E.Prev.Bot.Y)
            E = E.Prev;
          while (E != tedge1 && E.Dx == -3.4E+38)
            E = E.Next;
        }
        TEdge tedge2;
        if (E == tedge1)
        {
          tedge2 = !LeftBoundIsForward ? E.Prev : E.Next;
        }
        else
        {
          E = !LeftBoundIsForward ? tedge1.Prev : tedge1.Next;
          LocalMinima newLm = new LocalMinima();
          newLm.Next = null;
          newLm.Y = E.Bot.Y;
          newLm.LeftBound = null;
          newLm.RightBound = E;
          E.WindDelta = 0;
          tedge2 = ProcessBound(E, LeftBoundIsForward);
          InsertLocalMinima(newLm);
        }
        return tedge2;
      }
      if (E.Dx == -3.4E+38)
      {
        TEdge tedge3 = !LeftBoundIsForward ? E.Next : E.Prev;
        if (tedge3.OutIdx != -2)
        {
          if (tedge3.Dx == -3.4E+38)
          {
            if (tedge3.Bot.X != E.Bot.X && tedge3.Top.X != E.Bot.X)
              ReverseHorizontal(E);
          }
          else if (tedge3.Bot.X != E.Bot.X)
            ReverseHorizontal(E);
        }
      }
      TEdge tedge4 = E;
      TEdge tedge5;
      if (LeftBoundIsForward)
      {
        while (tedge1.Top.Y == tedge1.Next.Bot.Y && tedge1.Next.OutIdx != -2)
          tedge1 = tedge1.Next;
        if (tedge1.Dx == -3.4E+38 && tedge1.Next.OutIdx != -2)
        {
          TEdge tedge6 = tedge1;
          while (tedge6.Prev.Dx == -3.4E+38)
            tedge6 = tedge6.Prev;
          if (tedge6.Prev.Top.X == tedge1.Next.Top.X)
          {
            if (!LeftBoundIsForward)
              tedge1 = tedge6.Prev;
          }
          else if (tedge6.Prev.Top.X > tedge1.Next.Top.X)
            tedge1 = tedge6.Prev;
        }
        for (; E != tedge1; E = E.Next)
        {
          E.NextInLML = E.Next;
          if (E.Dx == -3.4E+38 && E != tedge4 && E.Bot.X != E.Prev.Top.X)
            ReverseHorizontal(E);
        }
        if (E.Dx == -3.4E+38 && E != tedge4 && E.Bot.X != E.Prev.Top.X)
          ReverseHorizontal(E);
        tedge5 = tedge1.Next;
      }
      else
      {
        while (tedge1.Top.Y == tedge1.Prev.Bot.Y && tedge1.Prev.OutIdx != -2)
          tedge1 = tedge1.Prev;
        if (tedge1.Dx == -3.4E+38 && tedge1.Prev.OutIdx != -2)
        {
          TEdge tedge7 = tedge1;
          while (tedge7.Next.Dx == -3.4E+38)
            tedge7 = tedge7.Next;
          if (tedge7.Next.Top.X == tedge1.Prev.Top.X)
          {
            if (!LeftBoundIsForward)
              tedge1 = tedge7.Next;
          }
          else if (tedge7.Next.Top.X > tedge1.Prev.Top.X)
            tedge1 = tedge7.Next;
        }
        for (; E != tedge1; E = E.Prev)
        {
          E.NextInLML = E.Prev;
          if (E.Dx == -3.4E+38 && E != tedge4 && E.Bot.X != E.Next.Top.X)
            ReverseHorizontal(E);
        }
        if (E.Dx == -3.4E+38 && E != tedge4 && E.Bot.X != E.Next.Top.X)
          ReverseHorizontal(E);
        tedge5 = tedge1.Prev;
      }
      return tedge5;
    }

    public bool AddPath(List<IntPoint> pg, PolyType polyType, bool Closed)
    {
      if (!Closed)
        throw new ClipperException("AddPath: Open paths have been disabled.");
      int index1 = pg.Count - 1;
      if (Closed)
      {
        while (index1 > 0 && pg[index1] == pg[0])
          --index1;
      }
      while (index1 > 0 && pg[index1] == pg[index1 - 1])
        --index1;
      if (Closed && index1 < 2 || !Closed && index1 < 1)
        return false;
      List<TEdge> tedgeList = new List<TEdge>(index1 + 1);
      for (int index2 = 0; index2 <= index1; ++index2)
        tedgeList.Add(new TEdge());
      bool flag = true;
      tedgeList[1].Curr = pg[1];
      RangeTest(pg[0], ref m_UseFullRange);
      RangeTest(pg[index1], ref m_UseFullRange);
      InitEdge(tedgeList[0], tedgeList[1], tedgeList[index1], pg[0]);
      InitEdge(tedgeList[index1], tedgeList[0], tedgeList[index1 - 1], pg[index1]);
      for (int index3 = index1 - 1; index3 >= 1; --index3)
      {
        RangeTest(pg[index3], ref m_UseFullRange);
        InitEdge(tedgeList[index3], tedgeList[index3 + 1], tedgeList[index3 - 1], pg[index3]);
      }
      TEdge next = tedgeList[0];
      TEdge e = next;
      TEdge tedge1 = next;
      while (true)
      {
        while (!(e.Curr == e.Next.Curr) || !Closed && e.Next == next)
        {
          if (e.Prev != e.Next)
          {
            if (Closed && SlopesEqual(e.Prev.Curr, e.Curr, e.Next.Curr, m_UseFullRange) && (!PreserveCollinear || !Pt2IsBetweenPt1AndPt3(e.Prev.Curr, e.Curr, e.Next.Curr)))
            {
              if (e == next)
                next = e.Next;
              e = RemoveEdge(e).Prev;
              tedge1 = e;
            }
            else
            {
              e = e.Next;
              if (e == tedge1 || !Closed && e.Next == next)
                goto label_27;
            }
          }
          else
            goto label_27;
        }
        if (e != e.Next)
        {
          if (e == next)
            next = e.Next;
          e = RemoveEdge(e);
          tedge1 = e;
        }
        else
          break;
      }
label_27:
      if (!Closed && e == e.Next || Closed && e.Prev == e.Next)
        return false;
      if (!Closed)
      {
        m_HasOpenPaths = true;
        next.Prev.OutIdx = -2;
      }
      TEdge tedge2 = next;
      do
      {
        InitEdge2(tedge2, polyType);
        tedge2 = tedge2.Next;
        if (flag && tedge2.Curr.Y != next.Curr.Y)
          flag = false;
      }
      while (tedge2 != next);
      if (flag)
      {
        if (Closed)
          return false;
        tedge2.Prev.OutIdx = -2;
        if (tedge2.Prev.Bot.X < tedge2.Prev.Top.X)
          ReverseHorizontal(tedge2.Prev);
        LocalMinima newLm = new LocalMinima {
          Next = null,
          Y = tedge2.Bot.Y,
          LeftBound = null,
          RightBound = tedge2
        };
        newLm.RightBound.Side = EdgeSide.esRight;
        newLm.RightBound.WindDelta = 0;
        for (; tedge2.Next.OutIdx != -2; tedge2 = tedge2.Next)
        {
          tedge2.NextInLML = tedge2.Next;
          if (tedge2.Bot.X != tedge2.Prev.Top.X)
            ReverseHorizontal(tedge2);
        }
        InsertLocalMinima(newLm);
        m_edges.Add(tedgeList);
        return true;
      }
      m_edges.Add(tedgeList);
      TEdge tedge3 = null;
      if (tedge2.Prev.Bot == tedge2.Prev.Top)
        tedge2 = tedge2.Next;
      while (true)
      {
        TEdge nextLocMin = FindNextLocMin(tedge2);
        if (nextLocMin != tedge3)
        {
          if (tedge3 == null)
            tedge3 = nextLocMin;
          LocalMinima newLm = new LocalMinima();
          newLm.Next = null;
          newLm.Y = nextLocMin.Bot.Y;
          bool LeftBoundIsForward;
          if (nextLocMin.Dx < nextLocMin.Prev.Dx)
          {
            newLm.LeftBound = nextLocMin.Prev;
            newLm.RightBound = nextLocMin;
            LeftBoundIsForward = false;
          }
          else
          {
            newLm.LeftBound = nextLocMin;
            newLm.RightBound = nextLocMin.Prev;
            LeftBoundIsForward = true;
          }
          newLm.LeftBound.Side = EdgeSide.esLeft;
          newLm.RightBound.Side = EdgeSide.esRight;
          newLm.LeftBound.WindDelta = Closed ? (newLm.LeftBound.Next != newLm.RightBound ? 1 : -1) : 0;
          newLm.RightBound.WindDelta = -newLm.LeftBound.WindDelta;
          tedge2 = ProcessBound(newLm.LeftBound, LeftBoundIsForward);
          if (tedge2.OutIdx == -2)
            tedge2 = ProcessBound(tedge2, LeftBoundIsForward);
          TEdge E = ProcessBound(newLm.RightBound, !LeftBoundIsForward);
          if (E.OutIdx == -2)
            E = ProcessBound(E, !LeftBoundIsForward);
          if (newLm.LeftBound.OutIdx == -2)
            newLm.LeftBound = null;
          else if (newLm.RightBound.OutIdx == -2)
            newLm.RightBound = null;
          InsertLocalMinima(newLm);
          if (!LeftBoundIsForward)
            tedge2 = E;
        }
        else
          break;
      }
      return true;
    }

    public bool AddPaths(List<List<IntPoint>> ppg, PolyType polyType, bool closed)
    {
      bool flag = false;
      for (int index = 0; index < ppg.Count; ++index)
      {
        if (AddPath(ppg[index], polyType, closed))
          flag = true;
      }
      return flag;
    }

    internal bool Pt2IsBetweenPt1AndPt3(IntPoint pt1, IntPoint pt2, IntPoint pt3)
    {
      if (pt1 == pt3 || pt1 == pt2 || pt3 == pt2)
        return false;
      return pt1.X != pt3.X ? pt2.X > pt1.X == pt2.X < pt3.X : pt2.Y > pt1.Y == pt2.Y < pt3.Y;
    }

    private TEdge RemoveEdge(TEdge e)
    {
      e.Prev.Next = e.Next;
      e.Next.Prev = e.Prev;
      TEdge next = e.Next;
      e.Prev = null;
      return next;
    }

    private void SetDx(TEdge e)
    {
      e.Delta.X = e.Top.X - e.Bot.X;
      e.Delta.Y = e.Top.Y - e.Bot.Y;
      if (e.Delta.Y == 0L)
        e.Dx = -3.4E+38;
      else
        e.Dx = e.Delta.X / (double) e.Delta.Y;
    }

    private void InsertLocalMinima(LocalMinima newLm)
    {
      if (m_MinimaList == null)
        m_MinimaList = newLm;
      else if (newLm.Y >= m_MinimaList.Y)
      {
        newLm.Next = m_MinimaList;
        m_MinimaList = newLm;
      }
      else
      {
        LocalMinima localMinima = m_MinimaList;
        while (localMinima.Next != null && newLm.Y < localMinima.Next.Y)
          localMinima = localMinima.Next;
        newLm.Next = localMinima.Next;
        localMinima.Next = newLm;
      }
    }

    protected void PopLocalMinima()
    {
      if (m_CurrentLM == null)
        return;
      m_CurrentLM = m_CurrentLM.Next;
    }

    private void ReverseHorizontal(TEdge e) => Swap(ref e.Top.X, ref e.Bot.X);

    protected virtual void Reset()
    {
      m_CurrentLM = m_MinimaList;
      if (m_CurrentLM == null)
        return;
      for (LocalMinima localMinima = m_MinimaList; localMinima != null; localMinima = localMinima.Next)
      {
        TEdge leftBound = localMinima.LeftBound;
        if (leftBound != null)
        {
          leftBound.Curr = leftBound.Bot;
          leftBound.Side = EdgeSide.esLeft;
          leftBound.OutIdx = -1;
        }
        TEdge rightBound = localMinima.RightBound;
        if (rightBound != null)
        {
          rightBound.Curr = rightBound.Bot;
          rightBound.Side = EdgeSide.esRight;
          rightBound.OutIdx = -1;
        }
      }
    }

    public static IntRect GetBounds(List<List<IntPoint>> paths)
    {
      int index1 = 0;
      int count = paths.Count;
      while (index1 < count && paths[index1].Count == 0)
        ++index1;
      if (index1 == count)
        return new IntRect(0L, 0L, 0L, 0L);
      IntRect bounds = new IntRect {
        left = paths[index1][0].X
      };
      bounds.right = bounds.left;
      bounds.top = paths[index1][0].Y;
      bounds.bottom = bounds.top;
      for (; index1 < count; ++index1)
      {
        for (int index2 = 0; index2 < paths[index1].Count; ++index2)
        {
          if (paths[index1][index2].X < bounds.left)
            bounds.left = paths[index1][index2].X;
          else if (paths[index1][index2].X > bounds.right)
            bounds.right = paths[index1][index2].X;
          if (paths[index1][index2].Y < bounds.top)
            bounds.top = paths[index1][index2].Y;
          else if (paths[index1][index2].Y > bounds.bottom)
            bounds.bottom = paths[index1][index2].Y;
        }
      }
      return bounds;
    }
  }
}
