// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.MindMap.MMNodeView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
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
      if ((Object) this.nodeImage == (Object) null)
        this.nodeImage = this.GetComponent<RawImage>();
      if ((Object) this.nodeImage != (Object) null)
        this.baseColor = this.nodeImage.color.ToRGBHex();
      this.rectTransform = this.GetComponent<RectTransform>();
    }

    public Rect GetSpriteRect()
    {
      return (Object) this.rectTransform != (Object) null ? this.rectTransform.rect : Rect.zero;
    }

    public void SetActive(bool active)
    {
      Color color;
      if (!UnityEngine.ColorUtility.TryParseHtmlString(active ? this.highlightedColor : this.baseColor, out color) || !((Object) this.nodeImage != (Object) null))
        return;
      this.nodeImage.color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      this.SetActive(true);
      this.GetComponentInParent<MMPageView>().ShowNodeInfo(this);
    }

    public void MarkNodeDiscovered()
    {
      if (this.Node.Undiscovered)
        this.Node.Undiscovered = false;
      if (!((Object) this.NewIndicator != (Object) null))
        return;
      Object.Destroy((Object) this.NewIndicator);
      this.NewIndicator = (GameObject) null;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      this.SetActive(false);
      this.GetComponentInParent<MMPageView>().HideNodeInfo(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      this.MarkNodeDiscovered();
      if (!this.HasMapItem)
        return;
      this.GetComponentInParent<MMPageView>().CallMap(this);
    }
  }
}
