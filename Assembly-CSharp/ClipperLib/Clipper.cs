// Decompiled with JetBrains decompiler
// Type: ClipperLib.Clipper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
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
      this.m_Scanbeam = (Scanbeam) null;
      this.m_ActiveEdges = (TEdge) null;
      this.m_SortedEdges = (TEdge) null;
      this.m_IntersectList = new List<IntersectNode>();
      this.m_IntersectNodeComparer = (IComparer<IntersectNode>) new MyIntersectNodeSort();
      this.m_ExecuteLocked = false;
      this.m_UsingPolyTree = false;
      this.m_PolyOuts = new List<OutRec>();
      this.m_Joins = new List<Join>();
      this.m_GhostJoins = new List<Join>();
      this.ReverseSolution = (1 & InitOptions) != 0;
      this.StrictlySimple = (2 & InitOptions) != 0;
      this.PreserveCollinear = (4 & InitOptions) != 0;
    }

    private void DisposeScanbeamList()
    {
      Scanbeam next;
      for (; this.m_Scanbeam != null; this.m_Scanbeam = next)
      {
        next = this.m_Scanbeam.Next;
        this.m_Scanbeam = (Scanbeam) null;
      }
    }

    protected override void Reset()
    {
      base.Reset();
      this.m_Scanbeam = (Scanbeam) null;
      this.m_ActiveEdges = (TEdge) null;
      this.m_SortedEdges = (TEdge) null;
      for (LocalMinima localMinima = this.m_MinimaList; localMinima != null; localMinima = localMinima.Next)
        this.InsertScanbeam(localMinima.Y);
    }

    public bool ReverseSolution { get; set; }

    public bool StrictlySimple { get; set; }

    private void InsertScanbeam(long Y)
    {
      if (this.m_Scanbeam == null)
      {
        this.m_Scanbeam = new Scanbeam();
        this.m_Scanbeam.Next = (Scanbeam) null;
        this.m_Scanbeam.Y = Y;
      }
      else if (Y > this.m_Scanbeam.Y)
      {
        this.m_Scanbeam = new Scanbeam()
        {
          Y = Y,
          Next = this.m_Scanbeam
        };
      }
      else
      {
        Scanbeam scanbeam = this.m_Scanbeam;
        while (scanbeam.Next != null && Y <= scanbeam.Next.Y)
          scanbeam = scanbeam.Next;
        if (Y == scanbeam.Y)
          return;
        scanbeam.Next = new Scanbeam()
        {
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
      if (this.m_ExecuteLocked)
        return false;
      if (this.m_HasOpenPaths)
        throw new ClipperException("Error: PolyTree struct is need for open path clipping.");
      this.m_ExecuteLocked = true;
      solution.Clear();
      this.m_SubjFillType = subjFillType;
      this.m_ClipFillType = clipFillType;
      this.m_ClipType = clipType;
      this.m_UsingPolyTree = false;
      bool flag;
      try
      {
        flag = this.ExecuteInternal();
        if (flag)
          this.BuildResult(solution);
      }
      finally
      {
        this.DisposeAllPolyPts();
        this.m_ExecuteLocked = false;
      }
      return flag;
    }

    public bool Execute(
      ClipType clipType,
      PolyTree polytree,
      PolyFillType subjFillType,
      PolyFillType clipFillType)
    {
      if (this.m_ExecuteLocked)
        return false;
      this.m_ExecuteLocked = true;
      this.m_SubjFillType = subjFillType;
      this.m_ClipFillType = clipFillType;
      this.m_ClipType = clipType;
      this.m_UsingPolyTree = true;
      bool flag;
      try
      {
        flag = this.ExecuteInternal();
        if (flag)
          this.BuildResult2(polytree);
      }
      finally
      {
        this.DisposeAllPolyPts();
        this.m_ExecuteLocked = false;
      }
      return flag;
    }

    public bool Execute(ClipType clipType, List<List<IntPoint>> solution)
    {
      return this.Execute(clipType, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
    }

    public bool Execute(ClipType clipType, PolyTree polytree)
    {
      return this.Execute(clipType, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
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
        this.Reset();
        if (this.m_CurrentLM == null)
          return false;
        long botY = this.PopScanbeam();
        do
        {
          this.InsertLocalMinimaIntoAEL(botY);
          this.m_GhostJoins.Clear();
          this.ProcessHorizontals(false);
          if (this.m_Scanbeam != null)
          {
            long topY = this.PopScanbeam();
            if (!this.ProcessIntersections(topY))
              return false;
            this.ProcessEdgesAtTopOfScanbeam(topY);
            botY = topY;
          }
          else
            break;
        }
        while (this.m_Scanbeam != null || this.m_CurrentLM != null);
        for (int index = 0; index < this.m_PolyOuts.Count; ++index)
        {
          OutRec polyOut = this.m_PolyOuts[index];
          if (polyOut.Pts != null && !polyOut.IsOpen && (polyOut.IsHole ^ this.ReverseSolution) == this.Area(polyOut) > 0.0)
            this.ReversePolyPtLinks(polyOut.Pts);
        }
        this.JoinCommonEdges();
        for (int index = 0; index < this.m_PolyOuts.Count; ++index)
        {
          OutRec polyOut = this.m_PolyOuts[index];
          if (polyOut.Pts != null && !polyOut.IsOpen)
            this.FixupOutPolygon(polyOut);
        }
        if (this.StrictlySimple)
          this.DoSimplePolygons();
        return true;
      }
      finally
      {
        this.m_Joins.Clear();
        this.m_GhostJoins.Clear();
      }
    }

    private long PopScanbeam()
    {
      long y = this.m_Scanbeam.Y;
      this.m_Scanbeam = this.m_Scanbeam.Next;
      return y;
    }

    private void DisposeAllPolyPts()
    {
      for (int index = 0; index < this.m_PolyOuts.Count; ++index)
        this.DisposeOutRec(index);
      this.m_PolyOuts.Clear();
    }

    private void DisposeOutRec(int index)
    {
      this.m_PolyOuts[index].Pts = (OutPt) null;
      this.m_PolyOuts[index] = (OutRec) null;
    }

    private void AddJoin(OutPt Op1, OutPt Op2, IntPoint OffPt)
    {
      this.m_Joins.Add(new Join()
      {
        OutPt1 = Op1,
        OutPt2 = Op2,
        OffPt = OffPt
      });
    }

    private void AddGhostJoin(OutPt Op, IntPoint OffPt)
    {
      this.m_GhostJoins.Add(new Join()
      {
        OutPt1 = Op,
        OffPt = OffPt
      });
    }

    private void InsertLocalMinimaIntoAEL(long botY)
    {
      while (this.m_CurrentLM != null && this.m_CurrentLM.Y == botY)
      {
        TEdge leftBound = this.m_CurrentLM.LeftBound;
        TEdge rightBound = this.m_CurrentLM.RightBound;
        this.PopLocalMinima();
        OutPt outPt = (OutPt) null;
        if (leftBound == null)
        {
          this.InsertEdgeIntoAEL(rightBound, (TEdge) null);
          this.SetWindingCount(rightBound);
          if (this.IsContributing(rightBound))
            outPt = this.AddOutPt(rightBound, rightBound.Bot);
        }
        else if (rightBound == null)
        {
          this.InsertEdgeIntoAEL(leftBound, (TEdge) null);
          this.SetWindingCount(leftBound);
          if (this.IsContributing(leftBound))
            outPt = this.AddOutPt(leftBound, leftBound.Bot);
          this.InsertScanbeam(leftBound.Top.Y);
        }
        else
        {
          this.InsertEdgeIntoAEL(leftBound, (TEdge) null);
          this.InsertEdgeIntoAEL(rightBound, leftBound);
          this.SetWindingCount(leftBound);
          rightBound.WindCnt = leftBound.WindCnt;
          rightBound.WindCnt2 = leftBound.WindCnt2;
          if (this.IsContributing(leftBound))
            outPt = this.AddLocalMinPoly(leftBound, rightBound, leftBound.Bot);
          this.InsertScanbeam(leftBound.Top.Y);
        }
        if (rightBound != null)
        {
          if (ClipperBase.IsHorizontal(rightBound))
            this.AddEdgeToSEL(rightBound);
          else
            this.InsertScanbeam(rightBound.Top.Y);
        }
        if (leftBound != null && rightBound != null)
        {
          if (outPt != null && ClipperBase.IsHorizontal(rightBound) && this.m_GhostJoins.Count > 0 && rightBound.WindDelta != 0)
          {
            for (int index = 0; index < this.m_GhostJoins.Count; ++index)
            {
              Join ghostJoin = this.m_GhostJoins[index];
              if (this.HorzSegmentsOverlap(ghostJoin.OutPt1.Pt.X, ghostJoin.OffPt.X, rightBound.Bot.X, rightBound.Top.X))
                this.AddJoin(ghostJoin.OutPt1, outPt, ghostJoin.OffPt);
            }
          }
          if (leftBound.OutIdx >= 0 && leftBound.PrevInAEL != null && leftBound.PrevInAEL.Curr.X == leftBound.Bot.X && leftBound.PrevInAEL.OutIdx >= 0 && ClipperBase.SlopesEqual(leftBound.PrevInAEL, leftBound, this.m_UseFullRange) && leftBound.WindDelta != 0 && leftBound.PrevInAEL.WindDelta != 0)
          {
            OutPt Op2 = this.AddOutPt(leftBound.PrevInAEL, leftBound.Bot);
            this.AddJoin(outPt, Op2, leftBound.Top);
          }
          if (leftBound.NextInAEL != rightBound)
          {
            if (rightBound.OutIdx >= 0 && rightBound.PrevInAEL.OutIdx >= 0 && ClipperBase.SlopesEqual(rightBound.PrevInAEL, rightBound, this.m_UseFullRange) && rightBound.WindDelta != 0 && rightBound.PrevInAEL.WindDelta != 0)
            {
              OutPt Op2 = this.AddOutPt(rightBound.PrevInAEL, rightBound.Bot);
              this.AddJoin(outPt, Op2, rightBound.Top);
            }
            TEdge nextInAel = leftBound.NextInAEL;
            if (nextInAel != null)
            {
              for (; nextInAel != rightBound; nextInAel = nextInAel.NextInAEL)
                this.IntersectEdges(rightBound, nextInAel, leftBound.Curr);
            }
          }
        }
      }
    }

    private void InsertEdgeIntoAEL(TEdge edge, TEdge startEdge)
    {
      if (this.m_ActiveEdges == null)
      {
        edge.PrevInAEL = (TEdge) null;
        edge.NextInAEL = (TEdge) null;
        this.m_ActiveEdges = edge;
      }
      else if (startEdge == null && this.E2InsertsBeforeE1(this.m_ActiveEdges, edge))
      {
        edge.PrevInAEL = (TEdge) null;
        edge.NextInAEL = this.m_ActiveEdges;
        this.m_ActiveEdges.PrevInAEL = edge;
        this.m_ActiveEdges = edge;
      }
      else
      {
        if (startEdge == null)
          startEdge = this.m_ActiveEdges;
        while (startEdge.NextInAEL != null && !this.E2InsertsBeforeE1(startEdge.NextInAEL, edge))
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
      return e2.Top.Y > e1.Top.Y ? e2.Top.X < Clipper.TopX(e1, e2.Top.Y) : e1.Top.X > Clipper.TopX(e2, e1.Top.Y);
    }

    private bool IsEvenOddFillType(TEdge edge)
    {
      return edge.PolyTyp == PolyType.ptSubject ? this.m_SubjFillType == PolyFillType.pftEvenOdd : this.m_ClipFillType == PolyFillType.pftEvenOdd;
    }

    private bool IsEvenOddAltFillType(TEdge edge)
    {
      return edge.PolyTyp == PolyType.ptSubject ? this.m_ClipFillType == PolyFillType.pftEvenOdd : this.m_SubjFillType == PolyFillType.pftEvenOdd;
    }

    private bool IsContributing(TEdge edge)
    {
      PolyFillType polyFillType1;
      PolyFillType polyFillType2;
      if (edge.PolyTyp == PolyType.ptSubject)
      {
        polyFillType1 = this.m_SubjFillType;
        polyFillType2 = this.m_ClipFillType;
      }
      else
      {
        polyFillType1 = this.m_ClipFillType;
        polyFillType2 = this.m_SubjFillType;
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
      switch (this.m_ClipType)
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
          else
          {
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
        tedge = this.m_ActiveEdges;
      }
      else if (edge.WindDelta == 0 && this.m_ClipType != ClipType.ctUnion)
      {
        edge.WindCnt = 1;
        edge.WindCnt2 = prevInAel1.WindCnt2;
        tedge = prevInAel1.NextInAEL;
      }
      else if (this.IsEvenOddFillType(edge))
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
      if (this.IsEvenOddAltFillType(edge))
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
      if (this.m_SortedEdges == null)
      {
        this.m_SortedEdges = edge;
        edge.PrevInSEL = (TEdge) null;
        edge.NextInSEL = (TEdge) null;
      }
      else
      {
        edge.NextInSEL = this.m_SortedEdges;
        edge.PrevInSEL = (TEdge) null;
        this.m_SortedEdges.PrevInSEL = edge;
        this.m_SortedEdges = edge;
      }
    }

    private void CopyAELToSEL()
    {
      TEdge tedge = this.m_ActiveEdges;
      this.m_SortedEdges = tedge;
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
        this.m_ActiveEdges = edge1;
      }
      else
      {
        if (edge2.PrevInAEL != null)
          return;
        this.m_ActiveEdges = edge2;
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
        this.m_SortedEdges = edge1;
      }
      else
      {
        if (edge2.PrevInSEL != null)
          return;
        this.m_SortedEdges = edge2;
      }
    }

    private void AddLocalMaxPoly(TEdge e1, TEdge e2, IntPoint pt)
    {
      this.AddOutPt(e1, pt);
      if (e2.WindDelta == 0)
        this.AddOutPt(e2, pt);
      if (e1.OutIdx == e2.OutIdx)
      {
        e1.OutIdx = -1;
        e2.OutIdx = -1;
      }
      else if (e1.OutIdx < e2.OutIdx)
        this.AppendPolygon(e1, e2);
      else
        this.AppendPolygon(e2, e1);
    }

    private OutPt AddLocalMinPoly(TEdge e1, TEdge e2, IntPoint pt)
    {
      OutPt Op1;
      TEdge tedge1;
      TEdge tedge2;
      if (ClipperBase.IsHorizontal(e2) || e1.Dx > e2.Dx)
      {
        Op1 = this.AddOutPt(e1, pt);
        e2.OutIdx = e1.OutIdx;
        e1.Side = EdgeSide.esLeft;
        e2.Side = EdgeSide.esRight;
        tedge1 = e1;
        tedge2 = tedge1.PrevInAEL != e2 ? tedge1.PrevInAEL : e2.PrevInAEL;
      }
      else
      {
        Op1 = this.AddOutPt(e2, pt);
        e1.OutIdx = e2.OutIdx;
        e1.Side = EdgeSide.esRight;
        e2.Side = EdgeSide.esLeft;
        tedge1 = e2;
        tedge2 = tedge1.PrevInAEL != e1 ? tedge1.PrevInAEL : e1.PrevInAEL;
      }
      if (tedge2 != null && tedge2.OutIdx >= 0 && Clipper.TopX(tedge2, pt.Y) == Clipper.TopX(tedge1, pt.Y) && ClipperBase.SlopesEqual(tedge1, tedge2, this.m_UseFullRange) && tedge1.WindDelta != 0 && tedge2.WindDelta != 0)
      {
        OutPt Op2 = this.AddOutPt(tedge2, pt);
        this.AddJoin(Op1, Op2, tedge1.Top);
      }
      return Op1;
    }

    private OutRec CreateOutRec()
    {
      OutRec outRec = new OutRec();
      outRec.Idx = -1;
      outRec.IsHole = false;
      outRec.IsOpen = false;
      outRec.FirstLeft = (OutRec) null;
      outRec.Pts = (OutPt) null;
      outRec.BottomPt = (OutPt) null;
      outRec.PolyNode = (PolyNode) null;
      this.m_PolyOuts.Add(outRec);
      outRec.Idx = this.m_PolyOuts.Count - 1;
      return outRec;
    }

    private OutPt AddOutPt(TEdge e, IntPoint pt)
    {
      bool flag = e.Side == EdgeSide.esLeft;
      if (e.OutIdx < 0)
      {
        OutRec outRec = this.CreateOutRec();
        outRec.IsOpen = e.WindDelta == 0;
        OutPt outPt = new OutPt();
        outRec.Pts = outPt;
        outPt.Idx = outRec.Idx;
        outPt.Pt = pt;
        outPt.Next = outPt;
        outPt.Prev = outPt;
        if (!outRec.IsOpen)
          this.SetHoleState(e, outRec);
        e.OutIdx = outRec.Idx;
        return outPt;
      }
      OutRec polyOut = this.m_PolyOuts[e.OutIdx];
      OutPt pts = polyOut.Pts;
      if (flag && pt == pts.Pt)
        return pts;
      if (!flag && pt == pts.Prev.Pt)
        return pts.Prev;
      OutPt outPt1 = new OutPt()
      {
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
        this.Swap(ref seg1a, ref seg1b);
      if (seg2a > seg2b)
        this.Swap(ref seg2a, ref seg2b);
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
            outRec.FirstLeft = this.m_PolyOuts[prevInAel.OutIdx];
        }
      }
      if (!flag)
        return;
      outRec.IsHole = true;
    }

    private double GetDx(IntPoint pt1, IntPoint pt2)
    {
      return pt1.Y == pt2.Y ? -3.4E+38 : (double) (pt2.X - pt1.X) / (double) (pt2.Y - pt1.Y);
    }

    private bool FirstIsBottomPt(OutPt btmPt1, OutPt btmPt2)
    {
      OutPt prev1 = btmPt1.Prev;
      while (prev1.Pt == btmPt1.Pt && prev1 != btmPt1)
        prev1 = prev1.Prev;
      double num1 = Math.Abs(this.GetDx(btmPt1.Pt, prev1.Pt));
      OutPt next1 = btmPt1.Next;
      while (next1.Pt == btmPt1.Pt && next1 != btmPt1)
        next1 = next1.Next;
      double num2 = Math.Abs(this.GetDx(btmPt1.Pt, next1.Pt));
      OutPt prev2 = btmPt2.Prev;
      while (prev2.Pt == btmPt2.Pt && prev2 != btmPt2)
        prev2 = prev2.Prev;
      double num3 = Math.Abs(this.GetDx(btmPt2.Pt, prev2.Pt));
      OutPt next2 = btmPt2.Next;
      while (next2.Pt == btmPt2.Pt && next2 != btmPt2)
        next2 = next2.Next;
      double num4 = Math.Abs(this.GetDx(btmPt2.Pt, next2.Pt));
      return num1 >= num3 && num1 >= num4 || num2 >= num3 && num2 >= num4;
    }

    private OutPt GetBottomPt(OutPt pp)
    {
      OutPt btmPt2 = (OutPt) null;
      OutPt next;
      for (next = pp.Next; next != pp; next = next.Next)
      {
        if (next.Pt.Y > pp.Pt.Y)
        {
          pp = next;
          btmPt2 = (OutPt) null;
        }
        else if (next.Pt.Y == pp.Pt.Y && next.Pt.X <= pp.Pt.X)
        {
          if (next.Pt.X < pp.Pt.X)
          {
            btmPt2 = (OutPt) null;
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
          if (!this.FirstIsBottomPt(next, btmPt2))
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
        outRec1.BottomPt = this.GetBottomPt(outRec1.Pts);
      if (outRec2.BottomPt == null)
        outRec2.BottomPt = this.GetBottomPt(outRec2.Pts);
      OutPt bottomPt1 = outRec1.BottomPt;
      OutPt bottomPt2 = outRec2.BottomPt;
      return bottomPt1.Pt.Y > bottomPt2.Pt.Y || bottomPt1.Pt.Y >= bottomPt2.Pt.Y && (bottomPt1.Pt.X < bottomPt2.Pt.X || bottomPt1.Pt.X <= bottomPt2.Pt.X && bottomPt1.Next != bottomPt1 && (bottomPt2.Next == bottomPt2 || this.FirstIsBottomPt(bottomPt1, bottomPt2))) ? outRec1 : outRec2;
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
      OutRec polyOut = this.m_PolyOuts[idx];
      while (polyOut != this.m_PolyOuts[polyOut.Idx])
        polyOut = this.m_PolyOuts[polyOut.Idx];
      return polyOut;
    }

    private void AppendPolygon(TEdge e1, TEdge e2)
    {
      OutRec polyOut1 = this.m_PolyOuts[e1.OutIdx];
      OutRec polyOut2 = this.m_PolyOuts[e2.OutIdx];
      OutRec outRec = !this.Param1RightOfParam2(polyOut1, polyOut2) ? (!this.Param1RightOfParam2(polyOut2, polyOut1) ? this.GetLowermostRec(polyOut1, polyOut2) : polyOut1) : polyOut2;
      OutPt pts1 = polyOut1.Pts;
      OutPt prev1 = pts1.Prev;
      OutPt pts2 = polyOut2.Pts;
      OutPt prev2 = pts2.Prev;
      EdgeSide edgeSide;
      if (e1.Side == EdgeSide.esLeft)
      {
        if (e2.Side == EdgeSide.esLeft)
        {
          this.ReversePolyPtLinks(pts2);
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
          this.ReversePolyPtLinks(pts2);
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
      polyOut1.BottomPt = (OutPt) null;
      if (outRec == polyOut2)
      {
        if (polyOut2.FirstLeft != polyOut1)
          polyOut1.FirstLeft = polyOut2.FirstLeft;
        polyOut1.IsHole = polyOut2.IsHole;
      }
      polyOut2.Pts = (OutPt) null;
      polyOut2.BottomPt = (OutPt) null;
      polyOut2.FirstLeft = polyOut1;
      int outIdx1 = e1.OutIdx;
      int outIdx2 = e2.OutIdx;
      e1.OutIdx = -1;
      e2.OutIdx = -1;
      for (TEdge tedge = this.m_ActiveEdges; tedge != null; tedge = tedge.NextInAEL)
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
        if (this.IsEvenOddFillType(e1))
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
        if (!this.IsEvenOddFillType(e2))
          e1.WindCnt2 += e2.WindDelta;
        else
          e1.WindCnt2 = e1.WindCnt2 == 0 ? 1 : 0;
        if (!this.IsEvenOddFillType(e1))
          e2.WindCnt2 -= e1.WindDelta;
        else
          e2.WindCnt2 = e2.WindCnt2 == 0 ? 1 : 0;
      }
      PolyFillType polyFillType1;
      PolyFillType polyFillType2;
      if (e1.PolyTyp == PolyType.ptSubject)
      {
        polyFillType1 = this.m_SubjFillType;
        polyFillType2 = this.m_ClipFillType;
      }
      else
      {
        polyFillType1 = this.m_ClipFillType;
        polyFillType2 = this.m_SubjFillType;
      }
      PolyFillType polyFillType3;
      PolyFillType polyFillType4;
      if (e2.PolyTyp == PolyType.ptSubject)
      {
        polyFillType3 = this.m_SubjFillType;
        polyFillType4 = this.m_ClipFillType;
      }
      else
      {
        polyFillType3 = this.m_ClipFillType;
        polyFillType4 = this.m_SubjFillType;
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
        if (num1 != 0 && num1 != 1 || num2 != 0 && num2 != 1 || e1.PolyTyp != e2.PolyTyp && this.m_ClipType != ClipType.ctXor)
        {
          this.AddLocalMaxPoly(e1, e2, pt);
        }
        else
        {
          this.AddOutPt(e1, pt);
          this.AddOutPt(e2, pt);
          Clipper.SwapSides(e1, e2);
          Clipper.SwapPolyIndexes(e1, e2);
        }
      }
      else if (flag1)
      {
        if (num2 != 0 && num2 != 1)
          return;
        this.AddOutPt(e1, pt);
        Clipper.SwapSides(e1, e2);
        Clipper.SwapPolyIndexes(e1, e2);
      }
      else if (flag2)
      {
        if (num1 != 0 && num1 != 1)
          return;
        this.AddOutPt(e2, pt);
        Clipper.SwapSides(e1, e2);
        Clipper.SwapPolyIndexes(e1, e2);
      }
      else
      {
        if (num1 != 0 && num1 != 1 || num2 != 0 && num2 != 1)
          return;
        long num3;
        switch (polyFillType2)
        {
          case PolyFillType.pftPositive:
            num3 = (long) e1.WindCnt2;
            break;
          case PolyFillType.pftNegative:
            num3 = (long) -e1.WindCnt2;
            break;
          default:
            num3 = (long) Math.Abs(e1.WindCnt2);
            break;
        }
        long num4;
        switch (polyFillType4)
        {
          case PolyFillType.pftPositive:
            num4 = (long) e2.WindCnt2;
            break;
          case PolyFillType.pftNegative:
            num4 = (long) -e2.WindCnt2;
            break;
          default:
            num4 = (long) Math.Abs(e2.WindCnt2);
            break;
        }
        if (e1.PolyTyp != e2.PolyTyp)
          this.AddLocalMinPoly(e1, e2, pt);
        else if (num1 == 1 && num2 == 1)
        {
          switch (this.m_ClipType)
          {
            case ClipType.ctIntersection:
              if (num3 > 0L && num4 > 0L)
              {
                this.AddLocalMinPoly(e1, e2, pt);
                break;
              }
              break;
            case ClipType.ctUnion:
              if (num3 <= 0L && num4 <= 0L)
              {
                this.AddLocalMinPoly(e1, e2, pt);
                break;
              }
              break;
            case ClipType.ctDifference:
              if (e1.PolyTyp == PolyType.ptClip && num3 > 0L && num4 > 0L || e1.PolyTyp == PolyType.ptSubject && num3 <= 0L && num4 <= 0L)
              {
                this.AddLocalMinPoly(e1, e2, pt);
                break;
              }
              break;
            case ClipType.ctXor:
              this.AddLocalMinPoly(e1, e2, pt);
              break;
          }
        }
        else
          Clipper.SwapSides(e1, e2);
      }
    }

    private void DeleteFromAEL(TEdge e)
    {
      TEdge prevInAel = e.PrevInAEL;
      TEdge nextInAel = e.NextInAEL;
      if (prevInAel == null && nextInAel == null && e != this.m_ActiveEdges)
        return;
      if (prevInAel != null)
        prevInAel.NextInAEL = nextInAel;
      else
        this.m_ActiveEdges = nextInAel;
      if (nextInAel != null)
        nextInAel.PrevInAEL = prevInAel;
      e.NextInAEL = (TEdge) null;
      e.PrevInAEL = (TEdge) null;
    }

    private void DeleteFromSEL(TEdge e)
    {
      TEdge prevInSel = e.PrevInSEL;
      TEdge nextInSel = e.NextInSEL;
      if (prevInSel == null && nextInSel == null && e != this.m_SortedEdges)
        return;
      if (prevInSel != null)
        prevInSel.NextInSEL = nextInSel;
      else
        this.m_SortedEdges = nextInSel;
      if (nextInSel != null)
        nextInSel.PrevInSEL = prevInSel;
      e.NextInSEL = (TEdge) null;
      e.PrevInSEL = (TEdge) null;
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
        this.m_ActiveEdges = e.NextInLML;
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
      if (ClipperBase.IsHorizontal(e))
        return;
      this.InsertScanbeam(e.Top.Y);
    }

    private void ProcessHorizontals(bool isTopOfScanbeam)
    {
      for (TEdge sortedEdges = this.m_SortedEdges; sortedEdges != null; sortedEdges = this.m_SortedEdges)
      {
        this.DeleteFromSEL(sortedEdges);
        this.ProcessHorizontal(sortedEdges, isTopOfScanbeam);
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
      Direction Dir;
      long Left;
      long Right;
      this.GetHorzDirection(horzEdge, out Dir, out Left, out Right);
      TEdge e1 = horzEdge;
      TEdge tedge1 = (TEdge) null;
      while (e1.NextInLML != null && ClipperBase.IsHorizontal(e1.NextInLML))
        e1 = e1.NextInLML;
      if (e1.NextInLML == null)
        tedge1 = this.GetMaximaPair(e1);
      while (true)
      {
        bool flag = horzEdge == e1;
        TEdge nextInAel;
        for (TEdge tedge2 = this.GetNextInAEL(horzEdge, Dir); tedge2 != null && (tedge2.Curr.X != horzEdge.Top.X || horzEdge.NextInLML == null || tedge2.Dx >= horzEdge.NextInLML.Dx); tedge2 = nextInAel)
        {
          nextInAel = this.GetNextInAEL(tedge2, Dir);
          if (Dir == Direction.dLeftToRight && tedge2.Curr.X <= Right || Dir == Direction.dRightToLeft && tedge2.Curr.X >= Left)
          {
            if (tedge2 == tedge1 & flag)
            {
              if (horzEdge.OutIdx >= 0)
              {
                OutPt outPt = this.AddOutPt(horzEdge, horzEdge.Top);
                for (TEdge e2 = this.m_SortedEdges; e2 != null; e2 = e2.NextInSEL)
                {
                  if (e2.OutIdx >= 0 && this.HorzSegmentsOverlap(horzEdge.Bot.X, horzEdge.Top.X, e2.Bot.X, e2.Top.X))
                    this.AddJoin(this.AddOutPt(e2, e2.Bot), outPt, e2.Top);
                }
                this.AddGhostJoin(outPt, horzEdge.Bot);
                this.AddLocalMaxPoly(horzEdge, tedge1, horzEdge.Top);
              }
              this.DeleteFromAEL(horzEdge);
              this.DeleteFromAEL(tedge1);
              return;
            }
            if (Dir == Direction.dLeftToRight)
            {
              IntPoint pt = new IntPoint(tedge2.Curr.X, horzEdge.Curr.Y);
              this.IntersectEdges(horzEdge, tedge2, pt);
            }
            else
            {
              IntPoint pt = new IntPoint(tedge2.Curr.X, horzEdge.Curr.Y);
              this.IntersectEdges(tedge2, horzEdge, pt);
            }
            this.SwapPositionsInAEL(horzEdge, tedge2);
          }
          else if (Dir == Direction.dLeftToRight && tedge2.Curr.X >= Right || Dir == Direction.dRightToLeft && tedge2.Curr.X <= Left)
            break;
        }
        if (horzEdge.NextInLML != null && ClipperBase.IsHorizontal(horzEdge.NextInLML))
        {
          this.UpdateEdgeIntoAEL(ref horzEdge);
          if (horzEdge.OutIdx >= 0)
            this.AddOutPt(horzEdge, horzEdge.Bot);
          this.GetHorzDirection(horzEdge, out Dir, out Left, out Right);
        }
        else
          break;
      }
      if (horzEdge.NextInLML != null)
      {
        if (horzEdge.OutIdx >= 0)
        {
          OutPt outPt = this.AddOutPt(horzEdge, horzEdge.Top);
          if (isTopOfScanbeam)
            this.AddGhostJoin(outPt, horzEdge.Bot);
          this.UpdateEdgeIntoAEL(ref horzEdge);
          if (horzEdge.WindDelta == 0)
            return;
          TEdge prevInAel = horzEdge.PrevInAEL;
          TEdge nextInAel = horzEdge.NextInAEL;
          if (prevInAel != null && prevInAel.Curr.X == horzEdge.Bot.X && prevInAel.Curr.Y == horzEdge.Bot.Y && prevInAel.WindDelta != 0 && prevInAel.OutIdx >= 0 && prevInAel.Curr.Y > prevInAel.Top.Y && ClipperBase.SlopesEqual(horzEdge, prevInAel, this.m_UseFullRange))
          {
            OutPt Op2 = this.AddOutPt(prevInAel, horzEdge.Bot);
            this.AddJoin(outPt, Op2, horzEdge.Top);
          }
          else
          {
            if (nextInAel == null || nextInAel.Curr.X != horzEdge.Bot.X || nextInAel.Curr.Y != horzEdge.Bot.Y || nextInAel.WindDelta == 0 || nextInAel.OutIdx < 0 || nextInAel.Curr.Y <= nextInAel.Top.Y || !ClipperBase.SlopesEqual(horzEdge, nextInAel, this.m_UseFullRange))
              return;
            OutPt Op2 = this.AddOutPt(nextInAel, horzEdge.Bot);
            this.AddJoin(outPt, Op2, horzEdge.Top);
          }
        }
        else
          this.UpdateEdgeIntoAEL(ref horzEdge);
      }
      else
      {
        if (horzEdge.OutIdx >= 0)
          this.AddOutPt(horzEdge, horzEdge.Top);
        this.DeleteFromAEL(horzEdge);
      }
    }

    private TEdge GetNextInAEL(TEdge e, Direction Direction)
    {
      return Direction == Direction.dLeftToRight ? e.NextInAEL : e.PrevInAEL;
    }

    private bool IsMinima(TEdge e) => e != null && e.Prev.NextInLML != e && e.Next.NextInLML != e;

    private bool IsMaxima(TEdge e, double Y)
    {
      return e != null && (double) e.Top.Y == Y && e.NextInLML == null;
    }

    private bool IsIntermediate(TEdge e, double Y) => (double) e.Top.Y == Y && e.NextInLML != null;

    private TEdge GetMaximaPair(TEdge e)
    {
      TEdge e1 = (TEdge) null;
      if (e.Next.Top == e.Top && e.Next.NextInLML == null)
        e1 = e.Next;
      else if (e.Prev.Top == e.Top && e.Prev.NextInLML == null)
        e1 = e.Prev;
      return e1 != null && (e1.OutIdx == -2 || e1.NextInAEL == e1.PrevInAEL && !ClipperBase.IsHorizontal(e1)) ? (TEdge) null : e1;
    }

    private bool ProcessIntersections(long topY)
    {
      if (this.m_ActiveEdges == null)
        return true;
      try
      {
        this.BuildIntersectList(topY);
        if (this.m_IntersectList.Count == 0)
          return true;
        if (this.m_IntersectList.Count != 1 && !this.FixupIntersectionOrder())
          return false;
        this.ProcessIntersectList();
      }
      catch
      {
        this.m_SortedEdges = (TEdge) null;
        this.m_IntersectList.Clear();
        throw new ClipperException("ProcessIntersections error");
      }
      this.m_SortedEdges = (TEdge) null;
      return true;
    }

    private void BuildIntersectList(long topY)
    {
      if (this.m_ActiveEdges == null)
        return;
      TEdge edge = this.m_ActiveEdges;
      this.m_SortedEdges = edge;
      for (; edge != null; edge = edge.NextInAEL)
      {
        edge.PrevInSEL = edge.PrevInAEL;
        edge.NextInSEL = edge.NextInAEL;
        edge.Curr.X = Clipper.TopX(edge, topY);
      }
      bool flag = true;
      while (flag && this.m_SortedEdges != null)
      {
        flag = false;
        TEdge edge1 = this.m_SortedEdges;
        while (edge1.NextInSEL != null)
        {
          TEdge nextInSel = edge1.NextInSEL;
          if (edge1.Curr.X > nextInSel.Curr.X)
          {
            IntPoint ip;
            this.IntersectPoint(edge1, nextInSel, out ip);
            this.m_IntersectList.Add(new IntersectNode()
            {
              Edge1 = edge1,
              Edge2 = nextInSel,
              Pt = ip
            });
            this.SwapPositionsInSEL(edge1, nextInSel);
            flag = true;
          }
          else
            edge1 = nextInSel;
        }
        if (edge1.PrevInSEL != null)
          edge1.PrevInSEL.NextInSEL = (TEdge) null;
        else
          break;
      }
      this.m_SortedEdges = (TEdge) null;
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
      this.m_IntersectList.Sort(this.m_IntersectNodeComparer);
      this.CopyAELToSEL();
      int count = this.m_IntersectList.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        if (!this.EdgesAdjacent(this.m_IntersectList[index1]))
        {
          int index2 = index1 + 1;
          while (index2 < count && !this.EdgesAdjacent(this.m_IntersectList[index2]))
            ++index2;
          if (index2 == count)
            return false;
          IntersectNode intersect = this.m_IntersectList[index1];
          this.m_IntersectList[index1] = this.m_IntersectList[index2];
          this.m_IntersectList[index2] = intersect;
        }
        this.SwapPositionsInSEL(this.m_IntersectList[index1].Edge1, this.m_IntersectList[index1].Edge2);
      }
      return true;
    }

    private void ProcessIntersectList()
    {
      for (int index = 0; index < this.m_IntersectList.Count; ++index)
      {
        IntersectNode intersect = this.m_IntersectList[index];
        this.IntersectEdges(intersect.Edge1, intersect.Edge2, intersect.Pt);
        this.SwapPositionsInAEL(intersect.Edge1, intersect.Edge2);
      }
      this.m_IntersectList.Clear();
    }

    internal static long Round(double value)
    {
      return value < 0.0 ? (long) (value - 0.5) : (long) (value + 0.5);
    }

    private static long TopX(TEdge edge, long currentY)
    {
      return currentY == edge.Top.Y ? edge.Top.X : edge.Bot.X + Clipper.Round(edge.Dx * (double) (currentY - edge.Bot.Y));
    }

    private void IntersectPoint(TEdge edge1, TEdge edge2, out IntPoint ip)
    {
      ip = new IntPoint();
      if (edge1.Dx == edge2.Dx)
      {
        ip.Y = edge1.Curr.Y;
        ip.X = Clipper.TopX(edge1, ip.Y);
      }
      else
      {
        if (edge1.Delta.X == 0L)
        {
          ip.X = edge1.Bot.X;
          if (ClipperBase.IsHorizontal(edge2))
          {
            ip.Y = edge2.Bot.Y;
          }
          else
          {
            double num = (double) edge2.Bot.Y - (double) edge2.Bot.X / edge2.Dx;
            ip.Y = Clipper.Round((double) ip.X / edge2.Dx + num);
          }
        }
        else if (edge2.Delta.X == 0L)
        {
          ip.X = edge2.Bot.X;
          if (ClipperBase.IsHorizontal(edge1))
          {
            ip.Y = edge1.Bot.Y;
          }
          else
          {
            double num = (double) edge1.Bot.Y - (double) edge1.Bot.X / edge1.Dx;
            ip.Y = Clipper.Round((double) ip.X / edge1.Dx + num);
          }
        }
        else
        {
          double num1 = (double) edge1.Bot.X - (double) edge1.Bot.Y * edge1.Dx;
          double num2 = (double) edge2.Bot.X - (double) edge2.Bot.Y * edge2.Dx;
          double num3 = (num2 - num1) / (edge1.Dx - edge2.Dx);
          ip.Y = Clipper.Round(num3);
          ip.X = Math.Abs(edge1.Dx) >= Math.Abs(edge2.Dx) ? Clipper.Round(edge2.Dx * num3 + num2) : Clipper.Round(edge1.Dx * num3 + num1);
        }
        if (ip.Y < edge1.Top.Y || ip.Y < edge2.Top.Y)
        {
          ip.Y = edge1.Top.Y <= edge2.Top.Y ? edge2.Top.Y : edge1.Top.Y;
          ip.X = Math.Abs(edge1.Dx) >= Math.Abs(edge2.Dx) ? Clipper.TopX(edge2, ip.Y) : Clipper.TopX(edge1, ip.Y);
        }
        if (ip.Y <= edge1.Curr.Y)
          return;
        ip.Y = edge1.Curr.Y;
        ip.X = Math.Abs(edge1.Dx) <= Math.Abs(edge2.Dx) ? Clipper.TopX(edge1, ip.Y) : Clipper.TopX(edge2, ip.Y);
      }
    }

    private void ProcessEdgesAtTopOfScanbeam(long topY)
    {
      TEdge e1 = this.m_ActiveEdges;
      while (e1 != null)
      {
        bool flag = this.IsMaxima(e1, (double) topY);
        if (flag)
        {
          TEdge maximaPair = this.GetMaximaPair(e1);
          flag = maximaPair == null || !ClipperBase.IsHorizontal(maximaPair);
        }
        if (flag)
        {
          TEdge prevInAel = e1.PrevInAEL;
          this.DoMaxima(e1);
          e1 = prevInAel != null ? prevInAel.NextInAEL : this.m_ActiveEdges;
        }
        else
        {
          if (this.IsIntermediate(e1, (double) topY) && ClipperBase.IsHorizontal(e1.NextInLML))
          {
            this.UpdateEdgeIntoAEL(ref e1);
            if (e1.OutIdx >= 0)
              this.AddOutPt(e1, e1.Bot);
            this.AddEdgeToSEL(e1);
          }
          else
          {
            e1.Curr.X = Clipper.TopX(e1, topY);
            e1.Curr.Y = topY;
          }
          if (this.StrictlySimple)
          {
            TEdge prevInAel = e1.PrevInAEL;
            if (e1.OutIdx >= 0 && e1.WindDelta != 0 && prevInAel != null && prevInAel.OutIdx >= 0 && prevInAel.Curr.X == e1.Curr.X && prevInAel.WindDelta != 0)
            {
              IntPoint intPoint = new IntPoint(e1.Curr);
              this.AddJoin(this.AddOutPt(prevInAel, intPoint), this.AddOutPt(e1, intPoint), intPoint);
            }
          }
          e1 = e1.NextInAEL;
        }
      }
      this.ProcessHorizontals(true);
      for (TEdge e2 = this.m_ActiveEdges; e2 != null; e2 = e2.NextInAEL)
      {
        if (this.IsIntermediate(e2, (double) topY))
        {
          OutPt Op1 = (OutPt) null;
          if (e2.OutIdx >= 0)
            Op1 = this.AddOutPt(e2, e2.Top);
          this.UpdateEdgeIntoAEL(ref e2);
          TEdge prevInAel = e2.PrevInAEL;
          TEdge nextInAel = e2.NextInAEL;
          if (prevInAel != null && prevInAel.Curr.X == e2.Bot.X && prevInAel.Curr.Y == e2.Bot.Y && Op1 != null && prevInAel.OutIdx >= 0 && prevInAel.Curr.Y > prevInAel.Top.Y && ClipperBase.SlopesEqual(e2, prevInAel, this.m_UseFullRange) && e2.WindDelta != 0 && prevInAel.WindDelta != 0)
          {
            OutPt Op2 = this.AddOutPt(prevInAel, e2.Bot);
            this.AddJoin(Op1, Op2, e2.Top);
          }
          else if (nextInAel != null && nextInAel.Curr.X == e2.Bot.X && nextInAel.Curr.Y == e2.Bot.Y && Op1 != null && nextInAel.OutIdx >= 0 && nextInAel.Curr.Y > nextInAel.Top.Y && ClipperBase.SlopesEqual(e2, nextInAel, this.m_UseFullRange) && e2.WindDelta != 0 && nextInAel.WindDelta != 0)
          {
            OutPt Op2 = this.AddOutPt(nextInAel, e2.Bot);
            this.AddJoin(Op1, Op2, e2.Top);
          }
        }
      }
    }

    private void DoMaxima(TEdge e)
    {
      TEdge maximaPair = this.GetMaximaPair(e);
      if (maximaPair == null)
      {
        if (e.OutIdx >= 0)
          this.AddOutPt(e, e.Top);
        this.DeleteFromAEL(e);
      }
      else
      {
        for (TEdge nextInAel = e.NextInAEL; nextInAel != null && nextInAel != maximaPair; nextInAel = e.NextInAEL)
        {
          this.IntersectEdges(e, nextInAel, e.Top);
          this.SwapPositionsInAEL(e, nextInAel);
        }
        if (e.OutIdx == -1 && maximaPair.OutIdx == -1)
        {
          this.DeleteFromAEL(e);
          this.DeleteFromAEL(maximaPair);
        }
        else
        {
          if (e.OutIdx < 0 || maximaPair.OutIdx < 0)
            throw new ClipperException("DoMaxima error");
          if (e.OutIdx >= 0)
            this.AddLocalMaxPoly(e, maximaPair, e.Top);
          this.DeleteFromAEL(e);
          this.DeleteFromAEL(maximaPair);
        }
      }
    }

    public static void ReversePaths(List<List<IntPoint>> polys)
    {
      foreach (List<IntPoint> poly in polys)
        poly.Reverse();
    }

    public static bool Orientation(List<IntPoint> poly) => Clipper.Area(poly) >= 0.0;

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
      polyg.Capacity = this.m_PolyOuts.Count;
      for (int index1 = 0; index1 < this.m_PolyOuts.Count; ++index1)
      {
        OutRec polyOut = this.m_PolyOuts[index1];
        if (polyOut.Pts != null)
        {
          OutPt prev = polyOut.Pts.Prev;
          int capacity = this.PointCount(prev);
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
      polytree.m_AllPolys.Capacity = this.m_PolyOuts.Count;
      for (int index1 = 0; index1 < this.m_PolyOuts.Count; ++index1)
      {
        OutRec polyOut = this.m_PolyOuts[index1];
        int num = this.PointCount(polyOut.Pts);
        if ((!polyOut.IsOpen || num >= 2) && (polyOut.IsOpen || num >= 3))
        {
          this.FixHoleLinkage(polyOut);
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
      polytree.m_Childs.Capacity = this.m_PolyOuts.Count;
      for (int index = 0; index < this.m_PolyOuts.Count; ++index)
      {
        OutRec polyOut = this.m_PolyOuts[index];
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
      OutPt outPt1 = (OutPt) null;
      outRec.BottomPt = (OutPt) null;
      OutPt outPt2 = outRec.Pts;
      while (outPt2.Prev != outPt2 && outPt2.Prev != outPt2.Next)
      {
        if (outPt2.Pt == outPt2.Next.Pt || outPt2.Pt == outPt2.Prev.Pt || ClipperBase.SlopesEqual(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt, this.m_UseFullRange) && (!this.PreserveCollinear || !this.Pt2IsBetweenPt1AndPt3(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt)))
        {
          outPt1 = (OutPt) null;
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
      outRec.Pts = (OutPt) null;
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
        op1b = this.DupOutPt(op1, !DiscardLeft);
        if (op1b.Pt != Pt)
        {
          op1 = op1b;
          op1.Pt = Pt;
          op1b = this.DupOutPt(op1, !DiscardLeft);
        }
      }
      else
      {
        while (op1.Next.Pt.X >= Pt.X && op1.Next.Pt.X <= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
          op1 = op1.Next;
        if (!DiscardLeft && op1.Pt.X != Pt.X)
          op1 = op1.Next;
        op1b = this.DupOutPt(op1, DiscardLeft);
        if (op1b.Pt != Pt)
        {
          op1 = op1b;
          op1.Pt = Pt;
          op1b = this.DupOutPt(op1, DiscardLeft);
        }
      }
      if (direction2 == Direction.dLeftToRight)
      {
        while (op2.Next.Pt.X <= Pt.X && op2.Next.Pt.X >= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
          op2 = op2.Next;
        if (DiscardLeft && op2.Pt.X != Pt.X)
          op2 = op2.Next;
        op2b = this.DupOutPt(op2, !DiscardLeft);
        if (op2b.Pt != Pt)
        {
          op2 = op2b;
          op2.Pt = Pt;
          op2b = this.DupOutPt(op2, !DiscardLeft);
        }
      }
      else
      {
        while (op2.Next.Pt.X >= Pt.X && op2.Next.Pt.X <= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
          op2 = op2.Next;
        if (!DiscardLeft && op2.Pt.X != Pt.X)
          op2 = op2.Next;
        op2b = this.DupOutPt(op2, DiscardLeft);
        if (op2b.Pt != Pt)
        {
          op2 = op2b;
          op2.Pt = Pt;
          op2b = this.DupOutPt(op2, DiscardLeft);
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
          OutPt outPt3 = this.DupOutPt(outPt1, false);
          OutPt outPt4 = this.DupOutPt(outPt2, true);
          outPt1.Prev = outPt2;
          outPt2.Next = outPt1;
          outPt3.Next = outPt4;
          outPt4.Prev = outPt3;
          j.OutPt1 = outPt1;
          j.OutPt2 = outPt3;
          return true;
        }
        OutPt outPt5 = this.DupOutPt(outPt1, true);
        OutPt outPt6 = this.DupOutPt(outPt2, false);
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
        long Left;
        long Right;
        if (op2b.Next == outPt2 || op2b.Next == outPt1 || !this.GetOverlap(outPt1.Pt.X, op1b.Pt.X, outPt2.Pt.X, op2b.Pt.X, out Left, out Right))
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
        return this.JoinHorz(outPt1, op1b, outPt2, op2b, pt, DiscardLeft);
      }
      OutPt outPt7 = outPt1.Next;
      while (outPt7.Pt == outPt1.Pt && outPt7 != outPt1)
        outPt7 = outPt7.Next;
      bool flag4 = outPt7.Pt.Y > outPt1.Pt.Y || !ClipperBase.SlopesEqual(outPt1.Pt, outPt7.Pt, j.OffPt, this.m_UseFullRange);
      if (flag4)
      {
        outPt7 = outPt1.Prev;
        while (outPt7.Pt == outPt1.Pt && outPt7 != outPt1)
          outPt7 = outPt7.Prev;
        if (outPt7.Pt.Y > outPt1.Pt.Y || !ClipperBase.SlopesEqual(outPt1.Pt, outPt7.Pt, j.OffPt, this.m_UseFullRange))
          return false;
      }
      OutPt outPt8 = outPt2.Next;
      while (outPt8.Pt == outPt2.Pt && outPt8 != outPt2)
        outPt8 = outPt8.Next;
      bool flag5 = outPt8.Pt.Y > outPt2.Pt.Y || !ClipperBase.SlopesEqual(outPt2.Pt, outPt8.Pt, j.OffPt, this.m_UseFullRange);
      if (flag5)
      {
        outPt8 = outPt2.Prev;
        while (outPt8.Pt == outPt2.Pt && outPt8 != outPt2)
          outPt8 = outPt8.Prev;
        if (outPt8.Pt.Y > outPt2.Pt.Y || !ClipperBase.SlopesEqual(outPt2.Pt, outPt8.Pt, j.OffPt, this.m_UseFullRange))
          return false;
      }
      if (outPt7 == outPt1 || outPt8 == outPt2 || outPt7 == outPt8 || outRec1 == outRec2 && flag4 == flag5)
        return false;
      if (flag4)
      {
        OutPt outPt9 = this.DupOutPt(outPt1, false);
        OutPt outPt10 = this.DupOutPt(outPt2, true);
        outPt1.Prev = outPt2;
        outPt2.Next = outPt1;
        outPt9.Next = outPt10;
        outPt10.Prev = outPt9;
        j.OutPt1 = outPt1;
        j.OutPt2 = outPt9;
        return true;
      }
      OutPt outPt11 = this.DupOutPt(outPt1, true);
      OutPt outPt12 = this.DupOutPt(outPt2, false);
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
              double num2 = (double) (intPoint1.X - pt.X) * (double) (intPoint2.Y - pt.Y) - (double) (intPoint2.X - pt.X) * (double) (intPoint1.Y - pt.Y);
              if (num2 == 0.0)
                return -1;
              if (num2 > 0.0 == intPoint2.Y > intPoint1.Y)
                num1 = 1 - num1;
            }
          }
          else if (intPoint2.X > pt.X)
          {
            double num3 = (double) (intPoint1.X - pt.X) * (double) (intPoint2.Y - pt.Y) - (double) (intPoint2.X - pt.X) * (double) (intPoint1.Y - pt.Y);
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
              double num4 = (double) (num2 - x1) * (double) (y2 - y1) - (double) (x2 - x1) * (double) (num3 - y1);
              if (num4 == 0.0)
                return -1;
              if (num4 > 0.0 == y2 > num3)
                num1 = 1 - num1;
            }
          }
          else if (x2 > x1)
          {
            double num5 = (double) (num2 - x1) * (double) (y2 - y1) - (double) (x2 - x1) * (double) (num3 - y1);
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
        int num = Clipper.PointInPolygon(outPt.Pt, outPt2);
        if (num >= 0)
          return num > 0;
        outPt = outPt.Next;
      }
      while (outPt != outPt1);
      return true;
    }

    private void FixupFirstLefts1(OutRec OldOutRec, OutRec NewOutRec)
    {
      for (int index = 0; index < this.m_PolyOuts.Count; ++index)
      {
        OutRec polyOut = this.m_PolyOuts[index];
        if (polyOut.Pts != null && polyOut.FirstLeft != null && Clipper.ParseFirstLeft(polyOut.FirstLeft) == OldOutRec && Clipper.Poly2ContainsPoly1(polyOut.Pts, NewOutRec.Pts))
          polyOut.FirstLeft = NewOutRec;
      }
    }

    private void FixupFirstLefts2(OutRec OldOutRec, OutRec NewOutRec)
    {
      foreach (OutRec polyOut in this.m_PolyOuts)
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
      for (int index1 = 0; index1 < this.m_Joins.Count; ++index1)
      {
        Join join = this.m_Joins[index1];
        OutRec outRec1 = this.GetOutRec(join.OutPt1.Idx);
        OutRec outRec2 = this.GetOutRec(join.OutPt2.Idx);
        if (outRec1.Pts != null && outRec2.Pts != null)
        {
          OutRec outRec3 = outRec1 != outRec2 ? (!this.Param1RightOfParam2(outRec1, outRec2) ? (!this.Param1RightOfParam2(outRec2, outRec1) ? this.GetLowermostRec(outRec1, outRec2) : outRec1) : outRec2) : outRec1;
          if (this.JoinPoints(join, outRec1, outRec2))
          {
            if (outRec1 == outRec2)
            {
              outRec1.Pts = join.OutPt1;
              outRec1.BottomPt = (OutPt) null;
              OutRec outRec4 = this.CreateOutRec();
              outRec4.Pts = join.OutPt2;
              this.UpdateOutPtIdxs(outRec4);
              if (this.m_UsingPolyTree)
              {
                for (int index2 = 0; index2 < this.m_PolyOuts.Count - 1; ++index2)
                {
                  OutRec polyOut = this.m_PolyOuts[index2];
                  if (polyOut.Pts != null && Clipper.ParseFirstLeft(polyOut.FirstLeft) == outRec1 && polyOut.IsHole != outRec1.IsHole && Clipper.Poly2ContainsPoly1(polyOut.Pts, join.OutPt2))
                    polyOut.FirstLeft = outRec4;
                }
              }
              if (Clipper.Poly2ContainsPoly1(outRec4.Pts, outRec1.Pts))
              {
                outRec4.IsHole = !outRec1.IsHole;
                outRec4.FirstLeft = outRec1;
                if (this.m_UsingPolyTree)
                  this.FixupFirstLefts2(outRec4, outRec1);
                if ((outRec4.IsHole ^ this.ReverseSolution) == this.Area(outRec4) > 0.0)
                  this.ReversePolyPtLinks(outRec4.Pts);
              }
              else if (Clipper.Poly2ContainsPoly1(outRec1.Pts, outRec4.Pts))
              {
                outRec4.IsHole = outRec1.IsHole;
                outRec1.IsHole = !outRec4.IsHole;
                outRec4.FirstLeft = outRec1.FirstLeft;
                outRec1.FirstLeft = outRec4;
                if (this.m_UsingPolyTree)
                  this.FixupFirstLefts2(outRec1, outRec4);
                if ((outRec1.IsHole ^ this.ReverseSolution) == this.Area(outRec1) > 0.0)
                  this.ReversePolyPtLinks(outRec1.Pts);
              }
              else
              {
                outRec4.IsHole = outRec1.IsHole;
                outRec4.FirstLeft = outRec1.FirstLeft;
                if (this.m_UsingPolyTree)
                  this.FixupFirstLefts1(outRec1, outRec4);
              }
            }
            else
            {
              outRec2.Pts = (OutPt) null;
              outRec2.BottomPt = (OutPt) null;
              outRec2.Idx = outRec1.Idx;
              outRec1.IsHole = outRec3.IsHole;
              if (outRec3 == outRec2)
                outRec1.FirstLeft = outRec2.FirstLeft;
              outRec2.FirstLeft = outRec1;
              if (this.m_UsingPolyTree)
                this.FixupFirstLefts2(outRec2, outRec1);
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
      while (num < this.m_PolyOuts.Count)
      {
        OutRec polyOut = this.m_PolyOuts[num++];
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
                OutRec outRec = this.CreateOutRec();
                outRec.Pts = outPt2;
                this.UpdateOutPtIdxs(outRec);
                if (Clipper.Poly2ContainsPoly1(outRec.Pts, polyOut.Pts))
                {
                  outRec.IsHole = !polyOut.IsHole;
                  outRec.FirstLeft = polyOut;
                  if (this.m_UsingPolyTree)
                    this.FixupFirstLefts2(outRec, polyOut);
                }
                else if (Clipper.Poly2ContainsPoly1(polyOut.Pts, outRec.Pts))
                {
                  outRec.IsHole = polyOut.IsHole;
                  polyOut.IsHole = !outRec.IsHole;
                  outRec.FirstLeft = polyOut.FirstLeft;
                  polyOut.FirstLeft = outRec;
                  if (this.m_UsingPolyTree)
                    this.FixupFirstLefts2(polyOut, outRec);
                }
                else
                {
                  outRec.IsHole = polyOut.IsHole;
                  outRec.FirstLeft = polyOut.FirstLeft;
                  if (this.m_UsingPolyTree)
                    this.FixupFirstLefts1(polyOut, outRec);
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
        num += ((double) poly[index2].X + (double) poly[index1].X) * ((double) poly[index2].Y - (double) poly[index1].Y);
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
        num += (double) (outPt.Prev.Pt.X + outPt.Pt.X) * (double) (outPt.Prev.Pt.Y - outPt.Pt.Y);
        outPt = outPt.Next;
      }
      while (outPt != outRec.Pts);
      return num * 0.5;
    }

    public static List<List<IntPoint>> SimplifyPolygon(List<IntPoint> poly, PolyFillType fillType = PolyFillType.pftEvenOdd)
    {
      List<List<IntPoint>> solution = new List<List<IntPoint>>();
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
      List<List<IntPoint>> solution = new List<List<IntPoint>>();
      Clipper clipper = new Clipper();
      clipper.StrictlySimple = true;
      clipper.AddPaths(polys, PolyType.ptSubject, true);
      clipper.Execute(ClipType.ctUnion, solution, fillType, fillType);
      return solution;
    }

    private static double DistanceSqrd(IntPoint pt1, IntPoint pt2)
    {
      double num1 = (double) pt1.X - (double) pt2.X;
      double num2 = (double) pt1.Y - (double) pt2.Y;
      return num1 * num1 + num2 * num2;
    }

    private static double DistanceFromLineSqrd(IntPoint pt, IntPoint ln1, IntPoint ln2)
    {
      double num1 = (double) (ln1.Y - ln2.Y);
      double num2 = (double) (ln2.X - ln1.X);
      double num3 = num1 * (double) ln1.X + num2 * (double) ln1.Y;
      double num4 = num1 * (double) pt.X + num2 * (double) pt.Y - num3;
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
          return Clipper.DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
        return pt2.X > pt1.X == pt2.X < pt3.X ? Clipper.DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd : Clipper.DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
      }
      if (pt1.Y > pt2.Y == pt1.Y < pt3.Y)
        return Clipper.DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
      return pt2.Y > pt1.Y == pt2.Y < pt3.Y ? Clipper.DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd : Clipper.DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
    }

    private static bool PointsAreClose(IntPoint pt1, IntPoint pt2, double distSqrd)
    {
      double num1 = (double) pt1.X - (double) pt2.X;
      double num2 = (double) pt1.Y - (double) pt2.Y;
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
        return new List<IntPoint>();
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
        if (Clipper.PointsAreClose(op.Pt, op.Prev.Pt, distSqrd))
        {
          op = Clipper.ExcludeOp(op);
          --capacity;
        }
        else if (Clipper.PointsAreClose(op.Prev.Pt, op.Next.Pt, distSqrd))
        {
          Clipper.ExcludeOp(op.Next);
          op = Clipper.ExcludeOp(op);
          capacity -= 2;
        }
        else if (Clipper.SlopesNearCollinear(op.Prev.Pt, op.Pt, op.Next.Pt, distSqrd))
        {
          op = Clipper.ExcludeOp(op);
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
        intPointListList.Add(Clipper.CleanPolygon(polys[index], distance));
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
          List<IntPoint> poly = new List<IntPoint>(4);
          poly.Add(intPointListList1[index1 % count2][index2 % count1]);
          poly.Add(intPointListList1[(index1 + 1) % count2][index2 % count1]);
          poly.Add(intPointListList1[(index1 + 1) % count2][(index2 + 1) % count1]);
          poly.Add(intPointListList1[index1 % count2][(index2 + 1) % count1]);
          if (!Clipper.Orientation(poly))
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
      List<List<IntPoint>> intPointListList = Clipper.Minkowski(pattern, path, true, pathIsClosed);
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
      List<List<IntPoint>> solution = new List<List<IntPoint>>();
      Clipper clipper = new Clipper();
      for (int index = 0; index < paths.Count; ++index)
      {
        List<List<IntPoint>> ppg = Clipper.Minkowski(pattern, paths[index], true, pathIsClosed);
        clipper.AddPaths(ppg, PolyType.ptSubject, true);
        if (pathIsClosed)
        {
          List<IntPoint> pg = Clipper.TranslatePath(paths[index], pattern[0]);
          clipper.AddPath(pg, PolyType.ptClip, true);
        }
      }
      clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
      return solution;
    }

    public static List<List<IntPoint>> MinkowskiDiff(List<IntPoint> poly1, List<IntPoint> poly2)
    {
      List<List<IntPoint>> intPointListList = Clipper.Minkowski(poly1, poly2, false, true);
      Clipper clipper = new Clipper();
      clipper.AddPaths(intPointListList, PolyType.ptSubject, true);
      clipper.Execute(ClipType.ctUnion, intPointListList, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
      return intPointListList;
    }

    public static List<List<IntPoint>> PolyTreeToPaths(PolyTree polytree)
    {
      List<List<IntPoint>> paths = new List<List<IntPoint>>();
      paths.Capacity = polytree.Total;
      Clipper.AddPolyNodeToPaths((PolyNode) polytree, Clipper.NodeType.ntAny, paths);
      return paths;
    }

    internal static void AddPolyNodeToPaths(
      PolyNode polynode,
      Clipper.NodeType nt,
      List<List<IntPoint>> paths)
    {
      bool flag = true;
      switch (nt)
      {
        case Clipper.NodeType.ntOpen:
          return;
        case Clipper.NodeType.ntClosed:
          flag = !polynode.IsOpen;
          break;
      }
      if (polynode.m_polygon.Count > 0 & flag)
        paths.Add(polynode.m_polygon);
      foreach (PolyNode child in polynode.Childs)
        Clipper.AddPolyNodeToPaths(child, nt, paths);
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
      Clipper.AddPolyNodeToPaths((PolyNode) polytree, Clipper.NodeType.ntClosed, paths);
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
