// Decompiled with JetBrains decompiler
// Type: MapRegionInfoWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Localization;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Components.Maps;
using RegionReputation;
using UnityEngine;

#nullable disable
public class MapRegionInfoWindow : MonoBehaviour
{
  [SerializeField]
  private Localizer title = (Localizer) null;
  [SerializeField]
  private Localizer text = (Localizer) null;
  [SerializeField]
  private RegionReputationNames regionReputationNames;
  private MapItemView targetView;

  public void Show(MapItemView itemView)
  {
    if ((Object) itemView == (Object) this.targetView)
      return;
    if ((Object) this.targetView == (Object) null)
      this.gameObject.SetActive(true);
    else
      this.targetView.SetHightlight(false);
    this.targetView = itemView;
    this.targetView.SetHightlight(true);
    MapItemComponent mapItemComponent = this.targetView.Item as MapItemComponent;
    if (mapItemComponent.Resource == null)
      return;
    this.title.Signature = ServiceLocator.GetService<LocalizationService>().GetText(mapItemComponent.Title);
    this.text.Signature = this.regionReputationNames.GetReputationName(mapItemComponent.Region.Region, mapItemComponent.Reputation);
  }

  public void Hide()
  {
    if (!((Object) this.targetView != (Object) null))
      return;
    this.gameObject.SetActive(false);
    this.targetView.SetHightlight(false);
    this.targetView = (MapItemView) null;
  }
}
