using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
internal class PostEffectsHelper : MonoBehaviour {
	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		Debug.Log("OnRenderImage in Helper called ...");
	}

	private static void DrawLowLevelPlaneAlignedWithCamera(
		float dist,
		RenderTexture source,
		RenderTexture dest,
		Material material,
		Camera cameraForProjectionMatrix) {
		RenderTexture.active = dest;
		material.SetTexture("_MainTex", source);
		var flag = true;
		GL.PushMatrix();
		GL.LoadIdentity();
		GL.LoadProjectionMatrix(cameraForProjectionMatrix.projectionMatrix);
		var f = (float)(cameraForProjectionMatrix.fieldOfView * 0.5 * (Math.PI / 180.0));
		var num1 = Mathf.Cos(f) / Mathf.Sin(f);
		var aspect = cameraForProjectionMatrix.aspect;
		var num2 = aspect / -num1;
		var num3 = aspect / num1;
		var num4 = (float)(1.0 / -(double)num1);
		var num5 = 1f / num1;
		var num6 = 1f;
		var x1 = num2 * (dist * num6);
		var x2 = num3 * (dist * num6);
		var y1 = num4 * (dist * num6);
		var y2 = num5 * (dist * num6);
		var z = -dist;
		for (var pass = 0; pass < material.passCount; ++pass) {
			material.SetPass(pass);
			GL.Begin(7);
			float y3;
			float y4;
			if (flag) {
				y3 = 1f;
				y4 = 0.0f;
			} else {
				y3 = 0.0f;
				y4 = 1f;
			}

			GL.TexCoord2(0.0f, y3);
			GL.Vertex3(x1, y1, z);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y1, z);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, z);
			GL.TexCoord2(0.0f, y4);
			GL.Vertex3(x1, y2, z);
			GL.End();
		}

		GL.PopMatrix();
	}

	private static void DrawBorder(RenderTexture dest, Material material) {
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

	private static void DrawLowLevelQuad(
		float x1,
		float x2,
		float y1,
		float y2,
		RenderTexture source,
		RenderTexture dest,
		Material material) {
		RenderTexture.active = dest;
		material.SetTexture("_MainTex", source);
		var flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (var pass = 0; pass < material.passCount; ++pass) {
			material.SetPass(pass);
			GL.Begin(7);
			float y3;
			float y4;
			if (flag) {
				y3 = 1f;
				y4 = 0.0f;
			} else {
				y3 = 0.0f;
				y4 = 1f;
			}

			GL.TexCoord2(0.0f, y3);
			GL.Vertex3(x1, y1, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y1, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2(0.0f, y4);
			GL.Vertex3(x1, y2, 0.1f);
			GL.End();
		}

		GL.PopMatrix();
	}
}