using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class POIAnimationSetupAngle : POIAnimationSetupBase {
	[SerializeField] public POIAnimationSetupElementSlow SetupLeft = new();
	[SerializeField] public POIAnimationSetupElementSlow SetupMiddle = new();
	[SerializeField] public POIAnimationSetupElementSlow SetupRight = new();
	[SerializeField] private List<POIAnimationSetupElementBase> elements;

	public override List<POIAnimationSetupElementBase> Elements {
		get {
			if (!CheckElements()) {
				elements = new List<POIAnimationSetupElementBase>();
				elements.Add(SetupLeft);
				elements.Add(SetupMiddle);
				elements.Add(SetupRight);
			}

			return elements;
		}
	}

	private bool CheckElements() {
		if (elements == null || elements.Count == 0)
			return false;
		foreach (var element in elements)
			if (!(element is POIAnimationSetupElementSlow))
				return false;
		return true;
	}
}