using Engine.Source.Commons.Effects;

namespace Engine.Source.Effects.Engine;

public interface IEffectValueSetter {
	string ValueView { get; }

	string TypeView { get; }

	void Compute(IEffect context);
}