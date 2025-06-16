using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapItemView : 
  MonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler
{
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

  public IMapItem Item => this.item;

  public bool IsRegion => this.isRegion;

  public Vector2 WorldPosition => (Vector2) this.transform.TransformPoint(Vector3.zero);

  public void SetHightlight(bool value)
  {
    if (this.highlighted == value)
      return;
    this.highlighted = value;
    if (this.highlighted)
      this.CreateHoverImage();
    else if ((UnityEngine.Object) this.hoverImage != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.hoverImage.gameObject);
      this.hoverImage = (Image) null;
    }
    this.UpdateColors();
  }

  private void HoverEnd()
  {
    if (!this.active)
      return;
    this.mapView.HideInfo(this);
    this.active = false;
  }

  private void HoverStart()
  {
    if (this.active)
      return;
    this.mapView.ShowInfo(this);
    this.active = true;
  }

  public void Initialize(
    MapWindow mapView,
    IMapItem item,
    Sprite activeSprite,
    Color baseColor,
    Color activeColor,
    Color nearBaseColor,
    Color nearActiveColor)
  {
    this.active = false;
    this.isRegion = item.Resource.Kind == MapPlaceholderKind.Region;
    this.zoom = 0.0f;
    this.mapView = mapView;
    this.item = item;
    this.activeSprite = activeSprite;
    this.baseColor = baseColor;
    this.activeColor = activeColor;
    this.nearBaseColor = nearBaseColor;
    this.nearActiveColor = nearActiveColor;
  }

  public void Initialize(MapWindow mapView, IMapItem item, Color baseColor, Color activeColor)
  {
    this.Initialize(mapView, item, (Sprite) null, baseColor, activeColor, baseColor, activeColor);
  }

  public void OnZoomChange(float value)
  {
    this.zoom = value;
    this.UpdateColors();
  }

  public void UpdateColors()
  {
    if (this.isRegion)
    {
      this.GetComponent<Image>().color = Color.Lerp(this.baseColor, this.nearBaseColor, this.zoom);
      if (!((UnityEngine.Object) this.hoverImage != (UnityEngine.Object) null))
        return;
      this.hoverImage.color = Color.Lerp(this.activeColor, this.nearActiveColor, this.zoom);
    }
    else
      this.GetComponent<Image>().color = this.highlighted ? Color.Lerp(this.activeColor, this.nearActiveColor, this.zoom) : Color.Lerp(this.baseColor, this.nearBaseColor, this.zoom);
  }

  public void OnPointerEnter(PointerEventData eventData) => this.HoverStart();

  public void OnPointerExit(PointerEventData eventData) => this.HoverEnd();

  private void OnDisable() => this.HoverEnd();

  private void CreateHoverImage()
  {
    if ((UnityEngine.Object) this.activeSprite == (UnityEngine.Object) null)
      return;
    GameObject gameObject = new GameObject("Hover", new System.Type[2]
    {
      typeof (RectTransform),
      typeof (Image)
    });
    RectTransform transform = (RectTransform) gameObject.transform;
    transform.SetParent(this.transform, false);
    transform.localRotation = Quaternion.identity;
    transform.localScale = Vector3.one;
    transform.anchorMin = Vector2.zero;
    transform.anchorMax = Vector2.one;
    transform.offsetMin = Vector2.zero;
    transform.offsetMax = Vector2.zero;
    this.hoverImage = gameObject.GetComponent<Image>();
    this.hoverImage.raycastTarget = false;
    this.hoverImage.sprite = this.activeSprite;
    this.hoverImage.color = this.activeColor;
  }
}
