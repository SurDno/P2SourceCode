using Engine.Impl.UI.Controls;
using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class SpriteEffectNode : FlowControlNode, IUpdatable {
	[Port("Value")] private ValueInput<float> valueInput;
	[Port("Angle")] private ValueInput<float> angleInput;
	[Port("Position")] private ValueInput<Vector2> positionInput;
	[Port("Scale", 1f, 1f, 1f)] private ValueInput<Vector3> scaleInput;
	[Port("Sprite")] private ValueInput<ProgressViewBase> spriteInput;
	private ProgressViewBase sprite;
	private float prevValue;

	public void Update() {
		var num = valueInput.value;
		if (prevValue == (double)num)
			return;
		prevValue = num;
		if (num != 0.0) {
			CreateSprite();
			if (sprite != null)
				sprite.Progress = num;
		} else
			DestroySprite();
	}

	public override void OnDestroy() {
		DestroySprite();
		base.OnDestroy();
	}

	private void CreateSprite() {
		if (!(sprite == null))
			return;
		var prefab = spriteInput.value;
		if (prefab != null) {
			sprite = UnityFactory.Instantiate(prefab, "[Effects]");
			MonoBehaviourInstance<UiEffectsController>.Instance.AddEffect(sprite.gameObject);
			SetAngle();
			SetOffset();
			SetScale();
		}
	}

	private void SetAngle() {
		if (!(sprite != null))
			return;
		sprite.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angleInput.value);
	}

	private void SetOffset() {
		if (!(sprite != null))
			return;
		var rect = ((RectTransform)sprite.transform.parent).rect;
		var vector2 = positionInput.value;
		var z = sprite.transform.localPosition.z;
		sprite.transform.localPosition = new Vector3(vector2.x * rect.width, vector2.y * rect.height, z);
	}

	private void SetScale() {
		if (!(sprite != null))
			return;
		sprite.transform.localScale = scaleInput.value;
	}

	private void DestroySprite() {
		if (!(sprite != null))
			return;
		MonoBehaviourInstance<UiEffectsController>.Instance.RemoveEffect(sprite.gameObject);
		UnityFactory.Destroy(sprite);
		sprite = null;
	}
}