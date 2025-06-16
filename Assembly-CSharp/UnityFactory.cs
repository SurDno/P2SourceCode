using System.Collections.Generic;
using Cofe.Utility;
using UnityEngine;

public static class UnityFactory {
	private static Dictionary<string, GameObject> groups = new();

	private static GameObject CreateGroup(string group) {
		var group1 = new GameObject(group);
		group1.AddComponent<EngineGameObjectContainer>();
		return group1;
	}

	public static GameObject GetOrCreateGroup(string group) {
		GameObject group1;
		if (!groups.TryGetValue(group, out group1)) {
			group1 = CreateGroup(group);
			groups.Add(group, group1);
		}

		return group1;
	}

	public static GameObject Create(string group, string name) {
		if (group.IsNullOrEmpty())
			return new GameObject(name);
		var group1 = GetOrCreateGroup(group);
		var gameObject = new GameObject(name);
		gameObject.transform.SetParent(group1.transform);
		return gameObject;
	}

	public static T Instantiate<T>(GameObject prefab, string group) where T : MonoBehaviour {
		return InstantiateObject(prefab, group).GetComponent<T>();
	}

	public static GameObject Instantiate(GameObject prefab, string group) {
		return InstantiateObject(prefab, group);
	}

	public static T Instantiate<T>(T prefab, string group) where T : MonoBehaviour {
		return InstantiateObject(prefab, group);
	}

	private static T InstantiateObject<T>(T prefab, string group) where T : Object {
		if (!Application.isEditor || group.IsNullOrEmpty())
			return Object.Instantiate(prefab);
		var group1 = GetOrCreateGroup(group);
		return Object.Instantiate(prefab, group1.transform);
	}

	public static void Destroy(GameObject go) {
		Object.Destroy(go);
	}

	public static void Destroy(MonoBehaviour mono) {
		Object.Destroy(mono.gameObject);
	}
}