using Engine.Impl.UI.Controls;
using UnityEngine;

namespace Engine.Source.UI.Controls.BoolViews;

public static class HideableViewUtility {
	public static void SetVisible(GameObject gameObject, bool value) {
		if (gameObject == null)
			return;
		var component = gameObject.GetComponent<HideableView>();
		if (component != null)
			component.Visible = value;
		else
			gameObject.SetActive(value);
	}
}