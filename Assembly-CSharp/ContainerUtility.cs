using System;
using System.Collections.Generic;
using System.Linq;

public static class ContainerUtility {
	private static Random rng = new(DateTime.UtcNow.Millisecond);

	public static void Cleanup<T>(this List<T> list) where T : class {
		var index = 0;
		while (index < list.Count)
			if (list[index] == null) {
				list[index] = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
			} else
				++index;
	}

	public static T FirstOrDefaultNoAlloc<T>(this List<T> source, Func<T, bool> compute) {
		for (var index = 0; index < source.Count; ++index) {
			var obj = source[index];
			if (obj != null && compute(obj))
				return obj;
		}

		return default;
	}

	public static void Shuffle<T>(this IList<T> list) {
		var count = list.Count;
		while (count > 1) {
			--count;
			var index = rng.Next(count + 1);
			var obj = list[index];
			list[index] = list[count];
			list[count] = obj;
		}
	}

	public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
		this IEnumerable<TSource> source,
		Func<TSource, TKey> keySelector) {
		var knownKeys = new HashSet<TKey>();
		foreach (var source1 in source) {
			var element = source1;
			if (knownKeys.Add(keySelector(element)))
				yield return element;
			element = default;
		}
	}

	public static T Random<T>(this List<T> source) {
		var count = source.Count;
		switch (count) {
			case 0:
				return default;
			case 1:
				return source[0];
			default:
				var index = UnityEngine.Random.Range(0, count);
				return source[index];
		}
	}

	public static T Random<T>(this IEnumerable<T> source) {
		return source.ToList().Random();
	}

	public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate) {
		if (items == null)
			throw new ArgumentNullException(nameof(items));
		if (predicate == null)
			throw new ArgumentNullException(nameof(predicate));
		var index = 0;
		foreach (var obj in items) {
			if (predicate(obj))
				return index;
			++index;
		}

		return -1;
	}

	public static int IndexOf<T>(this IEnumerable<T> items, T item) {
		return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
	}

	public static TValue GetOrCreateValue<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key)
		where TValue : new() {
		TValue obj;
		if (!map.TryGetValue(key, out obj)) {
			obj = new TValue();
			map.Add(key, obj);
		}

		return obj;
	}

	public static void AddRange<T>(this HashSet<T> target, IEnumerable<T> source) {
		foreach (var obj in source)
			target.Add(obj);
	}
}