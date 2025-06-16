namespace ClipperLib;

internal class OutRec {
	internal OutPt BottomPt;
	internal OutRec FirstLeft;
	internal int Idx;
	internal bool IsHole;
	internal bool IsOpen;
	internal PolyNode PolyNode;
	internal OutPt Pts;
}