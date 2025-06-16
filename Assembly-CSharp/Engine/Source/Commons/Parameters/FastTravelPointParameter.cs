using Engine.Common.Components.Regions;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class FastTravelPointParameter : Parameter<FastTravelPointEnum>
  {
    protected override bool Compare(FastTravelPointEnum a, FastTravelPointEnum b) => a == b;
  }
}
