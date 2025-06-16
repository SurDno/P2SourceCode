using System;
using System.Collections.Generic;

namespace Engine.Common.Comparers;

public class GuidComparer : IEqualityComparer<Guid> {
	public static readonly GuidComparer Instance = new();

	public bool Equals(Guid x, Guid y) {
		return x == y;
	}

	public int GetHashCode(Guid obj) {
		return obj.GetHashCode();
	}
}