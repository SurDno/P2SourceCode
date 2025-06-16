using System.Collections.Generic;
using UnityEngine;

public static class RendererUtility {
	private static List<Renderer> searchBuffer;

	public static Renderer GetBiggestRenderer(GameObject gameObject) {
		if (gameObject == null)
			return null;
		if (searchBuffer == null)
			searchBuffer = new List<Renderer>();
		gameObject.GetComponentsInChildren(searchBuffer);
		var num1 = 0.0f;
		Renderer biggestRenderer = null;
		for (var index = 0; index < searchBuffer.Count; ++index) {
			var renderer = searchBuffer[index];
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer) {
				var extents = renderer.bounds.extents;
				var num2 = extents.x * extents.y * extents.z;
				if (num2 > (double)num1) {
					num1 = num2;
					biggestRenderer = renderer;
				}
			}
		}

		searchBuffer.Clear();
		return biggestRenderer;
	}
}