// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.IEvent
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using PLVirtualMachine.Common.EngineAPI;
using System.Collections.Generic;

#nullable disable
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
