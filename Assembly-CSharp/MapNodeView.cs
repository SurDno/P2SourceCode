// Decompiled with JetBrains decompiler
// Type: MapNodeView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.MindMap;
using Engine.Source.UI.Menu.Protagonist.MindMap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
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

  public void SetMapView(MapWindow mapView) => this.mapView = mapView;

  public void SetNode(MMTooltip tooltip) => this.tooltip = tooltip;

  public IMMNode GetNode() => this.tooltip.node;

  public void OnPointerClick(PointerEventData eventData)
  {
    if (this.tooltip.node == null)
      return;
    this.mapView.CallMindMap(this.tooltip.node);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    this.SetActive(true);
    this.mapView.ShowMapNodeInfo(this, this.tooltip);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    this.SetActive(false);
    this.mapView.HideMapNodeInfo();
  }
}
