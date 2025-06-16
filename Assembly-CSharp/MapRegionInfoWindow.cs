using Engine.Behaviours.Localization;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Components.Maps;
using RegionReputation;
using UnityEngine;

public class MapRegionInfoWindow : MonoBehaviour
{
  [SerializeField]
  private Localizer title;
  [SerializeField]
  private Localizer text;
  [SerializeField]
  private RegionReputationNames regionReputationNames;
  private MapItemView targetView;

  public void Show(MapItemView itemView)
  {
    if (itemView == targetView)
      return;
    if (targetView == null)
      gameObject.SetActive(true);
    else
      targetView.SetHightlight(false);
    targetView = itemView;
    targetView.SetHightlight(true);
    MapItemComponent mapItemComponent = targetView.Item as MapItemComponent;
    if (mapItemComponent.Resource == null)
      return;
    title.Signature = ServiceLocator.GetService<LocalizationService>().GetText(mapItemComponent.Title);
    text.Signature = regionReputationNames.GetReputationName(mapItemComponent.Region.Region, mapItemComponent.Reputation);
  }

  public void Hide()
  {
    if (!(targetView != null))
      return;
    gameObject.SetActive(false);
    targetView.SetHightlight(false);
    targetView = null;
  }
}
