using System;
using System.Collections.Generic;
using Cofe.Serializations.Converters;
using Inspectors;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Engine.Behaviours;

public class SceneObjectContainer : MonoBehaviour {
	[SerializeField] [HideInInspector] private string id;
	[SerializeField] [HideInInspector] private List<Element> elements = new();

	public GameObject GetGameObject(Guid id) {
		var str = DefaultConverter.ToString(id);
		foreach (var element in elements)
			if (element.Item != null && element.Id == str)
				return element.Item.gameObject;
		return null;
	}

	public Guid GetId(GameObject go) {
		if (go != null)
			foreach (var element in elements)
				if (element.Item != null && element.Item.gameObject == go)
					return DefaultConverter.ParseGuid(element.Id);
		return Guid.Empty;
	}

	public static SceneObjectContainer GetContainer(Scene scene) {
		foreach (var rootGameObject in scene.GetRootGameObjects()) {
			var component = rootGameObject.GetComponent<SceneObjectContainer>();
			if (component != null)
				return component;
		}

		return null;
	}

	[Serializable]
	public struct Element {
		[Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime, Mutable = false)]
		public string Id;

		[Inspected(Mode = ExecuteMode.EditAndRuntime, Mutable = false)]
		public EngineGameObject Item;
	}
}