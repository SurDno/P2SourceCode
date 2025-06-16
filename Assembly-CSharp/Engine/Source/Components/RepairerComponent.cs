using System.Collections.Generic;
using System.Linq;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Components;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class RepairerComponent : EngineComponent {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected List<StorableGroup> repairableGroups = new();

	public IEnumerable<StorableGroup> RepairableGroups => repairableGroups;

	public bool CanRepairItem(IStorableComponent item) {
		foreach (var repairableGroup in repairableGroups)
			if (item.Groups.Contains(repairableGroup))
				return true;
		return false;
	}
}