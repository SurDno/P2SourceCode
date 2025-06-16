// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.OutdoorCrowdComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Crowds;
using Engine.Common.Components.Movable;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components.Crowds;
using Engine.Source.Components.Regions;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using Engine.Source.OutdoorCrowds;
using Engine.Source.Services;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components
{
  [Factory]
  [Required(typeof (LocationItemComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class OutdoorCrowdComponent : 
    EngineComponent,
    IOutdoorCrowdComponent,
    IComponent,
    IUpdatable,
    INeedSave,
    IEntityEventsListener
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<OutdoorCrowdData> data;
    [FromThis]
    private LocationItemComponent locationItem;
    [FromLocator]
    private ITemplateService templateService;
    [FromLocator]
    private ITimeService timeService;
    [FromLocator]
    private OutdoorCrowdService сrowdService;
    [Inspected]
    private List<OutdoorPointInfo> points = new List<OutdoorPointInfo>();
    [Inspected]
    private OutdoorPointInfo waitingPoint;
    [Inspected]
    private bool pointsReady;
    [Inspected]
    private bool initialised;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected OutdoorCrowdLayoutEnum layout;
    private CrowdPointsComponent targetCrowdPointsComponent;
    private RegionComponent targetRegionComponent;
    private bool isDay;
    [Inspected]
    private int diseaseLevel;

    [Inspected(Mutable = true)]
    public OutdoorCrowdLayoutEnum Layout
    {
      get => this.layout;
      set
      {
        this.layout = value;
        this.Reset();
      }
    }

    [Inspected(Mutable = true)]
    private bool IsDay
    {
      get => this.isDay;
      set
      {
        if (this.isDay == value)
          return;
        this.isDay = value;
        this.Reset();
      }
    }

    private bool IsAvailable => this.pointsReady && this.Owner.IsEnabledInHierarchy;

    public bool NeedSave => true;

    public event Action<IEntity> OnCreateEntity;

    public event Action<IEntity> OnDeleteEntity;

    [Inspected]
    public void Reset()
    {
      this.initialised = false;
      this.DestroyEntities();
      this.DestroyPoints();
      this.OnInvalidate();
    }

    public override void OnAdded()
    {
      base.OnAdded();
      this.locationItem.OnHibernationChanged += new Action<ILocationItemComponent>(this.OnChangeHibernation);
      this.OnChangeHibernation((ILocationItemComponent) this.locationItem);
      ((Entity) this.Owner).AddListener((IEntityEventsListener) this);
      this.OnEnableChangedEvent();
      InstanceByRequest<UpdateService>.Instance.OutdoorCrowdUpdater.AddUpdatable((IUpdatable) this);
    }

    public override void OnRemoved()
    {
      InstanceByRequest<UpdateService>.Instance.OutdoorCrowdUpdater.RemoveUpdatable((IUpdatable) this);
      ((Entity) this.Owner).RemoveListener((IEntityEventsListener) this);
      this.locationItem.OnHibernationChanged -= new Action<ILocationItemComponent>(this.OnChangeHibernation);
      this.locationItem = (LocationItemComponent) null;
      if (this.targetCrowdPointsComponent != null)
      {
        this.targetCrowdPointsComponent.ChangePointsEvent -= new Action<ICrowdPointsComponent, bool>(this.CrowdPointsComponent_ChangePointsEvent);
        this.targetCrowdPointsComponent = (CrowdPointsComponent) null;
      }
      if (this.targetRegionComponent != null)
      {
        this.targetRegionComponent.DiseaseLevel.ChangeValueEvent -= new Action<int>(this.TargetRegionComponent_ChangeDiseaseEvent);
        this.targetRegionComponent = (RegionComponent) null;
      }
      base.OnRemoved();
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        return;
      this.IsDay = this.timeService.SolarTime.GetTimesOfDay().IsDay();
      if (!this.IsAvailable || this.waitingPoint != null)
        return;
      this.ComputeCreateEntity();
      if (this.waitingPoint != null)
        return;
      this.ComputeDestroyEntity();
    }

    private void ComputeCreateEntity()
    {
      if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DestroyOutdoorCrowdInIndoor && ServiceLocator.GetService<InsideIndoorListener>().InsideIndoor)
        return;
      for (int index = 0; index < this.points.Count; ++index)
      {
        OutdoorPointInfo point = this.points[index];
        if (point.Entity == null && this.сrowdService.CanCreateEntity(point.Area))
        {
          if (point.NotReady)
          {
            point.NotReady = !this.targetCrowdPointsComponent.TryFindPoint(out point.Radius, out point.CenterPoint, out point.Position, point.Area);
            break;
          }
          float magnitude = (point.Position - EngineApplication.PlayerPosition).magnitude;
          if ((double) magnitude > (double) ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.CrowdSpawnMinDistance && (double) magnitude < (double) ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.CrowdSpawnMaxDistance)
          {
            this.waitingPoint = point;
            DynamicModelComponent.GroupContext = "[OutdoorCrowds]";
            if (this.OnCreateEntity != null)
            {
              this.OnCreateEntity(this.waitingPoint.Template);
              break;
            }
            Debug.LogError((object) ("OnCreateEntity not listener, owner : " + this.Owner.GetInfo()));
            break;
          }
        }
      }
    }

    private void ComputeDestroyEntity()
    {
      bool flag = false;
      if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DestroyOutdoorCrowdInIndoor)
        flag = ServiceLocator.GetService<InsideIndoorListener>().InsideIndoor;
      for (int index = 0; index < this.points.Count; ++index)
      {
        OutdoorPointInfo point = this.points[index];
        if (point.Entity != null && ((IEntityView) point.Entity).IsAttached && (flag || (double) (((IEntityView) point.Entity).Position - EngineApplication.PlayerPosition).magnitude > (double) ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.CrowdDestroyDistance))
        {
          this.RemoveEntity(point);
          break;
        }
      }
    }

    private void OnChangeHibernation(ILocationItemComponent sender)
    {
      if (this.locationItem.IsHibernation)
        return;
      OutdoorCrowdData outdoorCrowdData = this.data.Value;
      if (outdoorCrowdData == null)
      {
        Debug.LogError((object) ("Data not found , owner : " + this.Owner.GetInfo()));
      }
      else
      {
        RegionEnum region = outdoorCrowdData.Region;
        RegionComponent regionByName = RegionUtility.GetRegionByName(region);
        if (regionByName == null)
        {
          Debug.LogError((object) string.Format("Region not found : {0} , owner : {1}", (object) region, (object) this.Owner.GetInfo()));
        }
        else
        {
          this.targetCrowdPointsComponent = regionByName.GetComponent<CrowdPointsComponent>();
          if (this.targetCrowdPointsComponent != null)
          {
            this.targetCrowdPointsComponent.ChangePointsEvent += new Action<ICrowdPointsComponent, bool>(this.CrowdPointsComponent_ChangePointsEvent);
            if (this.targetCrowdPointsComponent.PointsReady)
              this.CrowdPointsComponent_ChangePointsEvent((ICrowdPointsComponent) this.targetCrowdPointsComponent, this.targetCrowdPointsComponent.PointsReady);
          }
          else
            Debug.LogError((object) ("CrowdPointsComponent not found : " + this.Owner.GetInfo()));
          this.targetRegionComponent = regionByName.GetComponent<RegionComponent>();
          if (this.targetRegionComponent != null)
          {
            this.targetRegionComponent.DiseaseLevel.ChangeValueEvent += new Action<int>(this.TargetRegionComponent_ChangeDiseaseEvent);
            this.TargetRegionComponent_ChangeDiseaseEvent(this.targetRegionComponent.DiseaseLevel.Value);
          }
          else
            Debug.LogError((object) ("RegionComponent not found : " + this.Owner.GetInfo()));
        }
      }
    }

    private void CrowdPointsComponent_ChangePointsEvent(
      ICrowdPointsComponent crowdPointsComponent,
      bool pointsReady)
    {
      this.pointsReady = pointsReady;
      if (!this.IsAvailable)
        return;
      this.OnInvalidate();
    }

    private void DestroyEntities()
    {
      foreach (OutdoorPointInfo point in this.points)
      {
        if (point.Entity != null)
          this.RemoveEntity(point);
      }
    }

    public void AddEntity(IEntity entity)
    {
      CrowdUtility.SetAsCrowd(entity);
      if (this.points.FirstOrDefault<OutdoorPointInfo>((Func<OutdoorPointInfo, bool>) (o => o.Entity == entity)) != null)
      {
        Debug.LogError((object) ("Entity already added : " + entity.GetInfo()));
      }
      else
      {
        OutdoorPointInfo waitingPoint = this.waitingPoint;
        this.waitingPoint = (OutdoorPointInfo) null;
        if (waitingPoint == null)
        {
          Debug.LogError((object) ("Point not found : " + this.Owner.GetInfo()));
        }
        else
        {
          this.сrowdService.OnCreateEntity(waitingPoint.Area);
          waitingPoint.Entity = entity;
          CrowdItemComponent component1 = entity.GetComponent<CrowdItemComponent>();
          if (component1 != null)
            component1.AttachToCrowd(this.Owner, (PointInfo) waitingPoint);
          else
            Debug.LogError((object) ("CrowdItemComponent not found : " + entity.GetInfo()));
          foreach (ICrowdContextComponent component2 in entity.GetComponents<ICrowdContextComponent>())
            component2.RestoreState(waitingPoint.States, false);
          waitingPoint.States.Clear();
          NavigationComponent component3 = entity.GetComponent<NavigationComponent>();
          if (component3 != null)
          {
            Vector3 position = waitingPoint.Position;
            if (waitingPoint.OnNavMesh)
              NavMeshUtility.SamplePosition(ref position, AreaEnum.All.ToMask(), 32);
            ILocationComponent parentComponent = LocationItemUtility.FindParentComponent<ILocationComponent>(waitingPoint.Region);
            component3.TeleportTo(parentComponent, waitingPoint.Position, waitingPoint.Rotation);
          }
          else
            Debug.LogError((object) ("NavigationComponent not found : " + entity.GetInfo()));
          if (!this.IsAvailable)
            this.RemoveEntity(waitingPoint);
          else
            this.ComputeModel(waitingPoint.Entity);
        }
      }
    }

    private void RemoveEntity(OutdoorPointInfo info)
    {
      IEntity entity = info.Entity;
      info.Entity = (IEntity) null;
      info.States.Clear();
      foreach (ICrowdContextComponent component in entity.GetComponents<ICrowdContextComponent>())
        component.StoreState(info.States, false);
      entity.GetComponent<CrowdItemComponent>()?.DetachFromCrowd();
      this.сrowdService.OnDestroyEntity(info.Area);
      Action<IEntity> onDeleteEntity = this.OnDeleteEntity;
      if (onDeleteEntity == null)
        return;
      onDeleteEntity(entity);
    }

    private void ComputePoints()
    {
      OutdoorCrowdData outdoorCrowdData = this.data.Value;
      if (outdoorCrowdData == null || this.layout == OutdoorCrowdLayoutEnum.None)
        return;
      OutdoorCrowdLayout outdoorCrowdLayout = outdoorCrowdData.Layouts.FirstOrDefaultNoAlloc<OutdoorCrowdLayout>((Func<OutdoorCrowdLayout, bool>) (o => o.Layout == this.layout));
      if (outdoorCrowdLayout == null)
      {
        Debug.LogError((object) (string.Format("Layout {0} not found, ", (object) this.layout) + this.Owner.GetInfo()));
      }
      else
      {
        DiseasedStateEnum stateName = DiseasedUtility.GetStateByLevel(this.diseaseLevel);
        OutdoorCrowdState outdoorCrowdState = outdoorCrowdLayout.States.FirstOrDefaultNoAlloc<OutdoorCrowdState>((Func<OutdoorCrowdState, bool>) (o => o.State == stateName));
        if (outdoorCrowdState == null)
        {
          Debug.LogWarning((object) (string.Format("State {0} not found in layout {1} , ", (object) stateName, (object) this.layout) + this.Owner.GetInfo()));
        }
        else
        {
          Dictionary<AreaEnum, List<IEntity>> map = new Dictionary<AreaEnum, List<IEntity>>();
          foreach (OutdoorCrowdTemplateLink templateLink in outdoorCrowdState.TemplateLinks)
          {
            OutdoorCrowdTemplateLink link = templateLink;
            if (link.Areas.Count != 0)
            {
              OutdoorCrowdTemplates outdoorCrowdTemplates = outdoorCrowdData.Templates.FirstOrDefaultNoAlloc<OutdoorCrowdTemplates>((Func<OutdoorCrowdTemplates, bool>) (o => o.Name == link.Link));
              if (outdoorCrowdTemplates != null)
              {
                foreach (OutdoorCrowdTemplate template in outdoorCrowdTemplates.Templates)
                {
                  IEntity entity = template.Template.Value;
                  if (entity != null)
                  {
                    int count = this.GetCount(template, this.isDay);
                    for (int index = 0; index < count; ++index)
                    {
                      AreaEnum key = link.Areas.Random<AreaEnum>();
                      map.GetOrCreateValue<AreaEnum, List<IEntity>>(key).Add(entity);
                    }
                  }
                }
              }
            }
          }
          using (Dictionary<AreaEnum, List<IEntity>>.Enumerator enumerator = map.GetEnumerator())
          {
label_26:
            while (enumerator.MoveNext())
            {
              KeyValuePair<AreaEnum, List<IEntity>> current = enumerator.Current;
              CrowdUtility.Points.Clear();
              this.targetCrowdPointsComponent.GetEnabledPoints(current.Key, current.Value.Count, CrowdUtility.Points);
              CrowdUtility.Points.Shuffle<CrowdPointInfo>();
              int index = 0;
              while (true)
              {
                if (index < current.Value.Count && index < CrowdUtility.Points.Count)
                {
                  IEntity entity = current.Value[index];
                  CrowdPointInfo point = CrowdUtility.Points[index];
                  OutdoorPointInfo outdoorPointInfo = new OutdoorPointInfo();
                  outdoorPointInfo.Region = this.targetCrowdPointsComponent.Owner;
                  outdoorPointInfo.Area = point.Area;
                  outdoorPointInfo.Radius = point.Radius;
                  outdoorPointInfo.CenterPoint = point.CenterPoint;
                  outdoorPointInfo.Position = point.Position;
                  outdoorPointInfo.Rotation = point.Rotation;
                  outdoorPointInfo.Template = entity;
                  outdoorPointInfo.EntityPoint = point.EntityPoint;
                  outdoorPointInfo.OnNavMesh = point.OnNavMesh;
                  outdoorPointInfo.NotReady = point.NotReady;
                  this.points.Add(outdoorPointInfo);
                  ++index;
                }
                else
                  goto label_26;
              }
            }
          }
        }
      }
    }

    private int GetCount(OutdoorCrowdTemplate template, bool isDay)
    {
      OutdoorCrowdTemplateCount crowdTemplateCount = isDay ? template.Day : template.Night;
      return UnityEngine.Random.Range(crowdTemplateCount.Min, crowdTemplateCount.Max + 1);
    }

    private void OnEnableChangedEvent() => this.OnInvalidate();

    private void TargetRegionComponent_ChangeDiseaseEvent(int level)
    {
      this.diseaseLevel = level;
      this.Reset();
    }

    private void OnInvalidate()
    {
      this.DestroyEntities();
      if (!this.IsAvailable || this.initialised)
        return;
      this.initialised = true;
      this.ComputePoints();
    }

    private void ComputeModel(IEntity entity)
    {
      DynamicModelComponent component1 = entity.GetComponent<DynamicModelComponent>();
      if (component1 == null)
        return;
      List<IModel> list = component1.Models.ToList<IModel>();
      if (list.Count == 0)
        return;
      ParametersComponent component2 = entity.GetComponent<ParametersComponent>();
      if (component2 == null)
        return;
      IParameter<int> byName = component2.GetByName<int>(ParameterNameEnum.Model);
      if (byName != null)
      {
        int index = byName.Value % list.Count;
        IModel model = list[index];
        component1.Model = model;
      }
    }

    private void DestroyPoints() => this.points.Clear();

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded() => this.OnInvalidate();

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.EnableChangedEvent)
        return;
      this.OnEnableChangedEvent();
    }
  }
}
