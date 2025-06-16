using System;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("BlueprintComponent", typeof(IBlueprintComponent))]
public class VMBlueprintComponent : VMEngineComponent<IBlueprintComponent> {
	public const string ComponentName = "BlueprintComponent";

	[Property("BlueprintObject", "", false)]
	public IBlueprintObject Blueprint {
		get => Component.Blueprint;
		set => Component.Blueprint = value;
	}

	[Property("Is Started", "", false)] public bool IsStarted => Component.IsStarted;

	[Property("Is Attached", "", false)] public bool IsAttached => Component.IsAttached;

	[Method("Start", "", "")]
	public void Start() {
		try {
			Component.Start();
		} catch (Exception ex) {
			Logger.AddError(string.Format("Blueprint component start method error: {0} in object {1} at {2}", ex,
				Parent.Name, EngineAPIManager.Instance.CurrentFSMStateInfo));
		}
	}

	[Method("Start with target", "IEntity", "")]
	public void Start_v1(IEntity target) {
		try {
			Component.Start(target);
		} catch (Exception ex) {
			Logger.AddError(string.Format("Blueprint component start method error: {0} in object {1} at {2}", ex,
				Parent.Name, EngineAPIManager.Instance.CurrentFSMStateInfo));
		}
	}

	[Method("Stop", "", "")]
	public void Stop() {
		Component.Stop();
	}

	[Method("Send event", "name", "")]
	public void SendEvent(string name) {
		try {
			Component.SendEvent(name);
		} catch (Exception ex) {
			Logger.AddError(string.Format("Blueprint component send event method error: {0} in object {1} at {2}", ex,
				Parent.Name, EngineAPIManager.Instance.CurrentFSMStateInfo));
		}
	}

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.CompleteEvent -= CompleteEvent;
		Component.AttachEvent -= AttachEvent;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.CompleteEvent += CompleteEvent;
		Component.AttachEvent += AttachEvent;
	}

	private void CompleteEvent(IBlueprintComponent target) {
		var complete = Complete;
		if (complete == null)
			return;
		complete();
	}

	private void AttachEvent(IBlueprintComponent target) {
		var attach = Attach;
		if (attach == null)
			return;
		attach();
	}

	[Event("Complete", "")] public event Action Complete;

	[Event("Attach", "")] public event Action Attach;
}