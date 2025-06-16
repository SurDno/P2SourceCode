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
