using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory(typeof (IBuildingComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class BuildingComponent : EngineComponent, IBuildingComponent, IComponent
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected BuildingEnum building;

    public BuildingEnum Building => this.building;
  }
}
