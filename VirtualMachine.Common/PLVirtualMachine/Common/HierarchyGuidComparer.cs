using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public class HierarchyGuidComparer : IEqualityComparer<HierarchyGuid> {
	public static readonly HierarchyGuidComparer Instance = new();

	public bool Equals(HierarchyGuid x, HierarchyGuid y) {
		return x == y;
	}

	public int GetHashCode(HierarchyGuid obj) {
		return obj.GetHashCode();
	}
}