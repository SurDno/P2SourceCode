// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.INpcControllerComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using System;

#nullable disable
namespace Engine.Common.Components
{
  public interface INpcControllerComponent : IComponent
  {
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

    IParameterValue<Engine.Common.Commons.StammKind> StammKind { get; }

    IParameterValue<FractionEnum> Fraction { get; }

    IParameterValue<CombatStyleEnum> CombatStyle { get; }

    IParameterValue<BoundHealthStateEnum> BoundHealthState { get; }

    IParameterValue<bool> HealingAttempted { get; }

    IParameterValue<bool> ImmuneBoostAttempted { get; }

    IParameterValue<bool> IsCombatIgnored { get; }

    IParameterValue<bool> SayReplicsInIdle { get; }
  }
}
