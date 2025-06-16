using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Components;

[Required(typeof(LocationItemComponent))]
[Factory(typeof(SpreadingComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class SpreadingComponent : EngineComponent {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected DiseasedStateEnum diseasedState;

	[FromLocator] private SpreadingService spreadingService;

	public DiseasedStateEnum DiseasedState => diseasedState;

	public override void OnAdded() {
		base.OnAdded();
		spreadingService.AddSpreading(this);
	}

	public override void OnRemoved() {
		spreadingService.RemoveSpreading(this);
		base.OnRemoved();
	}
}