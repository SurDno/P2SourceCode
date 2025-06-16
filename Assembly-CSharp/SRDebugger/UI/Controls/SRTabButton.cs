using SRF;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Controls;

public class SRTabButton : SRMonoBehaviour {
	public Behaviour ActiveToggle;
	public Button Button;
	public Text TitleText;

	public bool IsActive {
		get => ActiveToggle.enabled;
		set => ActiveToggle.enabled = value;
	}
}