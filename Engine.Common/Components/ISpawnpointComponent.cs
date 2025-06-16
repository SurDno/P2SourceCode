using Engine.Common.Components.Milestone;

namespace Engine.Common.Components;

public interface ISpawnpointComponent : IComponent {
	Kind Type { get; }
}