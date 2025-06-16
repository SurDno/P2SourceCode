using Engine.Common.Components.Regions;
using System.Collections.Generic;
using UnityEngine;

public class MapRouteViewSplit : MapRouteView
{
  [SerializeField]
  private MapRouteView[] views;

  public override void SetRoute(IList<FastTravelPointEnum> route)
  {
    for (int index = 0; index < this.views.Length; ++index)
      this.views[index]?.SetRoute(route);
  }
}
