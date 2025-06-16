// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.IFiniteStateMachine
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.Collections.Generic;

#nullable disable
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
