using UnityEngine;

[ExecuteInEditMode]
public class MaterialLevelController : MonoBehaviour {
	private static MaterialPropertyBlock propertyBlock;
	[Range(0.0f, 1f)] public float Level;

	private void OnEnable() {
		UpdateMaterial();
	}

	private void LateUpdate() {
		UpdateMaterial();
	}

	private void UpdateMaterial() {
		var component = GetComponent<Renderer>();
		if (component == null)
			return;
		if (Level > 0.0) {
			if (propertyBlock == null)
				propertyBlock = new MaterialPropertyBlock();
			propertyBlock.SetFloat("_Level", Level);
			component.SetPropertyBlock(propertyBlock);
			component.enabled = true;
		} else
			component.enabled = false;
	}
}