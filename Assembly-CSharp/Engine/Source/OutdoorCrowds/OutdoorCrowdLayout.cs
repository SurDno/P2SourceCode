using System.Collections.Generic;
using Engine.Common.Components.Crowds;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.OutdoorCrowds
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class OutdoorCrowdLayout
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public OutdoorCrowdLayoutEnum Layout;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public List<OutdoorCrowdState> States = new List<OutdoorCrowdState>();
  }
}
