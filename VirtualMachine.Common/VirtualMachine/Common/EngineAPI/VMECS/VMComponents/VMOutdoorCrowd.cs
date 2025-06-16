using System;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Crowds;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace VirtualMachine.Common.EngineAPI.VMECS.VMComponents;

[Info("OutdoorCrowdComponent", typeof(IOutdoorCrowdComponent))]
public class VMOutdoorCrowd : VMEngineComponent<IOutdoorCrowdComponent> {
	public const string ComponentName = "OutdoorCrowdComponent";

	[Property("Layout", "")]
	public OutdoorCrowdLayoutEnum Layout {
		get => Component.Layout;
		set => Component.Layout = value;
	}

	[Event("Need create object event", "template object", false)]
	public event NeedCreateObjectEventType NeedCreateObjectEvent;

	[Event("Need delete object event", "object", false)]
	public event Action<IEntity> NeedDeleteObjectEvent;

	[Method("Add entity", "Target", "")]
	public void AddEntity(IEntity entity) {
		try {
			Component.AddEntity(entity);
		} catch (Exception ex) {
			Logger.AddError(string.Format("Outdoor crowd entity adding error at {0}: {1} !", Parent.Name, ex));
		}
	}

	[Method("Reset", "", "")]
	public void Reset() {
		Component.Reset();
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

	private void OnDeleteEntity(IEntity entity) {
		var deleteObjectEvent = NeedDeleteObjectEvent;
		if (deleteObjectEvent == null)
			return;
		deleteObjectEvent(entity);
	}

	private void OnCreateEntity(IEntity entity) {
		var createObjectEvent = NeedCreateObjectEvent;
		if (createObjectEvent == null)
			return;
		createObjectEvent(entity);
	}

	public delegate void NeedCreateObjectEventType([Template] IEntity entity);
}