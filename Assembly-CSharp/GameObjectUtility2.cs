using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class GameObjectUtility2 {
	private static List<Component> tmp = new();

	public static T FindObjectOfType<T>() where T : Object {
		for (var index = 0; index < SceneManager.sceneCount; ++index) {
			var sceneAt = SceneManager.GetSceneAt(index);
			if (sceneAt.isLoaded)
				foreach (var rootGameObject in sceneAt.GetRootGameObjects()) {
					var componentInChildren = rootGameObject.GetComponentInChildren<T>(true);
					if (componentInChildren != null)
						return componentInChildren;
				}
		}

		return default;
	}

	public static T GetComponentNonAlloc<T>(this Component component) where T : Component {
		return component.gameObject.GetComponentNonAlloc<T>();
	}

	public static T GetComponentNonAlloc<T>(this GameObject go) where T : Component {
		tmp.Clear();
		go.GetComponents(typeof(T), tmp);
		if (tmp.Count == 0)
			return default;
		var componentNonAlloc = (T)tmp[0];
		tmp.Clear();
		return componentNonAlloc;
	}

	public static GameObject GetByPath(string path) {
		var paths = path.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
		if (paths.Length < 2)
			return null;
		var sceneByName = SceneManager.GetSceneByName(paths[0]);
		return !sceneByName.IsValid() ? null : GetChildGameObject(sceneByName.GetRootGameObjects(), paths, 1);
	}

	private static GameObject GetChildGameObject(
		IEnumerable<GameObject> gos,
		string[] paths,
		int pathIndex) {
		var path = paths[pathIndex];
		foreach (var go in gos)
			if (go.name == path)
				return pathIndex == paths.Length - 1 ? go : GetChildGameObject(go.GetChilds(), paths, pathIndex + 1);
		return null;
	}

	public static IEnumerable<GameObject> GetChilds(this GameObject go) {
		if (go != null)
			for (var index = 0; index < go.transform.childCount; ++index) {
				var child = go.transform.GetChild(index);
				yield return child.gameObject;
				child = null;
			}
	}

	public static IEnumerable<GameObject> GetAllGameObjects(GameObject go) {
		yield return go;
		foreach (var child2 in GetAllChildren(go))
			yield return child2;
	}

	private static IEnumerable<GameObject> GetAllChildren(GameObject go) {
		for (var index = 0; index < go.transform.childCount; ++index) {
			var child = go.transform.GetChild(index).gameObject;
			yield return child;
			foreach (var child2 in GetAllChildren(child))
				yield return child2;
			child = null;
		}
	}
}