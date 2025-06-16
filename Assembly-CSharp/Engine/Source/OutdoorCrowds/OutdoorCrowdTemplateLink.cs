using Engine.Common.Components.Movable;
using Engine.Common.Generator;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.OutdoorCrowds
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class OutdoorCrowdTemplateLink
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy(MemberEnum.None)]
    public string Link;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy(MemberEnum.None)]
    public List<AreaEnum> Areas = new List<AreaEnum>();
  }
}
