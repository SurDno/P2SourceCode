using Engine.Common;
using Engine.Common.Generator;
using Engine.Source.Connections;
using Inspectors;

namespace Engine.Source.Components.Crowds;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class HerbRootsTemplate {
	[DataReadProxy]
	[DataWriteProxy]
	[Inspected(Mutable = true, Header = true, Mode = ExecuteMode.EditAndRuntime)]
	[CopyableProxy]
	public Typed<IEntity> Template;

	[DataReadProxy]
	[DataWriteProxy]
	[Inspected(Mutable = true, Header = true, Mode = ExecuteMode.EditAndRuntime)]
	[CopyableProxy()]
	public float Weight = 1f;
}