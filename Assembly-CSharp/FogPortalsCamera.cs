using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FogPortalsCamera : MonoBehaviour {
	[SerializeField] private Camera baseCamera;
	public bool HalfResolution;
	private RenderTexture rt;

	private void DestroyTexture() {
		if (rt == null)
			return;
		rt.Release();
		Destroy(rt);
		rt = null;
		GetComponent<Camera>().targetTexture = null;
		Shader.SetGlobalInt("_IndoorDepthEnabled", 0);
		Shader.SetGlobalTexture("_IndoorDepthTexture", null);
	}

	private void OnDisable() {
		DestroyTexture();
	}

	private void OnPreCull() {
		var component = GetComponent<Camera>();
		var width = HalfResolution ? component.pixelWidth / 2 : component.pixelWidth;
		var height = HalfResolution ? component.pixelHeight / 2 : component.pixelHeight;
		if (rt != null && (rt.width != width || rt.height != height))
			DestroyTexture();
		if (rt == null) {
			rt = new RenderTexture(width, height, 24, RenderTextureFormat.Depth);
			component.targetTexture = rt;
			Shader.SetGlobalInt("_IndoorDepthEnabled", 1);
			Shader.SetGlobalTexture("_IndoorDepthTexture", rt);
		}

		component.nearClipPlane = baseCamera.nearClipPlane;
		component.farClipPlane = baseCamera.farClipPlane;
		component.fieldOfView = baseCamera.fieldOfView;
	}
}