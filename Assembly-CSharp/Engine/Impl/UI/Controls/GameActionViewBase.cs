using Engine.Source.Services.Inputs;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public abstract class GameActionViewBase : GameActionView {
	[SerializeField] private GameActionType value;

	public override GameActionType GetValue() {
		return value;
	}

	public override void SetValue(GameActionType value, bool instant) {
		if (this.value == value)
			return;
		this.value = value;
		ApplyValue(instant);
	}

	protected abstract void ApplyValue(bool instant);
}