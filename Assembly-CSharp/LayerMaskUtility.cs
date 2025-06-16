using UnityEngine;

public static class LayerMaskUtility {
	public static bool Contains(this LayerMask mask, int layer) {
		return (mask.value & (1 << layer)) != 0;
	}

	public static int GetMask(this LayerMask mask) {
		return 1 << mask;
	}

	public static int GetIndex(this LayerMask mask) {
		var index = 0;
		var num = mask.value;
		while (num > 1) {
			num >>= 1;
			++index;
		}

		return index;
	}
}