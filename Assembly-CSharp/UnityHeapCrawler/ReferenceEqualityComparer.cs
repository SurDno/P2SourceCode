using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace UnityHeapCrawler;

public class ReferenceEqualityComparer : IEqualityComparer<object> {
	[NotNull] public static ReferenceEqualityComparer Instance = new();

	public bool Equals(object x, object y) {
		return x == y;
	}

	public int GetHashCode(object o) {
		return RuntimeHelpers.GetHashCode(o);
	}
}