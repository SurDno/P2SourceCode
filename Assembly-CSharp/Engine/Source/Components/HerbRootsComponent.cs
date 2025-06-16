using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Serializations.Data;
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
using UnityEngine;
using Random = UnityEngine.Random;

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
    [StateSaveProxy]
    [StateLoadProxy]
    protected HerbRootsComponentStateEnum state = HerbRootsComponentStateEnum.Sleeping;
    [Inspected]
    [StateSaveProxy]
    [StateLoadProxy]
    protected List<double> herbGrowTimesLeftSorted = new List<double>();
    [Inspected]
    [StateSaveProxy]
    [StateLoadProxy]
    protected int grownHerbsCount;
    [Inspected]
    [StateSaveProxy]
    [StateLoadProxy]
    protected int currentHerbsCount;
    [Inspected]
    private List<HerbRootsPointInfo> spawnPoints = new List<HerbRootsPointInfo>();
    [Inspected]
    private HerbRootsPointInfo waitingPoint;
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected List<HerbRootsTemplate> templates = new List<HerbRootsTemplate>();
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected int herbsBudget = 8;
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected int herbsCountMin;
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected int herbsCountMax;
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected float HerbsGrowTimeInMinutesMin;
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy()]
    protected float herbsGrowTimeInMinutesMax;
    private HerbRoots roots;

    public HerbRootsComponentStateEnum State
    {
      get => state;
      set => state = value;
    }

    public bool NeedSave
    {
      get
      {
        return currentHerbsCount != 0 || state != HerbRootsComponentStateEnum.Sleeping || herbGrowTimesLeftSorted.Count != 0 || grownHerbsCount != 0;
      }
    }

    [Inspected]
    private IEnumerable<IEntity> Entities
    {
      get
      {
        return spawnPoints.Select(o => o.Entity).Where(o => o != null);
      }
    }

    [Inspected]
    private bool IsOperating
    {
      get
      {
        return locationItem != null && !locationItem.IsHibernation && Owner != null && Owner.IsEnabledInHierarchy && spawnPointsAttached;
      }
    }

    [Inspected]
    public bool IsHerbsbudgetReached => grownHerbsCount >= herbsBudget;

    public event Action OnTriggerEnterEvent;

    public event Action OnTriggerLeaveEvent;

    public event Action OnActivateStartEvent;

    public event Action OnActivateEndEvent;

    public event Action OnHerbSpawnEvent;

    public event Action OnLastHerbSpawnEvent;

    public void FireOnActivateStartEvent() => OnActivateStartEvent();

    public void FireOnTriggerEnterEvent() => OnTriggerEnterEvent();

    public void FireOnTriggerLeaveEvent() => OnTriggerLeaveEvent();

    public void Reset()
    {
      currentHerbsCount = 0;
      herbGrowTimesLeftSorted.Clear();
      grownHerbsCount = 0;
      Simulation service = ServiceLocator.GetService<Simulation>();
      for (int index = 0; index < spawnPoints.Count; ++index)
      {
        if (spawnPoints[index].Entity != null)
        {
          service.Remove(spawnPoints[index].Entity);
          spawnPoints[index].Entity.Dispose();
          spawnPoints[index].Entity = null;
        }
      }
    }

    private IEntity WeightedRandomTemplate()
    {
      if (templates.Count == 0)
        return null;
      float max = templates.Sum(t => t.Weight);
      if (max == 0.0)
        return null;
      float num1 = Random.Range(0.0f, max);
      float num2 = 0.0f;
      for (int index = 0; index < templates.Count; ++index)
      {
        num2 += templates[index].Weight;
        if (num2 >= (double) num1)
          return templates[index].Template.Value;
      }
      return null;
    }

    public void GiveBlood()
    {
      roots.PlayGiveBloodSound();
      TimeSpan gameTime = ServiceLocator.GetService<TimeService>().GameTime;
      int num1 = Random.Range(herbsCountMin, herbsCountMax + 1);
      for (int index = 0; index < num1 && grownHerbsCount < herbsBudget; ++index)
      {
        float num2 = 60f * Random.Range(HerbsGrowTimeInMinutesMin, herbsGrowTimeInMinutesMax);
        herbGrowTimesLeftSorted.Add(gameTime.TotalSeconds + num2);
        ++grownHerbsCount;
      }
      herbGrowTimesLeftSorted.Sort();
    }

    public void SetState(HerbRootsComponentStateEnum state)
    {
      switch (state)
      {
        case HerbRootsComponentStateEnum.Sleeping:
          interactable.IsEnabled = false;
          break;
        case HerbRootsComponentStateEnum.Active:
          OnActivateEndEvent();
          interactable.IsEnabled = herbGrowTimesLeftSorted.Count == 0;
          break;
      }
      this.state = state;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      interactable.EndInteractEvent += Interactable_EndInteractEvent;
      locationItem.OnHibernationChanged += OnChangeHibernation;
      OnChangeHibernation(locationItem);
      ((Entity) Owner).AddListener(this);
      OnEnableChangedEvent();
      ((IEntityView) Owner).OnGameObjectChangedEvent += OnGameObjectChangedEvent;
      InstanceByRequest<UpdateService>.Instance.OutdoorCrowdUpdater.AddUpdatable(this);
    }

    public override void OnRemoved()
    {
      DestroyEntities();
      InstanceByRequest<UpdateService>.Instance.OutdoorCrowdUpdater.RemoveUpdatable(this);
      ((IEntityView) Owner).OnGameObjectChangedEvent -= OnGameObjectChangedEvent;
      ((Entity) Owner).RemoveListener(this);
      locationItem.OnHibernationChanged -= OnChangeHibernation;
      locationItem = null;
      interactable.EndInteractEvent -= Interactable_EndInteractEvent;
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
        if (storableComponent.Groups.Contains(StorableGroup.Craft_Organ) && !storableComponent.Groups.Contains(StorableGroup.Autopsy_Chest) && !storableComponent.Groups.Contains(StorableGroup.Autopsy_Head) && !storableComponent.Groups.Contains(StorableGroup.Autopsy_Stomach))
        {
          ServiceLocator.GetService<NotificationService>().AddNotify(NotificationEnum.ItemDrop, storableComponent.Owner.Template);
          if (storableComponent.Count <= 1)
            storableComponent.Owner.Dispose();
          else
            --storableComponent.Count;
          GiveBlood();
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
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !IsOperating)
        return;
      currentHerbsCount = 0;
      Simulation service = ServiceLocator.GetService<Simulation>();
      for (int index = 0; index < spawnPoints.Count; ++index)
      {
        if (spawnPoints[index].Entity != null)
        {
          if (IsCollected(spawnPoints[index].Entity))
          {
            service.Remove(spawnPoints[index].Entity);
            spawnPoints[index].Entity.Dispose();
            spawnPoints[index].Entity = null;
          }
          else
            ++currentHerbsCount;
        }
      }
      ComputeCreateEntity();
    }

    private void ComputeCreateEntity()
    {
      if (spawnPoints.Count == 0 || herbGrowTimesLeftSorted.Count == 0 || templates.Count == 0 || ServiceLocator.GetService<TimeService>().GameTime.TotalSeconds < herbGrowTimesLeftSorted[0])
        return;
      int num = Random.Range(0, spawnPoints.Count);
      herbGrowTimesLeftSorted.RemoveAt(0);
      if (state == HerbRootsComponentStateEnum.Active)
        interactable.IsEnabled = herbGrowTimesLeftSorted.Count == 0;
      for (int index1 = 0; index1 < spawnPoints.Count; ++index1)
      {
        int index2 = (index1 + num) % spawnPoints.Count;
        if (spawnPoints[index2].Entity == null)
        {
          waitingPoint = spawnPoints[index2];
          IEntity entity = ServiceCache.Factory.Instantiate(WeightedRandomTemplate());
          ((Entity) entity).DontSave = true;
          Simulation service = ServiceLocator.GetService<Simulation>();
          service.Add(entity, service.Objects);
          AddEntity(entity);
          Action onHerbSpawnEvent = OnHerbSpawnEvent;
          if (onHerbSpawnEvent != null)
            onHerbSpawnEvent();
          if (herbGrowTimesLeftSorted.Count != 0)
            break;
          Action lastHerbSpawnEvent = OnLastHerbSpawnEvent;
          if (lastHerbSpawnEvent == null)
            break;
          lastHerbSpawnEvent();
          break;
        }
      }
    }

    private void OnGameObjectChangedEvent()
    {
      GameObject gameObject = ((IEntityView) Owner).GameObject;
      if (gameObject == null)
        return;
      roots = gameObject.GetComponent<HerbRoots>();
      if (roots == null)
      {
        Debug.LogError(typeof (HerbRoots).Name + " is not attached : " + Owner.GetInfo(), gameObject);
      }
      else
      {
        for (int index = 0; index < gameObject.transform.childCount; ++index)
        {
          if (!(gameObject.transform.GetChild(index).GetComponentNonAlloc<HerbRootsSpawnPoint>() == null))
            spawnPoints.Add(new HerbRootsPointInfo {
              CenterPoint = gameObject.transform.GetChild(index).position,
              Entity = null
            });
        }
        if (spawnPoints.Count == 0)
          Debug.LogError("Zero spawn points count, owner : " + Owner.GetInfo());
        spawnPointsAttached = true;
      }
    }

    private void OnChangeHibernation(ILocationItemComponent sender)
    {
      if (!IsOperating)
      {
        DestroyEntities();
      }
      else
      {
        EnqueueCreateEntities();
        SetState(state);
      }
    }

    private void DestroyEntities()
    {
      Simulation service = ServiceLocator.GetService<Simulation>();
      foreach (HerbRootsPointInfo spawnPoint in spawnPoints)
      {
        if (spawnPoint.Entity != null)
        {
          service.Remove(spawnPoint.Entity);
          spawnPoint.Entity.Dispose();
          spawnPoint.Entity = null;
        }
      }
    }

    private void EnqueueCreateEntities()
    {
      for (int index = 0; index < currentHerbsCount; ++index)
        herbGrowTimesLeftSorted.Add(0.0);
      currentHerbsCount = 0;
    }

    private void AddEntity(IEntity entity)
    {
      if (spawnPoints.FirstOrDefault(o => o.Entity == entity) != null)
        throw new Exception("Entity already added : " + entity.GetInfo());
      HerbRootsPointInfo waitingPoint = this.waitingPoint;
      this.waitingPoint = null;
      if (waitingPoint == null)
      {
        Debug.LogError("Point not found : " + Owner.GetInfo());
      }
      else
      {
        waitingPoint.Entity = entity;
        NavigationComponent component = entity.GetComponent<NavigationComponent>();
        if (component != null)
          component.TeleportTo(locationItem.Location, waitingPoint.CenterPoint, Quaternion.identity);
        else
          Debug.LogError("NavigationComponent not found : " + entity.GetInfo());
      }
    }

    [OnLoaded]
    private void OnLoaded()
    {
      EnqueueCreateEntities();
      SetState(state);
    }

    private void OnEnableChangedEvent()
    {
      if (!IsOperating)
      {
        DestroyEntities();
      }
      else
      {
        EnqueueCreateEntities();
        SetState(state);
      }
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.EnableChangedEvent)
        return;
      OnEnableChangedEvent();
    }
  }
}
