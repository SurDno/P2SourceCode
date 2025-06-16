using Engine.Common.Components.Regions;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapRouteView : MonoBehaviour
{
  public abstract void SetRoute(IList<FastTravelPointEnum> route);
}
