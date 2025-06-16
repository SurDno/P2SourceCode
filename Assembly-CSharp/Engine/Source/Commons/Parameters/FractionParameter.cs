using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class FractionParameter : Parameter<FractionEnum>
  {
    protected override bool Compare(FractionEnum a, FractionEnum b) => a == b;
  }
}
