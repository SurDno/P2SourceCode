using UnityEngine;

namespace Rain;

[RequireComponent(typeof(MeshRenderer))]
public class Wall : MonoBehaviour {
	public float speed = 1f;
	private MeshRenderer _renderer;
	private Material _material;
	private float _phase;

	private void Start() {
		_renderer = GetComponent<MeshRenderer>();
		_material = _renderer.material;
	}

	private void LateUpdate() {
		var instance = RainManager.Instance;
		if (instance == null || instance.actualRainIntensity <= 0.0)
			_renderer.enabled = false;
		else {
			transform.position = instance.PlayerPosition;
			_phase += Time.deltaTime * speed;
			_phase = Mathf.Repeat(_phase, 1f);
			if (_phase > 1.0)
				_phase -= Mathf.Floor(_phase);
			_material.mainTextureOffset = new Vector2(0.0f, _phase);
			_material.SetVector("_Params",
				new Vector4(instance.actualWindVector.x, instance.actualWindVector.y, instance.actualRainIntensity,
					0.0f));
			_renderer.enabled = true;
		}
	}
}