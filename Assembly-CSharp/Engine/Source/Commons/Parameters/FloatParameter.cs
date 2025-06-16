// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Parameters.FloatParameter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class FloatParameter : MinMaxParameter<float>
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
