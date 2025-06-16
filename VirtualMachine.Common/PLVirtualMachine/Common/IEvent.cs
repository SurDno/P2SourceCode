using PLVirtualMachine.Common.EngineAPI;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public interface IEvent : 
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable
  {
    ICondition Condition { get; }

    IParam EventParameter { get; }

    GameTime EventTime { get; }

    bool ChangeTo { get; }

    bool Repeated { get; }

    bool IsInitial(IObject obj);

    string FunctionalName { get; }

    IGameMode GameTimeContext { get; }

    bool IsManual { get; }

    EEventRaisingType EventRaisingType { get; }

    List<BaseMessage> ReturnMessages { get; }
  }
}
