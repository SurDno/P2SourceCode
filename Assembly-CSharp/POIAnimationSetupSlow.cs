using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class POIAnimationSetupSlow : POIAnimationSetupBase {
	[SerializeField] public List<POIAnimationSetupElementSlow> ElementsNew = new();

	public override List<POIAnimationSetupElementBase> Elements => new(ElementsNew);
}