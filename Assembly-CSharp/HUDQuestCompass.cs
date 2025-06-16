using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using UnityEngine;

public class HUDQuestCompass : MonoBehaviour {
	[SerializeField] private HUDQuestMarker markerPrototype;
	[SerializeField] private RectTransform anchor;
	[SerializeField] private HideableView visible;
	private List<HUDQuestMarker> markers = new();

	private void AddMarker(IMapItem mapItem) {
		if (mapItem == null)
			return;
		for (var index = 0; index < markers.Count; ++index)
			if (markers[index].MapItem == mapItem)
				return;
		var hudQuestMarker = Instantiate(markerPrototype, anchor, false);
		hudQuestMarker.MapItem = mapItem;
		markers.Add(hudQuestMarker);
		hudQuestMarker.gameObject.SetActive(true);
	}

	private void OnEnable() {
		var service = ServiceLocator.GetService<MapService>();
		service.HUDItemAddEvent += AddMarker;
		service.HUDItemRemoveEvent += RemoveMarker;
		ServiceLocator.GetService<QuestCompassService>().OnEnableChanged += OnFocusEnableChanged;
		ServiceLocator.GetService<InterfaceBlockingService>().OnBlockChanged += UpdateVisibility;
		foreach (var questItem in service.QuestItems)
			AddMarker(questItem);
		UpdateVisibility();
	}

	private void OnDisable() {
		var service = ServiceLocator.GetService<MapService>();
		service.HUDItemAddEvent -= AddMarker;
		service.HUDItemRemoveEvent -= RemoveMarker;
		ServiceLocator.GetService<QuestCompassService>().OnEnableChanged -= OnFocusEnableChanged;
		ServiceLocator.GetService<InterfaceBlockingService>().OnBlockChanged -= UpdateVisibility;
		for (var index = 0; index < markers.Count; ++index)
			Destroy(markers[index].gameObject);
		markers.Clear();
		visible.Visible = false;
	}

	private void OnFocusEnableChanged(bool value) {
		UpdateVisibility();
	}

	private void RemoveMarker(IMapItem mapItem) {
		if (mapItem == null)
			return;
		for (var index = 0; index < markers.Count; ++index) {
			var marker = markers[index];
			if (marker.MapItem == mapItem) {
				Destroy(marker.gameObject);
				markers.RemoveAt(index);
				break;
			}
		}
	}

	private void UpdateVisibility() {
		visible.Visible = ServiceLocator.GetService<QuestCompassService>().IsEnabled &&
		                  !ServiceLocator.GetService<InterfaceBlockingService>().BlockMapInterface;
	}
}