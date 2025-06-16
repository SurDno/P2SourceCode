using System;
using System.Collections.Generic;
using System.Linq;
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
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine.Source.Components;

[Factory]
[Required(typeof(LocationItemComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class IndoorCrowdComponent :
	EngineComponent,
	IIndoorCrowdComponent,
	IComponent,
	IUpdatable,
	IEntityEventsListener {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected SceneGameObject region;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected List<CrowdAreaInfo> areas = new();

	[FromThis] private LocationItemComponent locationItem;
	[FromLocator] private ITemplateService templateService;
	[FromLocator] private OutdoorCrowdService сrowdService;
	[Inspected] private List<IndoorPointInfo> points = new();
	[Inspected] private IndoorPointInfo waitingPoint;
	private bool initialise;
	[Inspected] private bool pointsReady;
	private CrowdPointsComponent targetCrowdPointsComponent;

	public event Action<IEntity> OnCreateEntity;

	public event Action<IEntity> OnDeleteEntity;

	private bool IsAvailable => pointsReady && Owner.IsEnabledInHierarchy;

	public override void OnAdded() {
		base.OnAdded();
		UpdateRandom();
		locationItem.OnHibernationChanged += OnChangeHibernation;
		OnChangeHibernation(locationItem);
		((Entity)Owner).AddListener(this);
		OnEnableChangedEvent();
		InstanceByRequest<UpdateService>.Instance.IndoorCrowdUpdater.AddUpdatable(this);
	}

	public override void OnRemoved() {
		InstanceByRequest<UpdateService>.Instance.IndoorCrowdUpdater.RemoveUpdatable(this);
		((Entity)Owner).RemoveListener(this);
		locationItem.OnHibernationChanged -= OnChangeHibernation;
		locationItem = null;
		if (targetCrowdPointsComponent != null) {
			targetCrowdPointsComponent.ChangePointsEvent -= CrowdPointsComponent_ChangePointsEvent;
			targetCrowdPointsComponent = null;
		}

		base.OnRemoved();
	}

	private void UpdateRandom() {
		foreach (var area in areas)
			if (area != null)
				foreach (var templateInfo in area.TemplateInfos)
					templateInfo.Seed = 0;
			else
				Debug.LogWarning("info == null : " + Owner.GetInfo());
	}

	private void OnChangeHibernation(ILocationItemComponent sender) {
		if (locationItem.IsHibernation)
			return;
		var gameObject1 = ((IEntityView)Owner).GameObject;
		if (gameObject1 == null)
			Debug.LogError("GameObject not found , " + Owner.GetInfo());
		else {
			var container = SceneObjectContainer.GetContainer(gameObject1.scene);
			if (container == null)
				Debug.LogError("SceneObjectContainer not found in scene " + gameObject1.scene.name + " , " +
				               Owner.GetInfo());
			else {
				var gameObject2 = container.GetGameObject(region.Id);
				if (gameObject2 == null)
					Debug.LogError(string.Format("GameObject region not found : {0} , {1}", region.Id,
						Owner.GetInfo()));
				else {
					var entity = EntityUtility.GetEntity(gameObject2);
					if (entity == null)
						Debug.LogError("Entity not found : " + Owner.GetInfo(), gameObject2);
					else {
						targetCrowdPointsComponent = entity.GetComponent<CrowdPointsComponent>();
						if (targetCrowdPointsComponent == null)
							Debug.LogError("CrowdPointsComponent not found : " + Owner.GetInfo(), gameObject2);
						else {
							targetCrowdPointsComponent.ChangePointsEvent += CrowdPointsComponent_ChangePointsEvent;
							if (!targetCrowdPointsComponent.PointsReady)
								return;
							CrowdPointsComponent_ChangePointsEvent(targetCrowdPointsComponent,
								targetCrowdPointsComponent.PointsReady);
						}
					}
				}
			}
		}
	}

	private void CrowdPointsComponent_ChangePointsEvent(
		ICrowdPointsComponent crowdPointsComponent,
		bool pointsReady) {
		this.pointsReady = pointsReady;
		if (pointsReady)
			ComputePoints();
		else
			DestroyEntities();
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
				waitingPoint.Entity = entity;
				var component1 = entity.GetComponent<CrowdItemComponent>();
				if (component1 != null)
					component1.AttachToCrowd(Owner, waitingPoint);
				else
					Debug.LogError("CrowdItemComponent not found : " + entity.GetInfo());
				foreach (var component2 in entity.GetComponents<ICrowdContextComponent>())
					component2.RestoreState(waitingPoint.States, true);
				waitingPoint.States.Clear();
				var component3 = entity.GetComponent<NavigationComponent>();
				if (component3 != null) {
					var parentComponent =
						LocationItemUtility.FindParentComponent<ILocationComponent>(waitingPoint.Region);
					if (component1 != null)
						component3.TeleportTo(parentComponent, component1.Point.Position, component1.Point.Rotation);
					else {
						var position = waitingPoint.Position;
						if (waitingPoint.OnNavMesh)
							NavMeshUtility.SamplePosition(ref position, AreaEnum.All.ToMask(), 32);
						component3.TeleportTo(parentComponent, position, waitingPoint.Rotation);
					}
				} else
					Debug.LogError("NavigationComponent not found : " + entity.GetInfo());

				if (!IsAvailable)
					RemoveEntity(waitingPoint);
				else
					ComputeModel(waitingPoint.Entity);
			}
		}
	}

	private void RemoveEntity(IndoorPointInfo info) {
		var entity = info.Entity;
		info.Entity = null;
		info.States.Clear();
		foreach (var component in entity.GetComponents<ICrowdContextComponent>())
			component.StoreState(info.States, true);
		entity.GetComponent<CrowdItemComponent>()?.DetachFromCrowd();
		var onDeleteEntity = OnDeleteEntity;
		if (onDeleteEntity == null)
			return;
		onDeleteEntity(entity);
	}

	public void Reset() {
		DestroyEntities();
		UpdateRandom();
	}

	private void ComputePoints() {
		if (initialise)
			return;
		initialise = true;
		foreach (var area in areas) {
			CrowdUtility.Points.Clear();
			targetCrowdPointsComponent.GetEnabledPoints(area.Area, area.TemplateInfos.Sum(o => o.Max),
				CrowdUtility.Points);
			CrowdUtility.Points.Shuffle();
			var index1 = 0;
			foreach (var templateInfo in area.TemplateInfos) {
				if (templateInfo.Seed == 0)
					templateInfo.Seed = Random.Range(int.MinValue, int.MaxValue);
				Random.InitState(templateInfo.Seed);
				var num = Random.Range(templateInfo.Min, templateInfo.Max + 1);
				if (templateInfo.Chance < (double)Random.value)
					num = 0;
				for (var index2 = 0; index2 < num; ++index2)
					if (index1 < CrowdUtility.Points.Count) {
						var point = CrowdUtility.Points[index1];
						var indoorPointInfo = new IndoorPointInfo();
						indoorPointInfo.Region = targetCrowdPointsComponent.Owner;
						indoorPointInfo.Area = point.Area;
						indoorPointInfo.Position = point.Position;
						indoorPointInfo.Rotation = point.Rotation;
						indoorPointInfo.TemplateInfo = templateInfo;
						indoorPointInfo.Template =
							templateInfo.Templates[Random.Range(0, templateInfo.Templates.Count)].Value;
						indoorPointInfo.EntityPoint = point.EntityPoint;
						indoorPointInfo.OnNavMesh = point.OnNavMesh;
						points.Add(indoorPointInfo);
						++index1;
					}
			}
		}
	}

	private void DestroyEntities() {
		foreach (var point in points)
			if (point.Entity != null)
				RemoveEntity(point);
	}

	public void ComputeUpdate() {
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !IsAvailable || waitingPoint != null)
			return;
		waitingPoint = points.FirstOrDefaultNoAlloc(o => o.Entity == null && o.Template != null);
		if (waitingPoint == null)
			return;
		DynamicModelComponent.GroupContext = "[IndoorCrowds]";
		var onCreateEntity = OnCreateEntity;
		if (onCreateEntity == null)
			return;
		onCreateEntity(waitingPoint.Template);
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

	private void OnEnableChangedEvent() {
		if (Owner.IsEnabledInHierarchy)
			return;
		DestroyEntities();
	}

	public void OnEntityEvent(IEntity sender, EntityEvents kind) {
		if (kind != EntityEvents.EnableChangedEvent)
			return;
		OnEnableChangedEvent();
	}
}