using Engine.Source.Behaviours;
using Inspectors;
using UnityEngine;

public class PlagueSpots : MonoBehaviour, IValueController {
	private static MaterialPropertyBlock propertyBlock;
	private static int propertyID;
	private float value;

	[Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public float Value {
		get => value;
		set {
			this.value = Mathf.Clamp01(value);
			ApplyLevel();
		}
	}

	private void ApplyLevel() {
		if (propertyBlock == null) {
			propertyBlock = new MaterialPropertyBlock();
			propertyID = Shader.PropertyToID("_Level");
		}

		propertyBlock.SetFloat(propertyID, value);
		foreach (Renderer componentsInChild in GetComponentsInChildren<MeshRenderer>())
			componentsInChild.SetPropertyBlock(propertyBlock);
	}
}