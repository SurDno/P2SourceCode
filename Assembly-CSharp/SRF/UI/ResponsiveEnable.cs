using System;
using UnityEngine;

namespace SRF.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("SRF/UI/Responsive (Enable)")]
public class ResponsiveEnable : ResponsiveBase {
	public Entry[] Entries = new Entry[0];

	protected override void Refresh() {
		var rect = RectTransform.rect;
		for (var index1 = 0; index1 < Entries.Length; ++index1) {
			var entry = Entries[index1];
			var flag = true;
			switch (entry.Mode) {
				case Modes.EnableAbove:
					if (entry.ThresholdHeight > 0.0)
						flag = (rect.height >= (double)entry.ThresholdHeight) & flag;
					if (entry.ThresholdWidth > 0.0) flag = (rect.width >= (double)entry.ThresholdWidth) & flag;
					break;
				case Modes.EnableBelow:
					if (entry.ThresholdHeight > 0.0)
						flag = (rect.height <= (double)entry.ThresholdHeight) & flag;
					if (entry.ThresholdWidth > 0.0) flag = (rect.width <= (double)entry.ThresholdWidth) & flag;
					break;
				default:
					throw new IndexOutOfRangeException();
			}

			if (entry.GameObjects != null)
				for (var index2 = 0; index2 < entry.GameObjects.Length; ++index2) {
					var gameObject = entry.GameObjects[index2];
					if (gameObject != null)
						gameObject.SetActive(flag);
				}

			if (entry.Components != null)
				for (var index3 = 0; index3 < entry.Components.Length; ++index3) {
					var component = entry.Components[index3];
					if (component != null)
						component.enabled = flag;
				}
		}
	}

	public enum Modes {
		EnableAbove,
		EnableBelow
	}

	[Serializable]
	public struct Entry {
		public Behaviour[] Components;
		public GameObject[] GameObjects;
		public Modes Mode;
		public float ThresholdHeight;
		public float ThresholdWidth;
	}
}