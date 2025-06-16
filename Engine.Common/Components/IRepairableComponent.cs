using Engine.Common.Components.Parameters;

namespace Engine.Common.Components;

public interface IRepairableComponent : IComponent {
	IParameterValue<float> Durability { get; }
}