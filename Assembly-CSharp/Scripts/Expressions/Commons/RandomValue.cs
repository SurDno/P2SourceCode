// Decompiled with JetBrains decompiler
// Type: Scripts.Expressions.Commons.RandomValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Expressions;
using Inspectors;
using UnityEngine;

#nullable disable
namespace Scripts.Expressions.Commons
{
  [TypeName(TypeName = "[random] : float", MenuItem = "random/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RandomValue : IValue<float>
  {
    public float GetValue(IEffect context) => Random.value;

    public string ValueView => "random";

    public string TypeView => TypeUtility.GetTypeName(this.GetType());
  }
}
