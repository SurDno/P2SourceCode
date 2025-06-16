using System;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Services;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("NpcControllerComponent", typeof(INpcControllerComponent))]
public class VMNpcController : VMEngineComponent<INpcControllerComponent> {
	public const string ComponentName = "NpcControllerComponent";

	[Property("Is dead", "", false, true, false)]
	public bool IsDead {
		get {
			if (Component != null)
				return Component.IsDead.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return false;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.IsDead.Value = value;
		}
	}

	[Property("Is immortal", "", false, true, false)]
	public bool IsImmortal {
		get {
			if (Component != null)
				return Component.IsImmortal.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return false;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.IsImmortal.Value = value;
		}
	}

	[Property("Health", "", false, 1f, false)]
	public float Health {
		get {
			if (Component != null)
				return Component.Health.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return 0.0f;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.Health.Value = value;
		}
	}

	[Property("Can autopsy", "", false, true, false)]
	public bool CanAutopsy {
		get {
			if (Component != null)
				return Component.CanAutopsy.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return false;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.CanAutopsy.Value = value;
		}
	}

	[Property("Can trade", "", false, true, false)]
	public bool CanTrade {
		get {
			if (Component != null)
				return Component.CanTrade.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return false;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.CanTrade.Value = value;
		}
	}

	[Property("Force trade", "", false, true, false)]
	public bool ForceTrade {
		get {
			if (Component != null)
				return Component.ForceTrade.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return false;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.ForceTrade.Value = value;
		}
	}

	[Property("Can heal", "", false, true, false)]
	public bool CanHeal {
		get {
			if (Component != null)
				return Component.CanHeal.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return false;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.CanHeal.Value = value;
		}
	}

	[Property("Infection", "", false, 0.0f, false)]
	public float Infection {
		get {
			if (Component != null)
				return Component.Infection.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return 0.0f;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.Infection.Value = value;
		}
	}

	[Property("PreInfection", "", false, 0.0f, false)]
	public float PreInfection {
		get {
			if (Component != null)
				return Component.PreInfection.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return 0.0f;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.PreInfection.Value = value;
		}
	}

	[Property("Pain", "", false, 0.0f, false)]
	public float Pain {
		get {
			if (Component != null)
				return Component.Pain.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return 0.0f;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.Pain.Value = value;
		}
	}

	[Property("Immunity", "", false, 1f, false)]
	public float Immunity {
		get {
			if (Component != null)
				return Component.Immunity.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return 0.0f;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.Immunity.Value = value;
		}
	}

	[Property("Is away", "")] public bool IsAway => Component.IsAway.Value;

	[Property("Stamm Kind", "")]
	public StammKind StammKind {
		get {
			if (Component != null)
				return Component.StammKind.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return StammKind.Unknown;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.StammKind.Value = value;
		}
	}

	[Property("Fraction", "", false, FractionEnum.Civilian, false)]
	public FractionEnum Fraction {
		get {
			if (Component != null)
				return Component.Fraction.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return FractionEnum.None;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.Fraction.Value = value;
		}
	}

	[Property("Combat style", "", false, CombatStyleEnum.Default, false)]
	public CombatStyleEnum CombatStyle {
		get {
			if (Component != null)
				return Component.CombatStyle.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return CombatStyleEnum.None;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.CombatStyle.Value = value;
		}
	}

	[Property("Bound health state", "", false, BoundHealthStateEnum.Normal, false)]
	public BoundHealthStateEnum BoundHealthState {
		get {
			if (Component != null)
				return Component.BoundHealthState.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return BoundHealthStateEnum.None;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.BoundHealthState.Value = value;
		}
	}

	[Property("Healing attempted", "", false, false, false)]
	public bool HealingAttempted {
		get {
			if (Component != null)
				return Component.HealingAttempted.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return false;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.HealingAttempted.Value = value;
		}
	}

	[Property("Immune boost attempted", "", false, false, false)]
	public bool ImmuneBoostAttempted {
		get {
			if (Component != null)
				return Component.ImmuneBoostAttempted.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return false;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.ImmuneBoostAttempted.Value = value;
		}
	}

	[Property("Is combat ignored", "", false, false, false)]
	public bool IsCombatIgnored {
		get {
			if (Component != null)
				return Component.IsCombatIgnored.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return false;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.IsCombatIgnored.Value = value;
		}
	}

	[Property("Say replics in idle", "", false, true, false)]
	public bool SayReplicsInIdle {
		get {
			if (Component != null)
				return Component.SayReplicsInIdle.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return false;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.SayReplicsInIdle.Value = value;
		}
	}

	[Method("Add Personal Enemy", "target", "")]
	public void AddPersonalEnemy(IEntity target) {
		ServiceLocator.GetService<ICombatService>().AddPersonalEnemy(Component.Owner, target);
	}

	[Method("Remove Personal Enemy", "target", "")]
	public void RemovePersonalEnemy(IEntity target) {
		ServiceLocator.GetService<ICombatService>().RemovePersonalEnemy(Component.Owner, target);
	}

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.ActionEvent -= OnActionEvent;
		Component.CombatActionEvent -= OnCombatActionEvent;
		Component.Health.ChangeValueEvent -= ChangeHealthValueEvent;
		Component.Pain.ChangeValueEvent -= ChangePainValueEvent;
		Component.IsAway.ChangeValueEvent -= ChangeIsAwayValueEvent;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.ActionEvent += OnActionEvent;
		Component.CombatActionEvent += OnCombatActionEvent;
		Component.Health.ChangeValueEvent += ChangeHealthValueEvent;
		Component.Pain.ChangeValueEvent += ChangePainValueEvent;
		Component.IsAway.ChangeValueEvent += ChangeIsAwayValueEvent;
	}

	private void ChangeIsAwayValueEvent(bool value) {
		var changeAwayEvent = ChangeAwayEvent;
		if (changeAwayEvent == null)
			return;
		changeAwayEvent(value);
	}

	private void ChangePainValueEvent(float value) {
		var onChangePain = OnChangePain;
		if (onChangePain == null)
			return;
		onChangePain(value);
	}

	private void ChangeHealthValueEvent(float value) {
		var onChangeHealth = OnChangeHealth;
		if (onChangeHealth == null)
			return;
		onChangeHealth(value);
	}

	private void OnActionEvent(ActionEnum action) {
		var actionEvent = ActionEvent;
		if (actionEvent == null)
			return;
		actionEvent(action);
	}

	private void OnCombatActionEvent(CombatActionEnum action, IEntity target) {
		var combatActionEvent = CombatActionEvent;
		if (combatActionEvent == null)
			return;
		combatActionEvent(action, target);
	}

	[Event("Action", "Action type")] public event Action<ActionEnum> ActionEvent;

	[Event("CombatAction", "Action type, Entity")]
	public event Action<CombatActionEnum, IEntity> CombatActionEvent;

	[Event("Change health", "Value")] public event Action<float> OnChangeHealth;

	[Event("Change pain", "Value")] public event Action<float> OnChangePain;

	[Event("Change away", "Value")] public event Action<bool> ChangeAwayEvent;
}