using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Detectors;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components;

[Required(typeof(ParametersComponent))]
[Factory(typeof(IDetectableComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class DetectableComponent : EngineComponent, IDetectableComponent, IComponent, INeedSave {
	[StateSaveProxy]
	[StateLoadProxy]
	[DataReadProxy]
	[DataWriteProxy]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	[CopyableProxy()]
	protected bool isEnabled = true;

	private IParameter<float> visibleDistanceParameter;
	private IParameter<DetectType> visibleDetectTypeParameter;
	private IParameter<float> noiseDistanceParameter;
	private IParameter<DetectType> noiseDetectTypeParameter;
	[FromThis] private ParametersComponent parametersComponent;
	[FromThis] private LocationItemComponent locationItemComponent;

	[Inspected(Mutable = true)]
	public bool IsEnabled {
		get => isEnabled;
		set {
			isEnabled = value;
			OnChangeEnabled();
		}
	}

	[Inspected]
	public float BaseVisibleDistance => visibleDistanceParameter != null ? visibleDistanceParameter.BaseValue : 0.0f;

	[Inspected]
	public float VisibleDistance => visibleDistanceParameter != null ? visibleDistanceParameter.Value : 0.0f;

	[Inspected]
	public DetectType VisibleDetectType =>
		visibleDetectTypeParameter != null ? visibleDetectTypeParameter.Value : DetectType.None;

	[Inspected] public float NoiseDistance => noiseDistanceParameter != null ? noiseDistanceParameter.Value : 0.0f;

	[Inspected]
	public DetectType NoiseDetectType =>
		noiseDetectTypeParameter != null ? noiseDetectTypeParameter.Value : DetectType.None;

	public bool NeedSave {
		get {
			if (!(Owner.Template is IEntity template)) {
				Debug.LogError("Template not found, owner : " + Owner.GetInfo());
				return true;
			}

			var component = template.GetComponent<DetectableComponent>();
			if (component == null) {
				Debug.LogError(GetType().Name + " not found, owner : " + Owner.GetInfo());
				return true;
			}

			return isEnabled != component.isEnabled;
		}
	}

	public override void OnAdded() {
		base.OnAdded();
		if (parametersComponent != null) {
			visibleDistanceParameter = parametersComponent.GetByName<float>(ParameterNameEnum.VisibleDistance);
			visibleDetectTypeParameter = parametersComponent.GetByName<DetectType>(ParameterNameEnum.VisibleDetectType);
			noiseDistanceParameter = parametersComponent.GetByName<float>(ParameterNameEnum.NoiseDistance);
			noiseDetectTypeParameter = parametersComponent.GetByName<DetectType>(ParameterNameEnum.NoiseDetectType);
		}

		((IEntityView)Owner).OnGameObjectChangedEvent += OnGameObjectChangedEvent;
		locationItemComponent.OnHibernationChanged += OnHibernationChanged;
		UpdateSunscribe();
	}

	private void OnHibernationChanged(ILocationItemComponent sender) {
		UpdateSunscribe();
	}

	private void OnGameObjectChangedEvent() {
		UpdateSunscribe();
	}

	public override void OnChangeEnabled() {
		base.OnChangeEnabled();
		if (locationItemComponent == null)
			return;
		UpdateSunscribe();
	}

	public override void OnRemoved() {
		visibleDistanceParameter = null;
		noiseDistanceParameter = null;
		noiseDistanceParameter = null;
		noiseDetectTypeParameter = null;
		((IEntityView)Owner).OnGameObjectChangedEvent -= OnGameObjectChangedEvent;
		locationItemComponent.OnHibernationChanged -= OnHibernationChanged;
		ServiceLocator.GetService<DetectorService>().RemoveDetectable(this);
		base.OnRemoved();
	}

	private void UpdateSunscribe() {
		if (!locationItemComponent.IsHibernation && ((IEntityView)Owner).IsAttached && Owner.IsEnabledInHierarchy &&
		    IsEnabled)
			ServiceLocator.GetService<DetectorService>().AddDetectable(this);
		else
			ServiceLocator.GetService<DetectorService>().RemoveDetectable(this);
	}
}