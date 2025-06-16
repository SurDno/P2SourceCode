using System;
using Inspectors;

namespace Engine.Source.Services;

[GameService(typeof(QuestCompassService))]
public class QuestCompassService {
	private bool enabled;
	public Action<bool> OnEnableChanged;

	[Inspected(Mutable = true)]
	public bool IsEnabled {
		get => enabled;
		set {
			if (enabled == value)
				return;
			enabled = value;
			var onEnableChanged = OnEnableChanged;
			if (onEnableChanged == null)
				return;
			onEnableChanged(value);
		}
	}
}