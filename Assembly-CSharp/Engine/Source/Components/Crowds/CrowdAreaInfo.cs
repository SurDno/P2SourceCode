using Engine.Common.Components.Movable;
using Engine.Common.Generator;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Components.Crowds
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CrowdAreaInfo
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public AreaEnum Area;
    [DataReadProxy(MemberEnum.None, Name = "Templates")]
    [DataWriteProxy(MemberEnum.None, Name = "Templates")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public List<CrowdTemplateInfo> TemplateInfos = new List<CrowdTemplateInfo>();
  }
}
