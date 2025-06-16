using UnityEngine;

namespace Rain;

public class Ripple {
	private const float FadeSpeed = 0.5f;
	private const float ExpandSpeed = 0.5f;
	public Material material;
	public Ripple nextNode;
	private float _radius;
	private float _strength;

	public Ripple(
		Ripple currentRootNode,
		Material puddleMaterial,
		Vector3 worldPosition,
		float startRadius,
		float endRadius) {
		nextNode = currentRootNode;
		material = new Material(puddleMaterial);
		material.SetVector("_RippleOrigin", new Vector4(worldPosition.x, worldPosition.y, worldPosition.z, 0.0f));
		_radius = startRadius;
		_strength = (float)((endRadius - (double)startRadius) * 0.5 / 0.5);
		UpdateMaterial();
	}

	public bool Update() {
		_strength -= Time.deltaTime * 0.5f;
		if (_strength <= 0.0) {
			Object.Destroy(material);
			return true;
		}

		_radius += Time.deltaTime * 0.5f;
		UpdateMaterial();
		return false;
	}

	private void UpdateMaterial() {
		material.SetFloat("_RippleStrength", Mathf.Min(Mathf.Sqrt(_strength), 1f));
		material.SetFloat("_RippleRadius", _radius);
	}
}