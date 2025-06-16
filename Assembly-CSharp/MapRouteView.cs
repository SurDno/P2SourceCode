using System.Collections.Generic;
using Engine.Common.Components.Regions;

public abstract class MapRouteView : MonoBehaviour
{
  public abstract void SetRoute(IList<FastTravelPointEnum> route);
}
