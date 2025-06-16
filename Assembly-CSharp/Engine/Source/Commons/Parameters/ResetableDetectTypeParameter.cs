using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class ResetableDetectTypeParameter : ResetableParameter<DetectType>
  {
    protected override bool Compare(DetectType a, DetectType b) => a == b;
  }
}
