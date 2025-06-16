using UnityEngine;
using UnityEngine.EventSystems;

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
