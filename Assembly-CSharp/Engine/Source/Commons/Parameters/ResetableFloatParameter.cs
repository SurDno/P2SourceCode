using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class ResetableFloatParameter : ResetableMinMaxParameter<float>
  {
    protected override bool Compare(float a, float b) => a == (double) b;

    protected override void Correct()
    {
      if (float.IsNaN(value))
        Value = 0.0f;
      Value = Mathf.Clamp(value, minValue, maxValue);
    }
  }
}
