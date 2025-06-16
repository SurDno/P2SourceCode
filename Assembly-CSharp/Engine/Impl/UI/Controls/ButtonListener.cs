using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls;

[RequireComponent(typeof(Button))]
public class ButtonListener : MonoBehaviour {
	[SerializeField] private EventView view;

	private void Awake() {
		GetComponent<Button>().onClick.AddListener(OnClick);
	}

	private void OnClick() {
		view?.Invoke();
	}
}