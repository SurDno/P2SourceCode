using System.Collections.Generic;

namespace ClipperLib;

public class MyIntersectNodeSort : IComparer<IntersectNode> {
	public int Compare(IntersectNode node1, IntersectNode node2) {
		var num = node2.Pt.Y - node1.Pt.Y;
		if (num > 0L)
			return 1;
		return num < 0L ? -1 : 0;
	}
}