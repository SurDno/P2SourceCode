using System;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public abstract class TOD_ImageEffect : MonoBehaviour {
	public TOD_Sky sky;
	protected Camera cam;

	protected Material CreateMaterial(Shader shader) {
		if (!(bool)(Object)shader) {
			Debug.Log("Missing shader in " + ToString());
			enabled = false;
			return null;
		}

		if (!shader.isSupported) {
			Debug.LogError("The shader " + shader + " on effect " + ToString() + " is not supported on this platform!");
			enabled = false;
			return null;
		}

		var material = new Material(shader);
		material.hideFlags = HideFlags.DontSave;
		return material;
	}

	protected void Awake() {
		if (!(bool)(Object)cam)
			cam = GetComponent<Camera>();
		if ((bool)(Object)sky)
			return;
		sky = FindObjectOfType(typeof(TOD_Sky)) as TOD_Sky;
	}

	protected bool CheckSupport(bool needDepth = false, bool needHdr = false) {
		if (!(bool)(Object)cam)
			cam = GetComponent<Camera>();
		if (!(bool)(Object)cam)
			return false;
		if (!(bool)(Object)sky)
			sky = FindObjectOfType(typeof(TOD_Sky)) as TOD_Sky;
		if (!(bool)(Object)sky || !sky.Initialized)
			return false;
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures) {
			Debug.LogWarning("The image effect " + ToString() +
			                 " has been disabled as it's not supported on the current platform.");
			enabled = false;
			return false;
		}

		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth)) {
			Debug.LogWarning("The image effect " + ToString() + " has been disabled as it requires a depth texture.");
			enabled = false;
			return false;
		}

		if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)) {
			Debug.LogWarning("The image effect " + ToString() + " has been disabled as it requires HDR.");
			enabled = false;
			return false;
		}

		if (needDepth)
			cam.depthTextureMode |= DepthTextureMode.Depth;
		if (needHdr)
			cam.allowHDR = true;
		return true;
	}

	protected void DrawBorder(RenderTexture dest, Material material) {
		RenderTexture.active = dest;
		var flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (var pass = 0; pass < material.passCount; ++pass) {
			material.SetPass(pass);
			float y1;
			float y2;
			if (flag) {
				y1 = 1f;
				y2 = 0.0f;
			} else {
				y1 = 0.0f;
				y2 = 1f;
			}

			var x1 = 0.0f;
			var x2 = (float)(0.0 + 1.0 / (dest.width * 1.0));
			var y3 = 0.0f;
			var y4 = 1f;
			GL.Begin(7);
			GL.TexCoord2(0.0f, y1);
			GL.Vertex3(x1, y3, 0.1f);
			GL.TexCoord2(1f, y1);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0.0f, y2);
			GL.Vertex3(x1, y4, 0.1f);
			var x3 = (float)(1.0 - 1.0 / (dest.width * 1.0));
			var x4 = 1f;
			var y5 = 0.0f;
			var y6 = 1f;
			GL.TexCoord2(0.0f, y1);
			GL.Vertex3(x3, y5, 0.1f);
			GL.TexCoord2(1f, y1);
			GL.Vertex3(x4, y5, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x4, y6, 0.1f);
			GL.TexCoord2(0.0f, y2);
			GL.Vertex3(x3, y6, 0.1f);
			var x5 = 0.0f;
			var x6 = 1f;
			var y7 = 0.0f;
			var y8 = (float)(0.0 + 1.0 / (dest.height * 1.0));
			GL.TexCoord2(0.0f, y1);
			GL.Vertex3(x5, y7, 0.1f);
			GL.TexCoord2(1f, y1);
			GL.Vertex3(x6, y7, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x6, y8, 0.1f);
			GL.TexCoord2(0.0f, y2);
			GL.Vertex3(x5, y8, 0.1f);
			var x7 = 0.0f;
			var x8 = 1f;
			var y9 = (float)(1.0 - 1.0 / (dest.height * 1.0));
			var y10 = 1f;
			GL.TexCoord2(0.0f, y1);
			GL.Vertex3(x7, y9, 0.1f);
			GL.TexCoord2(1f, y1);
			GL.Vertex3(x8, y9, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x8, y10, 0.1f);
			GL.TexCoord2(0.0f, y2);
			GL.Vertex3(x7, y10, 0.1f);
			GL.End();
		}

		GL.PopMatrix();
	}

	protected void CustomBlit(
		RenderTexture source,
		RenderTexture dest,
		Material fxMaterial,
		int passNr = 0) {
		RenderTexture.active = dest;
		fxMaterial.SetTexture("_MainTex", source);
		GL.PushMatrix();
		GL.LoadOrtho();
		fxMaterial.SetPass(passNr);
		GL.Begin(7);
		GL.MultiTexCoord2(0, 0.0f, 0.0f);
		GL.Vertex3(0.0f, 0.0f, 3f);
		GL.MultiTexCoord2(0, 1f, 0.0f);
		GL.Vertex3(1f, 0.0f, 2f);
		GL.MultiTexCoord2(0, 1f, 1f);
		GL.Vertex3(1f, 1f, 1f);
		GL.MultiTexCoord2(0, 0.0f, 1f);
		GL.Vertex3(0.0f, 1f, 0.0f);
		GL.End();
		GL.PopMatrix();
	}

	protected Matrix4x4 FrustumCorners() {
		var nearClipPlane = cam.nearClipPlane;
		var farClipPlane = cam.farClipPlane;
		var fieldOfView = cam.fieldOfView;
		var aspect = cam.aspect;
		var forward = cam.transform.forward;
		var right = cam.transform.right;
		var up = cam.transform.up;
		var identity = Matrix4x4.identity;
		var num1 = fieldOfView * 0.5f;
		var vector3_1 = right * nearClipPlane * Mathf.Tan(num1 * ((float)Math.PI / 180f)) * aspect;
		var vector3_2 = up * nearClipPlane * Mathf.Tan(num1 * ((float)Math.PI / 180f));
		var vector3_3 = forward * nearClipPlane - vector3_1 + vector3_2;
		var num2 = vector3_3.magnitude * farClipPlane / nearClipPlane;
		vector3_3.Normalize();
		var row1 = vector3_3 * num2;
		var vector3_4 = forward * nearClipPlane + vector3_1 + vector3_2;
		vector3_4.Normalize();
		var row2 = vector3_4 * num2;
		var vector3_5 = forward * nearClipPlane + vector3_1 - vector3_2;
		vector3_5.Normalize();
		var row3 = vector3_5 * num2;
		var vector3_6 = forward * nearClipPlane - vector3_1 - vector3_2;
		vector3_6.Normalize();
		var row4 = vector3_6 * num2;
		identity.SetRow(0, row1);
		identity.SetRow(1, row2);
		identity.SetRow(2, row3);
		identity.SetRow(3, row4);
		return identity;
	}
}