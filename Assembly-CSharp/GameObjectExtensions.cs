using UnityEngine;

public static class GameObjectExtensions {
	public static GameObject GetSceneInstance(this GameObject @object) {
		return @object == null ? null : @object;
	}
}