using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inspectors;

public class UnityHierarchy {
	private static UnityHierarchy instance = new();

	public static UnityHierarchy Instance => instance;

	[Inspected]
	public IEnumerable<SceneInfo> Scenes {
		get {
			var count = SceneManager.sceneCount;
			for (var index = 0; index < count; ++index)
				yield return new SceneInfo(SceneManager.GetSceneAt(index));
		}
	}

	public class SceneInfo {
		private Scene scene;

		public SceneInfo(Scene scene) {
			this.scene = scene;
		}

		[Inspected(Header = true)] private string Name => scene.IsValid() ? scene.name : "null";

		[Inspected]
		private IEnumerable<GameObjectInfo> Childs {
			get {
				if (scene.IsValid()) {
					var roots = scene.GetRootGameObjects();
					for (var index = 0; index < roots.Length; ++index)
						yield return new GameObjectInfo(roots[index]);
					roots = null;
				}
			}
		}
	}

	public class TransformInfo {
		private Transform transform;

		public TransformInfo(Transform transform) {
			this.transform = transform;
		}

		[Inspected] private Vector3 Position => transform != null ? transform.localPosition : Vector3.zero;

		[Inspected] private Quaternion Rotation => transform != null ? transform.localRotation : Quaternion.identity;

		[Inspected] private Vector3 Scale => transform != null ? transform.localScale : Vector3.one;
	}

	public class GameObjectInfo {
		private GameObject gameObject;

		public GameObjectInfo(GameObject gameObject) {
			this.gameObject = gameObject;
		}

		[Inspected(Header = true)] private string Name => gameObject != null ? gameObject.name : "";

		[Inspected(Mutable = true)]
		public bool Enable {
			get => gameObject != null && gameObject.activeSelf;
			set {
				if (!(gameObject != null))
					return;
				gameObject.SetActive(value);
			}
		}

		[Inspected] public string Tag => gameObject != null ? gameObject.tag : "";

		[Inspected] public string Layer => gameObject != null ? LayerMask.LayerToName(gameObject.layer) : "";

		[Inspected]
		private IEnumerable<object> Components {
			get {
				if (gameObject != null) {
					var components = gameObject.GetComponents<Component>();
					var componentArray = components;
					for (var index = 0; index < componentArray.Length; ++index) {
						var component = componentArray[index];
						var transform = component as Transform;
						if (transform != null)
							yield return new TransformInfo(transform);
						else
							yield return component;
						transform = null;
						component = null;
					}

					componentArray = null;
					components = null;
				}
			}
		}

		[Inspected]
		private IEnumerable<GameObjectInfo> Childs {
			get {
				if (gameObject != null) {
					var count = gameObject.transform.childCount;
					for (var index = 0; index < count; ++index)
						yield return new GameObjectInfo(gameObject.transform.GetChild(index).gameObject);
				}
			}
		}
	}
}