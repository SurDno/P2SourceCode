namespace Engine.Common.Components.Parameters;

public interface IParameter {
	ParameterNameEnum Name { get; }

	bool Resetable { get; }

	object ValueData { get; }

	void AddListener(IChangeParameterListener listener);

	void RemoveListener(IChangeParameterListener listener);
}