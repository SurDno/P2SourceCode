using System;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Settings;

public abstract class SettingsInstanceByRequest<T> : InstanceByRequest<T> where T : class, new() {
	public SettingsInstanceByRequest() {
		SettingsViewService.AddSettings(this);
	}

	protected virtual void OnInvalidate() { }

	public event Action OnApply;

	[Inspected]
	public void Apply() {
		OnInvalidate();
		var onApply = OnApply;
		if (onApply == null)
			return;
		onApply();
	}
}