using Engine.Common.MindMap;
using Engine.Impl.MindMap;
using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.UI;

public class HUDQuestMarker : MonoBehaviour {
	[SerializeField] private ProgressView forwardDotView;
	[SerializeField] private ProgressView rightDotView;
	[SerializeField] private FloatView distanceView;
	[SerializeField] private RawImage image;
	[SerializeField] private RawImage shadowImage;
	[Inspected] private IMapItem mapItem;

	public IMapItem MapItem {
		get => mapItem;
		set {
			if (mapItem == value)
				return;
			mapItem = value;
			ApplyMapItem();
		}
	}

	private void OnEnable() {
		ApplyMapItem();
		ApplyPosition();
	}

	private void LateUpdate() {
		ApplyPosition();
	}

	private void ApplyMapItem() {
		if (mapItem == null) {
			image.texture = null;
			shadowImage.texture = null;
		} else {
			Texture texture = null;
			foreach (var node in mapItem.Nodes)
				if (node.Content?.Placeholder is MMPlaceholder placeholder) {
					texture = placeholder.Image.Value;
					break;
				}

			if (texture == null)
				texture = mapItem.TooltipResource is MapTooltipResource tooltipResource
					? tooltipResource.Image.Value
					: null;
			image.texture = texture;
			shadowImage.texture = texture;
		}
	}

	private void ApplyPosition() {
		if (mapItem == null)
			return;
		var cameraTransform = GameCamera.Instance?.CameraTransform;
		if (cameraTransform == null)
			return;
		var position = cameraTransform.position;
		var vector2 = mapItem.WorldPosition - new Vector2(position.x, position.z);
		var magnitude = vector2.magnitude;
		var lhs = vector2 / magnitude;
		var forward = cameraTransform.forward;
		var rhs1 = new Vector2(forward.x, forward.z);
		rhs1.Normalize();
		forwardDotView.Progress = Mathf.Clamp01(Vector2.Dot(lhs, rhs1));
		var rhs2 = new Vector2(rhs1.y, -rhs1.x);
		rightDotView.Progress = (float)((Vector2.Dot(lhs, rhs2) + 1.0) / 2.0);
		distanceView.FloatValue = magnitude;
	}
}