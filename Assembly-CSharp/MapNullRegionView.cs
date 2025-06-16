using UnityEngine;
using UnityEngine.EventSystems;

public class MapNullRegionView : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
  [SerializeField]
  private MapRegionInfoWindow regionInfoWindow;

  public void OnPointerEnter(PointerEventData eventData) => this.regionInfoWindow.Hide();
}
