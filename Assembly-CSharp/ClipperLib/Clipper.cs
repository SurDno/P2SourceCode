using System;
using System.Collections.Generic;

namespace ClipperLib
{
  public class Clipper : ClipperBase
  {
    public const int ioReverseSolution = 1;
    public const int ioStrictlySimple = 2;
    public const int ioPreserveCollinear = 4;
    private List<OutRec> m_PolyOuts;
    private ClipType m_ClipType;
    private Scanbeam m_Scanbeam;
    private TEdge m_ActiveEdges;
    private TEdge m_SortedEdges;
    private List<IntersectNode> m_IntersectList;
    private IComparer<IntersectNode> m_IntersectNodeComparer;
    private bool m_ExecuteLocked;
    private PolyFillType m_ClipFillType;
    private PolyFillType m_SubjFillType;
    private List<Join> m_Joins;
    private List<Join> m_GhostJoins;
    private bool m_UsingPolyTree;

    public Clipper(int InitOptions = 0)
    {
      m_Scanbeam = null;
      m_ActiveEdges = null;
      m_SortedEdges = null;
      m_IntersectList = [];
      m_IntersectNodeComparer = new MyIntersectNodeSort();
      m_ExecuteLocked = false;
      m_UsingPolyTree = false;
      m_PolyOuts = [];
      m_Joins = [];
      m_GhostJoins = [];
      ReverseSolution = (1 & InitOptions) != 0;
      StrictlySimple = (2 & InitOptions) != 0;
      PreserveCollinear = (4 & InitOptions) != 0;
    }

    private void DisposeScanbeamList()
    {
      Scanbeam next;
      for (; m_Scanbeam != null; m_Scanbeam = next)
      {
        next = m_Scanbeam.Next;
        m_Scanbeam = null;
      }
    }

    protected override void Reset()
    {
      base.Reset();
      m_Scanbeam = null;
      m_ActiveEdges = null;
      m_SortedEdges = null;
      for (LocalMinima localMinima = m_MinimaList; localMinima != null; localMinima = localMinima.Next)
        InsertScanbeam(localMinima.Y);
    }

    public bool ReverseSolution { get; set; }

    public bool StrictlySimple { get; set; }

    private void InsertScanbeam(long Y)
    {
      if (m_Scanbeam == null)
      {
        m_Scanbeam = new Scanbeam();
        m_Scanbeam.Next = null;
        m_Scanbeam.Y = Y;
      }
      else if (Y > m_Scanbeam.Y)
      {
        m_Scanbeam = new Scanbeam {
          Y = Y,
          Next = m_Scanbeam
        };
      }
      else
      {
        Scanbeam scanbeam = m_Scanbeam;
        while (scanbeam.Next != null && Y <= scanbeam.Next.Y)
          scanbeam = scanbeam.Next;
        if (Y == scanbeam.Y)
          return;
        scanbeam.Next = new Scanbeam {
          Y = Y,
          Next = scanbeam.Next
        };
      }
    }

    public bool Execute(
      ClipType clipType,
      List<List<IntPoint>> solution,
      PolyFillType subjFillType,
      PolyFillType clipFillType)
    {
      if (m_ExecuteLocked)
        return false;
      if (m_HasOpenPaths)
        throw new ClipperException("Error: PolyTree struct is need for open path clipping.");
      m_ExecuteLocked = true;
      solution.Clear();
      m_SubjFillType = subjFillType;
      m_ClipFillType = clipFillType;
      m_ClipType = clipType;
      m_UsingPolyTree = false;
      bool flag;
      try
      {
        flag = ExecuteInternal();
        if (flag)
          BuildResult(solution);
      }
      finally
      {
        DisposeAllPolyPts();
        m_ExecuteLocked = false;
      }
      return flag;
    }

    public bool Execute(
      ClipType clipType,
      PolyTree polytree,
      PolyFillType subjFillType,
      PolyFillType clipFillType)
    {
      if (m_ExecuteLocked)
        return false;
      m_ExecuteLocked = true;
      m_SubjFillType = subjFillType;
      m_ClipFillType = clipFillType;
      m_ClipType = clipType;
      m_UsingPolyTree = true;
      bool flag;
      try
      {
        flag = ExecuteInternal();
        if (flag)
          BuildResult2(polytree);
      }
      finally
      {
        DisposeAllPolyPts();
        m_ExecuteLocked = false;
      }
      return flag;
    }

