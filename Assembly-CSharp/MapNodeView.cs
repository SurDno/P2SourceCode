using Engine.Common.MindMap;
using Engine.Source.UI.Menu.Protagonist.MindMap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeView : 
  MonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler,
  IPointerClickHandler
{
  private MMTooltip tooltip;
  private MapWindow mapView;
  private RawImage nodeImage;
  private string baseColor;
  private string highlightedColor = "#bcac88";
  private RectTransform rectTransform;

  private void OnEnable()
  {
    if (nodeImage == null)
      nodeImage = GetComponent<RawImage>();
    if (nodeImage != null)
      baseColor = nodeImage.color.ToRGBHex();
    rectTransform = GetComponent<RectTransform>();
  }

  public Rect GetSpriteRect()
  {
    return rectTransform != null ? rectTransform.rect : Rect.zero;
  }

  public void SetActive(bool active)
  {
    Color color;
    if (!UnityEngine.ColorUtility.TryParseHtmlString(active ? highlightedColor : baseColor, out color) || !(nodeImage != null))
      return;
    nodeImage.color = color;
  }

  public void SetMapView(MapWindow mapView) => this.mapView = mapView;

  public void SetNode(MMTooltip tooltip) => this.tooltip = tooltip;

  public IMMNode GetNode() => tooltip.node;

  public void OnPointerClick(PointerEventData eventData)
  {
    if (tooltip.node == null)
      return;
    mapView.CallMindMap(tooltip.node);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    SetActive(true);
    mapView.ShowMapNodeInfo(this, tooltip);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    SetActive(false);
    mapView.HideMapNodeInfo();
  }
}
