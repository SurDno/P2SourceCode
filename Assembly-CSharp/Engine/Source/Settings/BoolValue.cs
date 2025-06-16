using Inspectors;

namespace Engine.Source.Settings;

public class BoolValue : IValue<bool> {
	[Inspected] private string name;
	[Inspected] private bool defaultValue;
	private bool value;

	public BoolValue(string name, bool defaultValue = false) {
		this.name = name;
		value = PlayerSettings.Instance.GetBool(name, defaultValue);
		this.defaultValue = defaultValue;
	}

	[Inspected(Mutable = true)]
	public bool Value {
		get => value;
		set {
			if (this.value == value)
				return;
			this.value = value;
			PlayerSettings.Instance.SetBool(name, value);
			PlayerSettings.Instance.Save();
		}
	}

	public bool DefaultValue => defaultValue;

	public bool MinValue => defaultValue;

	public bool MaxValue => defaultValue;
}