using Engine.Source.Settings;

namespace Engine.Source.Difficulties;

public class DifficultyValue : IValue<float> {
	public float Value { get; set; }

	public float DefaultValue { get; set; }

	public float MinValue { get; set; }

	public float MaxValue { get; set; }
}