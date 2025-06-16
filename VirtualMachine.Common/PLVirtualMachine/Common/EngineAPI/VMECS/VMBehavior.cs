using System;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("BehaviorComponent", typeof(IBehaviorComponent))]
public class VMBehavior : VMEngineComponent<IBehaviorComponent> {
	public const string ComponentName = "BehaviorComponent";

	[Event("Behavior Success", "")] public event Action Success;

	[Event("Behavior Fail", "")] public event Action Fail;

	[Event("Behavior Custom", "value")] public event Action<string> Custom;

	[Method("SetValue", "name,object", "")]
	public void SetValue(string name, IEntity value) {
		Component.SetValue(name, value);
	}

	[Method("SetBoolValue", "name,value", "")]
	public void SetBoolValue(string name, bool value) {
		Component.SetBoolValue(name, value);
	}

	[Method("SetIntValue", "name,value", "")]
	public void SetIntValue(string name, int value) {
		Component.SetIntValue(name, value);
	}

	[Method("SetFloatValue", "name,value", "")]
	public void SetFloatValue(string name, float value) {
		Component.SetFloatValue(name, value);
	}

	[Method("SetBehaviorForced", "behavior", "")]
	public void SetBehaviorForced(IBehaviorObject behavior) {
		Component.SetBehaviorForced(behavior);
	}

	[Property("BehaviorObject", "", false)]
	public IBehaviorObject Behavior {
		get {
			if (Component != null)
				return Component.BehaviorObject;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return null;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				try {
					Component.BehaviorObject = value;
				} catch (Exception ex) {
					Logger.AddError(string.Format("Behavior object set error: {0} at {1}", ex, Parent.Name));
				}
		}
	}

	[Property("BehaviorObjectForced", "", false)]
	public IBehaviorObject BehaviorForced {
		get {
			if (Component != null)
				return Component.BehaviorObjectForced;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return null;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				try {
					Component.BehaviorObjectForced = value;
				} catch (Exception ex) {
					Logger.AddError(string.Format("Behavior object set error: {0} at {1}", ex, Parent.Name));
				}
		}
	}

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.SuccessEvent -= SuccessEvent;
		Component.FailEvent -= FailEvent;
		Component.CustomEvent -= CustomEvent;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.SuccessEvent += SuccessEvent;
		Component.FailEvent += FailEvent;
		Component.CustomEvent += CustomEvent;
	}

	private void SuccessEvent(IBehaviorComponent target) {
		var success = Success;
		if (success == null)
			return;
		success();
	}

	private void FailEvent(IBehaviorComponent target) {
		var fail = Fail;
		if (fail == null)
			return;
		fail();
	}

	private void CustomEvent(IBehaviorComponent target, string message) {
		var custom = Custom;
		if (custom == null)
			return;
		custom(message);
	}
}