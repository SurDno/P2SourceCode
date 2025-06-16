// Decompiled with JetBrains decompiler
// Type: HUDQuestCompass
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
    for (int index = 0; index < this.markers.Count; ++index)
    {
      if (this.markers[index].MapItem == mapItem)
        return;
    }
    HUDQuestMarker hudQuestMarker = UnityEngine.Object.Instantiate<HUDQuestMarker>(this.markerPrototype, (Transform) this.anchor, false);
    hudQuestMarker.MapItem = mapItem;
    this.markers.Add(hudQuestMarker);
    hudQuestMarker.gameObject.SetActive(true);
  }

  private void OnEnable()
  {
    MapService service = ServiceLocator.GetService<MapService>();
    service.HUDItemAddEvent += new Action<IMapItem>(this.AddMarker);
    service.HUDItemRemoveEvent += new Action<IMapItem>(this.RemoveMarker);
    ServiceLocator.GetService<QuestCompassService>().OnEnableChanged += new Action<bool>(this.OnFocusEnableChanged);
    ServiceLocator.GetService<InterfaceBlockingService>().OnBlockChanged += new Action(this.UpdateVisibility);
    foreach (IMapItem questItem in service.QuestItems)
      this.AddMarker(questItem);
    this.UpdateVisibility();
  }

  private void OnDisable()
  {
    MapService service = ServiceLocator.GetService<MapService>();
    service.HUDItemAddEvent -= new Action<IMapItem>(this.AddMarker);
    service.HUDItemRemoveEvent -= new Action<IMapItem>(this.RemoveMarker);
    ServiceLocator.GetService<QuestCompassService>().OnEnableChanged -= new Action<bool>(this.OnFocusEnableChanged);
    ServiceLocator.GetService<InterfaceBlockingService>().OnBlockChanged -= new Action(this.UpdateVisibility);
    for (int index = 0; index < this.markers.Count; ++index)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.markers[index].gameObject);
    this.markers.Clear();
    this.visible.Visible = false;
  }

  private void OnFocusEnableChanged(bool value) => this.UpdateVisibility();

  private void RemoveMarker(IMapItem mapItem)
  {
    if (mapItem == null)
      return;
    for (int index = 0; index < this.markers.Count; ++index)
    {
      HUDQuestMarker marker = this.markers[index];
      if (marker.MapItem == mapItem)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) marker.gameObject);
        this.markers.RemoveAt(index);
        break;
      }
    }
  }

  private void UpdateVisibility()
  {
    this.visible.Visible = ServiceLocator.GetService<QuestCompassService>().IsEnabled && !ServiceLocator.GetService<InterfaceBlockingService>().BlockMapInterface;
  }
}
