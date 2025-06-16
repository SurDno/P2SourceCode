using System;
using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.Detectablies;
using Engine.Source.Settings.External;
using Engine.Source.Utility;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components;

[Required(typeof(ParametersComponent))]
[Factory(typeof(IDetectorComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class DetectorComponent :
	EngineComponent,
	IDetectorComponent,
	IComponent,
	IUpdatable,
	INeedSave {
	[StateSaveProxy]
	[StateLoadProxy]
	[DataReadProxy]
	[DataWriteProxy]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	[CopyableProxy()]
	protected bool isEnabled = true;

	[Inspected] private List<DetectableCandidatInfo> eyeCandidates = new();
	[Inspected] private List<DetectableCandidatInfo> hearingCandidates = new();
	private HashSet<IDetectableComponent> visible = new();
	private HashSet<IDetectableComponent> hearing = new();
	[FromThis] private ILocationItemComponent locationItem;
	[FromThis] private ParametersComponent parametersComponent;
	private static HashSet<IDetectableComponent> tmps = new();
	private static HashSet<IDetectableComponent> tmps2 = new();
	private static List<RaycastHit> raycastBuffer = new();
	private IParameter<float> eyeDistanceParameter;
	private IParameter<float> eyeAngleParameter;
	private IParameter<float> hearingDistanceParameter;
	private bool isIndoor;
	private GameObject gameObject;
	private Pivot pivot;
	private bool updateSkipped;

	[Inspected(Mutable = true)]
	public bool IsEnabled {
		get => isEnabled;
		set {
			isEnabled = value;
			OnChangeEnabled();
		}
	}

	[Inspected] public HashSet<IDetectableComponent> Visible => visible;

	[Inspected] public HashSet<IDetectableComponent> Hearing => hearing;

	[Inspected] public float EyeDistance => eyeDistanceParameter != null ? eyeDistanceParameter.Value : 0.0f;

	[Inspected] public float BaseEyeDistance => eyeDistanceParameter != null ? eyeDistanceParameter.BaseValue : 0.0f;

	[Inspected] public float EyeAngle => eyeAngleParameter != null ? eyeAngleParameter.Value : 0.0f;

	[Inspected]
	public float HearingDistance => hearingDistanceParameter != null ? hearingDistanceParameter.Value : 0.0f;

	[Inspected]
	public float BaseHearingDistance => hearingDistanceParameter != null ? hearingDistanceParameter.BaseValue : 0.0f;

	public bool NeedSave {
		get {
			if (!(Owner.Template is IEntity template)) {
				Debug.LogError("Template not found, owner : " + Owner.GetInfo());
				return true;
			}

			var component = template.GetComponent<DetectorComponent>();
			if (component == null) {
				Debug.LogError(GetType().Name + " not found, owner : " + Owner.GetInfo());
				return true;
			}

			return isEnabled != component.isEnabled;
		}
	}

	public event Action<IDetectableComponent> OnSee;

	public event Action<IDetectableComponent> OnStopSee;

	public event Action<IDetectableComponent> OnHear;

	public event Action<IDetectableComponent> OnStopHear;

	public override void OnAdded() {
		base.OnAdded();
		if (parametersComponent != null) {
			eyeDistanceParameter = parametersComponent.GetByName<float>(ParameterNameEnum.EyeDistance);
			eyeAngleParameter = parametersComponent.GetByName<float>(ParameterNameEnum.EyeAngle);
			hearingDistanceParameter = parametersComponent.GetByName<float>(ParameterNameEnum.HearingDistance);
		}

		locationItem.OnHibernationChanged += LocationItemOnChangeHibernation;
		locationItem.OnChangeLocation += LocationItemOnLocationChanged;
		isIndoor = locationItem.IsIndoor;
		InstanceByRequest<UpdateService>.Instance.DetectorUpdater.AddUpdatable(this);
		((IEntityView)Owner).OnGameObjectChangedEvent += Owner_OnGameObjectChangedEvent;
	}

	public override void OnRemoved() {
		((IEntityView)Owner).OnGameObjectChangedEvent -= Owner_OnGameObjectChangedEvent;
		locationItem.OnHibernationChanged -= LocationItemOnChangeHibernation;
		locationItem.OnChangeLocation -= LocationItemOnLocationChanged;
		locationItem = null;
		InstanceByRequest<UpdateService>.Instance.DetectorUpdater.RemoveUpdatable(this);
		base.OnRemoved();
	}

	private void Owner_OnGameObjectChangedEvent() {
		gameObject = ((IEntityView)Owner).GameObject;
		pivot = gameObject != null ? gameObject.GetComponent<Pivot>() : null;
	}

	public void ComputeUpdate() {
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !Owner.IsEnabledInHierarchy || !IsEnabled ||
		    locationItem.IsHibernation)
			return;
		var owner = (IEntityView)Owner;
		if (!owner.IsAttached)
			return;
		if (gameObject == null)
			throw new Exception("gameObject == null , owner : " + Owner.GetInfo());
		if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ReduceUpdateFarObjects &&
		    !DetectorUtility.CheckDistance(owner.Position, EngineApplication.PlayerPosition,
			    ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ReduceUpdateFarObjectsDistance)) {
			updateSkipped = !updateSkipped;
			if (updateSkipped)
				return;
		}

		ComputeCandidats();
		ComputeEye();
		ComputeHearing();
	}

	private void ComputeHearing() {
		ComputeHearing(hearingCandidates);
	}

	private void ComputeEye() {
		ComputeEye(eyeCandidates);
	}

	private void ComputeEye(List<DetectableCandidatInfo> candidates) {
		if (pivot == null || pivot.Head == null)
			return;
		tmps.Clear();
		var eyeAngle = EyeAngle;
		var eyeDistance = EyeDistance;
		for (var index1 = 0; index1 < candidates.Count; ++index1) {
			var candidate = candidates[index1];
			var position = ((IEntityView)candidate.Detectable.Owner).Position;
			var forward = position - ((IEntityView)Owner).Position;
			var magnitude1 = forward.magnitude;
			var rotation = ((IEntityView)Owner).Rotation;
			var quaternion = Quaternion.identity;
			if (!Mathf.Approximately(magnitude1, 0.0f))
				quaternion = rotation * Quaternion.Inverse(Quaternion.LookRotation(forward));
			var f = Mathf.DeltaAngle(quaternion.eulerAngles.y, 0.0f);
			if (Mathf.Abs(f) < eyeAngle * 0.5) {
				var num1 = FunctionUtility.EyeFunction(f + eyeAngle * 0.5f, eyeAngle);
				var num2 = candidate.Detectable.VisibleDistance + eyeDistance * num1;
				if (magnitude1 <= (double)num2) {
					var direction = position + candidate.Offset - pivot.Head.transform.position;
					var magnitude2 = direction.magnitude;
					var raycastHit1 = new RaycastHit();
					var num3 = float.MaxValue;
					var triggerInteractLayer = ScriptableObjectInstance<GameSettingsData>.Instance.TriggerInteractLayer;
					PhysicsUtility.Raycast(raycastBuffer, pivot.Head.transform.position, direction, magnitude2,
						-1 ^ triggerInteractLayer);
					for (var index2 = 0; index2 < raycastBuffer.Count; ++index2) {
						var raycastHit2 = raycastBuffer[index2];
						if (!raycastHit2.collider.isTrigger && raycastHit2.distance < (double)num3) {
							raycastHit1 = raycastHit2;
							num3 = raycastHit2.distance;
						}
					}

					if (num3 <= (double)magnitude2 && !(raycastHit1.collider == null) &&
					    !(raycastHit1.collider.gameObject != candidate.GameObject))
						tmps.Add(candidate.Detectable);
				}
			}
		}

		foreach (var tmp in tmps)
			if (visible.Add(tmp)) {
				var onSee = OnSee;
				if (onSee != null)
					onSee(tmp);
			}

		tmps2.Clear();
		foreach (var detectableComponent in visible)
			if (!tmps.Contains(detectableComponent)) {
				tmps2.Add(detectableComponent);
				var onStopSee = OnStopSee;
				if (onStopSee != null)
					onStopSee(detectableComponent);
			}

		foreach (var detectableComponent in tmps2)
			visible.Remove(detectableComponent);
	}

	private void ComputeHearing(List<DetectableCandidatInfo> candidates) {
		tmps.Clear();
		var hearingDistance = HearingDistance;
		for (var index = 0; index < candidates.Count; ++index) {
			var candidate = candidates[index];
			if (isIndoor) {
				if (!DetectorUtility.CanHear(gameObject, ((IEntityView)Owner).Position, candidate.GameObject,
					    ((IEntityView)candidate.Detectable.Owner).Position, hearingDistance,
					    candidate.Detectable.NoiseDistance))
					continue;
			} else if ((((IEntityView)candidate.Detectable.Owner).Position - ((IEntityView)Owner).Position).magnitude >
			           hearingDistance + (double)candidate.Detectable.NoiseDistance)
				continue;

			tmps.Add(candidate.Detectable);
		}

		foreach (var tmp in tmps)
			if (hearing.Add(tmp)) {
				var onHear = OnHear;
				if (onHear != null)
					onHear(tmp);
			}

		tmps2.Clear();
		foreach (var detectableComponent in hearing)
			if (!tmps.Contains(detectableComponent))
				tmps2.Add(detectableComponent);
		foreach (var detectableComponent in tmps2) {
			var onStopHear = OnStopHear;
			if (onStopHear != null)
				onStopHear(detectableComponent);
			hearing.Remove(detectableComponent);
		}
	}

	private void LocationItemOnChangeHibernation(ILocationItemComponent sender) {
		if (!locationItem.IsHibernation)
			return;
		Cleanup();
	}

	private void LocationItemOnLocationChanged(
		ILocationItemComponent locationItem,
		ILocationComponent location) {
		isIndoor = locationItem.IsIndoor;
	}

	private void Cleanup() {
		foreach (var detectableComponent in visible) {
			var onStopSee = OnStopSee;
			if (onStopSee != null)
				onStopSee(detectableComponent);
		}

		visible.Clear();
		hearing.Clear();
	}

	public void ComputeCandidats() {
		eyeCandidates.Clear();
		hearingCandidates.Clear();
		if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableDetectors)
			return;
		var detectablies = ServiceLocator.GetService<DetectorService>().Detectablies;
		var detectorComponent = this;
		var locationItem = this.locationItem;
		if (!detectorComponent.Owner.IsEnabledInHierarchy || !detectorComponent.IsEnabled || locationItem == null ||
		    locationItem.IsHibernation)
			return;
		var owner1 = (IEntityView)detectorComponent.Owner;
		if (!owner1.IsAttached)
			return;
		var eyeDistance = EyeDistance;
		var hearingDistance = HearingDistance;
		var position1 = owner1.Position;
		foreach (var detectableCandidatInfo1 in detectablies) {
			var owner2 = (IEntityView)detectableCandidatInfo1.Detectable.Owner;
			if (owner2.IsAttached) {
				var detectableCandidatInfo2 = detectableCandidatInfo1;
				var position2 = owner2.Position;
				var flag1 = DetectorUtility.CheckDistance(position1, position2,
					eyeDistance + detectableCandidatInfo2.Detectable.BaseVisibleDistance);
				var flag2 = DetectorUtility.CheckDistance(position1, position2,
					hearingDistance + detectableCandidatInfo2.Detectable.NoiseDistance);
				if ((flag1 || flag2) && detectableCandidatInfo1.Detectable.Owner.IsEnabledInHierarchy &&
				    detectableCandidatInfo1.Detectable.IsEnabled &&
				    !detectableCandidatInfo2.LocationItem.IsHibernation &&
				    LocationItemUtility.CheckLocation(locationItem, detectableCandidatInfo2.LocationItem) &&
				    detectorComponent.Owner != detectableCandidatInfo1.Detectable.Owner) {
					if (flag1)
						eyeCandidates.Add(detectableCandidatInfo2);
					if (flag2)
						hearingCandidates.Add(detectableCandidatInfo2);
				}
			}
		}
	}
}