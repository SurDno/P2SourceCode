namespace Engine.Source.Effects.Values;

public interface IAbilityValue<T> : IAbilityValue where T : struct {
	T Value { get; }
}