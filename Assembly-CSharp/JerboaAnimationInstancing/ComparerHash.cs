using System.Collections.Generic;

namespace JerboaAnimationInstancing;

public class ComparerHash : IComparer<AnimationInfo> {
	public int Compare(AnimationInfo x, AnimationInfo y) {
		return x.animationNameHash.CompareTo(y.animationNameHash);
	}
}