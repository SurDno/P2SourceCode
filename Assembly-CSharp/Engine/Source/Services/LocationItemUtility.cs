using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Locations;
using Engine.Source.Components;

namespace Engine.Source.Services
{
  public static class LocationItemUtility
  {
    public static bool CheckLocation(ILocationItemComponent target, ILocationItemComponent owner)
    {
      ILocationComponent logicLocation1 = target.LogicLocation;
      if (logicLocation1 == null)
        return false;
      ILocationComponent logicLocation2 = owner.LogicLocation;
      return logicLocation2 != null && ((LocationComponent) logicLocation2).LocationType == ((LocationComponent) logicLocation1).LocationType && (((LocationComponent) logicLocation2).LocationType != LocationType.Indoor || logicLocation2 == logicLocation1);
    }

    public static LocationType GetLocationType(GameObject go)
    {
      for (IEntity entity = GetFirstEngineObject(go.transform); entity != null; entity = entity.Parent)
      {
        LocationComponent component = entity.GetComponent<LocationComponent>();
        if (component != null)
        {
          ILocationComponent logicLocation = component.LogicLocation;
          if (logicLocation != null)
            return ((LocationComponent) logicLocation).LocationType;
        }
      }
      return LocationType.None;
    }

    public static IEntity GetFirstEngineObject(Transform trans)
    {
      do
      {
        EngineGameObject componentNonAlloc = trans.GetComponentNonAlloc<EngineGameObject>();
        if ((Object) componentNonAlloc != (Object) null)
          return componentNonAlloc.Owner;
        trans = trans.parent;
      }
      while ((Object) trans != (Object) null);
      return null;
    }

    public static T FindParentComponent<T>(IEntity entity) where T : class, IComponent
    {
      for (; entity != null; entity = entity.Parent)
      {
        T component = entity.GetComponent<T>();
        if (component != null)
          return component;
      }
      return default (T);
    }

    public static ILocationComponent GetLocation(IEntity entity)
    {
      LocationItemComponent parentComponent = FindParentComponent<LocationItemComponent>(entity);
      return parentComponent != null ? parentComponent.Location : FindParentComponent<LocationComponent>(entity);
    }
  }
}
