using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapItemView :
	MonoBehaviour,
	IPointerEnterHandler,
	IEventSystemHandler,
	IPointerExitHandler {
	private MapWindow mapView;
	private IMapItem item;
	private Sprite activeSprite;
	private Color baseColor;
	private Color activeColor;
	private Color nearBaseColor;
	private Color nearActiveColor;
	private bool active;
	private bool isRegion;
	private bool highlighted;
	private Image hoverImage;
	private float zoom;

	public IMapItem Item => item;

	public bool IsRegion => isRegion;

	public Vector2 WorldPosition => transform.TransformPoint(Vector3.zero);

	public void SetHightlight(bool value) {
		if (highlighted == value)
			return;
		highlighted = value;
		if (highlighted)
			CreateHoverImage();
		else if (hoverImage != null) {
			Destroy(hoverImage.gameObject);
			hoverImage = null;
		}

		UpdateColors();
	}

	private void HoverEnd() {
		if (!active)
			return;
		mapView.HideInfo(this);
		active = false;
	}

	private void HoverStart() {
		if (active)
			return;
		mapView.ShowInfo(this);
		active = true;
	}

	public void Initialize(
		MapWindow mapView,
		IMapItem item,
		Sprite activeSprite,
		Color baseColor,
		Color activeColor,
		Color nearBaseColor,
		Color nearActiveColor) {
		active = false;
		isRegion = item.Resource.Kind == MapPlaceholderKind.Region;
		zoom = 0.0f;
		this.mapView = mapView;
		this.item = item;
		this.activeSprite = activeSprite;
		this.baseColor = baseColor;
		this.activeColor = activeColor;
		this.nearBaseColor = nearBaseColor;
		this.nearActiveColor = nearActiveColor;
	}

	public void Initialize(MapWindow mapView, IMapItem item, Color baseColor, Color activeColor) {
		Initialize(mapView, item, null, baseColor, activeColor, baseColor, activeColor);
	}

	public void OnZoomChange(float value) {
		zoom = value;
		UpdateColors();
	}

	public void UpdateColors() {
		if (isRegion) {
			GetComponent<Image>().color = Color.Lerp(baseColor, nearBaseColor, zoom);
			if (!(hoverImage != null))
				return;
			hoverImage.color = Color.Lerp(activeColor, nearActiveColor, zoom);
		} else
			GetComponent<Image>().color = highlighted
				? Color.Lerp(activeColor, nearActiveColor, zoom)
				: Color.Lerp(baseColor, nearBaseColor, zoom);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		HoverStart();
	}

	public void OnPointerExit(PointerEventData eventData) {
		HoverEnd();
	}

	private void OnDisable() {
		HoverEnd();
	}

	private void CreateHoverImage() {
		if (activeSprite == null)
			return;
		var gameObject = new GameObject("Hover", typeof(RectTransform), typeof(Image));
		var transform = (RectTransform)gameObject.transform;
		transform.SetParent(this.transform, false);
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		transform.anchorMin = Vector2.zero;
		transform.anchorMax = Vector2.one;
		transform.offsetMin = Vector2.zero;
		transform.offsetMax = Vector2.zero;
		hoverImage = gameObject.GetComponent<Image>();
		hoverImage.raycastTarget = false;
		hoverImage.sprite = activeSprite;
		hoverImage.color = activeColor;
	}
}