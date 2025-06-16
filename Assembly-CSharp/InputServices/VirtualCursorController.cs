using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

namespace InputServices;

public class VirtualCursorController : ICursorController {
	private GameObject cursor;
	private RectTransform cursorTransform;
	private bool visible;

	[Inspected(Mutable = true)]
	public bool Visible {
		get => visible;
		set {
			visible = value;
			UpdateCursor();
		}
	}

	[Inspected(Mutable = true)] public bool Free { get; set; }

	[Inspected(Mutable = true)] public Vector2 Position { get; private set; }

	public VirtualCursorController() {
		InstanceByRequest<EngineApplication>.Instance.OnInitialized += OnInitialized;
	}

	private void OnInitialized() {
		Position = new Vector2(Screen.width / 2, Screen.height / 2);
		cursor = ServiceLocator.GetService<UIService>().VirtualCursor;
		if (!(cursor != null))
			return;
		cursorTransform = cursor.GetComponent<RectTransform>();
		UpdateCursor();
	}

	public void Move(float diffX, float diffY) {
		var num1 = Position.x + diffX;
		var num2 = Position.y - diffY;
		Position = new Vector2(Mathf.Clamp(num1, 0.0f, Screen.width), Mathf.Clamp(num2, 0.0f, Screen.height));
		UpdateCursor();
	}

	private void UpdateCursor() {
		if (cursor == null)
			return;
		if (cursor.activeSelf != Visible)
			cursor.SetActive(Visible && !InputService.Instance.JoystickUsed);
		cursorTransform.position = new Vector3(Position.x, Position.y, cursorTransform.position.z);
	}
}