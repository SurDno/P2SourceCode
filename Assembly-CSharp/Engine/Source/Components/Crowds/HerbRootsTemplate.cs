using Engine.Common;
using Engine.Common.Generator;
using Engine.Source.Connections;
using Inspectors;

namespace Engine.Source.Components.Crowds
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class HerbRootsTemplate
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Header = true, Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy(MemberEnum.None)]
    public Typed<IEntity> Template;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Header = true, Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy(MemberEnum.None)]
    public float Weight = 1f;
  }
}
