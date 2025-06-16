using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public interface IState : 
    IGraphObject,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    ILocalContext
  {
    List<IEntryPoint> EntryPoints { get; }

    int GetExitPointsCount();

    bool Initial { get; }

    bool IgnoreBlock { get; }

    bool IsProcedure { get; }

    bool IsStable { get; }

    EStateType StateType { get; }
  }
}
