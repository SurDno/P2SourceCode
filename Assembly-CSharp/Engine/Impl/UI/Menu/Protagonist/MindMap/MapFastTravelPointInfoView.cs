using System;
using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Protagonist.MindMap
{
  public class MapFastTravelPointInfoView : MonoBehaviour
  {
    [SerializeField]
    private FastTravelGraph graph;
    [SerializeField]
    private IEntitySerializable resourceTemplate;
    [SerializeField]
    private StringView titleView;
    [SerializeField]
    private StringView textView;
    [SerializeField]
    private StringView timeView;
    [SerializeField]
    private StringView costView;
    [SerializeField]
    private HideableView enoughResourcesView;
    [SerializeField]
    private MapRouteView routeView;
    private MapFastTravelPointView targetView;
    private FastTravelComponent origin;
    private List<FastTravelPointEnum> path = new List<FastTravelPointEnum>();
    private int time;
    private bool enoughResource;

    public void Hide()
    {
      targetView = null;
      origin = null;
      path.Clear();
      routeView.SetRoute(null);
      gameObject.SetActive(false);
    }

    private void LateUpdate() => UpdatePosition();

    public void Show(FastTravelComponent origin, MapFastTravelPointView targetView)
    {
      IMapItem mapItem = targetView.Item;
      time = graph.GetPath(origin.FastTravelPointIndex.Value, mapItem.FastTravelPoint.Value, path);
      if (time == -1)
        return;
      this.origin = origin;
      this.targetView = targetView;
      int itemAmount = StorageUtility.GetItemAmount(ServiceLocator.GetService<ISimulation>().Player?.GetComponent<IStorageComponent>().Items, resourceTemplate.Value);
      int num = origin.FastTravelPrice.Value;
      enoughResource = itemAmount >= num;
      LocalizationService service = ServiceLocator.GetService<LocalizationService>();
      if (mapItem.Title != LocalizedText.Empty)
      {
        titleView.StringValue = service.GetText(mapItem.Title);
        titleView.gameObject.SetActive(true);
      }
      else
        titleView.gameObject.SetActive(false);
      if (mapItem.Text != LocalizedText.Empty)
      {
        textView.StringValue = service.GetText(mapItem.Text);
        textView.gameObject.SetActive(true);
      }
      else
        textView.gameObject.SetActive(false);
      timeView.StringValue = TimeSpan.FromMinutes(time).ToShortTimeString();
      costView.StringValue = num + " (" + itemAmount + ")";
      enoughResourcesView.Visible = enoughResource;
      routeView.SetRoute(path);
      gameObject.SetActive(true);
      UpdatePosition();
    }

    public void Travel()
    {
      if (path.Count <= 1 || !enoughResource)
        return;
      origin.FireTravelToPoint(targetView.Item.FastTravelPoint.Value, TimeSpan.FromMinutes(time));
      ServiceLocator.GetService<UIService>().Pop();
    }

    private void UpdatePosition()
    {
      RectTransform transform1 = (RectTransform) transform;
      RectTransform transform2 = (RectTransform) GetComponentInParent<Canvas>().transform;
      Vector2 vector2 = new Vector2(transform2.sizeDelta.x, transform2.sizeDelta.y);
      Vector2 position = targetView.transform.position;
      position.x = Mathf.Round(position.x);
      position.y = Mathf.Round(position.y);
      position.x /= transform2.localScale.x;
      position.y /= transform2.localScale.y;
      transform1.pivot = new Vector2(position.x > vector2.x * 0.699999988079071 ? 1f : 0.0f, position.y > vector2.y * 0.30000001192092896 ? 1f : 0.0f);
      transform1.anchoredPosition = position;
    }
  }
}
