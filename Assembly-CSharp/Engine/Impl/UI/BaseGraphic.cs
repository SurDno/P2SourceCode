using System;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI;

[RequireComponent(typeof(RectTransform))]
[Serializable]
public class BaseGraphic : Graphic, IDisposable {
	protected GameObject gameObject2;
	protected bool isDisposed;
	protected RectTransform transform2;

	public virtual string Name {
		get => gameObject2.name;
		set => gameObject2.name = value;
	}

	public RectTransform Transform {
		get => transform2;
		set => transform2 = value;
	}

	public BaseGraphic Parent {
		get => transform2.parent.GetComponent<BaseGraphic>();
		set {
			if (value == null)
				transform2.SetParent(null);
			else
				transform2.SetParent(value.transform2, false);
		}
	}

	public Vector2 Pivot {
		get => transform2.pivot;
		set => transform2.pivot = value;
	}

	public Vector2 Anchor {
		get => transform2.anchorMax;
		set {
			transform2.anchorMax = value;
			transform2.anchorMin = value;
		}
	}

	public Quaternion Rotation {
		get => transform2.rotation;
		set => transform2.rotation = value;
	}

	public Vector2 Position {
		get => transform2.position;
		set => transform2.position = value;
	}

	public Vector2 LocalPosition {
		get => transform2.localPosition;
		set => transform2.localPosition = value;
	}

	public Rect Rectangle => transform2.rect;

	public bool IsEnabled {
		get => gameObject2.activeSelf;
		set {
			gameObject2.SetActive(value);
			if (value) {
				var enableEvent = Enable_Event;
				if (enableEvent == null)
					return;
				enableEvent(this);
			} else {
				var disableEvent = Disable_Event;
				if (disableEvent != null)
					disableEvent(this);
			}
		}
	}

	public void Dispose() {
		if (isDisposed || this == null)
			return;
		isDisposed = true;
		Destroy(gameObject);
	}

	[field: NonSerialized] public event Action<BaseGraphic> Enable_Event;

	[field: NonSerialized] public event Action<BaseGraphic> Disable_Event;

	protected override void Awake() {
		gameObject2 = gameObject;
		transform2 = gameObject.GetComponent<RectTransform>();
	}
}