using System;
using System.Collections.Generic;
using Engine.Behaviours;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.HierarchyServices;
using Engine.Source.Commons;
using UnityEngine;

public class FirstSceneLoader : MonoBehaviour {
	private void Start() {
		InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent += OnViewEnabledEvent;
	}

	private void OnDestroy() {
		InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent -= OnViewEnabledEvent;
	}

	private void OnViewEnabledEvent(bool enabled) {
		var container = SceneObjectContainer.GetContainer(gameObject.scene);
		if (container == null)
			Debug.LogWarning(
				typeof(SceneObjectContainer).Name + " not found, path : " + gameObject.scene.path + " , root count : " +
				gameObject.scene.GetRootGameObjects().Length, this);
		else {
			var id = container.GetId(gameObject);
			if (id == Guid.Empty)
				Debug.LogWarning("GameObject not found", this);
			else {
				IEntity entity1 = null;
				foreach (var hierarchyItem in ServiceLocator.GetService<HierarchyService>().MainContainer.Items)
					if (hierarchyItem.Reference != null && hierarchyItem.Reference.Id == id) {
						entity1 = hierarchyItem.Template;
						break;
					}

				if (entity1 == null)
					Debug.LogWarning("Template not found", this);
				else {
					IEntity entity2 = null;
					var childs = ServiceLocator.GetService<ISimulation>().Hierarchy.Childs;
					if (childs != null)
						foreach (var entity3 in childs)
							if (entity3.TemplateId == entity1.Id) {
								entity2 = entity3;
								break;
							}

					if (entity2 == null)
						Debug.LogWarning("Entity not found", this);
					else
						((IEntityViewSetter)entity2).GameObject = enabled ? gameObject : null;
				}
			}
		}
	}
}