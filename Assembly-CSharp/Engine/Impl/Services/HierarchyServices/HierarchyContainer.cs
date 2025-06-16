using System;
using System.Collections.Generic;
using Engine.Assets.Internal.Reference;
using Engine.Common;
using Engine.Common.Comparers;
using Inspectors;

namespace Engine.Impl.Services.HierarchyServices;

public class HierarchyContainer {
	private List<HierarchyItem> items = new();
	private Dictionary<Guid, HierarchyItem> hierarchyItems = new(GuidComparer.Instance);

	[Inspected] public Guid Id { get; private set; }

	[Inspected] public IEnumerable<HierarchyItem> Items => items;

	public HierarchyItem GetItemByTemplateId(Guid templateId) {
		HierarchyItem itemByTemplateId;
		hierarchyItems.TryGetValue(templateId, out itemByTemplateId);
		return itemByTemplateId;
	}

	public HierarchyContainer(
		IScene scene,
		Dictionary<Guid, HierarchyContainer> containers,
		Dictionary<IEntity, HierarchyItem> templates) {
		containers.Add(scene.Id, this);
		Id = scene.Id;
		foreach (var reference in ((SceneObject)scene).Items)
			items.Add(new HierarchyItem(reference, containers, templates, hierarchyItems));
	}
}