using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class ConfirmationWindowEventView : EventView {
	[SerializeField] private ConfirmationWindow prefab;
	[SerializeField] private Transform layout;
	[SerializeField] private EventView acceptAction;
	[SerializeField] private EventView cancelAction;
	[SerializeField] private string text;
	private ConfirmationWindow window;

	public override void Invoke() {
		if (window == null)
			window = Instantiate(prefab, (bool)(Object)layout ? layout : transform, false);
		window.Show(text, OnAccept, OnCancel);
	}

	private void OnAccept() {
		acceptAction?.Invoke();
	}

	private void OnCancel() {
		cancelAction?.Invoke();
	}
}