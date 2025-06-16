using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasRenderer))]
public class Splitter :
	Graphic,
	IBeginDragHandler,
	IEventSystemHandler,
	IDragHandler,
	IEndDragHandler,
	IPointerEnterHandler,
	IPointerExitHandler {
	public static Texture2D CursorNormal = null;
	public static Texture2D CursorHorizontalResize = null;

	[SerializeField] [FormerlySerializedAs("_Direction")]
	protected Direction direction = Direction.None;

	private bool isDragging;

	[SerializeField] [FormerlySerializedAs("Target")]
	private RectTransform target;

	public Direction Direction {
		get => direction;
		set => direction = value;
	}

	public void OnBeginDrag(PointerEventData EventData) {
		isDragging = true;
	}

	public void OnDrag(PointerEventData data) {
		switch (direction) {
			case Direction.LeftToRight:
				target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target.rect.width - data.delta.x);
				break;
			case Direction.RightToLeft:
				target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target.rect.width + data.delta.x);
				break;
			case Direction.TopToBottom:
				target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, target.rect.height - data.delta.y);
				break;
			case Direction.BottomToTop:
				target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, target.rect.height + data.delta.y);
				break;
		}
	}

	public void OnEndDrag(PointerEventData data) {
		isDragging = false;
	}

	public void OnPointerEnter(PointerEventData data) {
		if (isDragging)
			;
	}

	public void OnPointerExit(PointerEventData data) {
		if (isDragging)
			;
	}
}