using System;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;

namespace Engine.Common.Components;

public interface IPlayerControllerComponent : IComponent {
	event Action<CombatActionEnum, IEntity> CombatActionEvent;

	IParameterValue<bool> IsDead { get; }

	IParameterValue<bool> IsImmortal { get; }

	IParameterValue<float> Health { get; }

	IParameterValue<float> Hunger { get; }

	IParameterValue<float> Thirst { get; }

	IParameterValue<float> Fatigue { get; }

	IParameterValue<float> Reputation { get; }

	IParameterValue<float> PreInfection { get; }

	IParameterValue<float> Infection { get; }

	IParameterValue<float> Immunity { get; }

	IParameterValue<bool> Sleep { get; }

	IParameterValue<bool> CanTrade { get; }

	IParameterValue<FractionEnum> Fraction { get; }

	IParameterValue<bool> FundEnabled { get; }

	IParameterValue<bool> FundFinished { get; }

	IParameterValue<float> FundPoints { get; }

	IParameterValue<bool> CanReceiveMail { get; }
}