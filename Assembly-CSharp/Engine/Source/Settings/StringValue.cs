using Inspectors;

namespace Engine.Source.Settings;

public class StringValue : IValue<string> {
	[Inspected] private string name;
	[Inspected] private string defaultValue;
	private string value;

	public StringValue(string name, string defaultValue = "") {
		this.name = name;
		this.defaultValue = defaultValue;
		value = PlayerSettings.Instance.GetString(name, defaultValue);
	}

	[Inspected(Mutable = true)]
	public string Value {
		get => value;
		set {
			if (this.value == value)
				return;
			this.value = value;
			PlayerSettings.Instance.SetString(name, value);
			PlayerSettings.Instance.Save();
		}
	}

	public string DefaultValue => defaultValue;

	public string MinValue => defaultValue;

	public string MaxValue => defaultValue;
}