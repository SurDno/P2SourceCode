using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class StammKindParameter : Parameter<StammKind>
  {
    protected override bool Compare(StammKind a, StammKind b) => a == b;
  }
}
