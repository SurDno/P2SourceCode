namespace Engine.Common.Components.Parameters;

public interface IParameter<T> : IParameter where T : struct {
	T Value { get; set; }

	T BaseValue { get; set; }

	T MinValue { get; set; }

	T MaxValue { get; set; }
}