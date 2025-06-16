using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Impl.Services.HierarchyServices;
using Engine.Source.Commons;
using UnityEngine;

public static class EntityUtility
{
  public static T GetComponent<T>(this IComponent component) where T : class, IComponent
  {
    return component.Owner.GetComponent<T>();
  }

  public static IEnumerable<T> GetComponents<T>(this IEntity entity) where T : class, IComponent
  {
    foreach (IComponent component in entity.Components)
    {
      if (component is T result)
        yield return result;
      result = default (T);
    }
  }

  public static IEntity GetEntity(GameObject gameObject)
  {
    if (gameObject != null)
    {
      EngineGameObject engineGameObject = gameObject.GetComponentNonAlloc<EngineGameObject>();
      if (engineGameObject == null && gameObject.GetComponentNonAlloc<EntityUtilityBinder>() != null)
        engineGameObject = gameObject.GetComponentInParent<EngineGameObject>();
      if (engineGameObject != null)
        return engineGameObject.Owner;
    }
    return null;
  }

  public static IEntity FindChildByName(IEntity entity, string name)
  {
    if (entity == null || entity.Childs == null)
      return null;
    foreach (IEntity child in entity.Childs)
    {
      if (child.Name == name)
        return child;
      IEntity childByName = FindChildByName(child, name);
      if (childByName != null)
        return childByName;
    }
    return null;
  }

  public static IEntity GetEntityByTemplate(IEntity entity, Guid templateId)
  {
    if (entity.TemplateId == templateId)
      return entity;
    if (entity.Childs == null)
      return null;
    foreach (IEntity child in entity.Childs)
    {
      HierarchyItem hierarchyItem = ((IEntityHierarchy) child).HierarchyItem;
      if (hierarchyItem == null)
      {
        if (child.GetComponent<IInventoryComponent>() == null)
          Debug.LogError("Hierarchy item not found, child : " + child.GetInfo() + " , parent : " + entity.GetInfo());
      }
      else if (hierarchyItem.Container == null)
      {
        IEntity entityByTemplate = GetEntityByTemplate(child, templateId);
        if (entityByTemplate != null)
          return entityByTemplate;
      }
    }
    return null;
  }
}
