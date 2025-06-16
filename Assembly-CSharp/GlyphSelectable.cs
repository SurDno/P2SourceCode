using InputServices;
using UnityEngine;

public class GlyphSelectable : MonoBehaviour {
	[SerializeField] private GameObject _selectedFrame;
	private RectTransform objectRect;
	private RectTransform imageRect;

	public bool IsSelected { get; private set; }

	public void SetSelected(bool selected) {
		_selectedFrame.SetActive(selected);
		if (selected) {
			_selectedFrame.transform.position = transform.position;
			imageRect.sizeDelta = objectRect.sizeDelta.x > 100.0 ? new Vector2(120f, 120f) : new Vector2(110f, 100f);
		}

		IsSelected = selected;
	}

	private void OnEnable() {
		InputService.Instance.onJoystickUsedChanged += OnJoystick;
		objectRect = GetComponent<RectTransform>();
		imageRect = GetComponent<RectTransform>();
		SetSelected(false);
	}

	private void OnDisable() {
		InputService.Instance.onJoystickUsedChanged -= OnJoystick;
	}

	private void OnJoystick(bool joystick) {
		if (joystick) {
			if (!IsSelected)
				return;
			SetSelected(true);
		} else
			SetSelected(false);
	}
}