using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Impl.Services.HierarchyServices;
using Engine.Source.Commons;
using UnityEngine;

public static class EntityUtility {
	public static T GetComponent<T>(this IComponent component) where T : class, IComponent {
		return component.Owner.GetComponent<T>();
	}

	public static IEnumerable<T> GetComponents<T>(this IEntity entity) where T : class, IComponent {
		foreach (var component in entity.Components) {
			if (component is T result)
				yield return result;
			result = default;
		}
	}

	public static IEntity GetEntity(GameObject gameObject) {
		if (gameObject != null) {
			var engineGameObject = gameObject.GetComponentNonAlloc<EngineGameObject>();
			if (engineGameObject == null && gameObject.GetComponentNonAlloc<EntityUtilityBinder>() != null)
				engineGameObject = gameObject.GetComponentInParent<EngineGameObject>();
			if (engineGameObject != null)
				return engineGameObject.Owner;
		}

		return null;
	}

	public static IEntity FindChildByName(IEntity entity, string name) {
		if (entity == null || entity.Childs == null)
			return null;
		foreach (var child in entity.Childs) {
			if (child.Name == name)
				return child;
			var childByName = FindChildByName(child, name);
			if (childByName != null)
				return childByName;
		}

		return null;
	}

	public static IEntity GetEntityByTemplate(IEntity entity, Guid templateId) {
		if (entity.TemplateId == templateId)
			return entity;
		if (entity.Childs == null)
			return null;
		foreach (var child in entity.Childs) {
			var hierarchyItem = ((IEntityHierarchy)child).HierarchyItem;
			if (hierarchyItem == null) {
				if (child.GetComponent<IInventoryComponent>() == null)
					Debug.LogError("Hierarchy item not found, child : " + child.GetInfo() + " , parent : " +
					               entity.GetInfo());
			} else if (hierarchyItem.Container == null) {
				var entityByTemplate = GetEntityByTemplate(child, templateId);
				if (entityByTemplate != null)
					return entityByTemplate;
			}
		}

		return null;
	}
}