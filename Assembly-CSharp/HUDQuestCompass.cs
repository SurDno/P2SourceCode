using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;

public class HUDQuestCompass : MonoBehaviour
{
  [SerializeField]
  private HUDQuestMarker markerPrototype;
  [SerializeField]
  private RectTransform anchor;
  [SerializeField]
  private HideableView visible;
  private List<HUDQuestMarker> markers = new List<HUDQuestMarker>();

  private void AddMarker(IMapItem mapItem)
  {
    if (mapItem == null)
      return;
    for (int index = 0; index < markers.Count; ++index)
    {
      if (markers[index].MapItem == mapItem)
        return;
    }
    HUDQuestMarker hudQuestMarker = UnityEngine.Object.Instantiate<HUDQuestMarker>(markerPrototype, (Transform) anchor, false);
    hudQuestMarker.MapItem = mapItem;
    markers.Add(hudQuestMarker);
    hudQuestMarker.gameObject.SetActive(true);
  }

  private void OnEnable()
  {
    MapService service = ServiceLocator.GetService<MapService>();
    service.HUDItemAddEvent += AddMarker;
    service.HUDItemRemoveEvent += RemoveMarker;
    ServiceLocator.GetService<QuestCompassService>().OnEnableChanged += OnFocusEnableChanged;
    ServiceLocator.GetService<InterfaceBlockingService>().OnBlockChanged += UpdateVisibility;
    foreach (IMapItem questItem in service.QuestItems)
      AddMarker(questItem);
    UpdateVisibility();
  }

  private void OnDisable()
  {
    MapService service = ServiceLocator.GetService<MapService>();
    service.HUDItemAddEvent -= AddMarker;
    service.HUDItemRemoveEvent -= RemoveMarker;
    ServiceLocator.GetService<QuestCompassService>().OnEnableChanged -= OnFocusEnableChanged;
    ServiceLocator.GetService<InterfaceBlockingService>().OnBlockChanged -= UpdateVisibility;
    for (int index = 0; index < markers.Count; ++index)
      UnityEngine.Object.Destroy((UnityEngine.Object) markers[index].gameObject);
    markers.Clear();
    visible.Visible = false;
  }

  private void OnFocusEnableChanged(bool value) => UpdateVisibility();

  private void RemoveMarker(IMapItem mapItem)
  {
    if (mapItem == null)
      return;
    for (int index = 0; index < markers.Count; ++index)
    {
      HUDQuestMarker marker = markers[index];
      if (marker.MapItem == mapItem)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) marker.gameObject);
        markers.RemoveAt(index);
        break;
      }
    }
  }

  private void UpdateVisibility()
  {
    visible.Visible = ServiceLocator.GetService<QuestCompassService>().IsEnabled && !ServiceLocator.GetService<InterfaceBlockingService>().BlockMapInterface;
  }
}
