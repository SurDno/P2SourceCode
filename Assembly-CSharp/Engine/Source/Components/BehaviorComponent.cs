using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.BehaviorNodes;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components;

[Required(typeof(ParametersComponent))]
[Required(typeof(EffectsComponent))]
[Required(typeof(AbilitiesComponent))]
[Factory(typeof(IBehaviorComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class BehaviorComponent :
	EngineComponent,
	IBehaviorComponent,
	IComponent,
	INeedSave,
	IUpdatable {
	[DataReadProxy(Name = "BehaviorTree")]
	[DataWriteProxy(Name = "BehaviorTree")]
	[StateSaveProxy]
	[StateLoadProxy]
	[Inspected]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	[CopyableProxy()]
	protected UnityAsset<ExternalBehaviorTree> behaviorTreeResource;

	private EngineBehavior behavior;
	private Dictionary<string, IEntity> values = new();
	private Dictionary<string, bool> valuesBool = new();
	private Dictionary<string, int> valuesInt = new();
	private Dictionary<string, float> valuesFloat = new();
	private bool geometryVisible;
	private BehaviorTree behaviorTree;
	private bool needUpdate;

	public event Action<string, bool> OnAbility;

	public event Action<IBehaviorComponent> SuccessEvent;

	public event Action<IBehaviorComponent> FailEvent;

	public event Action<IBehaviorComponent, string> CustomEvent;

	public void FireSuccessEvent() {
		var successEvent = SuccessEvent;
		if (successEvent == null)
			return;
		successEvent(this);
	}

	public void FireFailEvent() {
		var failEvent = FailEvent;
		if (failEvent == null)
			return;
		failEvent(this);
	}

	public void FireCustomEvent(string message) {
		var customEvent = CustomEvent;
		if (customEvent == null)
			return;
		customEvent(this, message);
	}

	[Inspected]
	public IBehaviorObject BehaviorObject {
		get => ServiceLocator.GetService<ITemplateService>().GetTemplate<IBehaviorObject>(behaviorTreeResource.Id);
		set {
			var flag = false;
			var component = Owner.GetComponent<NpcControllerComponent>();
			if (component != null && component.IsDead != null)
				flag = component.IsDead.Value;
			if (flag)
				return;
			behaviorTreeResource = new UnityAsset<ExternalBehaviorTree>(value != null ? value.Id : Guid.Empty);
			values.Clear();
			valuesBool.Clear();
			valuesInt.Clear();
			valuesFloat.Clear();
			UpdateData(false);
		}
	}

	[Inspected]
	public IBehaviorObject BehaviorObjectForced {
		get => ServiceLocator.GetService<ITemplateService>().GetTemplate<IBehaviorObject>(behaviorTreeResource.Id);
		set {
			behaviorTreeResource = new UnityAsset<ExternalBehaviorTree>(value != null ? value.Id : Guid.Empty);
			UpdateData(false);
		}
	}

	[Inspected(Mutable = true)]
	public bool GeometryVisible {
		get => geometryVisible;
		set {
			geometryVisible = value;
			if (!(behavior != null))
				return;
			behavior.GeometryVisible = geometryVisible;
		}
	}

	public bool NeedSave => true;

	public void SetValue(string name, IEntity value) {
		values[name] = value;
		UpdateData(false);
	}

	public void SetBoolValue(string name, bool value) {
		valuesBool[name] = value;
		UpdateData(false);
	}

	public void SetIntValue(string name, int value) {
		valuesInt[name] = value;
		UpdateData(false);
	}

	public void SetFloatValue(string name, float value) {
		valuesFloat[name] = value;
		UpdateData(false);
	}

	public void SendEvent(string name) {
		var gameObject = ((IEntityView)Owner).GameObject;
		if (gameObject == null)
			return;
		if (needUpdate)
			UpdateData(true);
		var component = gameObject.GetComponent<BehaviorTree>();
		if (component == null)
			return;
		component.SendEvent(name);
	}

	public void SendEvent<T>(string name, T arg1) {
		var gameObject = ((IEntityView)Owner).GameObject;
		if (gameObject == null)
			return;
		if (needUpdate)
			UpdateData(true);
		var component = gameObject.GetComponent<BehaviorTree>();
		if (component == null)
			return;
		component.SendEvent<object>(name, arg1);
	}

	public override void OnAdded() {
		base.OnAdded();
		((IEntityView)Owner).OnGameObjectChangedEvent += OnGameObjectChangedEvent;
		InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += WorldPauseHandler;
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
		OnGameObjectChangedEvent();
	}

	public override void OnRemoved() {
		behavior = null;
		((IEntityView)Owner).OnGameObjectChangedEvent -= OnGameObjectChangedEvent;
		InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= WorldPauseHandler;
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
		base.OnRemoved();
	}

	private void WorldPauseHandler() {
		if (behaviorTree != null) {
			if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
				behaviorTree.DisableBehavior(true);
			else
				behaviorTree.EnableBehavior();
		}

		if (!(behavior != null))
			return;
		behavior.IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
	}

	private void OnGameObjectChangedEvent() {
		var owner = (IEntityView)Owner;
		if (owner.GameObject == null)
			behavior = null;
		else {
			UpdateData(false);
			behavior = owner.GameObject.GetComponent<EngineBehavior>();
			if (behavior == null)
				return;
			behavior.Gait = EngineBehavior.GaitType.Walk;
			WorldPauseHandler();
		}
	}

	public void SetBehaviorForced(IBehaviorObject behaviorObject) {
		behaviorTreeResource =
			new UnityAsset<ExternalBehaviorTree>(behaviorObject != null ? behaviorObject.Id : Guid.Empty);
		values.Clear();
		valuesBool.Clear();
		valuesInt.Clear();
		valuesFloat.Clear();
		UpdateData(false);
	}

	public void ComputeUpdate() {
		if (!needUpdate)
			return;
		UpdateData(false);
	}

	private void UpdateData(bool forced) {
		if (ServiceCache.OptimizationService.FrameHasSpike && !forced)
			needUpdate = true;
		else {
			ServiceCache.OptimizationService.FrameHasSpike = true;
			needUpdate = false;
			if (this.behaviorTree != null) {
				behaviorTree.UnregisterEvent("Ability", new Action<object, object>(AbilityEvent));
				behaviorTree = null;
			}

			var gameObject = ((IEntityView)Owner).GameObject;
			if (gameObject == null)
				return;
			this.behaviorTree = gameObject.GetComponent<BehaviorTree>();
			if (this.behaviorTree == null)
				return;
			this.behaviorTree.RegisterEvent("Ability", new Action<object, object>(AbilityEvent));
			var externalBehaviorTree = ((BehaviorObject)BehaviorObject)?.ExternalBehaviorTree;
			if (this.behaviorTree.ExternalBehaviorTree != externalBehaviorTree)
				this.behaviorTree.ExternalBehaviorTree = externalBehaviorTree;
			if (values.Count != 0) {
				foreach (var keyValuePair in values)
					behaviorTree.SetVariableValue(keyValuePair.Key, new SharedEntity {
						Entity = keyValuePair.Value
					});
				values.Clear();
			}

			if (valuesBool.Count != 0) {
				foreach (var keyValuePair in valuesBool) {
					var behaviorTree = this.behaviorTree;
					var key = keyValuePair.Key;
					var sharedBool = new SharedBool();
					sharedBool.Value = keyValuePair.Value;
					behaviorTree.SetVariableValue(key, sharedBool);
				}

				valuesBool.Clear();
			}

			if (valuesInt.Count != 0) {
				foreach (var keyValuePair in valuesInt) {
					var behaviorTree = this.behaviorTree;
					var key = keyValuePair.Key;
					var sharedInt = new SharedInt();
					sharedInt.Value = keyValuePair.Value;
					behaviorTree.SetVariableValue(key, sharedInt);
				}

				valuesInt.Clear();
			}

			if (valuesFloat.Count == 0)
				return;
			foreach (var keyValuePair in valuesFloat) {
				var behaviorTree = this.behaviorTree;
				var key = keyValuePair.Key;
				var sharedFloat = new SharedFloat();
				sharedFloat.Value = keyValuePair.Value;
				behaviorTree.SetVariableValue(key, sharedFloat);
			}

			valuesFloat.Clear();
		}
	}

	private void AbilityEvent(object name, object enable) {
		var onAbility = OnAbility;
		if (onAbility == null)
			return;
		onAbility((string)name, (bool)enable);
	}
}