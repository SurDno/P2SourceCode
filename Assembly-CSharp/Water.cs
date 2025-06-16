using UnityEngine;

public class Water : MonoBehaviour {
	private Vector2 _uvOffset = Vector2.zero;
	private Renderer _renderer;
	private Material material;

	private void Start() {
		_renderer = GetComponent<Renderer>();
		material = _renderer.materials[0];
	}

	private void Update() {
		_uvOffset += new Vector2(0.051f, 0.091f) * Time.deltaTime;
		material.SetTextureOffset("_MainTex", _uvOffset);
	}
}