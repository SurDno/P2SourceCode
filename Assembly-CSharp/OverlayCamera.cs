using UnityEngine;

public class OverlayCamera : MonoBehaviour {
	public static float Gamma = 2.2f;
	public Shader PostProcessShader;
	private Material material;
	private Camera camera;

	private void Awake() {
		CorrectPosition();
	}

	private void CorrectPosition() {
		var x = 0.5f * Screen.width;
		var y = 0.5f * Screen.height;
		transform.localPosition = new Vector3(x, y, 0.0f);
		if (camera == null)
			camera = GetComponent<Camera>();
		camera.orthographicSize = y;
	}

	private void LateUpdate() {
		CorrectPosition();
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if (material == null) {
			if (PostProcessShader == null) {
				enabled = false;
				Graphics.Blit(src, dest);
				return;
			}

			material = new Material(PostProcessShader);
		}

		material.SetFloat("_Power", Gamma / 2.2f);
		Graphics.Blit(src, dest, material);
	}
}