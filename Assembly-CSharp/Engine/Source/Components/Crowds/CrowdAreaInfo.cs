using System.Collections.Generic;
using Engine.Common.Components.Movable;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Components.Crowds
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CrowdAreaInfo
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public AreaEnum Area;
    [DataReadProxy(Name = "Templates")]
    [DataWriteProxy(Name = "Templates")]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public List<CrowdTemplateInfo> TemplateInfos = [];
  }
}
