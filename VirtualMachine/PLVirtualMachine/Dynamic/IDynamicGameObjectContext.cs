using System;
using PLVirtualMachine.Common;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic
{
  public interface IDynamicGameObjectContext
  {
    Guid DynamicGuid { get; }

    ulong StaticGuid { get; }

    VMEntity Entity { get; }

    VMLogicObject FSMStaticObject { get; }

    IState CurrentState { get; }

    IParam GetContextParam(string paramName);

    IParam GetContextParam(ulong stGuid);

    EventMessage GetContextMessage(string messageName);

    BaseFunction GetContextFunction(string functionName);

    DynamicEvent GetContextEvent(string eventName);

    DynamicEvent GetContextEvent(ulong eventId);

    object GetLocalVariableValue(string varName);

    bool IsStaticDerived(IBlueprint blueprint);

    bool Active { get; }

    IGameMode GameTimeContext { get; }

    EEventRaisingMode FsmEventProcessingMode { get; }
  }
}
