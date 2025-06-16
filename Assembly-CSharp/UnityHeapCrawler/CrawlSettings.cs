using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityHeapCrawler;

public class CrawlSettings {
	[NotNull] private static readonly Type[] hierarchyTypes = new Type[6] {
		typeof(GameObject),
		typeof(Component),
		typeof(Material),
		typeof(Texture),
		typeof(Sprite),
		typeof(Mesh)
	};

	public bool Enabled = true;
	internal readonly CrawlOrder Order;
	[NotNull] internal readonly Action RootsCollector;
	internal readonly string Caption;
	[NotNull] public string Filename;
	public bool PrintChildren = true;
	public bool PrintOnlyGameObjects;
	public bool IncludeAllUnityTypes;
	[NotNull] public List<Type> IncludedUnityTypes = new();
	public int MaxDepth;
	public int MaxChildren = 10;
	public int MinItemSize = 1024;

	[NotNull] public static IComparer<CrawlSettings> PriorityComparer { get; } = new PriorityRelationalComparer();

	public CrawlSettings([NotNull] string filename, [NotNull] string caption, [NotNull] Action rootsCollector,
		CrawlOrder order) {
		Filename = filename;
		Caption = caption;
		RootsCollector = rootsCollector;
		Order = order;
	}

	internal bool IsUnityTypeAllowed(Type type) {
		if (IncludeAllUnityTypes)
			return true;
		foreach (var includedUnityType in IncludedUnityTypes)
			if (includedUnityType.IsAssignableFrom(type))
				return true;
		return false;
	}

	[NotNull]
	public static CrawlSettings CreateUserRoots([NotNull] Action objectsProvider) {
		return new CrawlSettings("user-roots", "User Roots", objectsProvider, CrawlOrder.UserRoots) {
			MaxChildren = 0
		};
	}

	[NotNull]
	public static CrawlSettings CreateStaticFields([NotNull] Action objectsProvider) {
		return new CrawlSettings("static-fields", "Static Roots", objectsProvider, CrawlOrder.StaticFields) {
			MaxDepth = 1
		};
	}

	[NotNull]
	public static CrawlSettings CreateHierarchy([NotNull] Action objectsProvider) {
		return new CrawlSettings("hierarchy", "Hierarchy", objectsProvider, CrawlOrder.Hierarchy) {
			PrintOnlyGameObjects = true,
			MaxChildren = 0,
			IncludedUnityTypes = new List<Type>(hierarchyTypes)
		};
	}

	[NotNull]
	public static CrawlSettings CreateScriptableObjects([NotNull] Action objectsProvider) {
		return new CrawlSettings("scriptable_objects", "Scriptable Objects", objectsProvider, CrawlOrder.UnityObjects) {
			IncludeAllUnityTypes = true
		};
	}

	[NotNull]
	public static CrawlSettings CreatePrefabs([NotNull] Action objectsProvider) {
		return new CrawlSettings("prefabs", "Prefabs", objectsProvider, CrawlOrder.Prefabs) {
			PrintOnlyGameObjects = true,
			MaxChildren = 0,
			IncludedUnityTypes = new List<Type>(hierarchyTypes)
		};
	}

	[NotNull]
	public static CrawlSettings CreateUnityObjects([NotNull] Action objectsProvider) {
		return new CrawlSettings("unity_objects", "Unity Objects", objectsProvider, CrawlOrder.UnityObjects);
	}

	public override string ToString() {
		return "Crawl Settings [" + Caption + ", " + Filename + "]";
	}

	private sealed class PriorityRelationalComparer : IComparer<CrawlSettings> {
		public int Compare(CrawlSettings x, CrawlSettings y) {
			if (x == y)
				return 0;
			if (y == null)
				return 1;
			return x == null ? -1 : x.Order.CompareTo(y.Order);
		}
	}
}