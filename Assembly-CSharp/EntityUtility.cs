using Engine.Common;
using Engine.Common.Components;
using Engine.Impl.Services.HierarchyServices;
using Engine.Source.Commons;
using System;
using System.Collections.Generic;
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
    if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
    {
      EngineGameObject engineGameObject = gameObject.GetComponentNonAlloc<EngineGameObject>();
      if ((UnityEngine.Object) engineGameObject == (UnityEngine.Object) null && (UnityEngine.Object) gameObject.GetComponentNonAlloc<EntityUtilityBinder>() != (UnityEngine.Object) null)
        engineGameObject = gameObject.GetComponentInParent<EngineGameObject>();
      if ((UnityEngine.Object) engineGameObject != (UnityEngine.Object) null)
        return engineGameObject.Owner;
    }
    return (IEntity) null;
  }

  public static IEntity FindChildByName(IEntity entity, string name)
  {
    if (entity == null || entity.Childs == null)
      return (IEntity) null;
    foreach (IEntity child in entity.Childs)
    {
      if (child.Name == name)
        return child;
      IEntity childByName = EntityUtility.FindChildByName(child, name);
      if (childByName != null)
        return childByName;
    }
    return (IEntity) null;
  }

  public static IEntity GetEntityByTemplate(IEntity entity, Guid templateId)
  {
    if (entity.TemplateId == templateId)
      return entity;
    if (entity.Childs == null)
      return (IEntity) null;
    foreach (IEntity child in entity.Childs)
    {
      HierarchyItem hierarchyItem = ((IEntityHierarchy) child).HierarchyItem;
      if (hierarchyItem == null)
      {
        if (child.GetComponent<IInventoryComponent>() == null)
          Debug.LogError((object) ("Hierarchy item not found, child : " + child.GetInfo() + " , parent : " + entity.GetInfo()));
      }
      else if (hierarchyItem.Container == null)
      {
        IEntity entityByTemplate = EntityUtility.GetEntityByTemplate(child, templateId);
        if (entityByTemplate != null)
          return entityByTemplate;
      }
    }
    return (IEntity) null;
  }
}
