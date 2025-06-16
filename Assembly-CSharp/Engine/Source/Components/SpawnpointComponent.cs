using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Milestone;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory(typeof (ISpawnpointComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SpawnpointComponent : EngineComponent, ISpawnpointComponent, IComponent
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Kind type = Kind.Point;

    public Kind Type => this.type;
  }
}
