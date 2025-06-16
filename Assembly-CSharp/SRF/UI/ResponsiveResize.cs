using System;
using UnityEngine;
using UnityEngine.UI;

namespace SRF.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("SRF/UI/Responsive (Enable)")]
public class ResponsiveResize : ResponsiveBase {
	public Element[] Elements = new Element[0];

	protected override void Refresh() {
		var rect = RectTransform.rect;
		for (var index1 = 0; index1 < Elements.Length; ++index1) {
			var element = Elements[index1];
			if (!(element.Target == null)) {
				var num = float.MinValue;
				var size = -1f;
				for (var index2 = 0; index2 < element.SizeDefinitions.Length; ++index2) {
					var sizeDefinition = element.SizeDefinitions[index2];
					if (sizeDefinition.ThresholdWidth <= (double)rect.width &&
					    sizeDefinition.ThresholdWidth > (double)num) {
						num = sizeDefinition.ThresholdWidth;
						size = sizeDefinition.ElementWidth;
					}
				}

				if (size > 0.0) {
					element.Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
					var component = element.Target.GetComponent<LayoutElement>();
					if (component != null)
						component.preferredWidth = size;
				}
			}
		}
	}

	[Serializable]
	public struct Element {
		public SizeDefinition[] SizeDefinitions;
		public RectTransform Target;

		[Serializable]
		public struct SizeDefinition {
			[Tooltip("Width to apply when over the threshold width")]
			public float ElementWidth;

			[Tooltip("Threshold over which this width will take effect")]
			public float ThresholdWidth;
		}
	}
}