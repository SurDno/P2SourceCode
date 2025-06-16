using Engine.Impl.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Source.Behaviours;

public class AlphaImageUiEffect : ProgressViewBase {
	[SerializeField] private RawImage rawImage;
	[SerializeField] private Image image;

	public override float Progress {
		get {
			if (rawImage != null)
				return rawImage.color.a;
			return image != null ? image.color.a : 1f;
		}
		set {
			if (rawImage != null)
				rawImage.color = rawImage.color with {
					a = value
				};
			if (!(image != null))
				return;
			image.color = image.color with {
				a = value
			};
		}
	}

	public override void SkipAnimation() { }
}