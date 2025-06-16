using System.Collections.Generic;
using Engine.Common.Components.Regions;
using UnityEngine;

public class MapRouteViewSplit : MapRouteView {
	[SerializeField] private MapRouteView[] views;

	public override void SetRoute(IList<FastTravelPointEnum> route) {
		for (var index = 0; index < views.Length; ++index)
			views[index]?.SetRoute(route);
	}
}