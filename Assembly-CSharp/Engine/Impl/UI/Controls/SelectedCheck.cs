using UnityEngine;
using UnityEngine.EventSystems;

namespace Engine.Impl.UI.Controls;

public class SelectedCheck : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler {
	[SerializeField] private HideableView hideableView;

	public void OnDeselect(BaseEventData eventData) {
		if (!(hideableView != null))
			return;
		hideableView.Visible = false;
	}

	public void OnSelect(BaseEventData eventData) {
		if (!(hideableView != null))
			return;
		hideableView.Visible = true;
	}
}