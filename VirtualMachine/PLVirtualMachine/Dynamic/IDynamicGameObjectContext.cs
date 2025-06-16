// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.IDynamicGameObjectContext
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common;
using PLVirtualMachine.Objects;
using System;

#nullable disable
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
