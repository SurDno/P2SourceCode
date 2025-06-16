using UnityEngine;

namespace Engine.Impl.UI;

[RequireComponent(typeof(RectTransform))]
public class UIControl : MonoBehaviour {
	private RectTransform transform;

	public RectTransform Transform {
		get {
			if (transform == null)
				transform = gameObject.GetComponent<RectTransform>();
			return transform;
		}
	}

	public Vector2 PivotedPosition {
		get => (Vector2)Transform.position -
		       Vector2.Scale(Transform.pivot, Vector2.Scale(Transform.sizeDelta, Transform.lossyScale));
		set => Transform.position =
			value + Vector2.Scale(Transform.pivot, Vector2.Scale(Transform.sizeDelta, Transform.lossyScale));
	}

	public bool IsEnabled {
		get => gameObject.activeSelf;
		set => gameObject.SetActive(value);
	}

	protected virtual void Awake() { }
}