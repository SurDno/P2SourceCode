using System.Collections.Generic;

namespace Engine.Source.Commons.Parameters;

public class PriorityItemComprer<T> : IComparer<PriorityItem<T>> {
	public int Compare(PriorityItem<T> x, PriorityItem<T> y) {
		return x.Priority.CompareTo(y.Priority);
	}
}