using System;
using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace VirtualMachine.Common.EngineAPI.VMECS.VMComponents;

[Info("IndoorCrowdComponent", typeof(IIndoorCrowdComponent))]
public class VMIndoorCrowd : VMEngineComponent<IIndoorCrowdComponent> {
	public const string ComponentName = "IndoorCrowdComponent";

	[Event("Need create object event", "template object", false)]
	public event NeedCreateObjectEventType NeedCreateObjectEvent;

	[Event("Need delete object event", "object", false)]
	public event Action<IEntity> NeedDeleteObjectEvent;

	[Method("Add entity", "Target", "")]
	public void AddEntity(IEntity entity) {
		Component.AddEntity(entity);
	}

	[Method("Reset", "", "")]
	public void Reset() {
		Component.Reset();
	}

	public void OnCreateEntity(IEntity entity) {
		EngineAPIManager.ObjectCreationExtraDebugInfoMode = true;
		var createObjectEvent = NeedCreateObjectEvent;
		if (createObjectEvent != null)
			createObjectEvent(entity);
		EngineAPIManager.ObjectCreationExtraDebugInfoMode = false;
	}

	public void OnDeleteEntity(IEntity entity) {
		EngineAPIManager.ObjectCreationExtraDebugInfoMode = true;
		var deleteObjectEvent = NeedDeleteObjectEvent;
		if (deleteObjectEvent != null)
			deleteObjectEvent(entity);
		EngineAPIManager.ObjectCreationExtraDebugInfoMode = false;
	}

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.OnCreateEntity -= OnCreateEntity;
		Component.OnDeleteEntity -= OnDeleteEntity;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.OnCreateEntity += OnCreateEntity;
		Component.OnDeleteEntity += OnDeleteEntity;
	}

	public delegate void NeedCreateObjectEventType([Template] IEntity entity);
}