using System;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.BoundCharacters;
using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("BoundCharacterComponent", typeof(IBoundCharacterComponent))]
public class VMBoundCharacterComponent : VMEngineComponent<IBoundCharacterComponent> {
	public const string ComponentName = "BoundCharacterComponent";
	private ITextRef boundCharacterName;

	[Property("Bound health state ", "")]
	public BoundHealthStateEnum BoundHealthState {
		get => Component.BoundHealthState.Value;
		set {
			try {
				Component.BoundHealthState.Value = value;
			} catch (Exception ex) {
				Logger.AddError(string.Format("BoundHealthState set error: {0} at {1}", ex, Parent.Name));
			}
		}
	}

	[Property("Group", "")]
	public BoundCharacterGroup Group {
		get => Component.Group;
		set {
			try {
				Component.Group = value;
			} catch (Exception ex) {
				Logger.AddError(string.Format("Group set error: {0} at {1}", ex, Parent.Name));
			}
		}
	}

	[Property("Is Discovered", "", false, false)]
	public bool Discovered {
		get => Component.Discovered;
		set {
			try {
				Component.Discovered = value;
			} catch (Exception ex) {
				Logger.AddError(string.Format("Discovered set error: {0} at {1}", ex, Parent.Name));
			}
		}
	}

	[Property("Is enabled", "", false, false)]
	public bool IsEnabled {
		get => Component.IsEnabled;
		set {
			try {
				Component.IsEnabled = value;
			} catch (Exception ex) {
				Logger.AddError(string.Format("IsEnabled set error: {0} at {1}", ex, Parent.Name));
			}
		}
	}

	[Property("Name", "", false)]
	public ITextRef NameText {
		get => boundCharacterName;
		set {
			boundCharacterName = value;
			Component.Name = EngineAPIManager.CreateEngineTextInstance(boundCharacterName);
		}
	}

	[Property("Random Roll", "")]
	public float RandomRoll {
		get => Component.RandomRoll.Value;
		set {
			try {
				Component.RandomRoll.Value = value;
			} catch (Exception ex) {
				Logger.AddError(string.Format("Random Roll set error: {0} at {1}", ex, Parent.Name));
			}
		}
	}

	[Property("Sort order", "", false)]
	public int SortOrder {
		get => Component.SortOrder;
		set {
			try {
				Component.SortOrder = value;
			} catch (Exception ex) {
				Logger.AddError(string.Format("SortOrder set error: {0} at {1}", ex, Parent.Name));
			}
		}
	}

	[Property("Resource", "", false)]
	public IBoundCharacterPlaceholder Object {
		get => Component.Resource;
		set {
			try {
				Component.Resource = value;
			} catch (Exception ex) {
				Logger.AddError(string.Format("Resource set error: {0} at {1}", ex, Parent.Name));
			}
		}
	}

	[Property("Home Region", "", false)]
	public IEntity HomeRegion {
		get => Component.HomeRegion;
		set {
			try {
				Component.HomeRegion = value;
			} catch (Exception ex) {
				Logger.AddError(string.Format("Home Region set error: {0} at {1}", ex, Parent.Name));
			}
		}
	}

	[Method("Store Pre Roll State", "", "")]
	public void StorePreRollState() {
		if (Component == null)
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
		else
			Component.StorePreRollState();
	}

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.BoundHealthState.ChangeValueEvent -= ChangeBoundHealthStateEvent;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.BoundHealthState.ChangeValueEvent += ChangeBoundHealthStateEvent;
	}

	private void ChangeBoundHealthStateEvent(BoundHealthStateEnum value) {
		var boundHealthState = OnChangeBoundHealthState;
		if (boundHealthState == null)
			return;
		boundHealthState(value);
	}

	[Event("Change bound health state", "Value")]
	public event Action<BoundHealthStateEnum> OnChangeBoundHealthState;
}