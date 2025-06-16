namespace StateSetters;

public static class StateSetterItemUtility {
	public static void Apply(this StateSetterItem[] states, bool on) {
		if (states == null)
			return;
		foreach (var state in states)
			state.Apply(on ? 1f : 0.0f);
	}

	public static void Apply(this StateSetterItem[] states, float value) {
		if (states == null)
			return;
		foreach (var state in states)
			state.Apply(value);
	}
}