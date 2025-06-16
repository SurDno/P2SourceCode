using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Settings;
using UnityEngine;

public class AdditionalReflectionsEnabled : MonoBehaviour {
	[SerializeField] private HideableView view;

	private void UpdateValue() {
		if (!(view != null))
			return;
		view.Visible = InstanceByRequest<GraphicsGameSettings>.Instance.AdditionalReflections.Value;
	}

	private void OnEnable() {
		UpdateValue();
		InstanceByRequest<GraphicsGameSettings>.Instance.OnApply += UpdateValue;
	}

	private void OnDisable() {
		InstanceByRequest<GraphicsGameSettings>.Instance.OnApply -= UpdateValue;
	}
}