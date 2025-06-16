using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class POIAnimationSetupQuick : POIAnimationSetupBase {
	[SerializeField] public List<POIAnimationSetupElementQuick> ElementsNew = new();

	public override List<POIAnimationSetupElementBase> Elements => new(ElementsNew);
}