using System.Collections.Generic;
using Engine.Common.Commons.Cloneable;

namespace Engine.Source.Commons;

public static class ComponentCollectionUtility {
	public static void CopyListTo<T>(List<T> target, List<T> source) where T : class {
		target.Clear();
		target.Capacity = source.Count;
		foreach (var source1 in source) {
			var obj = CloneableObjectUtility.Clone(source1);
			target.Add(obj);
		}
	}
}