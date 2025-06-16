using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Serializations.Data;
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
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine.Source.Components;

[Factory]
[Required(typeof(LocationItemComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class OutdoorCrowdComponent :
	EngineComponent,
	IOutdoorCrowdComponent,
	IComponent,
	IUpdatable,
	INeedSave,
	IEntityEventsListener {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected Typed<OutdoorCrowdData> data;

	[FromThis] private LocationItemComponent locationItem;
	[FromLocator] private ITemplateService templateService;
	[FromLocator] private ITimeService timeService;
	[FromLocator] private OutdoorCrowdService сrowdService;
	[Inspected] private List<OutdoorPointInfo> points = new();
	[Inspected] private OutdoorPointInfo waitingPoint;
	[Inspected] private bool pointsReady;
	[Inspected] private bool initialised;
	[StateSaveProxy] [StateLoadProxy()] protected OutdoorCrowdLayoutEnum layout;
	private CrowdPointsComponent targetCrowdPointsComponent;
	private RegionComponent targetRegionComponent;
	private bool isDay;
	[Inspected] private int diseaseLevel;

	[Inspected(Mutable = true)]
	public OutdoorCrowdLayoutEnum Layout {
		get => layout;
		set {
			layout = value;
			Reset();
		}
	}

	[Inspected(Mutable = true)]
	private bool IsDay {
		get => isDay;
		set {
			if (isDay == value)
				return;
			isDay = value;
			Reset();
		}
	}

	private bool IsAvailable => pointsReady && Owner.IsEnabledInHierarchy;

	public bool NeedSave => true;

	public event Action<IEntity> OnCreateEntity;

	public event Action<IEntity> OnDeleteEntity;

	[Inspected]
	public void Reset() {
		initialised = false;
		DestroyEntities();
		DestroyPoints();
		OnInvalidate();
	}

	public override void OnAdded() {
		base.OnAdded();
		locationItem.OnHibernationChanged += OnChangeHibernation;
		OnChangeHibernation(locationItem);
		((Entity)Owner).AddListener(this);
		OnEnableChangedEvent();
		InstanceByRequest<UpdateService>.Instance.OutdoorCrowdUpdater.AddUpdatable(this);
	}

	public override void OnRemoved() {
		InstanceByRequest<UpdateService>.Instance.OutdoorCrowdUpdater.RemoveUpdatable(this);
		((Entity)Owner).RemoveListener(this);
		locationItem.OnHibernationChanged -= OnChangeHibernation;
		locationItem = null;
		if (targetCrowdPointsComponent != null) {
			targetCrowdPointsComponent.ChangePointsEvent -= CrowdPointsComponent_ChangePointsEvent;
			targetCrowdPointsComponent = null;
		}

		if (targetRegionComponent != null) {
			targetRegionComponent.DiseaseLevel.ChangeValueEvent -= TargetRegionComponent_ChangeDiseaseEvent;
			targetRegionComponent = null;
		}

		base.OnRemoved();
	}

	public void ComputeUpdate() {
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
			return;
		IsDay = timeService.SolarTime.GetTimesOfDay().IsDay();
		if (!IsAvailable || waitingPoint != null)
			return;
		ComputeCreateEntity();
		if (waitingPoint != null)
			return;
		ComputeDestroyEntity();
	}

	private void ComputeCreateEntity() {
		if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DestroyOutdoorCrowdInIndoor &&
		    ServiceLocator.GetService<InsideIndoorListener>().InsideIndoor)
			return;
		for (var index = 0; index < points.Count; ++index) {
			var point = points[index];
			if (point.Entity == null && сrowdService.CanCreateEntity(point.Area)) {
				if (point.NotReady) {
					point.NotReady = !targetCrowdPointsComponent.TryFindPoint(out point.Radius, out point.CenterPoint,
						out point.Position, point.Area);
					break;
				}

				var magnitude = (point.Position - EngineApplication.PlayerPosition).magnitude;
				if (magnitude > (double)ExternalSettingsInstance<ExternalOptimizationSettings>.Instance
					    .CrowdSpawnMinDistance &&
				    magnitude < (double)ExternalSettingsInstance<ExternalOptimizationSettings>.Instance
					    .CrowdSpawnMaxDistance) {
					waitingPoint = point;
					DynamicModelComponent.GroupContext = "[OutdoorCrowds]";
					if (OnCreateEntity != null) {
						OnCreateEntity(waitingPoint.Template);
						break;
					}

					Debug.LogError("OnCreateEntity not listener, owner : " + Owner.GetInfo());
					break;
				}
			}
		}
	}

	private void ComputeDestroyEntity() {
		var flag = false;
		if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DestroyOutdoorCrowdInIndoor)
			flag = ServiceLocator.GetService<InsideIndoorListener>().InsideIndoor;
		for (var index = 0; index < points.Count; ++index) {
			var point = points[index];
			if (point.Entity != null && ((IEntityView)point.Entity).IsAttached && (flag ||
				    (((IEntityView)point.Entity).Position - EngineApplication.PlayerPosition).magnitude >
				    (double)ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.CrowdDestroyDistance)) {
				RemoveEntity(point);
				break;
			}
		}
	}

	private void OnChangeHibernation(ILocationItemComponent sender) {
		if (locationItem.IsHibernation)
			return;
		var outdoorCrowdData = data.Value;
		if (outdoorCrowdData == null)
			Debug.LogError("Data not found , owner : " + Owner.GetInfo());
		else {
			var region = outdoorCrowdData.Region;
			var regionByName = RegionUtility.GetRegionByName(region);
			if (regionByName == null)
				Debug.LogError(string.Format("Region not found : {0} , owner : {1}", region, Owner.GetInfo()));
			else {
				targetCrowdPointsComponent = regionByName.GetComponent<CrowdPointsComponent>();
				if (targetCrowdPointsComponent != null) {
					targetCrowdPointsComponent.ChangePointsEvent += CrowdPointsComponent_ChangePointsEvent;
					if (targetCrowdPointsComponent.PointsReady)
						CrowdPointsComponent_ChangePointsEvent(targetCrowdPointsComponent,
							targetCrowdPointsComponent.PointsReady);
				} else
					Debug.LogError("CrowdPointsComponent not found : " + Owner.GetInfo());

				targetRegionComponent = regionByName.GetComponent<RegionComponent>();
				if (targetRegionComponent != null) {
					targetRegionComponent.DiseaseLevel.ChangeValueEvent += TargetRegionComponent_ChangeDiseaseEvent;
					TargetRegionComponent_ChangeDiseaseEvent(targetRegionComponent.DiseaseLevel.Value);
				} else
					Debug.LogError("RegionComponent not found : " + Owner.GetInfo());
			}
		}
	}

	private void CrowdPointsComponent_ChangePointsEvent(
		ICrowdPointsComponent crowdPointsComponent,
		bool pointsReady) {
		this.pointsReady = pointsReady;
		if (!IsAvailable)
			return;
		OnInvalidate();
	}

	private void DestroyEntities() {
		foreach (var point in points)
			if (point.Entity != null)
				RemoveEntity(point);
	}

	public void AddEntity(IEntity entity) {
		CrowdUtility.SetAsCrowd(entity);
		if (points.FirstOrDefault(o => o.Entity == entity) != null)
			Debug.LogError("Entity already added : " + entity.GetInfo());
		else {
			var waitingPoint = this.waitingPoint;
			this.waitingPoint = null;
			if (waitingPoint == null)
				Debug.LogError("Point not found : " + Owner.GetInfo());
			else {
				сrowdService.OnCreateEntity(waitingPoint.Area);
				waitingPoint.Entity = entity;
				var component1 = entity.GetComponent<CrowdItemComponent>();
				if (component1 != null)
					component1.AttachToCrowd(Owner, waitingPoint);
				else
					Debug.LogError("CrowdItemComponent not found : " + entity.GetInfo());
				foreach (var component2 in entity.GetComponents<ICrowdContextComponent>())
					component2.RestoreState(waitingPoint.States, false);
				waitingPoint.States.Clear();
				var component3 = entity.GetComponent<NavigationComponent>();
				if (component3 != null) {
					var position = waitingPoint.Position;
					if (waitingPoint.OnNavMesh)
						NavMeshUtility.SamplePosition(ref position, AreaEnum.All.ToMask(), 32);
					var parentComponent =
						LocationItemUtility.FindParentComponent<ILocationComponent>(waitingPoint.Region);
					component3.TeleportTo(parentComponent, waitingPoint.Position, waitingPoint.Rotation);
				} else
					Debug.LogError("NavigationComponent not found : " + entity.GetInfo());

				if (!IsAvailable)
					RemoveEntity(waitingPoint);
				else
					ComputeModel(waitingPoint.Entity);
			}
		}
	}

	private void RemoveEntity(OutdoorPointInfo info) {
		var entity = info.Entity;
		info.Entity = null;
		info.States.Clear();
		foreach (var component in entity.GetComponents<ICrowdContextComponent>())
			component.StoreState(info.States, false);
		entity.GetComponent<CrowdItemComponent>()?.DetachFromCrowd();
		сrowdService.OnDestroyEntity(info.Area);
		var onDeleteEntity = OnDeleteEntity;
		if (onDeleteEntity == null)
			return;
		onDeleteEntity(entity);
	}

	private void ComputePoints() {
		var outdoorCrowdData = data.Value;
		if (outdoorCrowdData == null || layout == OutdoorCrowdLayoutEnum.None)
			return;
		var outdoorCrowdLayout = outdoorCrowdData.Layouts.FirstOrDefaultNoAlloc(o => o.Layout == layout);
		if (outdoorCrowdLayout == null)
			Debug.LogError(string.Format("Layout {0} not found, ", layout) + Owner.GetInfo());
		else {
			var stateName = DiseasedUtility.GetStateByLevel(diseaseLevel);
			var outdoorCrowdState = outdoorCrowdLayout.States.FirstOrDefaultNoAlloc(o => o.State == stateName);
			if (outdoorCrowdState == null)
				Debug.LogWarning(string.Format("State {0} not found in layout {1} , ", stateName, layout) +
				                 Owner.GetInfo());
			else {
				var map = new Dictionary<AreaEnum, List<IEntity>>();
				foreach (var templateLink in outdoorCrowdState.TemplateLinks) {
					var link = templateLink;
					if (link.Areas.Count != 0) {
						var outdoorCrowdTemplates =
							outdoorCrowdData.Templates.FirstOrDefaultNoAlloc(o => o.Name == link.Link);
						if (outdoorCrowdTemplates != null)
							foreach (var template in outdoorCrowdTemplates.Templates) {
								var entity = template.Template.Value;
								if (entity != null) {
									var count = GetCount(template, isDay);
									for (var index = 0; index < count; ++index) {
										var key = link.Areas.Random();
										map.GetOrCreateValue(key).Add(entity);
									}
								}
							}
					}
				}

				using (var enumerator = map.GetEnumerator()) {
					label_26:
					while (enumerator.MoveNext()) {
						var current = enumerator.Current;
						CrowdUtility.Points.Clear();
						targetCrowdPointsComponent.GetEnabledPoints(current.Key, current.Value.Count,
							CrowdUtility.Points);
						CrowdUtility.Points.Shuffle();
						var index = 0;
						while (true)
							if (index < current.Value.Count && index < CrowdUtility.Points.Count) {
								var entity = current.Value[index];
								var point = CrowdUtility.Points[index];
								var outdoorPointInfo = new OutdoorPointInfo();
								outdoorPointInfo.Region = targetCrowdPointsComponent.Owner;
								outdoorPointInfo.Area = point.Area;
								outdoorPointInfo.Radius = point.Radius;
								outdoorPointInfo.CenterPoint = point.CenterPoint;
								outdoorPointInfo.Position = point.Position;
								outdoorPointInfo.Rotation = point.Rotation;
								outdoorPointInfo.Template = entity;
								outdoorPointInfo.EntityPoint = point.EntityPoint;
								outdoorPointInfo.OnNavMesh = point.OnNavMesh;
								outdoorPointInfo.NotReady = point.NotReady;
								points.Add(outdoorPointInfo);
								++index;
							} else
								goto label_26;
					}
				}
			}
		}
	}

	private int GetCount(OutdoorCrowdTemplate template, bool isDay) {
		var crowdTemplateCount = isDay ? template.Day : template.Night;
		return Random.Range(crowdTemplateCount.Min, crowdTemplateCount.Max + 1);
	}

	private void OnEnableChangedEvent() {
		OnInvalidate();
	}

	private void TargetRegionComponent_ChangeDiseaseEvent(int level) {
		diseaseLevel = level;
		Reset();
	}

	private void OnInvalidate() {
		DestroyEntities();
		if (!IsAvailable || initialised)
			return;
		initialised = true;
		ComputePoints();
	}

	private void ComputeModel(IEntity entity) {
		var component1 = entity.GetComponent<DynamicModelComponent>();
		if (component1 == null)
			return;
		var list = component1.Models.ToList();
		if (list.Count == 0)
			return;
		var component2 = entity.GetComponent<ParametersComponent>();
		if (component2 == null)
			return;
		var byName = component2.GetByName<int>(ParameterNameEnum.Model);
		if (byName != null) {
			var index = byName.Value % list.Count;
			var model = list[index];
			component1.Model = model;
		}
	}

	private void DestroyPoints() {
		points.Clear();
	}

	[OnLoaded]
	private void OnLoaded() {
		OnInvalidate();
	}

	public void OnEntityEvent(IEntity sender, EntityEvents kind) {
		if (kind != EntityEvents.EnableChangedEvent)
			return;
		OnEnableChangedEvent();
	}
}