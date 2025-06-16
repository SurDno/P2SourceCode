using System;

namespace Engine.Common.Components;

public interface IDiseaseComponent : IComponent {
	void SetDiseaseValue(float value, TimeSpan deltaTime);

	float DiseaseValue { get; }
}