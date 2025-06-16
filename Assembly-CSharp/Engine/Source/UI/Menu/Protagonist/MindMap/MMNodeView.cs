using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  public class MMNodeView : 
    MonoBehaviour,
    IPointerEnterHandler,
    IEventSystemHandler,
    IPointerExitHandler,
    IPointerClickHandler
  {
    private RawImage nodeImage;
    private string baseColor;
    private string highlightedColor = "#bcac88";
    private RectTransform rectTransform;

    public MMNode Node { get; set; }

    public bool HasMapItem { get; set; }

    public GameObject NewIndicator { get; set; }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
      SetActive(true);
      GetComponentInParent<MMPageView>().ShowNodeInfo(this);
    }

    public void MarkNodeDiscovered()
    {
      if (Node.Undiscovered)
        Node.Undiscovered = false;
      if (!(NewIndicator != null))
        return;
      Destroy(NewIndicator);
      NewIndicator = null;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      SetActive(false);
      GetComponentInParent<MMPageView>().HideNodeInfo(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      MarkNodeDiscovered();
      if (!HasMapItem)
        return;
      GetComponentInParent<MMPageView>().CallMap(this);
    }
  }
}
