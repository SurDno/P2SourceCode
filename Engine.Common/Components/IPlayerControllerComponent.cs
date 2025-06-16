// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IPlayerControllerComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using System;

#nullable disable
namespace Engine.Common.Components
{
  public interface IPlayerControllerComponent : IComponent
  {
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
}
