using System.Collections;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class HideableViewEvents : HideableView {
	[SerializeField] private EventView trueEvent;
	[SerializeField] private EventView falseEvent;
	private Coroutine coroutine;

	protected override void ApplyVisibility() {
		Cancel();
		if (!gameObject.activeInHierarchy)
			return;
		coroutine = StartCoroutine(InvokeEvent(Visible));
	}

	private void Cancel() {
		if (coroutine == null)
			return;
		StopCoroutine(coroutine);
	}

	private IEnumerator InvokeEvent(bool value) {
		yield return new WaitForEndOfFrame();
		if (value)
			trueEvent?.Invoke();
		else
			falseEvent?.Invoke();
	}

	public override void SkipAnimation() {
		Cancel();
	}
}