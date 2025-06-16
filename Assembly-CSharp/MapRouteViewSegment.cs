using Engine.Common.Components.Regions;
using Engine.Impl.UI.Controls;
using System.Collections.Generic;
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
    if ((Object) this.view == (Object) null)
      return;
    if (route == null || route.Count <= 1)
    {
      this.view.Visible = false;
    }
    else
    {
      FastTravelPointEnum fastTravelPointEnum1 = route[0];
      for (int index = 1; index < route.Count; ++index)
      {
        FastTravelPointEnum fastTravelPointEnum2 = fastTravelPointEnum1;
        fastTravelPointEnum1 = route[index];
        if (fastTravelPointEnum2 == this.pointA && fastTravelPointEnum1 == this.pointB || fastTravelPointEnum1 == this.pointA && fastTravelPointEnum2 == this.pointB)
        {
          this.view.Visible = true;
          return;
        }
      }
      this.view.Visible = false;
    }
  }
}
