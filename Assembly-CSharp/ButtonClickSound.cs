using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour {
	private void Awake() {
		GetComponent<Button>().onClick.AddListener(PlaySound);
	}

	private void PlaySound() {
		MonoBehaviourInstance<UISounds>.Instance.PlayClickSound();
	}
}