using UnityEngine.UI;

namespace AmplifyBloom;

public sealed class DemoUIToggle : DemoUIElement {
	private Toggle m_toggle;

	private void Start() {
		m_toggle = GetComponent<Toggle>();
	}

	public override void DoAction(DemoUIElementAction action, params object[] vars) {
		if (!m_toggle.IsInteractable() || action != DemoUIElementAction.Press)
			return;
		m_toggle.isOn = !m_toggle.isOn;
	}
}