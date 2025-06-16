using Engine.Common.Generator;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.OutdoorCrowds
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class OutdoorCrowdTemplates
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public string Name;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public List<OutdoorCrowdTemplate> Templates = new List<OutdoorCrowdTemplate>();
  }
}
