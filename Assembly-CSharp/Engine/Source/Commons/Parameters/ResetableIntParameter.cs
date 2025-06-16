using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class ResetableIntParameter : ResetableMinMaxParameter<int>
  {
    protected override bool Compare(int a, int b) => a == b;

    protected override void Correct()
    {
      this.Value = Mathf.Clamp(this.value, this.minValue, this.maxValue);
    }
  }
}
