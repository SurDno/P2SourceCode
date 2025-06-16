using System;
using UnityEngine;

namespace Engine.Source.UI.Menu.Main;

public class SmallLoading : MonoBehaviour {
	private static bool _showBackground;
	private static Action<bool> onBackground;
	[SerializeField] private GameObject background;

	public static bool showBackground {
		get => _showBackground;
		set {
			_showBackground = value;
			var onBackground = SmallLoading.onBackground;
			if (onBackground == null)
				return;
			onBackground(value);
		}
	}

	private void Awake() {
		onBackground += OnBackground;
		OnBackground(_showBackground);
	}

	private void OnDestroy() {
		onBackground -= OnBackground;
	}

	private void OnBackground(bool active) {
		background.SetActive(active);
	}
}