using System;

namespace Engine.Source.Settings;

public interface ISettingsInstance<T> {
	event Action OnApply;

	void Apply();
}