    public bool Execute(ClipType clipType, List<List<IntPoint>> solution)
    {
      return Execute(clipType, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
    }

    public bool Execute(ClipType clipType, PolyTree polytree)
    {
      return Execute(clipType, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
    }

    internal void FixHoleLinkage(OutRec outRec)
    {
      if (outRec.FirstLeft == null || outRec.IsHole != outRec.FirstLeft.IsHole && outRec.FirstLeft.Pts != null)
        return;
      OutRec firstLeft = outRec.FirstLeft;
      while (firstLeft != null && (firstLeft.IsHole == outRec.IsHole || firstLeft.Pts == null))
        firstLeft = firstLeft.FirstLeft;
      outRec.FirstLeft = firstLeft;
    }

    private bool ExecuteInternal()
    {
      try
      {
        Reset();
        if (m_CurrentLM == null)
          return false;
        long botY = PopScanbeam();
        do
        {
          InsertLocalMinimaIntoAEL(botY);
          m_GhostJoins.Clear();
          ProcessHorizontals(false);
          if (m_Scanbeam != null)
          {
            long topY = PopScanbeam();
            if (!ProcessIntersections(topY))
              return false;
            ProcessEdgesAtTopOfScanbeam(topY);
            botY = topY;
          }
          else
            break;
        }
        while (m_Scanbeam != null || m_CurrentLM != null);
        for (int index = 0; index < m_PolyOuts.Count; ++index)
        {
          OutRec polyOut = m_PolyOuts[index];
          if (polyOut.Pts != null && !polyOut.IsOpen && (polyOut.IsHole ^ ReverseSolution) == Area(polyOut) > 0.0)
            ReversePolyPtLinks(polyOut.Pts);
        }
        JoinCommonEdges();
        for (int index = 0; index < m_PolyOuts.Count; ++index)
        {
          OutRec polyOut = m_PolyOuts[index];
          if (polyOut.Pts != null && !polyOut.IsOpen)
            FixupOutPolygon(polyOut);
        }
        if (StrictlySimple)
          DoSimplePolygons();
        return true;
      }
      finally
      {
        m_Joins.Clear();
        m_GhostJoins.Clear();
      }
    }

    private long PopScanbeam()
    {
      long y = m_Scanbeam.Y;
      m_Scanbeam = m_Scanbeam.Next;
      return y;
    }

    private void DisposeAllPolyPts()
    {
      for (int index = 0; index < m_PolyOuts.Count; ++index)
        DisposeOutRec(index);
      m_PolyOuts.Clear();
    }

    private void DisposeOutRec(int index)
    {
      m_PolyOuts[index].Pts = null;
      m_PolyOuts[index] = null;
    }

    private void AddJoin(OutPt Op1, OutPt Op2, IntPoint OffPt)
    {
      m_Joins.Add(new Join {
        OutPt1 = Op1,
        OutPt2 = Op2,
        OffPt = OffPt
      });
    }

    private void AddGhostJoin(OutPt Op, IntPoint OffPt)
    {
      m_GhostJoins.Add(new Join {
        OutPt1 = Op,
        OffPt = OffPt
      });
    }

    private void InsertLocalMinimaIntoAEL(long botY)
    {
      while (m_CurrentLM != null && m_CurrentLM.Y == botY)
      {
        TEdge leftBound = m_CurrentLM.LeftBound;
        TEdge rightBound = m_CurrentLM.RightBound;
        PopLocalMinima();
        OutPt outPt = null;
        if (leftBound == null)
        {
          InsertEdgeIntoAEL(rightBound, null);
          SetWindingCount(rightBound);
          if (IsContributing(rightBound))
            outPt = AddOutPt(rightBound, rightBound.Bot);
        }
        else if (rightBound == null)
        {
          InsertEdgeIntoAEL(leftBound, null);
          SetWindingCount(leftBound);
          if (IsContributing(leftBound))
            outPt = AddOutPt(leftBound, leftBound.Bot);
          InsertScanbeam(leftBound.Top.Y);
        }
        else
        {
          InsertEdgeIntoAEL(leftBound, null);
          InsertEdgeIntoAEL(rightBound, leftBound);
          SetWindingCount(leftBound);
          rightBound.WindCnt = leftBound.WindCnt;
          rightBound.WindCnt2 = leftBound.WindCnt2;
          if (IsContributing(leftBound))
            outPt = AddLocalMinPoly(leftBound, rightBound, leftBound.Bot);
          InsertScanbeam(leftBound.Top.Y);
        }
        if (rightBound != null)
        {
          if (IsHorizontal(rightBound))
            AddEdgeToSEL(rightBound);
          else
            InsertScanbeam(rightBound.Top.Y);
        }
        if (leftBound != null && rightBound != null)
        {
          if (outPt != null && IsHorizontal(rightBound) && m_GhostJoins.Count > 0 && rightBound.WindDelta != 0)
          {
            for (int index = 0; index < m_GhostJoins.Count; ++index)
            {
              Join ghostJoin = m_GhostJoins[index];
              if (HorzSegmentsOverlap(ghostJoin.OutPt1.Pt.X, ghostJoin.OffPt.X, rightBound.Bot.X, rightBound.Top.X))
                AddJoin(ghostJoin.OutPt1, outPt, ghostJoin.OffPt);
            }
          }
          if (leftBound.OutIdx >= 0 && leftBound.PrevInAEL != null && leftBound.PrevInAEL.Curr.X == leftBound.Bot.X && leftBound.PrevInAEL.OutIdx >= 0 && SlopesEqual(leftBound.PrevInAEL, leftBound, m_UseFullRange) && leftBound.WindDelta != 0 && leftBound.PrevInAEL.WindDelta != 0)
          {
            OutPt Op2 = AddOutPt(leftBound.PrevInAEL, leftBound.Bot);
            AddJoin(outPt, Op2, leftBound.Top);
          }
          if (leftBound.NextInAEL != rightBound)
          {
            if (rightBound.OutIdx >= 0 && rightBound.PrevInAEL.OutIdx >= 0 && SlopesEqual(rightBound.PrevInAEL, rightBound, m_UseFullRange) && rightBound.WindDelta != 0 && rightBound.PrevInAEL.WindDelta != 0)
            {
              OutPt Op2 = AddOutPt(rightBound.PrevInAEL, rightBound.Bot);
              AddJoin(outPt, Op2, rightBound.Top);
            }
            TEdge nextInAel = leftBound.NextInAEL;
            if (nextInAel != null)
            {
              for (; nextInAel != rightBound; nextInAel = nextInAel.NextInAEL)
                IntersectEdges(rightBound, nextInAel, leftBound.Curr);
            }
          }
        }
      }
    }

    private void InsertEdgeIntoAEL(TEdge edge, TEdge startEdge)
    {
      if (m_ActiveEdges == null)
      {
        edge.PrevInAEL = null;
        edge.NextInAEL = null;
        m_ActiveEdges = edge;
      }
      else if (startEdge == null && E2InsertsBeforeE1(m_ActiveEdges, edge))
      {
        edge.PrevInAEL = null;
        edge.NextInAEL = m_ActiveEdges;
        m_ActiveEdges.PrevInAEL = edge;
        m_ActiveEdges = edge;
      }
      else
      {
        if (startEdge == null)
          startEdge = m_ActiveEdges;
        while (startEdge.NextInAEL != null && !E2InsertsBeforeE1(startEdge.NextInAEL, edge))
          startEdge = startEdge.NextInAEL;
        edge.NextInAEL = startEdge.NextInAEL;
        if (startEdge.NextInAEL != null)
          startEdge.NextInAEL.PrevInAEL = edge;
        edge.PrevInAEL = startEdge;
        startEdge.NextInAEL = edge;
      }
    }

    private bool E2InsertsBeforeE1(TEdge e1, TEdge e2)
    {
      if (e2.Curr.X != e1.Curr.X)
        return e2.Curr.X < e1.Curr.X;
      return e2.Top.Y > e1.Top.Y ? e2.Top.X < TopX(e1, e2.Top.Y) : e1.Top.X > TopX(e2, e1.Top.Y);
    }

    private bool IsEvenOddFillType(TEdge edge)
    {
      return edge.PolyTyp == PolyType.ptSubject ? m_SubjFillType == PolyFillType.pftEvenOdd : m_ClipFillType == PolyFillType.pftEvenOdd;
    }

    private bool IsEvenOddAltFillType(TEdge edge)
    {
      return edge.PolyTyp == PolyType.ptSubject ? m_ClipFillType == PolyFillType.pftEvenOdd : m_SubjFillType == PolyFillType.pftEvenOdd;
    }

    private bool IsContributing(TEdge edge)
    {
      PolyFillType polyFillType1;
      PolyFillType polyFillType2;
      if (edge.PolyTyp == PolyType.ptSubject)
      {
        polyFillType1 = m_SubjFillType;
        polyFillType2 = m_ClipFillType;
      }
      else
      {
        polyFillType1 = m_ClipFillType;
        polyFillType2 = m_SubjFillType;
      }
      switch (polyFillType1)
      {
        case PolyFillType.pftEvenOdd:
          if (edge.WindDelta == 0 && edge.WindCnt != 1)
            return false;
          break;
        case PolyFillType.pftNonZero:
          if (Math.Abs(edge.WindCnt) != 1)
            return false;
          break;
        case PolyFillType.pftPositive:
          if (edge.WindCnt != 1)
            return false;
          break;
        default:
          if (edge.WindCnt != -1)
            return false;
          break;
      }
      switch (m_ClipType)
      {
        case ClipType.ctIntersection:
          switch (polyFillType2)
          {
            case PolyFillType.pftEvenOdd:
            case PolyFillType.pftNonZero:
              return edge.WindCnt2 != 0;
            case PolyFillType.pftPositive:
              return edge.WindCnt2 > 0;
            default:
              return edge.WindCnt2 < 0;
          }
        case ClipType.ctUnion:
          switch (polyFillType2)
          {
            case PolyFillType.pftEvenOdd:
            case PolyFillType.pftNonZero:
              return edge.WindCnt2 == 0;
            case PolyFillType.pftPositive:
              return edge.WindCnt2 <= 0;
            default:
              return edge.WindCnt2 >= 0;
          }
        case ClipType.ctDifference:
          if (edge.PolyTyp == PolyType.ptSubject)
          {
            switch (polyFillType2)
            {
              case PolyFillType.pftEvenOdd:
              case PolyFillType.pftNonZero:
                return edge.WindCnt2 == 0;
              case PolyFillType.pftPositive:
                return edge.WindCnt2 <= 0;
              default:
                return edge.WindCnt2 >= 0;
            }
          }

          switch (polyFillType2)
          {
            case PolyFillType.pftEvenOdd:
            case PolyFillType.pftNonZero:
              return edge.WindCnt2 != 0;
            case PolyFillType.pftPositive:
              return edge.WindCnt2 > 0;
            default:
              return edge.WindCnt2 < 0;
          }
        case ClipType.ctXor:
          if (edge.WindDelta != 0)
            return true;
          switch (polyFillType2)
          {
            case PolyFillType.pftEvenOdd:
            case PolyFillType.pftNonZero:
              return edge.WindCnt2 == 0;
            case PolyFillType.pftPositive:
              return edge.WindCnt2 <= 0;
            default:
              return edge.WindCnt2 >= 0;
          }
        default:
          return true;
      }
    }

    private void SetWindingCount(TEdge edge)
    {
      TEdge prevInAel1 = edge.PrevInAEL;
      while (prevInAel1 != null && (prevInAel1.PolyTyp != edge.PolyTyp || prevInAel1.WindDelta == 0))
        prevInAel1 = prevInAel1.PrevInAEL;
      TEdge tedge;
      if (prevInAel1 == null)
      {
        edge.WindCnt = edge.WindDelta == 0 ? 1 : edge.WindDelta;
        edge.WindCnt2 = 0;
        tedge = m_ActiveEdges;
      }
      else if (edge.WindDelta == 0 && m_ClipType != ClipType.ctUnion)
      {
        edge.WindCnt = 1;
        edge.WindCnt2 = prevInAel1.WindCnt2;
        tedge = prevInAel1.NextInAEL;
      }
      else if (IsEvenOddFillType(edge))
      {
        if (edge.WindDelta == 0)
        {
          bool flag = true;
          for (TEdge prevInAel2 = prevInAel1.PrevInAEL; prevInAel2 != null; prevInAel2 = prevInAel2.PrevInAEL)
          {
            if (prevInAel2.PolyTyp == prevInAel1.PolyTyp && prevInAel2.WindDelta != 0)
              flag = !flag;
          }
          edge.WindCnt = flag ? 0 : 1;
        }
        else
          edge.WindCnt = edge.WindDelta;
        edge.WindCnt2 = prevInAel1.WindCnt2;
        tedge = prevInAel1.NextInAEL;
      }
      else
      {
        edge.WindCnt = prevInAel1.WindCnt * prevInAel1.WindDelta >= 0 ? (edge.WindDelta != 0 ? (prevInAel1.WindDelta * edge.WindDelta >= 0 ? prevInAel1.WindCnt + edge.WindDelta : prevInAel1.WindCnt) : (prevInAel1.WindCnt < 0 ? prevInAel1.WindCnt - 1 : prevInAel1.WindCnt + 1)) : (Math.Abs(prevInAel1.WindCnt) <= 1 ? (edge.WindDelta == 0 ? 1 : edge.WindDelta) : (prevInAel1.WindDelta * edge.WindDelta >= 0 ? prevInAel1.WindCnt + edge.WindDelta : prevInAel1.WindCnt));
        edge.WindCnt2 = prevInAel1.WindCnt2;
        tedge = prevInAel1.NextInAEL;
      }
      if (IsEvenOddAltFillType(edge))
      {
        for (; tedge != edge; tedge = tedge.NextInAEL)
        {
          if (tedge.WindDelta != 0)
            edge.WindCnt2 = edge.WindCnt2 == 0 ? 1 : 0;
        }
      }
      else
      {
        for (; tedge != edge; tedge = tedge.NextInAEL)
          edge.WindCnt2 += tedge.WindDelta;
      }
    }

    private void AddEdgeToSEL(TEdge edge)
    {
      if (m_SortedEdges == null)
      {
        m_SortedEdges = edge;
        edge.PrevInSEL = null;
        edge.NextInSEL = null;
      }
      else
      {
        edge.NextInSEL = m_SortedEdges;
        edge.PrevInSEL = null;
        m_SortedEdges.PrevInSEL = edge;
        m_SortedEdges = edge;
      }
    }

    private void CopyAELToSEL()
    {
      TEdge tedge = m_ActiveEdges;
      m_SortedEdges = tedge;
      for (; tedge != null; tedge = tedge.NextInAEL)
      {
        tedge.PrevInSEL = tedge.PrevInAEL;
        tedge.NextInSEL = tedge.NextInAEL;
      }
    }

    private void SwapPositionsInAEL(TEdge edge1, TEdge edge2)
    {
      if (edge1.NextInAEL == edge1.PrevInAEL || edge2.NextInAEL == edge2.PrevInAEL)
        return;
      if (edge1.NextInAEL == edge2)
      {
        TEdge nextInAel = edge2.NextInAEL;
        if (nextInAel != null)
          nextInAel.PrevInAEL = edge1;
        TEdge prevInAel = edge1.PrevInAEL;
        if (prevInAel != null)
          prevInAel.NextInAEL = edge2;
        edge2.PrevInAEL = prevInAel;
        edge2.NextInAEL = edge1;
        edge1.PrevInAEL = edge2;
        edge1.NextInAEL = nextInAel;
      }
      else if (edge2.NextInAEL == edge1)
      {
        TEdge nextInAel = edge1.NextInAEL;
        if (nextInAel != null)
          nextInAel.PrevInAEL = edge2;
        TEdge prevInAel = edge2.PrevInAEL;
        if (prevInAel != null)
          prevInAel.NextInAEL = edge1;
        edge1.PrevInAEL = prevInAel;
        edge1.NextInAEL = edge2;
        edge2.PrevInAEL = edge1;
        edge2.NextInAEL = nextInAel;
      }
      else
      {
        TEdge nextInAel = edge1.NextInAEL;
        TEdge prevInAel = edge1.PrevInAEL;
        edge1.NextInAEL = edge2.NextInAEL;
        if (edge1.NextInAEL != null)
          edge1.NextInAEL.PrevInAEL = edge1;
        edge1.PrevInAEL = edge2.PrevInAEL;
        if (edge1.PrevInAEL != null)
          edge1.PrevInAEL.NextInAEL = edge1;
        edge2.NextInAEL = nextInAel;
        if (edge2.NextInAEL != null)
          edge2.NextInAEL.PrevInAEL = edge2;
        edge2.PrevInAEL = prevInAel;
        if (edge2.PrevInAEL != null)
          edge2.PrevInAEL.NextInAEL = edge2;
      }
      if (edge1.PrevInAEL == null)
      {
        m_ActiveEdges = edge1;
      }
      else
      {
        if (edge2.PrevInAEL != null)
          return;
        m_ActiveEdges = edge2;
      }
    }

    private void SwapPositionsInSEL(TEdge edge1, TEdge edge2)
    {
      if (edge1.NextInSEL == null && edge1.PrevInSEL == null || edge2.NextInSEL == null && edge2.PrevInSEL == null)
        return;
      if (edge1.NextInSEL == edge2)
      {
        TEdge nextInSel = edge2.NextInSEL;
        if (nextInSel != null)
          nextInSel.PrevInSEL = edge1;
        TEdge prevInSel = edge1.PrevInSEL;
        if (prevInSel != null)
          prevInSel.NextInSEL = edge2;
        edge2.PrevInSEL = prevInSel;
        edge2.NextInSEL = edge1;
        edge1.PrevInSEL = edge2;
        edge1.NextInSEL = nextInSel;
      }
      else if (edge2.NextInSEL == edge1)
      {
        TEdge nextInSel = edge1.NextInSEL;
        if (nextInSel != null)
          nextInSel.PrevInSEL = edge2;
        TEdge prevInSel = edge2.PrevInSEL;
        if (prevInSel != null)
          prevInSel.NextInSEL = edge1;
        edge1.PrevInSEL = prevInSel;
        edge1.NextInSEL = edge2;
        edge2.PrevInSEL = edge1;
        edge2.NextInSEL = nextInSel;
      }
      else
      {
        TEdge nextInSel = edge1.NextInSEL;
        TEdge prevInSel = edge1.PrevInSEL;
        edge1.NextInSEL = edge2.NextInSEL;
        if (edge1.NextInSEL != null)
          edge1.NextInSEL.PrevInSEL = edge1;
        edge1.PrevInSEL = edge2.PrevInSEL;
        if (edge1.PrevInSEL != null)
          edge1.PrevInSEL.NextInSEL = edge1;
        edge2.NextInSEL = nextInSel;
        if (edge2.NextInSEL != null)
          edge2.NextInSEL.PrevInSEL = edge2;
        edge2.PrevInSEL = prevInSel;
        if (edge2.PrevInSEL != null)
          edge2.PrevInSEL.NextInSEL = edge2;
      }
      if (edge1.PrevInSEL == null)
      {
        m_SortedEdges = edge1;
      }
      else
      {
        if (edge2.PrevInSEL != null)
          return;
        m_SortedEdges = edge2;
      }
    }

    private void AddLocalMaxPoly(TEdge e1, TEdge e2, IntPoint pt)
    {
      AddOutPt(e1, pt);
      if (e2.WindDelta == 0)
        AddOutPt(e2, pt);
      if (e1.OutIdx == e2.OutIdx)
      {
        e1.OutIdx = -1;
        e2.OutIdx = -1;
      }
      else if (e1.OutIdx < e2.OutIdx)
        AppendPolygon(e1, e2);
      else
        AppendPolygon(e2, e1);
    }

    private OutPt AddLocalMinPoly(TEdge e1, TEdge e2, IntPoint pt)
    {
      OutPt Op1;
      TEdge tedge1;
      TEdge tedge2;
      if (IsHorizontal(e2) || e1.Dx > e2.Dx)
      {
        Op1 = AddOutPt(e1, pt);
        e2.OutIdx = e1.OutIdx;
        e1.Side = EdgeSide.esLeft;
        e2.Side = EdgeSide.esRight;
        tedge1 = e1;
        tedge2 = tedge1.PrevInAEL != e2 ? tedge1.PrevInAEL : e2.PrevInAEL;
      }
      else
      {
        Op1 = AddOutPt(e2, pt);
        e1.OutIdx = e2.OutIdx;
        e1.Side = EdgeSide.esRight;
        e2.Side = EdgeSide.esLeft;
        tedge1 = e2;
        tedge2 = tedge1.PrevInAEL != e1 ? tedge1.PrevInAEL : e1.PrevInAEL;
      }
      if (tedge2 != null && tedge2.OutIdx >= 0 && TopX(tedge2, pt.Y) == TopX(tedge1, pt.Y) && SlopesEqual(tedge1, tedge2, m_UseFullRange) && tedge1.WindDelta != 0 && tedge2.WindDelta != 0)
      {
        OutPt Op2 = AddOutPt(tedge2, pt);
        AddJoin(Op1, Op2, tedge1.Top);
      }
      return Op1;
    }

    private OutRec CreateOutRec()
    {
      OutRec outRec = new OutRec();
      outRec.Idx = -1;
      outRec.IsHole = false;
      outRec.IsOpen = false;
      outRec.FirstLeft = null;
      outRec.Pts = null;
      outRec.BottomPt = null;
      outRec.PolyNode = null;
      m_PolyOuts.Add(outRec);
      outRec.Idx = m_PolyOuts.Count - 1;
      return outRec;
    }

    private OutPt AddOutPt(TEdge e, IntPoint pt)
    {
      bool flag = e.Side == EdgeSide.esLeft;
      if (e.OutIdx < 0)
      {
        OutRec outRec = CreateOutRec();
        outRec.IsOpen = e.WindDelta == 0;
        OutPt outPt = new OutPt();
        outRec.Pts = outPt;
        outPt.Idx = outRec.Idx;
        outPt.Pt = pt;
        outPt.Next = outPt;
        outPt.Prev = outPt;
        if (!outRec.IsOpen)
          SetHoleState(e, outRec);
        e.OutIdx = outRec.Idx;
        return outPt;
      }
      OutRec polyOut = m_PolyOuts[e.OutIdx];
      OutPt pts = polyOut.Pts;
      if (flag && pt == pts.Pt)
        return pts;
      if (!flag && pt == pts.Prev.Pt)
        return pts.Prev;
      OutPt outPt1 = new OutPt {
        Idx = polyOut.Idx,
        Pt = pt,
        Next = pts,
        Prev = pts.Prev
      };
      outPt1.Prev.Next = outPt1;
      pts.Prev = outPt1;
      if (flag)
        polyOut.Pts = outPt1;
      return outPt1;
    }

    internal void SwapPoints(ref IntPoint pt1, ref IntPoint pt2)
    {
      IntPoint intPoint = new IntPoint(pt1);
      pt1 = pt2;
      pt2 = intPoint;
    }

    private bool HorzSegmentsOverlap(long seg1a, long seg1b, long seg2a, long seg2b)
    {
      if (seg1a > seg1b)
        Swap(ref seg1a, ref seg1b);
      if (seg2a > seg2b)
        Swap(ref seg2a, ref seg2b);
      return seg1a < seg2b && seg2a < seg1b;
    }

    private void SetHoleState(TEdge e, OutRec outRec)
    {
      bool flag = false;
      for (TEdge prevInAel = e.PrevInAEL; prevInAel != null; prevInAel = prevInAel.PrevInAEL)
      {
        if (prevInAel.OutIdx >= 0 && prevInAel.WindDelta != 0)
        {
          flag = !flag;
          if (outRec.FirstLeft == null)
            outRec.FirstLeft = m_PolyOuts[prevInAel.OutIdx];
        }
      }
      if (!flag)
        return;
      outRec.IsHole = true;
    }

    private double GetDx(IntPoint pt1, IntPoint pt2)
    {
      return pt1.Y == pt2.Y ? -3.4E+38 : (pt2.X - pt1.X) / (double) (pt2.Y - pt1.Y);
    }

    private bool FirstIsBottomPt(OutPt btmPt1, OutPt btmPt2)
    {
      OutPt prev1 = btmPt1.Prev;
      while (prev1.Pt == btmPt1.Pt && prev1 != btmPt1)
        prev1 = prev1.Prev;
      double num1 = Math.Abs(GetDx(btmPt1.Pt, prev1.Pt));
      OutPt next1 = btmPt1.Next;
      while (next1.Pt == btmPt1.Pt && next1 != btmPt1)
        next1 = next1.Next;
      double num2 = Math.Abs(GetDx(btmPt1.Pt, next1.Pt));
      OutPt prev2 = btmPt2.Prev;
      while (prev2.Pt == btmPt2.Pt && prev2 != btmPt2)
        prev2 = prev2.Prev;
      double num3 = Math.Abs(GetDx(btmPt2.Pt, prev2.Pt));
      OutPt next2 = btmPt2.Next;
      while (next2.Pt == btmPt2.Pt && next2 != btmPt2)
        next2 = next2.Next;
      double num4 = Math.Abs(GetDx(btmPt2.Pt, next2.Pt));
      return num1 >= num3 && num1 >= num4 || num2 >= num3 && num2 >= num4;
    }

    private OutPt GetBottomPt(OutPt pp)
    {
      OutPt btmPt2 = null;
      OutPt next;
      for (next = pp.Next; next != pp; next = next.Next)
      {
        if (next.Pt.Y > pp.Pt.Y)
        {
          pp = next;
          btmPt2 = null;
        }
        else if (next.Pt.Y == pp.Pt.Y && next.Pt.X <= pp.Pt.X)
        {
          if (next.Pt.X < pp.Pt.X)
          {
            btmPt2 = null;
            pp = next;
          }
          else if (next.Next != pp && next.Prev != pp)
            btmPt2 = next;
        }
      }
      if (btmPt2 != null)
      {
        while (btmPt2 != next)
        {
          if (!FirstIsBottomPt(next, btmPt2))
            pp = btmPt2;
          btmPt2 = btmPt2.Next;
          while (btmPt2.Pt != pp.Pt)
            btmPt2 = btmPt2.Next;
        }
      }
      return pp;
    }

    private OutRec GetLowermostRec(OutRec outRec1, OutRec outRec2)
    {
      if (outRec1.BottomPt == null)
        outRec1.BottomPt = GetBottomPt(outRec1.Pts);
      if (outRec2.BottomPt == null)
        outRec2.BottomPt = GetBottomPt(outRec2.Pts);
      OutPt bottomPt1 = outRec1.BottomPt;
      OutPt bottomPt2 = outRec2.BottomPt;
      return bottomPt1.Pt.Y > bottomPt2.Pt.Y || bottomPt1.Pt.Y >= bottomPt2.Pt.Y && (bottomPt1.Pt.X < bottomPt2.Pt.X || bottomPt1.Pt.X <= bottomPt2.Pt.X && bottomPt1.Next != bottomPt1 && (bottomPt2.Next == bottomPt2 || FirstIsBottomPt(bottomPt1, bottomPt2))) ? outRec1 : outRec2;
    }

    private bool Param1RightOfParam2(OutRec outRec1, OutRec outRec2)
    {
      do
      {
        outRec1 = outRec1.FirstLeft;
        if (outRec1 == outRec2)
          return true;
      }
      while (outRec1 != null);
      return false;
    }

    private OutRec GetOutRec(int idx)
    {
      OutRec polyOut = m_PolyOuts[idx];
      while (polyOut != m_PolyOuts[polyOut.Idx])
        polyOut = m_PolyOuts[polyOut.Idx];
      return polyOut;
    }

    private void AppendPolygon(TEdge e1, TEdge e2)
    {
      OutRec polyOut1 = m_PolyOuts[e1.OutIdx];
      OutRec polyOut2 = m_PolyOuts[e2.OutIdx];
      OutRec outRec = !Param1RightOfParam2(polyOut1, polyOut2) ? (!Param1RightOfParam2(polyOut2, polyOut1) ? GetLowermostRec(polyOut1, polyOut2) : polyOut1) : polyOut2;
      OutPt pts1 = polyOut1.Pts;
      OutPt prev1 = pts1.Prev;
      OutPt pts2 = polyOut2.Pts;
      OutPt prev2 = pts2.Prev;
      EdgeSide edgeSide;
      if (e1.Side == EdgeSide.esLeft)
      {
        if (e2.Side == EdgeSide.esLeft)
        {
          ReversePolyPtLinks(pts2);
          pts2.Next = pts1;
          pts1.Prev = pts2;
          prev1.Next = prev2;
          prev2.Prev = prev1;
          polyOut1.Pts = prev2;
        }
        else
        {
          prev2.Next = pts1;
          pts1.Prev = prev2;
          pts2.Prev = prev1;
          prev1.Next = pts2;
          polyOut1.Pts = pts2;
        }
        edgeSide = EdgeSide.esLeft;
      }
      else
      {
        if (e2.Side == EdgeSide.esRight)
        {
          ReversePolyPtLinks(pts2);
          prev1.Next = prev2;
          prev2.Prev = prev1;
          pts2.Next = pts1;
          pts1.Prev = pts2;
        }
        else
        {
          prev1.Next = pts2;
          pts2.Prev = prev1;
          pts1.Prev = prev2;
          prev2.Next = pts1;
        }
        edgeSide = EdgeSide.esRight;
      }
      polyOut1.BottomPt = null;
      if (outRec == polyOut2)
      {
        if (polyOut2.FirstLeft != polyOut1)
          polyOut1.FirstLeft = polyOut2.FirstLeft;
        polyOut1.IsHole = polyOut2.IsHole;
      }
      polyOut2.Pts = null;
      polyOut2.BottomPt = null;
      polyOut2.FirstLeft = polyOut1;
      int outIdx1 = e1.OutIdx;
      int outIdx2 = e2.OutIdx;
      e1.OutIdx = -1;
      e2.OutIdx = -1;
      for (TEdge tedge = m_ActiveEdges; tedge != null; tedge = tedge.NextInAEL)
      {
        if (tedge.OutIdx == outIdx2)
        {
          tedge.OutIdx = outIdx1;
          tedge.Side = edgeSide;
          break;
        }
      }
      polyOut2.Idx = polyOut1.Idx;
    }

    private void ReversePolyPtLinks(OutPt pp)
    {
      if (pp == null)
        return;
      OutPt outPt = pp;
      do
      {
        OutPt next = outPt.Next;
        outPt.Next = outPt.Prev;
        outPt.Prev = next;
        outPt = next;
      }
      while (outPt != pp);
    }

    private static void SwapSides(TEdge edge1, TEdge edge2)
    {
      EdgeSide side = edge1.Side;
      edge1.Side = edge2.Side;
      edge2.Side = side;
    }

    private static void SwapPolyIndexes(TEdge edge1, TEdge edge2)
    {
      int outIdx = edge1.OutIdx;
      edge1.OutIdx = edge2.OutIdx;
      edge2.OutIdx = outIdx;
    }

    private void IntersectEdges(TEdge e1, TEdge e2, IntPoint pt)
    {
      bool flag1 = e1.OutIdx >= 0;
      bool flag2 = e2.OutIdx >= 0;
      if (e1.PolyTyp == e2.PolyTyp)
      {
        if (IsEvenOddFillType(e1))
        {
          int windCnt = e1.WindCnt;
          e1.WindCnt = e2.WindCnt;
          e2.WindCnt = windCnt;
        }
        else
        {
          if (e1.WindCnt + e2.WindDelta == 0)
            e1.WindCnt = -e1.WindCnt;
          else
            e1.WindCnt += e2.WindDelta;
          if (e2.WindCnt - e1.WindDelta == 0)
            e2.WindCnt = -e2.WindCnt;
          else
            e2.WindCnt -= e1.WindDelta;
        }
      }
      else
      {
        if (!IsEvenOddFillType(e2))
          e1.WindCnt2 += e2.WindDelta;
        else
          e1.WindCnt2 = e1.WindCnt2 == 0 ? 1 : 0;
        if (!IsEvenOddFillType(e1))
          e2.WindCnt2 -= e1.WindDelta;
        else
          e2.WindCnt2 = e2.WindCnt2 == 0 ? 1 : 0;
      }
      PolyFillType polyFillType1;
      PolyFillType polyFillType2;
      if (e1.PolyTyp == PolyType.ptSubject)
      {
        polyFillType1 = m_SubjFillType;
        polyFillType2 = m_ClipFillType;
      }
      else
      {
        polyFillType1 = m_ClipFillType;
        polyFillType2 = m_SubjFillType;
      }
      PolyFillType polyFillType3;
      PolyFillType polyFillType4;
      if (e2.PolyTyp == PolyType.ptSubject)
      {
        polyFillType3 = m_SubjFillType;
        polyFillType4 = m_ClipFillType;
      }
      else
      {
        polyFillType3 = m_ClipFillType;
        polyFillType4 = m_SubjFillType;
      }
      int num1;
      switch (polyFillType1)
      {
        case PolyFillType.pftPositive:
          num1 = e1.WindCnt;
          break;
        case PolyFillType.pftNegative:
          num1 = -e1.WindCnt;
          break;
        default:
          num1 = Math.Abs(e1.WindCnt);
          break;
      }
      int num2;
      switch (polyFillType3)
      {
        case PolyFillType.pftPositive:
          num2 = e2.WindCnt;
          break;
        case PolyFillType.pftNegative:
          num2 = -e2.WindCnt;
          break;
        default:
          num2 = Math.Abs(e2.WindCnt);
          break;
      }
      if (flag1 & flag2)
      {
        if (num1 != 0 && num1 != 1 || num2 != 0 && num2 != 1 || e1.PolyTyp != e2.PolyTyp && m_ClipType != ClipType.ctXor)
        {
          AddLocalMaxPoly(e1, e2, pt);
        }
        else
        {
          AddOutPt(e1, pt);
          AddOutPt(e2, pt);
          SwapSides(e1, e2);
          SwapPolyIndexes(e1, e2);
        }
      }
      else if (flag1)
      {
        if (num2 != 0 && num2 != 1)
          return;
        AddOutPt(e1, pt);
        SwapSides(e1, e2);
        SwapPolyIndexes(e1, e2);
      }
      else if (flag2)
      {
        if (num1 != 0 && num1 != 1)
          return;
        AddOutPt(e2, pt);
        SwapSides(e1, e2);
        SwapPolyIndexes(e1, e2);
      }
      else
      {
        if (num1 != 0 && num1 != 1 || num2 != 0 && num2 != 1)
          return;
        long num3;
        switch (polyFillType2)
        {
          case PolyFillType.pftPositive:
            num3 = e1.WindCnt2;
            break;
          case PolyFillType.pftNegative:
            num3 = -e1.WindCnt2;
            break;
          default:
            num3 = Math.Abs(e1.WindCnt2);
            break;
        }
        long num4;
        switch (polyFillType4)
        {
          case PolyFillType.pftPositive:
            num4 = e2.WindCnt2;
            break;
          case PolyFillType.pftNegative:
            num4 = -e2.WindCnt2;
            break;
          default:
            num4 = Math.Abs(e2.WindCnt2);
            break;
        }
        if (e1.PolyTyp != e2.PolyTyp)
          AddLocalMinPoly(e1, e2, pt);
        else if (num1 == 1 && num2 == 1)
        {
          switch (m_ClipType)
          {
            case ClipType.ctIntersection:
              if (num3 > 0L && num4 > 0L)
              {
                AddLocalMinPoly(e1, e2, pt);
              }
              break;
            case ClipType.ctUnion:
              if (num3 <= 0L && num4 <= 0L)
              {
                AddLocalMinPoly(e1, e2, pt);
              }
              break;
            case ClipType.ctDifference:
              if (e1.PolyTyp == PolyType.ptClip && num3 > 0L && num4 > 0L || e1.PolyTyp == PolyType.ptSubject && num3 <= 0L && num4 <= 0L)
              {
                AddLocalMinPoly(e1, e2, pt);
              }
              break;
            case ClipType.ctXor:
              AddLocalMinPoly(e1, e2, pt);
              break;
          }
        }
        else
          SwapSides(e1, e2);
      }
    }

    private void DeleteFromAEL(TEdge e)
    {
      TEdge prevInAel = e.PrevInAEL;
      TEdge nextInAel = e.NextInAEL;
      if (prevInAel == null && nextInAel == null && e != m_ActiveEdges)
        return;
      if (prevInAel != null)
        prevInAel.NextInAEL = nextInAel;
      else
        m_ActiveEdges = nextInAel;
      if (nextInAel != null)
        nextInAel.PrevInAEL = prevInAel;
      e.NextInAEL = null;
      e.PrevInAEL = null;
    }

    private void DeleteFromSEL(TEdge e)
    {
      TEdge prevInSel = e.PrevInSEL;
      TEdge nextInSel = e.NextInSEL;
      if (prevInSel == null && nextInSel == null && e != m_SortedEdges)
        return;
      if (prevInSel != null)
        prevInSel.NextInSEL = nextInSel;
      else
        m_SortedEdges = nextInSel;
      if (nextInSel != null)
        nextInSel.PrevInSEL = prevInSel;
      e.NextInSEL = null;
      e.PrevInSEL = null;
    }

    private void UpdateEdgeIntoAEL(ref TEdge e)
    {
      if (e.NextInLML == null)
        throw new ClipperException("UpdateEdgeIntoAEL: invalid call");
      TEdge prevInAel = e.PrevInAEL;
      TEdge nextInAel = e.NextInAEL;
      e.NextInLML.OutIdx = e.OutIdx;
      if (prevInAel != null)
        prevInAel.NextInAEL = e.NextInLML;
      else
        m_ActiveEdges = e.NextInLML;
      if (nextInAel != null)
        nextInAel.PrevInAEL = e.NextInLML;
      e.NextInLML.Side = e.Side;
      e.NextInLML.WindDelta = e.WindDelta;
      e.NextInLML.WindCnt = e.WindCnt;
      e.NextInLML.WindCnt2 = e.WindCnt2;
      e = e.NextInLML;
      e.Curr = e.Bot;
      e.PrevInAEL = prevInAel;
      e.NextInAEL = nextInAel;
      if (IsHorizontal(e))
        return;
      InsertScanbeam(e.Top.Y);
    }

    private void ProcessHorizontals(bool isTopOfScanbeam)
    {
      for (TEdge sortedEdges = m_SortedEdges; sortedEdges != null; sortedEdges = m_SortedEdges)
      {
        DeleteFromSEL(sortedEdges);
        ProcessHorizontal(sortedEdges, isTopOfScanbeam);
      }
    }

    private void GetHorzDirection(
      TEdge HorzEdge,
      out Direction Dir,
      out long Left,
      out long Right)
    {
      if (HorzEdge.Bot.X < HorzEdge.Top.X)
      {
        Left = HorzEdge.Bot.X;
        Right = HorzEdge.Top.X;
        Dir = Direction.dLeftToRight;
      }
      else
      {
        Left = HorzEdge.Top.X;
        Right = HorzEdge.Bot.X;
        Dir = Direction.dRightToLeft;
      }
    }

    private void ProcessHorizontal(TEdge horzEdge, bool isTopOfScanbeam)
    {
      GetHorzDirection(horzEdge, out Direction Dir, out long Left, out long Right);
      TEdge e1 = horzEdge;
      TEdge tedge1 = null;
      while (e1.NextInLML != null && IsHorizontal(e1.NextInLML))
        e1 = e1.NextInLML;
      if (e1.NextInLML == null)
        tedge1 = GetMaximaPair(e1);
      while (true)
      {
        bool flag = horzEdge == e1;
        TEdge nextInAel;
        for (TEdge tedge2 = GetNextInAEL(horzEdge, Dir); tedge2 != null && (tedge2.Curr.X != horzEdge.Top.X || horzEdge.NextInLML == null || tedge2.Dx >= horzEdge.NextInLML.Dx); tedge2 = nextInAel)
        {
          nextInAel = GetNextInAEL(tedge2, Dir);
          if (Dir == Direction.dLeftToRight && tedge2.Curr.X <= Right || Dir == Direction.dRightToLeft && tedge2.Curr.X >= Left)
          {
            if (tedge2 == tedge1 & flag)
            {
              if (horzEdge.OutIdx >= 0)
              {
                OutPt outPt = AddOutPt(horzEdge, horzEdge.Top);
                for (TEdge e2 = m_SortedEdges; e2 != null; e2 = e2.NextInSEL)
                {
                  if (e2.OutIdx >= 0 && HorzSegmentsOverlap(horzEdge.Bot.X, horzEdge.Top.X, e2.Bot.X, e2.Top.X))
                    AddJoin(AddOutPt(e2, e2.Bot), outPt, e2.Top);
                }
                AddGhostJoin(outPt, horzEdge.Bot);
                AddLocalMaxPoly(horzEdge, tedge1, horzEdge.Top);
              }
              DeleteFromAEL(horzEdge);
              DeleteFromAEL(tedge1);
              return;
            }
            if (Dir == Direction.dLeftToRight)
            {
              IntPoint pt = new IntPoint(tedge2.Curr.X, horzEdge.Curr.Y);
              IntersectEdges(horzEdge, tedge2, pt);
            }
            else
            {
              IntPoint pt = new IntPoint(tedge2.Curr.X, horzEdge.Curr.Y);
              IntersectEdges(tedge2, horzEdge, pt);
            }
            SwapPositionsInAEL(horzEdge, tedge2);
          }
          else if (Dir == Direction.dLeftToRight && tedge2.Curr.X >= Right || Dir == Direction.dRightToLeft && tedge2.Curr.X <= Left)
            break;
        }
        if (horzEdge.NextInLML != null && IsHorizontal(horzEdge.NextInLML))
        {
          UpdateEdgeIntoAEL(ref horzEdge);
          if (horzEdge.OutIdx >= 0)
            AddOutPt(horzEdge, horzEdge.Bot);
          GetHorzDirection(horzEdge, out Dir, out Left, out Right);
        }
        else
          break;
      }
      if (horzEdge.NextInLML != null)
      {
        if (horzEdge.OutIdx >= 0)
        {
          OutPt outPt = AddOutPt(horzEdge, horzEdge.Top);
          if (isTopOfScanbeam)
            AddGhostJoin(outPt, horzEdge.Bot);
          UpdateEdgeIntoAEL(ref horzEdge);
          if (horzEdge.WindDelta == 0)
            return;
          TEdge prevInAel = horzEdge.PrevInAEL;
          TEdge nextInAel = horzEdge.NextInAEL;
          if (prevInAel != null && prevInAel.Curr.X == horzEdge.Bot.X && prevInAel.Curr.Y == horzEdge.Bot.Y && prevInAel.WindDelta != 0 && prevInAel.OutIdx >= 0 && prevInAel.Curr.Y > prevInAel.Top.Y && SlopesEqual(horzEdge, prevInAel, m_UseFullRange))
          {
            OutPt Op2 = AddOutPt(prevInAel, horzEdge.Bot);
            AddJoin(outPt, Op2, horzEdge.Top);
          }
          else
          {
            if (nextInAel == null || nextInAel.Curr.X != horzEdge.Bot.X || nextInAel.Curr.Y != horzEdge.Bot.Y || nextInAel.WindDelta == 0 || nextInAel.OutIdx < 0 || nextInAel.Curr.Y <= nextInAel.Top.Y || !SlopesEqual(horzEdge, nextInAel, m_UseFullRange))
              return;
            OutPt Op2 = AddOutPt(nextInAel, horzEdge.Bot);
            AddJoin(outPt, Op2, horzEdge.Top);
          }
        }
        else
          UpdateEdgeIntoAEL(ref horzEdge);
      }
      else
      {
        if (horzEdge.OutIdx >= 0)
          AddOutPt(horzEdge, horzEdge.Top);
        DeleteFromAEL(horzEdge);
      }
    }

    private TEdge GetNextInAEL(TEdge e, Direction Direction)
    {
      return Direction == Direction.dLeftToRight ? e.NextInAEL : e.PrevInAEL;
    }

    private bool IsMinima(TEdge e) => e != null && e.Prev.NextInLML != e && e.Next.NextInLML != e;

    private bool IsMaxima(TEdge e, double Y)
    {
      return e != null && e.Top.Y == Y && e.NextInLML == null;
    }

    private bool IsIntermediate(TEdge e, double Y) => e.Top.Y == Y && e.NextInLML != null;

    private TEdge GetMaximaPair(TEdge e)
    {
      TEdge e1 = null;
      if (e.Next.Top == e.Top && e.Next.NextInLML == null)
        e1 = e.Next;
      else if (e.Prev.Top == e.Top && e.Prev.NextInLML == null)
        e1 = e.Prev;
      return e1 != null && (e1.OutIdx == -2 || e1.NextInAEL == e1.PrevInAEL && !IsHorizontal(e1)) ? null : e1;
    }

    private bool ProcessIntersections(long topY)
    {
      if (m_ActiveEdges == null)
        return true;
      try
      {
        BuildIntersectList(topY);
        if (m_IntersectList.Count == 0)
          return true;
        if (m_IntersectList.Count != 1 && !FixupIntersectionOrder())
          return false;
        ProcessIntersectList();
      }
      catch
      {
        m_SortedEdges = null;
        m_IntersectList.Clear();
        throw new ClipperException("ProcessIntersections error");
      }
      m_SortedEdges = null;
      return true;
    }

    private void BuildIntersectList(long topY)
    {
      if (m_ActiveEdges == null)
        return;
      TEdge edge = m_ActiveEdges;
      m_SortedEdges = edge;
      for (; edge != null; edge = edge.NextInAEL)
      {
        edge.PrevInSEL = edge.PrevInAEL;
        edge.NextInSEL = edge.NextInAEL;
        edge.Curr.X = TopX(edge, topY);
      }
      bool flag = true;
      while (flag && m_SortedEdges != null)
      {
        flag = false;
        TEdge edge1 = m_SortedEdges;
        while (edge1.NextInSEL != null)
        {
          TEdge nextInSel = edge1.NextInSEL;
          if (edge1.Curr.X > nextInSel.Curr.X)
          {
            IntersectPoint(edge1, nextInSel, out IntPoint ip);
            m_IntersectList.Add(new IntersectNode {
              Edge1 = edge1,
              Edge2 = nextInSel,
              Pt = ip
            });
            SwapPositionsInSEL(edge1, nextInSel);
            flag = true;
          }
          else
            edge1 = nextInSel;
        }
        if (edge1.PrevInSEL != null)
          edge1.PrevInSEL.NextInSEL = null;
        else
          break;
      }
      m_SortedEdges = null;
    }

    private bool EdgesAdjacent(IntersectNode inode)
    {
      return inode.Edge1.NextInSEL == inode.Edge2 || inode.Edge1.PrevInSEL == inode.Edge2;
    }

    private static int IntersectNodeSort(IntersectNode node1, IntersectNode node2)
    {
      return (int) (node2.Pt.Y - node1.Pt.Y);
    }

    private bool FixupIntersectionOrder()
    {
      m_IntersectList.Sort(m_IntersectNodeComparer);
      CopyAELToSEL();
      int count = m_IntersectList.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        if (!EdgesAdjacent(m_IntersectList[index1]))
        {
          int index2 = index1 + 1;
          while (index2 < count && !EdgesAdjacent(m_IntersectList[index2]))
            ++index2;
          if (index2 == count)
            return false;
          IntersectNode intersect = m_IntersectList[index1];
          m_IntersectList[index1] = m_IntersectList[index2];
          m_IntersectList[index2] = intersect;
        }
        SwapPositionsInSEL(m_IntersectList[index1].Edge1, m_IntersectList[index1].Edge2);
      }
      return true;
    }

    private void ProcessIntersectList()
    {
      for (int index = 0; index < m_IntersectList.Count; ++index)
      {
        IntersectNode intersect = m_IntersectList[index];
        IntersectEdges(intersect.Edge1, intersect.Edge2, intersect.Pt);
        SwapPositionsInAEL(intersect.Edge1, intersect.Edge2);
      }
      m_IntersectList.Clear();
    }

    internal static long Round(double value)
    {
      return value < 0.0 ? (long) (value - 0.5) : (long) (value + 0.5);
    }

    private static long TopX(TEdge edge, long currentY)
    {
      return currentY == edge.Top.Y ? edge.Top.X : edge.Bot.X + Round(edge.Dx * (currentY - edge.Bot.Y));
    }

    private void IntersectPoint(TEdge edge1, TEdge edge2, out IntPoint ip)
    {
      ip = new IntPoint();
      if (edge1.Dx == edge2.Dx)
      {
        ip.Y = edge1.Curr.Y;
        ip.X = TopX(edge1, ip.Y);
      }
      else
      {
        if (edge1.Delta.X == 0L)
        {
          ip.X = edge1.Bot.X;
          if (IsHorizontal(edge2))
          {
            ip.Y = edge2.Bot.Y;
          }
          else
          {
            double num = edge2.Bot.Y - edge2.Bot.X / edge2.Dx;
            ip.Y = Round(ip.X / edge2.Dx + num);
          }
        }
        else if (edge2.Delta.X == 0L)
        {
          ip.X = edge2.Bot.X;
          if (IsHorizontal(edge1))
          {
            ip.Y = edge1.Bot.Y;
          }
          else
          {
            double num = edge1.Bot.Y - edge1.Bot.X / edge1.Dx;
            ip.Y = Round(ip.X / edge1.Dx + num);
          }
        }
        else
        {
          double num1 = edge1.Bot.X - edge1.Bot.Y * edge1.Dx;
          double num2 = edge2.Bot.X - edge2.Bot.Y * edge2.Dx;
          double num3 = (num2 - num1) / (edge1.Dx - edge2.Dx);
          ip.Y = Round(num3);
          ip.X = Math.Abs(edge1.Dx) >= Math.Abs(edge2.Dx) ? Round(edge2.Dx * num3 + num2) : Round(edge1.Dx * num3 + num1);
        }
        if (ip.Y < edge1.Top.Y || ip.Y < edge2.Top.Y)
        {
          ip.Y = edge1.Top.Y <= edge2.Top.Y ? edge2.Top.Y : edge1.Top.Y;
          ip.X = Math.Abs(edge1.Dx) >= Math.Abs(edge2.Dx) ? TopX(edge2, ip.Y) : TopX(edge1, ip.Y);
        }
        if (ip.Y <= edge1.Curr.Y)
          return;
        ip.Y = edge1.Curr.Y;
        ip.X = Math.Abs(edge1.Dx) <= Math.Abs(edge2.Dx) ? TopX(edge1, ip.Y) : TopX(edge2, ip.Y);
      }
    }

    private void ProcessEdgesAtTopOfScanbeam(long topY)
    {
      TEdge e1 = m_ActiveEdges;
      while (e1 != null)
      {
        bool flag = IsMaxima(e1, topY);
        if (flag)
        {
          TEdge maximaPair = GetMaximaPair(e1);
          flag = maximaPair == null || !IsHorizontal(maximaPair);
        }
        if (flag)
        {
          TEdge prevInAel = e1.PrevInAEL;
          DoMaxima(e1);
          e1 = prevInAel != null ? prevInAel.NextInAEL : m_ActiveEdges;
        }
        else
        {
          if (IsIntermediate(e1, topY) && IsHorizontal(e1.NextInLML))
          {
            UpdateEdgeIntoAEL(ref e1);
            if (e1.OutIdx >= 0)
              AddOutPt(e1, e1.Bot);
            AddEdgeToSEL(e1);
          }
          else
          {
            e1.Curr.X = TopX(e1, topY);
            e1.Curr.Y = topY;
          }
          if (StrictlySimple)
          {
            TEdge prevInAel = e1.PrevInAEL;
            if (e1.OutIdx >= 0 && e1.WindDelta != 0 && prevInAel != null && prevInAel.OutIdx >= 0 && prevInAel.Curr.X == e1.Curr.X && prevInAel.WindDelta != 0)
            {
              IntPoint intPoint = new IntPoint(e1.Curr);
              AddJoin(AddOutPt(prevInAel, intPoint), AddOutPt(e1, intPoint), intPoint);
            }
          }
          e1 = e1.NextInAEL;
        }
      }
      ProcessHorizontals(true);
      for (TEdge e2 = m_ActiveEdges; e2 != null; e2 = e2.NextInAEL)
      {
        if (IsIntermediate(e2, topY))
        {
          OutPt Op1 = null;
          if (e2.OutIdx >= 0)
            Op1 = AddOutPt(e2, e2.Top);
          UpdateEdgeIntoAEL(ref e2);
          TEdge prevInAel = e2.PrevInAEL;
          TEdge nextInAel = e2.NextInAEL;
          if (prevInAel != null && prevInAel.Curr.X == e2.Bot.X && prevInAel.Curr.Y == e2.Bot.Y && Op1 != null && prevInAel.OutIdx >= 0 && prevInAel.Curr.Y > prevInAel.Top.Y && SlopesEqual(e2, prevInAel, m_UseFullRange) && e2.WindDelta != 0 && prevInAel.WindDelta != 0)
          {
            OutPt Op2 = AddOutPt(prevInAel, e2.Bot);
            AddJoin(Op1, Op2, e2.Top);
          }
          else if (nextInAel != null && nextInAel.Curr.X == e2.Bot.X && nextInAel.Curr.Y == e2.Bot.Y && Op1 != null && nextInAel.OutIdx >= 0 && nextInAel.Curr.Y > nextInAel.Top.Y && SlopesEqual(e2, nextInAel, m_UseFullRange) && e2.WindDelta != 0 && nextInAel.WindDelta != 0)
          {
            OutPt Op2 = AddOutPt(nextInAel, e2.Bot);
            AddJoin(Op1, Op2, e2.Top);
          }
        }
      }
    }

    private void DoMaxima(TEdge e)
    {
      TEdge maximaPair = GetMaximaPair(e);
      if (maximaPair == null)
      {
        if (e.OutIdx >= 0)
          AddOutPt(e, e.Top);
        DeleteFromAEL(e);
      }
      else
      {
        for (TEdge nextInAel = e.NextInAEL; nextInAel != null && nextInAel != maximaPair; nextInAel = e.NextInAEL)
        {
          IntersectEdges(e, nextInAel, e.Top);
          SwapPositionsInAEL(e, nextInAel);
        }
        if (e.OutIdx == -1 && maximaPair.OutIdx == -1)
        {
          DeleteFromAEL(e);
          DeleteFromAEL(maximaPair);
        }
        else
        {
          if (e.OutIdx < 0 || maximaPair.OutIdx < 0)
            throw new ClipperException("DoMaxima error");
          if (e.OutIdx >= 0)
            AddLocalMaxPoly(e, maximaPair, e.Top);
          DeleteFromAEL(e);
          DeleteFromAEL(maximaPair);
        }
      }
    }

    public static void ReversePaths(List<List<IntPoint>> polys)
    {
      foreach (List<IntPoint> poly in polys)
        poly.Reverse();
    }

    public static bool Orientation(List<IntPoint> poly) => Area(poly) >= 0.0;

    private int PointCount(OutPt pts)
    {
      if (pts == null)
        return 0;
      int num = 0;
      OutPt outPt = pts;
      do
      {
        ++num;
        outPt = outPt.Next;
      }
      while (outPt != pts);
      return num;
    }

    private void BuildResult(List<List<IntPoint>> polyg)
    {
      polyg.Clear();
      polyg.Capacity = m_PolyOuts.Count;
      for (int index1 = 0; index1 < m_PolyOuts.Count; ++index1)
      {
        OutRec polyOut = m_PolyOuts[index1];
        if (polyOut.Pts != null)
        {
          OutPt prev = polyOut.Pts.Prev;
          int capacity = PointCount(prev);
          if (capacity >= 2)
          {
            List<IntPoint> intPointList = new List<IntPoint>(capacity);
            for (int index2 = 0; index2 < capacity; ++index2)
            {
              intPointList.Add(prev.Pt);
              prev = prev.Prev;
            }
            polyg.Add(intPointList);
          }
        }
      }
    }

    private void BuildResult2(PolyTree polytree)
    {
      polytree.Clear();
      polytree.m_AllPolys.Capacity = m_PolyOuts.Count;
      for (int index1 = 0; index1 < m_PolyOuts.Count; ++index1)
      {
        OutRec polyOut = m_PolyOuts[index1];
        int num = PointCount(polyOut.Pts);
        if ((!polyOut.IsOpen || num >= 2) && (polyOut.IsOpen || num >= 3))
        {
          FixHoleLinkage(polyOut);
          PolyNode polyNode = new PolyNode();
          polytree.m_AllPolys.Add(polyNode);
          polyOut.PolyNode = polyNode;
          polyNode.m_polygon.Capacity = num;
          OutPt prev = polyOut.Pts.Prev;
          for (int index2 = 0; index2 < num; ++index2)
          {
            polyNode.m_polygon.Add(prev.Pt);
            prev = prev.Prev;
          }
        }
      }
      polytree.m_Childs.Capacity = m_PolyOuts.Count;
      for (int index = 0; index < m_PolyOuts.Count; ++index)
      {
        OutRec polyOut = m_PolyOuts[index];
        if (polyOut.PolyNode != null)
        {
          if (polyOut.IsOpen)
          {
            polyOut.PolyNode.IsOpen = true;
            polytree.AddChild(polyOut.PolyNode);
          }
          else if (polyOut.FirstLeft != null && polyOut.FirstLeft.PolyNode != null)
            polyOut.FirstLeft.PolyNode.AddChild(polyOut.PolyNode);
          else
            polytree.AddChild(polyOut.PolyNode);
        }
      }
    }

    private void FixupOutPolygon(OutRec outRec)
    {
      OutPt outPt1 = null;
      outRec.BottomPt = null;
      OutPt outPt2 = outRec.Pts;
      while (outPt2.Prev != outPt2 && outPt2.Prev != outPt2.Next)
      {
        if (outPt2.Pt == outPt2.Next.Pt || outPt2.Pt == outPt2.Prev.Pt || SlopesEqual(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt, m_UseFullRange) && (!PreserveCollinear || !Pt2IsBetweenPt1AndPt3(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt)))
        {
          outPt1 = null;
          outPt2.Prev.Next = outPt2.Next;
          outPt2.Next.Prev = outPt2.Prev;
          outPt2 = outPt2.Prev;
        }
        else if (outPt2 != outPt1)
        {
          if (outPt1 == null)
            outPt1 = outPt2;
          outPt2 = outPt2.Next;
        }
        else
        {
          outRec.Pts = outPt2;
          return;
        }
      }
      outRec.Pts = null;
    }

    private OutPt DupOutPt(OutPt outPt, bool InsertAfter)
    {
      OutPt outPt1 = new OutPt();
      outPt1.Pt = outPt.Pt;
      outPt1.Idx = outPt.Idx;
      if (InsertAfter)
      {
        outPt1.Next = outPt.Next;
        outPt1.Prev = outPt;
        outPt.Next.Prev = outPt1;
        outPt.Next = outPt1;
      }
      else
      {
        outPt1.Prev = outPt.Prev;
        outPt1.Next = outPt;
        outPt.Prev.Next = outPt1;
        outPt.Prev = outPt1;
      }
      return outPt1;
    }

    private bool GetOverlap(long a1, long a2, long b1, long b2, out long Left, out long Right)
    {
      if (a1 < a2)
      {
        if (b1 < b2)
        {
          Left = Math.Max(a1, b1);
          Right = Math.Min(a2, b2);
        }
        else
        {
          Left = Math.Max(a1, b2);
          Right = Math.Min(a2, b1);
        }
      }
      else if (b1 < b2)
      {
        Left = Math.Max(a2, b1);
        Right = Math.Min(a1, b2);
      }
      else
      {
        Left = Math.Max(a2, b2);
        Right = Math.Min(a1, b1);
      }
      return Left < Right;
    }

    private bool JoinHorz(
      OutPt op1,
      OutPt op1b,
      OutPt op2,
      OutPt op2b,
      IntPoint Pt,
      bool DiscardLeft)
    {
      Direction direction1 = op1.Pt.X > op1b.Pt.X ? Direction.dRightToLeft : Direction.dLeftToRight;
      Direction direction2 = op2.Pt.X > op2b.Pt.X ? Direction.dRightToLeft : Direction.dLeftToRight;
      if (direction1 == direction2)
        return false;
      if (direction1 == Direction.dLeftToRight)
      {
        while (op1.Next.Pt.X <= Pt.X && op1.Next.Pt.X >= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
          op1 = op1.Next;
        if (DiscardLeft && op1.Pt.X != Pt.X)
          op1 = op1.Next;
        op1b = DupOutPt(op1, !DiscardLeft);
        if (op1b.Pt != Pt)
        {
          op1 = op1b;
          op1.Pt = Pt;
          op1b = DupOutPt(op1, !DiscardLeft);
        }
      }
      else
      {
        while (op1.Next.Pt.X >= Pt.X && op1.Next.Pt.X <= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
          op1 = op1.Next;
        if (!DiscardLeft && op1.Pt.X != Pt.X)
          op1 = op1.Next;
        op1b = DupOutPt(op1, DiscardLeft);
        if (op1b.Pt != Pt)
        {
          op1 = op1b;
          op1.Pt = Pt;
          op1b = DupOutPt(op1, DiscardLeft);
        }
      }
      if (direction2 == Direction.dLeftToRight)
      {
        while (op2.Next.Pt.X <= Pt.X && op2.Next.Pt.X >= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
          op2 = op2.Next;
        if (DiscardLeft && op2.Pt.X != Pt.X)
          op2 = op2.Next;
        op2b = DupOutPt(op2, !DiscardLeft);
        if (op2b.Pt != Pt)
        {
          op2 = op2b;
          op2.Pt = Pt;
          op2b = DupOutPt(op2, !DiscardLeft);
        }
      }
      else
      {
        while (op2.Next.Pt.X >= Pt.X && op2.Next.Pt.X <= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
          op2 = op2.Next;
        if (!DiscardLeft && op2.Pt.X != Pt.X)
          op2 = op2.Next;
        op2b = DupOutPt(op2, DiscardLeft);
        if (op2b.Pt != Pt)
        {
          op2 = op2b;
          op2.Pt = Pt;
          op2b = DupOutPt(op2, DiscardLeft);
        }
      }
      if (direction1 == Direction.dLeftToRight == DiscardLeft)
      {
        op1.Prev = op2;
        op2.Next = op1;
        op1b.Next = op2b;
        op2b.Prev = op1b;
      }
      else
      {
        op1.Next = op2;
        op2.Prev = op1;
        op1b.Prev = op2b;
        op2b.Next = op1b;
      }
      return true;
    }

    private bool JoinPoints(Join j, OutRec outRec1, OutRec outRec2)
    {
      OutPt outPt1 = j.OutPt1;
      OutPt outPt2 = j.OutPt2;
      bool flag1 = j.OutPt1.Pt.Y == j.OffPt.Y;
      if (flag1 && j.OffPt == j.OutPt1.Pt && j.OffPt == j.OutPt2.Pt)
      {
        if (outRec1 != outRec2)
          return false;
        OutPt next1 = j.OutPt1.Next;
        while (next1 != outPt1 && next1.Pt == j.OffPt)
          next1 = next1.Next;
        bool flag2 = next1.Pt.Y > j.OffPt.Y;
        OutPt next2 = j.OutPt2.Next;
        while (next2 != outPt2 && next2.Pt == j.OffPt)
          next2 = next2.Next;
        bool flag3 = next2.Pt.Y > j.OffPt.Y;
        if (flag2 == flag3)
          return false;
        if (flag2)
        {
          OutPt outPt3 = DupOutPt(outPt1, false);
          OutPt outPt4 = DupOutPt(outPt2, true);
          outPt1.Prev = outPt2;
          outPt2.Next = outPt1;
          outPt3.Next = outPt4;
          outPt4.Prev = outPt3;
          j.OutPt1 = outPt1;
          j.OutPt2 = outPt3;
          return true;
        }
        OutPt outPt5 = DupOutPt(outPt1, true);
        OutPt outPt6 = DupOutPt(outPt2, false);
        outPt1.Next = outPt2;
        outPt2.Prev = outPt1;
        outPt5.Prev = outPt6;
        outPt6.Next = outPt5;
        j.OutPt1 = outPt1;
        j.OutPt2 = outPt5;
        return true;
      }
      if (flag1)
      {
        OutPt op1b = outPt1;
        while (outPt1.Prev.Pt.Y == outPt1.Pt.Y && outPt1.Prev != op1b && outPt1.Prev != outPt2)
          outPt1 = outPt1.Prev;
        while (op1b.Next.Pt.Y == op1b.Pt.Y && op1b.Next != outPt1 && op1b.Next != outPt2)
          op1b = op1b.Next;
        if (op1b.Next == outPt1 || op1b.Next == outPt2)
          return false;
        OutPt op2b = outPt2;
        while (outPt2.Prev.Pt.Y == outPt2.Pt.Y && outPt2.Prev != op2b && outPt2.Prev != op1b)
          outPt2 = outPt2.Prev;
        while (op2b.Next.Pt.Y == op2b.Pt.Y && op2b.Next != outPt2 && op2b.Next != outPt1)
          op2b = op2b.Next;
        if (op2b.Next == outPt2 || op2b.Next == outPt1 || !GetOverlap(outPt1.Pt.X, op1b.Pt.X, outPt2.Pt.X, op2b.Pt.X, out long Left, out long Right))
          return false;
        IntPoint pt;
        bool DiscardLeft;
        if (outPt1.Pt.X >= Left && outPt1.Pt.X <= Right)
        {
          pt = outPt1.Pt;
          DiscardLeft = outPt1.Pt.X > op1b.Pt.X;
        }
        else if (outPt2.Pt.X >= Left && outPt2.Pt.X <= Right)
        {
          pt = outPt2.Pt;
          DiscardLeft = outPt2.Pt.X > op2b.Pt.X;
        }
        else if (op1b.Pt.X >= Left && op1b.Pt.X <= Right)
        {
          pt = op1b.Pt;
          DiscardLeft = op1b.Pt.X > outPt1.Pt.X;
        }
        else
        {
          pt = op2b.Pt;
          DiscardLeft = op2b.Pt.X > outPt2.Pt.X;
        }
        j.OutPt1 = outPt1;
        j.OutPt2 = outPt2;
        return JoinHorz(outPt1, op1b, outPt2, op2b, pt, DiscardLeft);
      }
      OutPt outPt7 = outPt1.Next;
      while (outPt7.Pt == outPt1.Pt && outPt7 != outPt1)
        outPt7 = outPt7.Next;
      bool flag4 = outPt7.Pt.Y > outPt1.Pt.Y || !SlopesEqual(outPt1.Pt, outPt7.Pt, j.OffPt, m_UseFullRange);
      if (flag4)
      {
        outPt7 = outPt1.Prev;
        while (outPt7.Pt == outPt1.Pt && outPt7 != outPt1)
          outPt7 = outPt7.Prev;
        if (outPt7.Pt.Y > outPt1.Pt.Y || !SlopesEqual(outPt1.Pt, outPt7.Pt, j.OffPt, m_UseFullRange))
          return false;
      }
      OutPt outPt8 = outPt2.Next;
      while (outPt8.Pt == outPt2.Pt && outPt8 != outPt2)
        outPt8 = outPt8.Next;
      bool flag5 = outPt8.Pt.Y > outPt2.Pt.Y || !SlopesEqual(outPt2.Pt, outPt8.Pt, j.OffPt, m_UseFullRange);
      if (flag5)
      {
        outPt8 = outPt2.Prev;
        while (outPt8.Pt == outPt2.Pt && outPt8 != outPt2)
          outPt8 = outPt8.Prev;
        if (outPt8.Pt.Y > outPt2.Pt.Y || !SlopesEqual(outPt2.Pt, outPt8.Pt, j.OffPt, m_UseFullRange))
          return false;
      }
      if (outPt7 == outPt1 || outPt8 == outPt2 || outPt7 == outPt8 || outRec1 == outRec2 && flag4 == flag5)
        return false;
      if (flag4)
      {
        OutPt outPt9 = DupOutPt(outPt1, false);
        OutPt outPt10 = DupOutPt(outPt2, true);
        outPt1.Prev = outPt2;
        outPt2.Next = outPt1;
        outPt9.Next = outPt10;
        outPt10.Prev = outPt9;
        j.OutPt1 = outPt1;
        j.OutPt2 = outPt9;
        return true;
      }
      OutPt outPt11 = DupOutPt(outPt1, true);
      OutPt outPt12 = DupOutPt(outPt2, false);
      outPt1.Next = outPt2;
      outPt2.Prev = outPt1;
      outPt11.Prev = outPt12;
      outPt12.Next = outPt11;
      j.OutPt1 = outPt1;
      j.OutPt2 = outPt11;
      return true;
    }

    public static int PointInPolygon(IntPoint pt, List<IntPoint> path)
    {
      int num1 = 0;
      int count = path.Count;
      if (count < 3)
        return 0;
      IntPoint intPoint1 = path[0];
      for (int index = 1; index <= count; ++index)
      {
        IntPoint intPoint2 = index == count ? path[0] : path[index];
        if (intPoint2.Y == pt.Y && (intPoint2.X == pt.X || intPoint1.Y == pt.Y && intPoint2.X > pt.X == intPoint1.X < pt.X))
          return -1;
        if (intPoint1.Y < pt.Y != intPoint2.Y < pt.Y)
        {
          if (intPoint1.X >= pt.X)
          {
            if (intPoint2.X > pt.X)
            {
              num1 = 1 - num1;
            }
            else
            {
              double num2 = (intPoint1.X - pt.X) * (double) (intPoint2.Y - pt.Y) - (intPoint2.X - pt.X) * (double) (intPoint1.Y - pt.Y);
              if (num2 == 0.0)
                return -1;
              if (num2 > 0.0 == intPoint2.Y > intPoint1.Y)
                num1 = 1 - num1;
            }
          }
          else if (intPoint2.X > pt.X)
          {
            double num3 = (intPoint1.X - pt.X) * (double) (intPoint2.Y - pt.Y) - (intPoint2.X - pt.X) * (double) (intPoint1.Y - pt.Y);
            if (num3 == 0.0)
              return -1;
            if (num3 > 0.0 == intPoint2.Y > intPoint1.Y)
              num1 = 1 - num1;
          }
        }
        intPoint1 = intPoint2;
      }
      return num1;
    }

    private static int PointInPolygon(IntPoint pt, OutPt op)
    {
      int num1 = 0;
      OutPt outPt = op;
      long x1 = pt.X;
      long y1 = pt.Y;
      long num2 = op.Pt.X;
      long num3 = op.Pt.Y;
      do
      {
        op = op.Next;
        long x2 = op.Pt.X;
        long y2 = op.Pt.Y;
        if (y2 == y1 && (x2 == x1 || num3 == y1 && x2 > x1 == num2 < x1))
          return -1;
        if (num3 < y1 != y2 < y1)
        {
          if (num2 >= x1)
          {
            if (x2 > x1)
            {
              num1 = 1 - num1;
            }
            else
            {
              double num4 = (num2 - x1) * (double) (y2 - y1) - (x2 - x1) * (double) (num3 - y1);
              if (num4 == 0.0)
                return -1;
              if (num4 > 0.0 == y2 > num3)
                num1 = 1 - num1;
            }
          }
          else if (x2 > x1)
          {
            double num5 = (num2 - x1) * (double) (y2 - y1) - (x2 - x1) * (double) (num3 - y1);
            if (num5 == 0.0)
              return -1;
            if (num5 > 0.0 == y2 > num3)
              num1 = 1 - num1;
          }
        }
        num2 = x2;
        num3 = y2;
      }
      while (outPt != op);
      return num1;
    }

    private static bool Poly2ContainsPoly1(OutPt outPt1, OutPt outPt2)
    {
      OutPt outPt = outPt1;
      do
      {
        int num = PointInPolygon(outPt.Pt, outPt2);
        if (num >= 0)
          return num > 0;
        outPt = outPt.Next;
      }
      while (outPt != outPt1);
      return true;
    }

    private void FixupFirstLefts1(OutRec OldOutRec, OutRec NewOutRec)
    {
      for (int index = 0; index < m_PolyOuts.Count; ++index)
      {
        OutRec polyOut = m_PolyOuts[index];
        if (polyOut.Pts != null && polyOut.FirstLeft != null && ParseFirstLeft(polyOut.FirstLeft) == OldOutRec && Poly2ContainsPoly1(polyOut.Pts, NewOutRec.Pts))
          polyOut.FirstLeft = NewOutRec;
      }
    }

    private void FixupFirstLefts2(OutRec OldOutRec, OutRec NewOutRec)
    {
      foreach (OutRec polyOut in m_PolyOuts)
      {
        if (polyOut.FirstLeft == OldOutRec)
          polyOut.FirstLeft = NewOutRec;
      }
    }

    private static OutRec ParseFirstLeft(OutRec FirstLeft)
    {
      while (FirstLeft != null && FirstLeft.Pts == null)
        FirstLeft = FirstLeft.FirstLeft;
      return FirstLeft;
    }

    private void JoinCommonEdges()
    {
      for (int index1 = 0; index1 < m_Joins.Count; ++index1)
      {
        Join join = m_Joins[index1];
        OutRec outRec1 = GetOutRec(join.OutPt1.Idx);
        OutRec outRec2 = GetOutRec(join.OutPt2.Idx);
        if (outRec1.Pts != null && outRec2.Pts != null)
        {
          OutRec outRec3 = outRec1 != outRec2 ? (!Param1RightOfParam2(outRec1, outRec2) ? (!Param1RightOfParam2(outRec2, outRec1) ? GetLowermostRec(outRec1, outRec2) : outRec1) : outRec2) : outRec1;
          if (JoinPoints(join, outRec1, outRec2))
          {
            if (outRec1 == outRec2)
            {
              outRec1.Pts = join.OutPt1;
              outRec1.BottomPt = null;
              OutRec outRec4 = CreateOutRec();
              outRec4.Pts = join.OutPt2;
              UpdateOutPtIdxs(outRec4);
              if (m_UsingPolyTree)
              {
                for (int index2 = 0; index2 < m_PolyOuts.Count - 1; ++index2)
                {
                  OutRec polyOut = m_PolyOuts[index2];
                  if (polyOut.Pts != null && ParseFirstLeft(polyOut.FirstLeft) == outRec1 && polyOut.IsHole != outRec1.IsHole && Poly2ContainsPoly1(polyOut.Pts, join.OutPt2))
                    polyOut.FirstLeft = outRec4;
                }
              }
              if (Poly2ContainsPoly1(outRec4.Pts, outRec1.Pts))
              {
                outRec4.IsHole = !outRec1.IsHole;
                outRec4.FirstLeft = outRec1;
                if (m_UsingPolyTree)
                  FixupFirstLefts2(outRec4, outRec1);
                if ((outRec4.IsHole ^ ReverseSolution) == Area(outRec4) > 0.0)
                  ReversePolyPtLinks(outRec4.Pts);
              }
              else if (Poly2ContainsPoly1(outRec1.Pts, outRec4.Pts))
              {
                outRec4.IsHole = outRec1.IsHole;
                outRec1.IsHole = !outRec4.IsHole;
                outRec4.FirstLeft = outRec1.FirstLeft;
                outRec1.FirstLeft = outRec4;
                if (m_UsingPolyTree)
                  FixupFirstLefts2(outRec1, outRec4);
                if ((outRec1.IsHole ^ ReverseSolution) == Area(outRec1) > 0.0)
                  ReversePolyPtLinks(outRec1.Pts);
              }
              else
              {
                outRec4.IsHole = outRec1.IsHole;
                outRec4.FirstLeft = outRec1.FirstLeft;
                if (m_UsingPolyTree)
                  FixupFirstLefts1(outRec1, outRec4);
              }
            }
            else
            {
              outRec2.Pts = null;
              outRec2.BottomPt = null;
              outRec2.Idx = outRec1.Idx;
              outRec1.IsHole = outRec3.IsHole;
              if (outRec3 == outRec2)
                outRec1.FirstLeft = outRec2.FirstLeft;
              outRec2.FirstLeft = outRec1;
              if (m_UsingPolyTree)
                FixupFirstLefts2(outRec2, outRec1);
            }
          }
        }
      }
    }

    private void UpdateOutPtIdxs(OutRec outrec)
    {
      OutPt outPt = outrec.Pts;
      do
      {
        outPt.Idx = outrec.Idx;
        outPt = outPt.Prev;
      }
      while (outPt != outrec.Pts);
    }

    private void DoSimplePolygons()
    {
      int num = 0;
      while (num < m_PolyOuts.Count)
      {
        OutRec polyOut = m_PolyOuts[num++];
        OutPt outPt1 = polyOut.Pts;
        if (outPt1 != null && !polyOut.IsOpen)
        {
          do
          {
            for (OutPt outPt2 = outPt1.Next; outPt2 != polyOut.Pts; outPt2 = outPt2.Next)
            {
              if (outPt1.Pt == outPt2.Pt && outPt2.Next != outPt1 && outPt2.Prev != outPt1)
              {
                OutPt prev1 = outPt1.Prev;
                OutPt prev2 = outPt2.Prev;
                outPt1.Prev = prev2;
                prev2.Next = outPt1;
                outPt2.Prev = prev1;
                prev1.Next = outPt2;
                polyOut.Pts = outPt1;
                OutRec outRec = CreateOutRec();
                outRec.Pts = outPt2;
                UpdateOutPtIdxs(outRec);
                if (Poly2ContainsPoly1(outRec.Pts, polyOut.Pts))
                {
                  outRec.IsHole = !polyOut.IsHole;
                  outRec.FirstLeft = polyOut;
                  if (m_UsingPolyTree)
                    FixupFirstLefts2(outRec, polyOut);
                }
                else if (Poly2ContainsPoly1(polyOut.Pts, outRec.Pts))
                {
                  outRec.IsHole = polyOut.IsHole;
                  polyOut.IsHole = !outRec.IsHole;
                  outRec.FirstLeft = polyOut.FirstLeft;
                  polyOut.FirstLeft = outRec;
                  if (m_UsingPolyTree)
                    FixupFirstLefts2(polyOut, outRec);
                }
                else
                {
                  outRec.IsHole = polyOut.IsHole;
                  outRec.FirstLeft = polyOut.FirstLeft;
                  if (m_UsingPolyTree)
                    FixupFirstLefts1(polyOut, outRec);
                }
                outPt2 = outPt1;
              }
            }
            outPt1 = outPt1.Next;
          }
          while (outPt1 != polyOut.Pts);
        }
      }
    }

    public static double Area(List<IntPoint> poly)
    {
      int count = poly.Count;
      if (count < 3)
        return 0.0;
      double num = 0.0;
      int index1 = 0;
      int index2 = count - 1;
      for (; index1 < count; ++index1)
      {
        num += (poly[index2].X + (double) poly[index1].X) * (poly[index2].Y - (double) poly[index1].Y);
        index2 = index1;
      }
      return -num * 0.5;
    }

    private double Area(OutRec outRec)
    {
      OutPt outPt = outRec.Pts;
      if (outPt == null)
        return 0.0;
      double num = 0.0;
      do
      {
        num += (outPt.Prev.Pt.X + outPt.Pt.X) * (double) (outPt.Prev.Pt.Y - outPt.Pt.Y);
        outPt = outPt.Next;
      }
      while (outPt != outRec.Pts);
      return num * 0.5;
    }

    public static List<List<IntPoint>> SimplifyPolygon(List<IntPoint> poly, PolyFillType fillType = PolyFillType.pftEvenOdd)
    {
      List<List<IntPoint>> solution = [];
      Clipper clipper = new Clipper();
      clipper.StrictlySimple = true;
      clipper.AddPath(poly, PolyType.ptSubject, true);
      clipper.Execute(ClipType.ctUnion, solution, fillType, fillType);
      return solution;
    }

    public static List<List<IntPoint>> SimplifyPolygons(
      List<List<IntPoint>> polys,
      PolyFillType fillType = PolyFillType.pftEvenOdd)
    {
      List<List<IntPoint>> solution = [];
      Clipper clipper = new Clipper();
      clipper.StrictlySimple = true;
      clipper.AddPaths(polys, PolyType.ptSubject, true);
      clipper.Execute(ClipType.ctUnion, solution, fillType, fillType);
      return solution;
    }

    private static double DistanceSqrd(IntPoint pt1, IntPoint pt2)
    {
      double num1 = pt1.X - (double) pt2.X;
      double num2 = pt1.Y - (double) pt2.Y;
      return num1 * num1 + num2 * num2;
    }

    private static double DistanceFromLineSqrd(IntPoint pt, IntPoint ln1, IntPoint ln2)
    {
      double num1 = ln1.Y - ln2.Y;
      double num2 = ln2.X - ln1.X;
      double num3 = num1 * ln1.X + num2 * ln1.Y;
      double num4 = num1 * pt.X + num2 * pt.Y - num3;
      return num4 * num4 / (num1 * num1 + num2 * num2);
    }

    private static bool SlopesNearCollinear(
      IntPoint pt1,
      IntPoint pt2,
      IntPoint pt3,
      double distSqrd)
    {
      if (Math.Abs(pt1.X - pt2.X) > Math.Abs(pt1.Y - pt2.Y))
      {
        if (pt1.X > pt2.X == pt1.X < pt3.X)
          return DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
        return pt2.X > pt1.X == pt2.X < pt3.X ? DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd : DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
      }
      if (pt1.Y > pt2.Y == pt1.Y < pt3.Y)
        return DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
      return pt2.Y > pt1.Y == pt2.Y < pt3.Y ? DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd : DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
    }

    private static bool PointsAreClose(IntPoint pt1, IntPoint pt2, double distSqrd)
    {
      double num1 = pt1.X - (double) pt2.X;
      double num2 = pt1.Y - (double) pt2.Y;
      return num1 * num1 + num2 * num2 <= distSqrd;
    }

    private static OutPt ExcludeOp(OutPt op)
    {
      OutPt prev = op.Prev;
      prev.Next = op.Next;
      op.Next.Prev = prev;
      prev.Idx = 0;
      return prev;
    }

    public static List<IntPoint> CleanPolygon(List<IntPoint> path, double distance = 1.415)
    {
      int capacity = path.Count;
      if (capacity == 0)
        return [];
      OutPt[] outPtArray = new OutPt[capacity];
      for (int index = 0; index < capacity; ++index)
        outPtArray[index] = new OutPt();
      for (int index = 0; index < capacity; ++index)
      {
        outPtArray[index].Pt = path[index];
        outPtArray[index].Next = outPtArray[(index + 1) % capacity];
        outPtArray[index].Next.Prev = outPtArray[index];
        outPtArray[index].Idx = 0;
      }
      double distSqrd = distance * distance;
      OutPt op = outPtArray[0];
      while (op.Idx == 0 && op.Next != op.Prev)
      {
        if (PointsAreClose(op.Pt, op.Prev.Pt, distSqrd))
        {
          op = ExcludeOp(op);
          --capacity;
        }
        else if (PointsAreClose(op.Prev.Pt, op.Next.Pt, distSqrd))
        {
          ExcludeOp(op.Next);
          op = ExcludeOp(op);
          capacity -= 2;
        }
        else if (SlopesNearCollinear(op.Prev.Pt, op.Pt, op.Next.Pt, distSqrd))
        {
          op = ExcludeOp(op);
          --capacity;
        }
        else
        {
          op.Idx = 1;
          op = op.Next;
        }
      }
      if (capacity < 3)
        capacity = 0;
      List<IntPoint> intPointList = new List<IntPoint>(capacity);
      for (int index = 0; index < capacity; ++index)
      {
        intPointList.Add(op.Pt);
        op = op.Next;
      }
      return intPointList;
    }

    public static List<List<IntPoint>> CleanPolygons(List<List<IntPoint>> polys, double distance = 1.415)
    {
      List<List<IntPoint>> intPointListList = new List<List<IntPoint>>(polys.Count);
      for (int index = 0; index < polys.Count; ++index)
        intPointListList.Add(CleanPolygon(polys[index], distance));
      return intPointListList;
    }

    internal static List<List<IntPoint>> Minkowski(
      List<IntPoint> pattern,
      List<IntPoint> path,
      bool IsSum,
      bool IsClosed)
    {
      int num = IsClosed ? 1 : 0;
      int count1 = pattern.Count;
      int count2 = path.Count;
      List<List<IntPoint>> intPointListList1 = new List<List<IntPoint>>(count2);
      if (IsSum)
      {
        for (int index = 0; index < count2; ++index)
        {
          List<IntPoint> intPointList = new List<IntPoint>(count1);
          foreach (IntPoint intPoint in pattern)
            intPointList.Add(new IntPoint(path[index].X + intPoint.X, path[index].Y + intPoint.Y));
          intPointListList1.Add(intPointList);
        }
      }
      else
      {
        for (int index = 0; index < count2; ++index)
        {
          List<IntPoint> intPointList = new List<IntPoint>(count1);
          foreach (IntPoint intPoint in pattern)
            intPointList.Add(new IntPoint(path[index].X - intPoint.X, path[index].Y - intPoint.Y));
          intPointListList1.Add(intPointList);
        }
      }
      List<List<IntPoint>> intPointListList2 = new List<List<IntPoint>>((count2 + num) * (count1 + 1));
      for (int index1 = 0; index1 < count2 - 1 + num; ++index1)
      {
        for (int index2 = 0; index2 < count1; ++index2)
        {
          List<IntPoint> poly = [
            intPointListList1[index1 % count2][index2 % count1],
            intPointListList1[(index1 + 1) % count2][index2 % count1],
            intPointListList1[(index1 + 1) % count2][(index2 + 1) % count1],
            intPointListList1[index1 % count2][(index2 + 1) % count1]
          ];
          if (!Orientation(poly))
            poly.Reverse();
          intPointListList2.Add(poly);
        }
      }
      return intPointListList2;
    }

    public static List<List<IntPoint>> MinkowskiSum(
      List<IntPoint> pattern,
      List<IntPoint> path,
      bool pathIsClosed)
    {
      List<List<IntPoint>> intPointListList = Minkowski(pattern, path, true, pathIsClosed);
      Clipper clipper = new Clipper();
      clipper.AddPaths(intPointListList, PolyType.ptSubject, true);
      clipper.Execute(ClipType.ctUnion, intPointListList, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
      return intPointListList;
    }

    private static List<IntPoint> TranslatePath(List<IntPoint> path, IntPoint delta)
    {
      List<IntPoint> intPointList = new List<IntPoint>(path.Count);
      for (int index = 0; index < path.Count; ++index)
        intPointList.Add(new IntPoint(path[index].X + delta.X, path[index].Y + delta.Y));
      return intPointList;
    }

    public static List<List<IntPoint>> MinkowskiSum(
      List<IntPoint> pattern,
      List<List<IntPoint>> paths,
      bool pathIsClosed)
    {
      List<List<IntPoint>> solution = [];
      Clipper clipper = new Clipper();
      for (int index = 0; index < paths.Count; ++index)
      {
        List<List<IntPoint>> ppg = Minkowski(pattern, paths[index], true, pathIsClosed);
        clipper.AddPaths(ppg, PolyType.ptSubject, true);
        if (pathIsClosed)
        {
          List<IntPoint> pg = TranslatePath(paths[index], pattern[0]);
          clipper.AddPath(pg, PolyType.ptClip, true);
        }
      }
      clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
      return solution;
    }

    public static List<List<IntPoint>> MinkowskiDiff(List<IntPoint> poly1, List<IntPoint> poly2)
    {
      List<List<IntPoint>> intPointListList = Minkowski(poly1, poly2, false, true);
      Clipper clipper = new Clipper();
      clipper.AddPaths(intPointListList, PolyType.ptSubject, true);
      clipper.Execute(ClipType.ctUnion, intPointListList, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
      return intPointListList;
    }

    public static List<List<IntPoint>> PolyTreeToPaths(PolyTree polytree)
    {
      List<List<IntPoint>> paths = new List<List<IntPoint>>();
      paths.Capacity = polytree.Total;
      AddPolyNodeToPaths(polytree, NodeType.ntAny, paths);
      return paths;
    }

    internal static void AddPolyNodeToPaths(
      PolyNode polynode,
      NodeType nt,
      List<List<IntPoint>> paths)
    {
      bool flag = true;
      switch (nt)
      {
        case NodeType.ntOpen:
          return;
        case NodeType.ntClosed:
          flag = !polynode.IsOpen;
          break;
      }
      if (polynode.m_polygon.Count > 0 & flag)
        paths.Add(polynode.m_polygon);
      foreach (PolyNode child in polynode.Childs)
        AddPolyNodeToPaths(child, nt, paths);
    }

    public static List<List<IntPoint>> OpenPathsFromPolyTree(PolyTree polytree)
    {
      List<List<IntPoint>> intPointListList = new List<List<IntPoint>>();
      intPointListList.Capacity = polytree.ChildCount;
      for (int index = 0; index < polytree.ChildCount; ++index)
      {
        if (polytree.Childs[index].IsOpen)
          intPointListList.Add(polytree.Childs[index].m_polygon);
      }
      return intPointListList;
    }

    public static List<List<IntPoint>> ClosedPathsFromPolyTree(PolyTree polytree)
    {
      List<List<IntPoint>> paths = new List<List<IntPoint>>();
      paths.Capacity = polytree.Total;
      AddPolyNodeToPaths(polytree, NodeType.ntClosed, paths);
      return paths;
    }

    internal enum NodeType
    {
      ntAny,
      ntOpen,
      ntClosed,
    }
  }
}
