using Engine.Common;
using Engine.Source.Commons.Abilities;

namespace Engine.Source.Commons.Effects;

public interface IEffect {
	string Name { get; }

	AbilityItem AbilityItem { get; set; }

	IEntity Target { get; set; }

	ParameterEffectQueueEnum Queue { get; }

	bool Prepare(float currentRealTime, float currentGameTime);

	bool Compute(float currentRealTime, float currentGameTime);

	void Cleanup();
}