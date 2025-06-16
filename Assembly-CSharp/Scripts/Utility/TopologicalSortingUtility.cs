using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Utility;
using Engine.Source.Commons;

namespace Scripts.Utility;

public static class TopologicalSortingUtility {
	public static IEnumerable<T> TopologicalSort<T>(
		this IEnumerable<T> source,
		Func<T, IEnumerable<T>> dependencies,
		bool throwOnCycle) {
		var sorted = new List<T>();
		var visited = new HashSet<T>();
		foreach (var obj in source)
			Visit(obj, visited, sorted, dependencies, throwOnCycle);
		return sorted;
	}

	private static void Visit<T>(
		T item,
		HashSet<T> visited,
		List<T> sorted,
		Func<T, IEnumerable<T>> dependencies,
		bool throwOnCycle) {
		if (!visited.Contains(item)) {
			visited.Add(item);
			foreach (var obj in dependencies(item))
				Visit(obj, visited, sorted, dependencies, throwOnCycle);
			sorted.Add(item);
		} else if (throwOnCycle && !sorted.Contains(item))
			throw new Exception("Cyclic dependency found");
	}

	public static IEnumerable<T> GetDependencies<T, T2>(
		T item,
		List<T> services,
		Dictionary<T, List<Type>> cache)
		where T2 : BaseDependAttribute {
		List<Type> types;
		if (!cache.TryGetValue(item, out types)) {
			var depends = (T2[])item.GetType().GetCustomAttributes(typeof(T2), true);
			types = depends.Select(o => o.Type).ToList();
			types.Sort((a, b) => a.Name.CompareTo(b.Name));
			cache.Add(item, types);
			depends = null;
		}

		foreach (var service1 in services) {
			var service = service1;
			foreach (var type1 in types) {
				var type = type1;
				if (TypeUtility.IsAssignableFrom(type, service.GetType())) {
					yield return service;
					break;
				}

				type = null;
			}

			service = default;
		}
	}
}