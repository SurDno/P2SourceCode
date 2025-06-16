using System.Collections.Generic;
using Engine.Common.Components.Regions;
using UnityEngine;

public abstract class MapRouteView : MonoBehaviour
{
  public abstract void SetRoute(IList<FastTravelPointEnum> route);
}
