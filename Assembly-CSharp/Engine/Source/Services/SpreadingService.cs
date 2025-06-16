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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Engine.Source.Services
{
  [GameService(new System.Type[] {typeof (SpreadingService), typeof (ISpreadingService)})]
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

    public void Reset() => this.Clear();

    public void AddRegion(IRegionComponent regionComponent)
    {
      ILocationComponent component = regionComponent.Owner.GetComponent<ILocationComponent>();
      if (component == null)
        return;
      component.OnHibernationChanged += new Action<ILocationComponent>(this.RegionComponent_Location_OnHibernationChanged);
    }

    public void RemoveRegion(IRegionComponent regionComponent)
    {
      ILocationComponent component = regionComponent.Owner.GetComponent<ILocationComponent>();
      if (component == null)
        return;
      component.OnHibernationChanged -= new Action<ILocationComponent>(this.RegionComponent_Location_OnHibernationChanged);
    }

    private void RegionComponent_Location_OnHibernationChanged(ILocationComponent sender)
    {
      if (sender.IsHibernation)
        return;
      IRegionComponent component = sender.GetComponent<IRegionComponent>();
      Action<IEntity> onRegionLoaded = this.OnRegionLoaded;
      if (onRegionLoaded != null)
        onRegionLoaded(component.Owner);
      if (this.regionComponents.Contains(component))
        return;
      this.regionComponents.Add(component);
      Action<IEntity> regionLoadedOnce = this.OnRegionLoadedOnce;
      if (regionLoadedOnce != null)
        regionLoadedOnce(component.Owner);
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Spreading items, region : ").GetInfo((object) component.Owner));
    }

    public void AddSpreading(SpreadingComponent spreadingComponent)
    {
      ILocationItemComponent component = spreadingComponent.GetComponent<ILocationItemComponent>();
      if (component == null)
        return;
      component.OnHibernationChanged += new Action<ILocationItemComponent>(this.SpreadingComponent_LocationItem_OnHibernationChanged);
    }

    public void RemoveSpreading(SpreadingComponent spreadingComponent)
    {
      ILocationItemComponent component = spreadingComponent.GetComponent<ILocationItemComponent>();
      if (component == null)
        return;
      component.OnHibernationChanged -= new Action<ILocationItemComponent>(this.SpreadingComponent_LocationItem_OnHibernationChanged);
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
      Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> onFurnitureLoaded = this.OnFurnitureLoaded;
      if (onFurnitureLoaded != null)
        onFurnitureLoaded(parent, owner, buildingEnum, diseasedState);
      if (this.spreadingComponents.Contains(component))
        return;
      this.spreadingComponents.Add(component);
      Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> furnitureLoadedOnce = this.OnFurnitureLoadedOnce;
      if (furnitureLoadedOnce != null)
        furnitureLoadedOnce(parent, owner, buildingEnum, diseasedState);
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Spreading items, entity : ").GetInfo((object) parent).Append(" , region : ").Append(owner.Name).Append(" ,  building : ").Append((object) buildingEnum).Append(" , diseased : ").Append((object) diseasedState));
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      XmlElement node = element[TypeUtility.GetTypeName(this.GetType())];
      if (node == null)
      {
        errorHandler.LogError(TypeUtility.GetTypeName(this.GetType()) + " node not found , context : " + context);
      }
      else
      {
        XmlNodeDataReader reader = new XmlNodeDataReader((XmlNode) node, context);
        ((ISerializeStateLoad) this).StateLoad((IDataReader) reader, this.GetType());
        yield break;
      }
    }

    public void Unload() => this.Clear();

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize<SpreadingService>(writer, TypeUtility.GetTypeName(this.GetType()), this);
    }

    private void Clear()
    {
      this.spreadingComponents.Clear();
      this.regionComponents.Clear();
    }
  }
}
