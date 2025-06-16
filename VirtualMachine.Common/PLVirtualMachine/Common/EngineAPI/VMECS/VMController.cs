using System;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("Controller", typeof(IControllerComponent))]
public class VMController : VMEngineComponent<IControllerComponent> {
	public const string ComponentName = "Controller";

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.BeginInteractEvent -= FireBeginInteractEvent;
		Component.EndInteractEvent -= FireEndInteractEvent;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.BeginInteractEvent += FireBeginInteractEvent;
		Component.EndInteractEvent += FireEndInteractEvent;
	}

	private void FireBeginInteractEvent(
		IEntity owner,
		IInteractableComponent target,
		IInteractItem item) {
		var controllIteractEvent = BeginControllIteractEvent;
		if (controllIteractEvent == null)
			return;
		controllIteractEvent(owner, target.Owner, item.Type);
	}

	private void FireEndInteractEvent(
		IEntity owner,
		IInteractableComponent target,
		IInteractItem item) {
		var controllIteractEvent = EndControllIteractEvent;
		if (controllIteractEvent == null)
			return;
		controllIteractEvent(owner, target.Owner, item.Type);
	}

	[Event("Begin interact event", "агент:Controller,цель:Interactive,тип")]
	public event Action<IEntity, IEntity, InteractType> BeginControllIteractEvent;

	[Event("End interact event", "агент:Controller,цель:Interactive,тип")]
	public event Action<IEntity, IEntity, InteractType> EndControllIteractEvent;
}