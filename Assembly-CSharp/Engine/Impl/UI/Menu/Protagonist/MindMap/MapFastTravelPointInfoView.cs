// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.MindMap.MapFastTravelPointInfoView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
      this.targetView = (MapFastTravelPointView) null;
      this.origin = (FastTravelComponent) null;
      this.path.Clear();
      this.routeView.SetRoute((IList<FastTravelPointEnum>) null);
      this.gameObject.SetActive(false);
    }

    private void LateUpdate() => this.UpdatePosition();

    public void Show(FastTravelComponent origin, MapFastTravelPointView targetView)
    {
      IMapItem mapItem = targetView.Item;
      this.time = this.graph.GetPath(origin.FastTravelPointIndex.Value, mapItem.FastTravelPoint.Value, (IList<FastTravelPointEnum>) this.path);
      if (this.time == -1)
        return;
      this.origin = origin;
      this.targetView = targetView;
      int itemAmount = StorageUtility.GetItemAmount(ServiceLocator.GetService<ISimulation>().Player?.GetComponent<IStorageComponent>().Items, this.resourceTemplate.Value);
      int num = origin.FastTravelPrice.Value;
      this.enoughResource = itemAmount >= num;
      LocalizationService service = ServiceLocator.GetService<LocalizationService>();
      if (mapItem.Title != LocalizedText.Empty)
      {
        this.titleView.StringValue = service.GetText(mapItem.Title);
        this.titleView.gameObject.SetActive(true);
      }
      else
        this.titleView.gameObject.SetActive(false);
      if (mapItem.Text != LocalizedText.Empty)
      {
        this.textView.StringValue = service.GetText(mapItem.Text);
        this.textView.gameObject.SetActive(true);
      }
      else
        this.textView.gameObject.SetActive(false);
      this.timeView.StringValue = TimeSpan.FromMinutes((double) this.time).ToShortTimeString();
      this.costView.StringValue = num.ToString() + " (" + (object) itemAmount + ")";
      this.enoughResourcesView.Visible = this.enoughResource;
      this.routeView.SetRoute((IList<FastTravelPointEnum>) this.path);
      this.gameObject.SetActive(true);
      this.UpdatePosition();
    }

    public void Travel()
    {
      if (this.path.Count <= 1 || !this.enoughResource)
        return;
      this.origin.FireTravelToPoint(this.targetView.Item.FastTravelPoint.Value, TimeSpan.FromMinutes((double) this.time));
      ServiceLocator.GetService<UIService>().Pop();
    }

    private void UpdatePosition()
    {
      RectTransform transform1 = (RectTransform) this.transform;
      RectTransform transform2 = (RectTransform) this.GetComponentInParent<Canvas>().transform;
      Vector2 vector2 = new Vector2(transform2.sizeDelta.x, transform2.sizeDelta.y);
      Vector2 position = (Vector2) this.targetView.transform.position;
      position.x = Mathf.Round(position.x);
      position.y = Mathf.Round(position.y);
      position.x /= transform2.localScale.x;
      position.y /= transform2.localScale.y;
      transform1.pivot = new Vector2((double) position.x > (double) vector2.x * 0.699999988079071 ? 1f : 0.0f, (double) position.y > (double) vector2.y * 0.30000001192092896 ? 1f : 0.0f);
      transform1.anchoredPosition = position;
    }
  }
}
