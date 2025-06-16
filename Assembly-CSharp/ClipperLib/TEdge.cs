// Decompiled with JetBrains decompiler
// Type: ClipperLib.TEdge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
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
