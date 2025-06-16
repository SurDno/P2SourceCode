using Cofe.Utility;
using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Expressions;
using Inspectors;
using UnityEngine;

namespace Scripts.Expressions.Commons
{
  [TypeName(TypeName = "[random] : float", MenuItem = "random/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RandomValue : IValue<float>
  {
    public float GetValue(IEffect context) => Random.value;

    public string ValueView => "random";

    public string TypeView => TypeUtility.GetTypeName(GetType());
  }
}
