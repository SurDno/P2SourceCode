using System.Collections.Generic;
using Engine.Common.Components.Regions;
using Engine.Impl.UI.Controls;
using UnityEngine;

public class MapRouteViewSegment : MapRouteView
{
  [SerializeField]
  private HideableView view;
  [SerializeField]
  private FastTravelPointEnum pointA;
  [SerializeField]
  private FastTravelPointEnum pointB;

  public override void SetRoute(IList<FastTravelPointEnum> route)
  {
    if (view == null)
      return;
    if (route == null || route.Count <= 1)
    {
      view.Visible = false;
    }
    else
    {
      FastTravelPointEnum fastTravelPointEnum1 = route[0];
      for (int index = 1; index < route.Count; ++index)
      {
        FastTravelPointEnum fastTravelPointEnum2 = fastTravelPointEnum1;
        fastTravelPointEnum1 = route[index];
        if (fastTravelPointEnum2 == pointA && fastTravelPointEnum1 == pointB || fastTravelPointEnum1 == pointA && fastTravelPointEnum2 == pointB)
        {
          view.Visible = true;
          return;
        }
      }
      view.Visible = false;
    }
  }
}
