using Engine.Common.Generator;
using UnityEngine;

namespace BehaviorDesigner.Runtime;

public abstract class SharedVariable<T> : SharedVariable {
	[DataReadProxy(Name = "Value")] [DataWriteProxy(Name = "Value")] [CopyableProxy()] [SerializeField]
	protected T mValue;

	public T Value {
		get => mValue;
		set => mValue = value;
	}

	public override object GetValue() {
		return Value;
	}

	public override void SetValue(object value) {
		mValue = (T)value;
	}

	public override string ToString() {
		return Value == null ? "(null)" : Value.ToString();
	}
}