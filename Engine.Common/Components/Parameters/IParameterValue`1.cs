using System;

namespace Engine.Common.Components.Parameters;

public interface IParameterValue<T> where T : struct {
	T Value { get; set; }

	T MinValue { get; set; }

	T MaxValue { get; set; }

	event Action<T> ChangeValueEvent;
}