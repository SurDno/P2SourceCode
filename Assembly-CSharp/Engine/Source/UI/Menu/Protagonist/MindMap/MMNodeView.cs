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
      if ((Object) nodeImage == (Object) null)
        nodeImage = this.GetComponent<RawImage>();
      if ((Object) nodeImage != (Object) null)
        baseColor = nodeImage.color.ToRGBHex();
      rectTransform = this.GetComponent<RectTransform>();
    }

    public Rect GetSpriteRect()
    {
      return (Object) rectTransform != (Object) null ? rectTransform.rect : Rect.zero;
    }

    public void SetActive(bool active)
    {
      Color color;
      if (!UnityEngine.ColorUtility.TryParseHtmlString(active ? highlightedColor : baseColor, out color) || !((Object) nodeImage != (Object) null))
        return;
      nodeImage.color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      SetActive(true);
      this.GetComponentInParent<MMPageView>().ShowNodeInfo(this);
    }

    public void MarkNodeDiscovered()
    {
      if (Node.Undiscovered)
        Node.Undiscovered = false;
      if (!((Object) NewIndicator != (Object) null))
        return;
      Object.Destroy((Object) NewIndicator);
      NewIndicator = (GameObject) null;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      SetActive(false);
      this.GetComponentInParent<MMPageView>().HideNodeInfo(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      MarkNodeDiscovered();
      if (!HasMapItem)
        return;
      this.GetComponentInParent<MMPageView>().CallMap(this);
    }
  }
}
