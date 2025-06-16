using Engine.Common.Generator;
using Engine.Source.Connections;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Components.Selectors
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SelectorPreset
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public List<SceneGameObject> Objects = new List<SceneGameObject>();
  }
}
