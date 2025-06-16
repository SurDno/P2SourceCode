using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class ContainerOpenStateEnumParameter : Parameter<ContainerOpenStateEnum>
  {
    protected override bool Compare(ContainerOpenStateEnum a, ContainerOpenStateEnum b) => a == b;
  }
}
