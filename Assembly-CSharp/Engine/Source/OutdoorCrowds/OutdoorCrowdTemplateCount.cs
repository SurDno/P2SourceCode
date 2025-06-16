using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.OutdoorCrowds;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class OutdoorCrowdTemplateCount {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
	public int Min;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
	public int Max;
}