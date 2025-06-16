namespace ClipperLib
{
  internal class TEdge
  {
    internal IntPoint Bot;
    internal IntPoint Curr;
    internal IntPoint Delta;
    internal double Dx;
    internal TEdge Next;
    internal TEdge NextInAEL;
    internal TEdge NextInLML;
    internal TEdge NextInSEL;
    internal int OutIdx;
    internal PolyType PolyTyp;
    internal TEdge Prev;
    internal TEdge PrevInAEL;
    internal TEdge PrevInSEL;
    internal EdgeSide Side;
    internal IntPoint Top;
    internal int WindCnt;
    internal int WindCnt2;
    internal int WindDelta;
  }
}
