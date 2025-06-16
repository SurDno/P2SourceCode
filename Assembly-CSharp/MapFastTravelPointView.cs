// Decompiled with JetBrains decompiler
// Type: MapFastTravelPointView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
public class MapFastTravelPointView : 
  MonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler,
  IPointerClickHandler
{
  public IMapItem Item { get; set; }

  public MapWindow MapView { get; set; }

  public void OnPointerClick(PointerEventData eventData)
  {
    this.MapView.CallSelectedFastTravelPoint();
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    this.MapView.ShowFastTravelPointInfo(this);
  }

  public void OnPointerExit(PointerEventData eventData) => this.MapView.HideFastTravelPointInfo();
}
