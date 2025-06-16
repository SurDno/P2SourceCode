using Inspectors;
using UnityEngine;

namespace Engine.Source.Settings;

public class IntValue : IValue<int> {
	[Inspected] private string name;
	[Inspected] private int defaultValue;
	[Inspected] private int minValue;
	[Inspected] private int maxValue;
	private int value;

	public IntValue(string name, int defaultValue = 0, int minValue = -2147483648, int maxValue = 2147483647) {
		this.name = name;
		this.defaultValue = defaultValue;
		this.minValue = minValue;
		this.maxValue = maxValue;
		value = PlayerSettings.Instance.GetInt(name, defaultValue);
		value = Mathf.Clamp(value, minValue, maxValue);
	}

	[Inspected(Mutable = true)]
	public int Value {
		get => value;
		set {
			if (this.value == value)
				return;
			this.value = value;
			this.value = Mathf.Clamp(this.value, minValue, maxValue);
			PlayerSettings.Instance.SetInt(name, this.value);
			PlayerSettings.Instance.Save();
		}
	}

	public int DefaultValue => defaultValue;

	public int MinValue => minValue;

	public int MaxValue => maxValue;
}