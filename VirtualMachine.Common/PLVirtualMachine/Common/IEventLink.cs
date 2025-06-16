// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.IEventLink
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

#nullable disable
namespace PLVirtualMachine.Common
{
  public interface IEventLink : 
    ILink,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    ILocalContext
  {
    IState SourceState { get; }

    int SourceExitPoint { get; }

    IState DestState { get; }

    int DestEntryPoint { get; }

    EventInfo Event { get; }

    ELinkExitType LinkExitType { get; }

    bool ExitFromSubGraph { get; }

    bool Enabled { get; }

    bool IsInitial();
  }
}
