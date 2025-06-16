using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StateSetters;

[Serializable]
public class StateSetterItem {
	public string Type;
	public string StringValue1;
	public float FloatValue1;
	public float FloatValue2;
	public int IntValue1;
	public int IntValue2;
	public bool BoolValue1;
	public Object ObjectValue1;
	public Color ColorValue1;
	public Color ColorValue2;

	public void Apply(float value) {
		StateSetterService.Apply(this, value);
	}
}