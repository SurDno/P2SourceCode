using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Other;

[RequireComponent(typeof(ScrollRect))]
public class ScrollSettingsBehaviour : MonoBehaviour {
	public const float ScrollSensitivity = 40f;

	private void Awake() {
		var component = GetComponent<ScrollRect>();
		component.scrollSensitivity = 40f;
		component.movementType = ScrollRect.MovementType.Clamped;
		component.inertia = false;
	}
}