using System;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public abstract class IntView : FloatView {
	[SerializeField] private int intValue;

	public int IntValue {
		get => intValue;
		set {
			if (intValue == value)
				return;
			intValue = value;
			ApplyIntValue();
		}
	}

	public override float FloatValue {
		get => IntValue;
		set => IntValue = Convert.ToInt32(value);
	}

	protected virtual void OnValidate() {
		ApplyIntValue();
	}

	protected abstract void ApplyIntValue();
}