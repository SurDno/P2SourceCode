using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public interface IFiniteStateMachine : 
    IState,
    IGraphObject,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    ILocalContext,
    IGraph
  {
    EGraphType GraphType { get; }

    IState InitState { get; }

    List<IState> States { get; }

    IState GetSubgraphExitState();

    bool IsSubGraph { get; }

    bool Abstract { get; }

    List<InputParam> InputParams { get; }

    IFiniteStateMachine SubstituteGraph { get; }

    List<IFiniteStateMachine> GetSubGraphStructure(EGraphType graphType = EGraphType.GRAPH_TYPE_ALL, bool withBaseClasses = true);

    IState GetStateByGuid(ulong stateId, bool withBaseClasses = true);
  }
}
