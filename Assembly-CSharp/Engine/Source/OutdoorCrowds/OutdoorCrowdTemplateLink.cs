using System.Collections.Generic;
using Engine.Common.Components.Movable;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.OutdoorCrowds;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class OutdoorCrowdTemplateLink {
	[DataReadProxy] [DataWriteProxy] [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)] [CopyableProxy]
	public string Link;

	[DataReadProxy] [DataWriteProxy] [Inspected(Mode = ExecuteMode.EditAndRuntime)] [CopyableProxy()]
	public List<AreaEnum> Areas = new();
}