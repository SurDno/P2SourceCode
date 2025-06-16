// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.HerbRootsComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Impl.Services.Simulations;
using Engine.Source.Commons;
using Engine.Source.Components.Crowds;
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
  [Required(typeof (InteractableComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class HerbRootsComponent : 
    EngineComponent,
    IHerbRootsComponent,
    IComponent,
    IUpdatable,
    INeedSave,
    IEntityEventsListener
  {
    [FromThis]
    private LocationItemComponent locationItem;
    [FromThis]
    private InteractableComponent interactable;
    [Inspected]
    private bool spawnPointsAttached;
    [Inspected]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected HerbRootsComponentStateEnum state = HerbRootsComponentStateEnum.Sleeping;
    [Inspected]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected List<double> herbGrowTimesLeftSorted = new List<double>();
    [Inspected]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected int grownHerbsCount;
    [Inspected]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected int currentHerbsCount;
    [Inspected]
    private List<HerbRootsPointInfo> spawnPoints = new List<HerbRootsPointInfo>();
    [Inspected]
    private HerbRootsPointInfo waitingPoint;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected List<HerbRootsTemplate> templates = new List<HerbRootsTemplate>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected int herbsBudget = 8;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected int herbsCountMin;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected int herbsCountMax;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected float HerbsGrowTimeInMinutesMin;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected float herbsGrowTimeInMinutesMax;
    private HerbRoots roots;

    public HerbRootsComponentStateEnum State
    {
      get => this.state;
      set => this.state = value;
    }

    public bool NeedSave
    {
      get
      {
        return this.currentHerbsCount != 0 || this.state != HerbRootsComponentStateEnum.Sleeping || this.herbGrowTimesLeftSorted.Count != 0 || this.grownHerbsCount != 0;
      }
    }

    [Inspected]
    private IEnumerable<IEntity> Entities
    {
      get
      {
        return this.spawnPoints.Select<HerbRootsPointInfo, IEntity>((Func<HerbRootsPointInfo, IEntity>) (o => o.Entity)).Where<IEntity>((Func<IEntity, bool>) (o => o != null));
      }
    }

    [Inspected]
    private bool IsOperating
    {
      get
      {
        return this.locationItem != null && !this.locationItem.IsHibernation && this.Owner != null && this.Owner.IsEnabledInHierarchy && this.spawnPointsAttached;
      }
    }

    [Inspected]
    public bool IsHerbsbudgetReached => this.grownHerbsCount >= this.herbsBudget;

    public event Action OnTriggerEnterEvent;

    public event Action OnTriggerLeaveEvent;

    public event Action OnActivateStartEvent;

    public event Action OnActivateEndEvent;

    public event Action OnHerbSpawnEvent;

    public event Action OnLastHerbSpawnEvent;

    public void FireOnActivateStartEvent() => this.OnActivateStartEvent();

    public void FireOnTriggerEnterEvent() => this.OnTriggerEnterEvent();

    public void FireOnTriggerLeaveEvent() => this.OnTriggerLeaveEvent();

    public void Reset()
    {
      this.currentHerbsCount = 0;
      this.herbGrowTimesLeftSorted.Clear();
      this.grownHerbsCount = 0;
      Simulation service = ServiceLocator.GetService<Simulation>();
      for (int index = 0; index < this.spawnPoints.Count; ++index)
      {
        if (this.spawnPoints[index].Entity != null)
        {
          service.Remove(this.spawnPoints[index].Entity);
          this.spawnPoints[index].Entity.Dispose();
          this.spawnPoints[index].Entity = (IEntity) null;
        }
      }
    }

    private IEntity WeightedRandomTemplate()
    {
      if (this.templates.Count == 0)
        return (IEntity) null;
      float max = this.templates.Sum<HerbRootsTemplate>((Func<HerbRootsTemplate, float>) (t => t.Weight));
      if ((double) max == 0.0)
        return (IEntity) null;
      float num1 = UnityEngine.Random.Range(0.0f, max);
      float num2 = 0.0f;
      for (int index = 0; index < this.templates.Count; ++index)
      {
        num2 += this.templates[index].Weight;
        if ((double) num2 >= (double) num1)
          return this.templates[index].Template.Value;
      }
      return (IEntity) null;
    }

    public void GiveBlood()
    {
      this.roots.PlayGiveBloodSound();
      TimeSpan gameTime = ServiceLocator.GetService<TimeService>().GameTime;
      int num1 = UnityEngine.Random.Range(this.herbsCountMin, this.herbsCountMax + 1);
      for (int index = 0; index < num1 && this.grownHerbsCount < this.herbsBudget; ++index)
      {
        float num2 = 60f * UnityEngine.Random.Range(this.HerbsGrowTimeInMinutesMin, this.herbsGrowTimeInMinutesMax);
        this.herbGrowTimesLeftSorted.Add(gameTime.TotalSeconds + (double) num2);
        ++this.grownHerbsCount;
      }
      this.herbGrowTimesLeftSorted.Sort();
    }

    public void SetState(HerbRootsComponentStateEnum state)
    {
      switch (state)
      {
        case HerbRootsComponentStateEnum.Sleeping:
          this.interactable.IsEnabled = false;
          break;
        case HerbRootsComponentStateEnum.Active:
          this.OnActivateEndEvent();
          this.interactable.IsEnabled = this.herbGrowTimesLeftSorted.Count == 0;
          break;
      }
      this.state = state;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      this.interactable.EndInteractEvent += new Action<IEntity, IInteractableComponent, IInteractItem>(this.Interactable_EndInteractEvent);
      this.locationItem.OnHibernationChanged += new Action<ILocationItemComponent>(this.OnChangeHibernation);
      this.OnChangeHibernation((ILocationItemComponent) this.locationItem);
      ((Entity) this.Owner).AddListener((IEntityEventsListener) this);
      this.OnEnableChangedEvent();
      ((IEntityView) this.Owner).OnGameObjectChangedEvent += new Action(this.OnGameObjectChangedEvent);
      InstanceByRequest<UpdateService>.Instance.OutdoorCrowdUpdater.AddUpdatable((IUpdatable) this);
    }

    public override void OnRemoved()
    {
      this.DestroyEntities();
      InstanceByRequest<UpdateService>.Instance.OutdoorCrowdUpdater.RemoveUpdatable((IUpdatable) this);
      ((IEntityView) this.Owner).OnGameObjectChangedEvent -= new Action(this.OnGameObjectChangedEvent);
      ((Entity) this.Owner).RemoveListener((IEntityEventsListener) this);
      this.locationItem.OnHibernationChanged -= new Action<ILocationItemComponent>(this.OnChangeHibernation);
      this.locationItem = (LocationItemComponent) null;
      this.interactable.EndInteractEvent -= new Action<IEntity, IInteractableComponent, IInteractItem>(this.Interactable_EndInteractEvent);
      base.OnRemoved();
    }

    private void Interactable_EndInteractEvent(
      IEntity entity,
      IInteractableComponent component,
      IInteractItem interactItem)
    {
      if (interactItem.Type != InteractType.Interact)
        return;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return;
      StorageComponent component1 = player.GetComponent<StorageComponent>();
      if (component1 == null)
        return;
      foreach (IStorableComponent storableComponent in component1.Items)
      {
        if (storableComponent.Groups.Contains<StorableGroup>(StorableGroup.Craft_Organ) && !storableComponent.Groups.Contains<StorableGroup>(StorableGroup.Autopsy_Chest) && !storableComponent.Groups.Contains<StorableGroup>(StorableGroup.Autopsy_Head) && !storableComponent.Groups.Contains<StorableGroup>(StorableGroup.Autopsy_Stomach))
        {
          ServiceLocator.GetService<NotificationService>().AddNotify(NotificationEnum.ItemDrop, new object[1]
          {
            (object) storableComponent.Owner.Template
          });
          if (storableComponent.Count <= 1)
            storableComponent.Owner.Dispose();
          else
            --storableComponent.Count;
          this.GiveBlood();
          break;
        }
      }
    }

    private bool IsCollected(IEntity entity)
    {
      ParametersComponent component = entity.GetComponent<ParametersComponent>();
      if (component == null)
        return true;
      IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.Collected);
      return byName == null || byName.Value;
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !this.IsOperating)
        return;
      this.currentHerbsCount = 0;
      Simulation service = ServiceLocator.GetService<Simulation>();
      for (int index = 0; index < this.spawnPoints.Count; ++index)
      {
        if (this.spawnPoints[index].Entity != null)
        {
          if (this.IsCollected(this.spawnPoints[index].Entity))
          {
            service.Remove(this.spawnPoints[index].Entity);
            this.spawnPoints[index].Entity.Dispose();
            this.spawnPoints[index].Entity = (IEntity) null;
          }
          else
            ++this.currentHerbsCount;
        }
      }
      this.ComputeCreateEntity();
    }

    private void ComputeCreateEntity()
    {
      if (this.spawnPoints.Count == 0 || this.herbGrowTimesLeftSorted.Count == 0 || this.templates.Count == 0 || ServiceLocator.GetService<TimeService>().GameTime.TotalSeconds < this.herbGrowTimesLeftSorted[0])
        return;
      int num = UnityEngine.Random.Range(0, this.spawnPoints.Count);
      this.herbGrowTimesLeftSorted.RemoveAt(0);
      if (this.state == HerbRootsComponentStateEnum.Active)
        this.interactable.IsEnabled = this.herbGrowTimesLeftSorted.Count == 0;
      for (int index1 = 0; index1 < this.spawnPoints.Count; ++index1)
      {
        int index2 = (index1 + num) % this.spawnPoints.Count;
        if (this.spawnPoints[index2].Entity == null)
        {
          this.waitingPoint = this.spawnPoints[index2];
          IEntity entity = ServiceCache.Factory.Instantiate<IEntity>(this.WeightedRandomTemplate());
          ((Entity) entity).DontSave = true;
          Simulation service = ServiceLocator.GetService<Simulation>();
          service.Add(entity, service.Objects);
          this.AddEntity(entity);
          Action onHerbSpawnEvent = this.OnHerbSpawnEvent;
          if (onHerbSpawnEvent != null)
            onHerbSpawnEvent();
          if (this.herbGrowTimesLeftSorted.Count != 0)
            break;
          Action lastHerbSpawnEvent = this.OnLastHerbSpawnEvent;
          if (lastHerbSpawnEvent == null)
            break;
          lastHerbSpawnEvent();
          break;
        }
      }
    }

    private void OnGameObjectChangedEvent()
    {
      GameObject gameObject = ((IEntityView) this.Owner).GameObject;
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
        return;
      this.roots = gameObject.GetComponent<HerbRoots>();
      if ((UnityEngine.Object) this.roots == (UnityEngine.Object) null)
      {
        Debug.LogError((object) (typeof (HerbRoots).Name + " is not attached : " + this.Owner.GetInfo()), (UnityEngine.Object) gameObject);
      }
      else
      {
        for (int index = 0; index < gameObject.transform.childCount; ++index)
        {
          if (!((UnityEngine.Object) gameObject.transform.GetChild(index).GetComponentNonAlloc<HerbRootsSpawnPoint>() == (UnityEngine.Object) null))
            this.spawnPoints.Add(new HerbRootsPointInfo()
            {
              CenterPoint = gameObject.transform.GetChild(index).position,
              Entity = (IEntity) null
            });
        }
        if (this.spawnPoints.Count == 0)
          Debug.LogError((object) ("Zero spawn points count, owner : " + this.Owner.GetInfo()));
        this.spawnPointsAttached = true;
      }
    }

    private void OnChangeHibernation(ILocationItemComponent sender)
    {
      if (!this.IsOperating)
      {
        this.DestroyEntities();
      }
      else
      {
        this.EnqueueCreateEntities();
        this.SetState(this.state);
      }
    }

    private void DestroyEntities()
    {
      Simulation service = ServiceLocator.GetService<Simulation>();
      foreach (HerbRootsPointInfo spawnPoint in this.spawnPoints)
      {
        if (spawnPoint.Entity != null)
        {
          service.Remove(spawnPoint.Entity);
          spawnPoint.Entity.Dispose();
          spawnPoint.Entity = (IEntity) null;
        }
      }
    }

    private void EnqueueCreateEntities()
    {
      for (int index = 0; index < this.currentHerbsCount; ++index)
        this.herbGrowTimesLeftSorted.Add(0.0);
      this.currentHerbsCount = 0;
    }

    private void AddEntity(IEntity entity)
    {
      if (this.spawnPoints.FirstOrDefault<HerbRootsPointInfo>((Func<HerbRootsPointInfo, bool>) (o => o.Entity == entity)) != null)
        throw new Exception("Entity already added : " + entity.GetInfo());
      HerbRootsPointInfo waitingPoint = this.waitingPoint;
      this.waitingPoint = (HerbRootsPointInfo) null;
      if (waitingPoint == null)
      {
        Debug.LogError((object) ("Point not found : " + this.Owner.GetInfo()));
      }
      else
      {
        waitingPoint.Entity = entity;
        NavigationComponent component = entity.GetComponent<NavigationComponent>();
        if (component != null)
          component.TeleportTo(this.locationItem.Location, waitingPoint.CenterPoint, Quaternion.identity);
        else
          Debug.LogError((object) ("NavigationComponent not found : " + entity.GetInfo()));
      }
    }

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded()
    {
      this.EnqueueCreateEntities();
      this.SetState(this.state);
    }

    private void OnEnableChangedEvent()
    {
      if (!this.IsOperating)
      {
        this.DestroyEntities();
      }
      else
      {
        this.EnqueueCreateEntities();
        this.SetState(this.state);
      }
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.EnableChangedEvent)
        return;
      this.OnEnableChangedEvent();
    }
  }
}
