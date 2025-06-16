namespace Engine.Common.Components.Parameters;

public interface IPriorityParameterValue<T> : IParameterValue<T> where T : struct {
	void SetValue(PriorityParameterEnum priority, T value);

	bool TryGetValue(PriorityParameterEnum priority, out T value);

	void ResetValue(PriorityParameterEnum priority);
}