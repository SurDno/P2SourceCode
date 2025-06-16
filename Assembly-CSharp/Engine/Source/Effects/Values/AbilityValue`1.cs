using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Values;

public class AbilityValue<T> : IAbilityValue<T>, IAbilityValue where T : struct {
	[DataReadProxy] [DataWriteProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)] [CopyableProxy()]
	protected T value;

	public T Value {
		get => value;
		set => this.value = value;
	}
}