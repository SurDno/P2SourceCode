using Engine.Common.Components.Crowds;
using Engine.Common.Generator;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.OutdoorCrowds
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class OutdoorCrowdLayout
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public OutdoorCrowdLayoutEnum Layout;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public List<OutdoorCrowdState> States = new List<OutdoorCrowdState>();
  }
}
