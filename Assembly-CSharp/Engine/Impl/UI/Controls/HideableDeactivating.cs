using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class HideableDeactivating : HideableView {
	[SerializeField] private GameObject target;

	protected override void ApplyVisibility() {
		if (target != null)
			target.SetActive(Visible);
		else
			gameObject.SetActive(Visible);
	}
}