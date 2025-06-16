using UnityEngine;

namespace UnityStandardAssets.ImageEffects;

internal class Triangles {
	private static Mesh[] meshes;
	private static int currentTris;

	private static bool HasMeshes() {
		if (meshes == null)
			return false;
		for (var index = 0; index < meshes.Length; ++index)
			if (null == meshes[index])
				return false;
		return true;
	}

	private static void Cleanup() {
		if (meshes == null)
			return;
		for (var index = 0; index < meshes.Length; ++index)
			if (null != meshes[index]) {
				Object.DestroyImmediate(meshes[index]);
				meshes[index] = null;
			}

		meshes = null;
	}

	private static Mesh[] GetMeshes(int totalWidth, int totalHeight) {
		if (HasMeshes() && currentTris == totalWidth * totalHeight)
			return meshes;
		var max = 21666;
		var num = totalWidth * totalHeight;
		currentTris = num;
		meshes = new Mesh[Mathf.CeilToInt((float)(1.0 * num / (1.0 * max)))];
		var index = 0;
		for (var triOffset = 0; triOffset < num; triOffset += max) {
			var triCount = Mathf.FloorToInt(Mathf.Clamp(num - triOffset, 0, max));
			meshes[index] = GetMesh(triCount, triOffset, totalWidth, totalHeight);
			++index;
		}

		return meshes;
	}

	private static Mesh GetMesh(int triCount, int triOffset, int totalWidth, int totalHeight) {
		var mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSave;
		var vector3Array = new Vector3[triCount * 3];
		var vector2Array1 = new Vector2[triCount * 3];
		var vector2Array2 = new Vector2[triCount * 3];
		var numArray = new int[triCount * 3];
		for (var index1 = 0; index1 < triCount; ++index1) {
			var index2 = index1 * 3;
			var num = triOffset + index1;
			var x = Mathf.Floor(num % totalWidth) / totalWidth;
			var y = Mathf.Floor(num / totalWidth) / totalHeight;
			var vector3 = new Vector3((float)(x * 2.0 - 1.0), (float)(y * 2.0 - 1.0), 1f);
			vector3Array[index2] = vector3;
			vector3Array[index2 + 1] = vector3;
			vector3Array[index2 + 2] = vector3;
			vector2Array1[index2] = new Vector2(0.0f, 0.0f);
			vector2Array1[index2 + 1] = new Vector2(1f, 0.0f);
			vector2Array1[index2 + 2] = new Vector2(0.0f, 1f);
			vector2Array2[index2] = new Vector2(x, y);
			vector2Array2[index2 + 1] = new Vector2(x, y);
			vector2Array2[index2 + 2] = new Vector2(x, y);
			numArray[index2] = index2;
			numArray[index2 + 1] = index2 + 1;
			numArray[index2 + 2] = index2 + 2;
		}

		mesh.vertices = vector3Array;
		mesh.triangles = numArray;
		mesh.uv = vector2Array1;
		mesh.uv2 = vector2Array2;
		return mesh;
	}
}