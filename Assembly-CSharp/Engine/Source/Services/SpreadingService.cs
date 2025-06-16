using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Saves;
using Engine.Source.Services.Saves;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services
{
  [GameService(typeof (SpreadingService), typeof (ISpreadingService))]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class SpreadingService : ISpreadingService, ISavesController
  {
    [StateSaveProxy(MemberEnum.CustomListReference)]
    [StateLoadProxy(MemberEnum.CustomListReference)]
    [Inspected]
    protected List<SpreadingComponent> spreadingComponents = new List<SpreadingComponent>();
    [StateSaveProxy(MemberEnum.CustomListReference)]
    [StateLoadProxy(MemberEnum.CustomListReference)]
    [Inspected]
    protected List<IRegionComponent> regionComponents = new List<IRegionComponent>();

    public event Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> OnFurnitureLoadedOnce;

    public event Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> OnFurnitureLoaded;

    public event Action<IEntity> OnRegionLoadedOnce;

    public event Action<IEntity> OnRegionLoaded;

    public void Reset() => Clear();

    public void AddRegion(IRegionComponent regionComponent)
    {
      ILocationComponent component = regionComponent.Owner.GetComponent<ILocationComponent>();
      if (component == null)
        return;
      component.OnHibernationChanged += RegionComponent_Location_OnHibernationChanged;
    }

    public void RemoveRegion(IRegionComponent regionComponent)
    {
      ILocationComponent component = regionComponent.Owner.GetComponent<ILocationComponent>();
      if (component == null)
        return;
      component.OnHibernationChanged -= RegionComponent_Location_OnHibernationChanged;
    }

    private void RegionComponent_Location_OnHibernationChanged(ILocationComponent sender)
    {
      if (sender.IsHibernation)
        return;
      IRegionComponent component = sender.GetComponent<IRegionComponent>();
      Action<IEntity> onRegionLoaded = OnRegionLoaded;
      if (onRegionLoaded != null)
        onRegionLoaded(component.Owner);
      if (regionComponents.Contains(component))
        return;
      regionComponents.Add(component);
      Action<IEntity> regionLoadedOnce = OnRegionLoadedOnce;
      if (regionLoadedOnce != null)
        regionLoadedOnce(component.Owner);
      Debug.Log(ObjectInfoUtility.GetStream().Append("Spreading items, region : ").GetInfo(component.Owner));
    }

    public void AddSpreading(SpreadingComponent spreadingComponent)
    {
      ILocationItemComponent component = spreadingComponent.GetComponent<ILocationItemComponent>();
      if (component == null)
        return;
      component.OnHibernationChanged += SpreadingComponent_LocationItem_OnHibernationChanged;
    }

    public void RemoveSpreading(SpreadingComponent spreadingComponent)
    {
      ILocationItemComponent component = spreadingComponent.GetComponent<ILocationItemComponent>();
      if (component == null)
        return;
      component.OnHibernationChanged -= SpreadingComponent_LocationItem_OnHibernationChanged;
    }

    private void SpreadingComponent_LocationItem_OnHibernationChanged(ILocationItemComponent sender)
    {
      if (sender.IsHibernation)
        return;
      SpreadingComponent component = sender.GetComponent<SpreadingComponent>();
      IEntity owner = LocationItemUtility.FindParentComponent<IRegionComponent>(component.Owner)?.Owner;
      IEntity parent = ((IEntityHierarchy) component.Owner).Parent;
      IBuildingComponent parentComponent = LocationItemUtility.FindParentComponent<IBuildingComponent>(component.Owner);
      BuildingEnum buildingEnum = parentComponent != null ? parentComponent.Building : BuildingEnum.None;
      DiseasedStateEnum diseasedState = component.DiseasedState;
      Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> onFurnitureLoaded = OnFurnitureLoaded;
      if (onFurnitureLoaded != null)
        onFurnitureLoaded(parent, owner, buildingEnum, diseasedState);
      if (spreadingComponents.Contains(component))
        return;
      spreadingComponents.Add(component);
      Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> furnitureLoadedOnce = OnFurnitureLoadedOnce;
      if (furnitureLoadedOnce != null)
        furnitureLoadedOnce(parent, owner, buildingEnum, diseasedState);
      Debug.Log(ObjectInfoUtility.GetStream().Append("Spreading items, entity : ").GetInfo(parent).Append(" , region : ").Append(owner.Name).Append(" ,  building : ").Append(buildingEnum).Append(" , diseased : ").Append(diseasedState));
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      XmlElement node = element[TypeUtility.GetTypeName(GetType())];
      if (node == null)
      {
        errorHandler.LogError(TypeUtility.GetTypeName(GetType()) + " node not found , context : " + context);
      }
      else
      {
        XmlNodeDataReader reader = new XmlNodeDataReader(node, context);
        ((ISerializeStateLoad) this).StateLoad(reader, GetType());
        yield break;
      }
    }

    public void Unload() => Clear();

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize(writer, TypeUtility.GetTypeName(GetType()), this);
    }

    private void Clear()
    {
      spreadingComponents.Clear();
      regionComponents.Clear();
    }
  }
}
