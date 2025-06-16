using System;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;

namespace Engine.Common.Components;

public interface INpcControllerComponent : IComponent {
	event Action<ActionEnum> ActionEvent;

	event Action<CombatActionEnum, IEntity> CombatActionEvent;

	IParameterValue<bool> IsDead { get; }

	IParameterValue<bool> IsImmortal { get; }

	IParameterValue<bool> IsAway { get; }

	IParameterValue<bool> CanAutopsy { get; }

	IParameterValue<bool> CanTrade { get; }

	IParameterValue<bool> ForceTrade { get; }

	IParameterValue<bool> CanHeal { get; }

	IParameterValue<float> Health { get; }

	IParameterValue<float> Infection { get; }

	IParameterValue<float> PreInfection { get; }

	IParameterValue<float> Pain { get; }

	IParameterValue<float> Immunity { get; }

	IParameterValue<StammKind> StammKind { get; }

	IParameterValue<FractionEnum> Fraction { get; }

	IParameterValue<CombatStyleEnum> CombatStyle { get; }

	IParameterValue<BoundHealthStateEnum> BoundHealthState { get; }

	IParameterValue<bool> HealingAttempted { get; }

	IParameterValue<bool> ImmuneBoostAttempted { get; }

	IParameterValue<bool> IsCombatIgnored { get; }

	IParameterValue<bool> SayReplicsInIdle { get; }
}