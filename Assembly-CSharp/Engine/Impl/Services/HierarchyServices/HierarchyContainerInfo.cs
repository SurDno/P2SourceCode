using System.Collections.Generic;
using AssetDatabases;
using Inspectors;

namespace Engine.Impl.Services.HierarchyServices;

public struct HierarchyContainerInfo {
	private HierarchyContainer container;

	[Inspected(Header = true)] public string Name => AssetDatabaseUtility.GetFileName(Path);

	[Inspected] public string Path => AssetDatabaseService.Instance.GetPath(container.Id);

	[Inspected]
	public IEnumerable<HierarchyContainerInfo> Childs {
		get {
			foreach (var container in GetContainers(this.container.Items)) {
				var child = container;
				yield return child;
				child = new HierarchyContainerInfo();
			}
		}
	}

	public HierarchyContainerInfo(HierarchyContainer container) {
		this.container = container;
	}

	private static IEnumerable<HierarchyContainerInfo> GetContainers(
		IEnumerable<HierarchyItem> items) {
		foreach (var item in items) {
			if (item.Container != null)
				yield return new HierarchyContainerInfo(item.Container);
			foreach (var container in GetContainers(item.Items)) {
				var scene = container;
				yield return scene;
				scene = new HierarchyContainerInfo();
			}
		}
	}
}