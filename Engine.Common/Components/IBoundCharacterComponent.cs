using Engine.Common.BoundCharacters;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Types;

namespace Engine.Common.Components;

public interface IBoundCharacterComponent : IComponent {
	IParameterValue<BoundHealthStateEnum> BoundHealthState { get; }

	bool Discovered { get; set; }

	BoundCharacterGroup Group { get; set; }

	IEntity HomeRegion { get; set; }

	bool IsEnabled { get; set; }

	LocalizedText Name { get; set; }

	IParameterValue<float> RandomRoll { get; }

	IBoundCharacterPlaceholder Resource { get; set; }

	int SortOrder { get; set; }

	void StorePreRollState();
}