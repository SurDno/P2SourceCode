using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("Gate", typeof(IDoorComponent))]
[Depended("Interactive")]
public class VMDoor : VMEngineComponent<IDoorComponent> {
	public const string ComponentName = "Gate";
	private PriorityParameterEnum defaultPriority = PriorityParameterEnum.Default;

	[Property("IsFree", "")]
	public bool IsFree {
		get => Component.IsFree.Value;
		set => Component.IsFree.SetValue(defaultPriority, value);
	}

	[Method("Set IsFree value", "Priority,Value", "")]
	public void SetIsFreeValue(PriorityParameterEnum priority, bool value) {
		Component.IsFree.SetValue(priority, value);
	}

	[Method("Reset IsFree value", "Priority", "")]
	public void ResetIsFreeValue(PriorityParameterEnum priority) {
		Component.IsFree.ResetValue(priority);
	}

	[Property("Bolted", "")]
	public bool Bolted {
		get => Component.Bolted.Value;
		set => Component.Bolted.SetValue(defaultPriority, value);
	}

	[Method("Set Bolted value", "Priority,Value", "")]
	public void SetBoltedValue(PriorityParameterEnum priority, bool value) {
		Component.Bolted.SetValue(priority, value);
	}

	[Method("Reset Bolted value", "Priority", "")]
	public void ResetBoltedValue(PriorityParameterEnum priority) {
		Component.Bolted.ResetValue(priority);
	}

	[Property("Marked", "")]
	public bool Marked {
		get => Component.Marked.Value;
		set => Component.Marked.SetValue(defaultPriority, value);
	}

	[Property("CanBeMarked", "")]
	public bool CanBeMarked {
		get => Component.CanBeMarked.Value;
		set => Component.CanBeMarked.Value = value;
	}

	[Property("Knockable", "")]
	public bool Knockable {
		get => Component.Knockable.Value;
		set => Component.Knockable.Value = value;
	}

	[Property("Pickable", "")]
	public bool Pickable {
		get => Component.Pickable.Value;
		set => Component.Pickable.Value = value;
	}

	[Property("Difficulty", "")]
	public int Difficulty {
		get => Component.Difficulty.Value;
		set => Component.Difficulty.Value = value;
	}

	[Method("Set Marked value", "Priority,Value", "")]
	public void SetMarkedValue(PriorityParameterEnum priority, bool value) {
		Component.Marked.SetValue(priority, value);
	}

	[Method("Reset Marked value", "Priority", "")]
	public void ResetMarkedValue(PriorityParameterEnum priority) {
		Component.Marked.ResetValue(priority);
	}

	[Property("Opened", "")]
	public bool Opened {
		get => Component.Opened.Value;
		set => Component.Opened.SetValue(defaultPriority, value);
	}

	[Method("Set Opened value", "Priority,Value", "")]
	public void SetOpenedValue(PriorityParameterEnum priority, bool value) {
		Component.Opened.SetValue(priority, value);
	}

	[Method("Set Opened value by type", "Priority,Value,is outdoor", "")]
	public void SetOpenedValueByType(PriorityParameterEnum priority, bool value, bool isOutdoor) {
		if (Component.IsOutdoor != isOutdoor)
			return;
		Component.Opened.SetValue(priority, value);
	}

	[Method("Reset Opened value", "Priority", "")]
	public void ResetOpenedValue(PriorityParameterEnum priority) {
		Component.Opened.ResetValue(priority);
	}

	[Property("Lock State", "")]
	public LockState LockState {
		get => Component.LockState.Value;
		set => Component.LockState.Value = value;
	}

	[Method("Set LockState value", "Priority,Value", "")]
	public void SetLockStateValue(PriorityParameterEnum priority, LockState value) {
		Component.LockState.SetValue(priority, value);
	}

	[Method("Set LockState value by type", "Priority,Value,is outdoor", "")]
	public void SetLockStateValueByType(
		PriorityParameterEnum priority,
		LockState value,
		bool isOutdoor) {
		if (Component.IsOutdoor != isOutdoor)
			return;
		Component.LockState.SetValue(priority, value);
	}

	[Method("Reset LockState value", "Priority", "")]
	public void ResetLockStateValue(PriorityParameterEnum priority) {
		Component.LockState.ResetValue(priority);
	}

	[Property("Min Reputation", "")]
	public float MinReputation {
		get => Component.MinReputation;
		set => Component.MinReputation = value;
	}

	[Property("Max Reputation", "")]
	public float MaxReputation {
		get => Component.MaxReputation;
		set => Component.MaxReputation = value;
	}

	[Property("Is Outdoor", "")] public bool IsOutdoor => Component.IsOutdoor;

	[Method("Add lock pick", "Storable", "")]
	public void AddPicklock([Template] IEntity storable) {
		if (storable == null)
			Logger.AddError(string.Format("Storable template for Picklock adding not defined at {0} !",
				EngineAPIManager.Instance.CurrentFSMStateInfo));
		else
			Component.AddPicklock(storable);
	}

	[Method("Remove lock pick", "Storable", "")]
	public void RemoveLockPick([Template] IEntity storable) {
		if (storable == null)
			Logger.AddError(string.Format("Storable template for Picklock removing not defined at {0} !",
				EngineAPIManager.Instance.CurrentFSMStateInfo));
		else
			Component.RemovePicklock(storable);
	}

	[Method("Add picklock", "Priority,Storable", "")]
	public void AddPicklock_v1(PriorityParameterEnum priority, [Template] IEntity storable) {
		if (storable == null)
			Logger.AddError(string.Format("Storable template for Picklock adding not defined at {0} !",
				EngineAPIManager.Instance.CurrentFSMStateInfo));
		else
			Component.AddPicklock(priority, storable);
	}

	[Method("Remove picklock", "Priority,Storable", "")]
	public void RemovePicklock(PriorityParameterEnum priority, [Template] IEntity storable) {
		if (storable == null)
			Logger.AddError(string.Format("Storable template for Picklock removing not defined at {0} !",
				EngineAPIManager.Instance.CurrentFSMStateInfo));
		else
			Component.RemovePicklock(priority, storable);
	}

	[Method("Reset picklocks", "Priority", "")]
	public void ResetPicklocks(PriorityParameterEnum priority) {
		Component.ResetPicklocks(priority);
	}

	[Method("Add key", "Storable", "")]
	public void AddKey([Template] IEntity storable) {
		Component.AddKey(storable);
	}

	[Method("Remove key", "Storable", "")]
	public void RemoveKey([Template] IEntity storable) {
		Component.RemoveKey(storable);
	}

	[Method("Add key", "Priority,Storable", "")]
	public void AddKey_v1(PriorityParameterEnum priority, [Template] IEntity storable) {
		Component.AddKey(priority, storable);
	}

	[Method("Remove key", "Priority,Storable", "")]
	public void RemoveKey_v1(PriorityParameterEnum priority, [Template] IEntity storable) {
		Component.RemoveKey(priority, storable);
	}

	[Method("Reset keys", "Priority", "")]
	public void ResetKeys(PriorityParameterEnum priority) {
		Component.ResetKeys(priority);
	}

	public void SetDefaultPriority(PriorityParameterEnum defPriority) {
		defaultPriority = defPriority;
	}
}