using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class RendererOpacityFloatView : FloatViewBase {
	private static MaterialPropertyBlock propertyBlock;
	private static int propertyId;
	[SerializeField] private Renderer targetRenderer;
	[SerializeField] private string propertyName = "_Color";

	public override void SkipAnimation() { }

	protected override void ApplyFloatValue() {
		if (targetRenderer == null)
			return;
		var sharedMaterial = targetRenderer.sharedMaterial;
		if (sharedMaterial == null)
			return;
		var id = Shader.PropertyToID(propertyName);
		if (!sharedMaterial.HasProperty(id))
			return;
		if (propertyBlock == null)
			propertyBlock = new MaterialPropertyBlock();
		var color = sharedMaterial.GetColor(id);
		color.a *= FloatValue;
		propertyBlock.Clear();
		propertyBlock.SetColor(id, color);
		targetRenderer.SetPropertyBlock(propertyBlock);
	}
}