using System.Collections.Generic;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;

namespace Engine.Common.Components
{
  public interface IDoorComponent : IComponent
  {
    IPriorityParameterValue<bool> IsFree { get; }

    IPriorityParameterValue<bool> Opened { get; }

    IPriorityParameterValue<bool> Bolted { get; }

    IPriorityParameterValue<bool> Marked { get; }

    IPriorityParameterValue<bool> SendEnterWithoutKnock { get; }

    IPriorityParameterValue<LockState> LockState { get; }

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
