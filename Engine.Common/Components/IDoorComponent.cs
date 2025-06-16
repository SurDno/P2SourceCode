// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IDoorComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Components.Parameters;
using System.Collections.Generic;

#nullable disable
namespace Engine.Common.Components
{
  public interface IDoorComponent : IComponent
  {
    IPriorityParameterValue<bool> IsFree { get; }

    IPriorityParameterValue<bool> Opened { get; }

    IPriorityParameterValue<bool> Bolted { get; }

    IPriorityParameterValue<bool> Marked { get; }

    IPriorityParameterValue<bool> SendEnterWithoutKnock { get; }

    IPriorityParameterValue<Engine.Common.Components.Gate.LockState> LockState { get; }

    IParameterValue<bool> CanBeMarked { get; }

    IParameterValue<bool> Knockable { get; }

    IParameterValue<bool> Pickable { get; }

    IParameterValue<int> Difficulty { get; }

    float MinReputation { get; set; }

    float MaxReputation { get; set; }

    bool IsOutdoor { get; }

    IEnumerable<IEntity> Picklocks { get; }

    void AddPicklock(IEntity item);

    void RemovePicklock(IEntity item);

    void AddPicklock(PriorityParameterEnum priority, IEntity item);

    void RemovePicklock(PriorityParameterEnum priority, IEntity item);

    void ResetPicklocks(PriorityParameterEnum priority);

    IEnumerable<IEntity> Keys { get; }

    void AddKey(IEntity item);

    void RemoveKey(IEntity item);

    void AddKey(PriorityParameterEnum priority, IEntity item);

    void RemoveKey(PriorityParameterEnum priority, IEntity item);

    void ResetKeys(PriorityParameterEnum priority);
  }
}
