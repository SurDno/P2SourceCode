using Engine.Impl.UI.Controls;
using UnityEngine;

public class OcclusionCullingSettings : HideableView {
	[SerializeField] private Camera targetCamera;

	protected override void ApplyVisibility() {
		if (!(targetCamera != null))
			return;
		targetCamera.useOcclusionCulling = Visible;
	}
}