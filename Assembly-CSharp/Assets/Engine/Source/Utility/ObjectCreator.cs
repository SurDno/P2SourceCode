using UnityEngine;

namespace Assets.Engine.Source.Utility;

public static class ObjectCreator {
	public static T InstantiateFromResources<T>(string path, Transform parent = null) where T : Object {
		return Object.Instantiate((Object)Resources.Load<T>(path), parent) as T;
	}
}