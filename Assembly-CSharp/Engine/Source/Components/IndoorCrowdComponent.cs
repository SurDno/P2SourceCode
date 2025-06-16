// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.IndoorCrowdComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Movable;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components.Crowds;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using Engine.Source.Services;
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
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class IndoorCrowdComponent : 
    EngineComponent,
    IIndoorCrowdComponent,
    IComponent,
    IUpdatable,
    IEntityEventsListener
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected SceneGameObject region;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<CrowdAreaInfo> areas = new List<CrowdAreaInfo>();
    [FromThis]
    private LocationItemComponent locationItem;
    [FromLocator]
    private ITemplateService templateService;
    [FromLocator]
    private OutdoorCrowdService сrowdService;
    [Inspected]
    private List<IndoorPointInfo> points = new List<IndoorPointInfo>();
    [Inspected]
    private IndoorPointInfo waitingPoint;
    private bool initialise;
    [Inspected]
    private bool pointsReady;
    private CrowdPointsComponent targetCrowdPointsComponent;

    public event Action<IEntity> OnCreateEntity;

    public event Action<IEntity> OnDeleteEntity;

    private bool IsAvailable => this.pointsReady && this.Owner.IsEnabledInHierarchy;

    public override void OnAdded()
    {
      base.OnAdded();
      this.UpdateRandom();
      this.locationItem.OnHibernationChanged += new Action<ILocationItemComponent>(this.OnChangeHibernation);
      this.OnChangeHibernation((ILocationItemComponent) this.locationItem);
      ((Entity) this.Owner).AddListener((IEntityEventsListener) this);
      this.OnEnableChangedEvent();
      InstanceByRequest<UpdateService>.Instance.IndoorCrowdUpdater.AddUpdatable((IUpdatable) this);
    }

    public override void OnRemoved()
    {
      InstanceByRequest<UpdateService>.Instance.IndoorCrowdUpdater.RemoveUpdatable((IUpdatable) this);
      ((Entity) this.Owner).RemoveListener((IEntityEventsListener) this);
      this.locationItem.OnHibernationChanged -= new Action<ILocationItemComponent>(this.OnChangeHibernation);
      this.locationItem = (LocationItemComponent) null;
      if (this.targetCrowdPointsComponent != null)
      {
        this.targetCrowdPointsComponent.ChangePointsEvent -= new Action<ICrowdPointsComponent, bool>(this.CrowdPointsComponent_ChangePointsEvent);
        this.targetCrowdPointsComponent = (CrowdPointsComponent) null;
      }
      base.OnRemoved();
    }

    private void UpdateRandom()
    {
      foreach (CrowdAreaInfo area in this.areas)
      {
        if (area != null)
        {
          foreach (CrowdTemplateInfo templateInfo in area.TemplateInfos)
            templateInfo.Seed = 0;
        }
        else
          Debug.LogWarning((object) ("info == null : " + this.Owner.GetInfo()));
      }
    }

    private void OnChangeHibernation(ILocationItemComponent sender)
    {
      if (this.locationItem.IsHibernation)
        return;
      GameObject gameObject1 = ((IEntityView) this.Owner).GameObject;
      if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
      {
        Debug.LogError((object) ("GameObject not found , " + this.Owner.GetInfo()));
      }
      else
      {
        SceneObjectContainer container = SceneObjectContainer.GetContainer(gameObject1.scene);
        if ((UnityEngine.Object) container == (UnityEngine.Object) null)
        {
          Debug.LogError((object) ("SceneObjectContainer not found in scene " + gameObject1.scene.name + " , " + this.Owner.GetInfo()));
        }
        else
        {
          GameObject gameObject2 = container.GetGameObject(this.region.Id);
          if ((UnityEngine.Object) gameObject2 == (UnityEngine.Object) null)
          {
            Debug.LogError((object) string.Format("GameObject region not found : {0} , {1}", (object) this.region.Id, (object) this.Owner.GetInfo()));
          }
          else
          {
            IEntity entity = EntityUtility.GetEntity(gameObject2);
            if (entity == null)
            {
              Debug.LogError((object) ("Entity not found : " + this.Owner.GetInfo()), (UnityEngine.Object) gameObject2);
            }
            else
            {
              this.targetCrowdPointsComponent = entity.GetComponent<CrowdPointsComponent>();
              if (this.targetCrowdPointsComponent == null)
              {
                Debug.LogError((object) ("CrowdPointsComponent not found : " + this.Owner.GetInfo()), (UnityEngine.Object) gameObject2);
              }
              else
              {
                this.targetCrowdPointsComponent.ChangePointsEvent += new Action<ICrowdPointsComponent, bool>(this.CrowdPointsComponent_ChangePointsEvent);
                if (!this.targetCrowdPointsComponent.PointsReady)
                  return;
                this.CrowdPointsComponent_ChangePointsEvent((ICrowdPointsComponent) this.targetCrowdPointsComponent, this.targetCrowdPointsComponent.PointsReady);
              }
            }
          }
        }
      }
    }

    private void CrowdPointsComponent_ChangePointsEvent(
      ICrowdPointsComponent crowdPointsComponent,
      bool pointsReady)
    {
      this.pointsReady = pointsReady;
      if (pointsReady)
        this.ComputePoints();
      else
        this.DestroyEntities();
    }

    public void AddEntity(IEntity entity)
    {
      CrowdUtility.SetAsCrowd(entity);
      if (this.points.FirstOrDefault<IndoorPointInfo>((Func<IndoorPointInfo, bool>) (o => o.Entity == entity)) != null)
      {
        Debug.LogError((object) ("Entity already added : " + entity.GetInfo()));
      }
      else
      {
        IndoorPointInfo waitingPoint = this.waitingPoint;
        this.waitingPoint = (IndoorPointInfo) null;
        if (waitingPoint == null)
        {
          Debug.LogError((object) ("Point not found : " + this.Owner.GetInfo()));
        }
        else
        {
          waitingPoint.Entity = entity;
          CrowdItemComponent component1 = entity.GetComponent<CrowdItemComponent>();
          if (component1 != null)
            component1.AttachToCrowd(this.Owner, (PointInfo) waitingPoint);
          else
            Debug.LogError((object) ("CrowdItemComponent not found : " + entity.GetInfo()));
          foreach (ICrowdContextComponent component2 in entity.GetComponents<ICrowdContextComponent>())
            component2.RestoreState(waitingPoint.States, true);
          waitingPoint.States.Clear();
          NavigationComponent component3 = entity.GetComponent<NavigationComponent>();
          if (component3 != null)
          {
            ILocationComponent parentComponent = LocationItemUtility.FindParentComponent<ILocationComponent>(waitingPoint.Region);
            if (component1 != null)
            {
              component3.TeleportTo(parentComponent, component1.Point.Position, component1.Point.Rotation);
            }
            else
            {
              Vector3 position = waitingPoint.Position;
              if (waitingPoint.OnNavMesh)
                NavMeshUtility.SamplePosition(ref position, AreaEnum.All.ToMask(), 32);
              component3.TeleportTo(parentComponent, position, waitingPoint.Rotation);
            }
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

    private void RemoveEntity(IndoorPointInfo info)
    {
      IEntity entity = info.Entity;
      info.Entity = (IEntity) null;
      info.States.Clear();
      foreach (ICrowdContextComponent component in entity.GetComponents<ICrowdContextComponent>())
        component.StoreState(info.States, true);
      entity.GetComponent<CrowdItemComponent>()?.DetachFromCrowd();
      Action<IEntity> onDeleteEntity = this.OnDeleteEntity;
      if (onDeleteEntity == null)
        return;
      onDeleteEntity(entity);
    }

    public void Reset()
    {
      this.DestroyEntities();
      this.UpdateRandom();
    }

    private void ComputePoints()
    {
      if (this.initialise)
        return;
      this.initialise = true;
      foreach (CrowdAreaInfo area in this.areas)
      {
        CrowdUtility.Points.Clear();
        this.targetCrowdPointsComponent.GetEnabledPoints(area.Area, area.TemplateInfos.Sum<CrowdTemplateInfo>((Func<CrowdTemplateInfo, int>) (o => o.Max)), CrowdUtility.Points);
        CrowdUtility.Points.Shuffle<CrowdPointInfo>();
        int index1 = 0;
        foreach (CrowdTemplateInfo templateInfo in area.TemplateInfos)
        {
          if (templateInfo.Seed == 0)
            templateInfo.Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
          UnityEngine.Random.InitState(templateInfo.Seed);
          int num = UnityEngine.Random.Range(templateInfo.Min, templateInfo.Max + 1);
          if ((double) templateInfo.Chance < (double) UnityEngine.Random.value)
            num = 0;
          for (int index2 = 0; index2 < num; ++index2)
          {
            if (index1 < CrowdUtility.Points.Count)
            {
              CrowdPointInfo point = CrowdUtility.Points[index1];
              IndoorPointInfo indoorPointInfo = new IndoorPointInfo();
              indoorPointInfo.Region = this.targetCrowdPointsComponent.Owner;
              indoorPointInfo.Area = point.Area;
              indoorPointInfo.Position = point.Position;
              indoorPointInfo.Rotation = point.Rotation;
              indoorPointInfo.TemplateInfo = templateInfo;
              indoorPointInfo.Template = templateInfo.Templates[UnityEngine.Random.Range(0, templateInfo.Templates.Count)].Value;
              indoorPointInfo.EntityPoint = point.EntityPoint;
              indoorPointInfo.OnNavMesh = point.OnNavMesh;
              this.points.Add(indoorPointInfo);
              ++index1;
            }
          }
        }
      }
    }

    private void DestroyEntities()
    {
      foreach (IndoorPointInfo point in this.points)
      {
        if (point.Entity != null)
          this.RemoveEntity(point);
      }
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !this.IsAvailable || this.waitingPoint != null)
        return;
      this.waitingPoint = this.points.FirstOrDefaultNoAlloc<IndoorPointInfo>((Func<IndoorPointInfo, bool>) (o => o.Entity == null && o.Template != null));
      if (this.waitingPoint == null)
        return;
      DynamicModelComponent.GroupContext = "[IndoorCrowds]";
      Action<IEntity> onCreateEntity = this.OnCreateEntity;
      if (onCreateEntity == null)
        return;
      onCreateEntity(this.waitingPoint.Template);
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

    private void OnEnableChangedEvent()
    {
      if (this.Owner.IsEnabledInHierarchy)
        return;
      this.DestroyEntities();
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.EnableChangedEvent)
        return;
      this.OnEnableChangedEvent();
    }
  }
}
