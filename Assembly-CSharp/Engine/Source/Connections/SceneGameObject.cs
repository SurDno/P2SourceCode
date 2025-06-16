using Engine.Common.Generator;
using Inspectors;
using System;

namespace Engine.Source.Connections
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SceneGameObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected Guid id;

    [Inspected]
    public Guid Id => this.id;
  }
}
