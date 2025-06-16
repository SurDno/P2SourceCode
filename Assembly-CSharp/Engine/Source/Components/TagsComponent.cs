using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Components
{
  [Factory(typeof (ITagsComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class TagsComponent : EngineComponent, ITagsComponent, IComponent
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<EntityTagEnum> tags = new List<EntityTagEnum>();

    public IEnumerable<EntityTagEnum> Tags => (IEnumerable<EntityTagEnum>) this.tags;
  }
}
