using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Components;

[Factory(typeof(ITagsComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class TagsComponent : EngineComponent, ITagsComponent, IComponent {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected List<EntityTagEnum> tags = new();

	public IEnumerable<EntityTagEnum> Tags => tags;
}