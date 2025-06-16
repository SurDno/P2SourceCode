using System;
using UnityEngine;

namespace NodeCanvas.Framework;

[Serializable]
public class BBParameter<T> : BBParameter {
	private Func<T> getter;
	private Action<T> setter;
	[SerializeField] protected T _value;

	public BBParameter() { }

	public BBParameter(T value) {
		_value = value;
	}

	public T value {
		get {
			if (getter != null)
				return getter();
			if (!Application.isPlaying || !(bb != null) || string.IsNullOrEmpty(name))
				return _value;
			varRef = bb.GetVariable(name, typeof(T));
			return getter != null ? getter() : default;
		}
		set {
			if (this.setter != null)
				setter(value);
			else {
				if (isNone)
					return;
				if (bb != null && !string.IsNullOrEmpty(name)) {
					varRef = PromoteToVariable(bb);
					var setter = this.setter;
					if (setter == null)
						return;
					setter(value);
				} else
					_value = value;
			}
		}
	}

	protected override object objectValue {
		get => value;
		set => this.value = (T)value;
	}

	public override Type varType => typeof(T);

	protected override void Bind(Variable variable) {
		if (variable == null) {
			getter = null;
			setter = null;
			_value = default;
		} else {
			BindGetter(variable);
			BindSetter(variable);
		}
	}

	private bool BindGetter(Variable variable) {
		if (variable is Variable<T>) {
			getter = (variable as Variable<T>).GetValue;
			return true;
		}

		if (!variable.CanConvertTo(varType))
			return false;
		var func = variable.GetGetConverter(varType);
		getter = (Func<T>)(() => (T)func());
		return true;
	}

	private bool BindSetter(Variable variable) {
		if (variable is Variable<T>) {
			setter = (variable as Variable<T>).SetValue;
			return true;
		}

		if (!variable.CanConvertFrom(varType))
			return false;
		var func = variable.GetSetConverter(varType);
		setter = value => func(value);
		return true;
	}

	public static implicit operator BBParameter<T>(T value) {
		return new BBParameter<T> { value = value };
	}
}