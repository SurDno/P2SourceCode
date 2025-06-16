using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class ResetableFloatParameter : ResetableMinMaxParameter<float>
  {
    protected override bool Compare(float a, float b) => (double) a == (double) b;

    protected override void Correct()
    {
      if (float.IsNaN(this.value))
        this.Value = 0.0f;
      this.Value = Mathf.Clamp(this.value, this.minValue, this.maxValue);
    }
  }
}
