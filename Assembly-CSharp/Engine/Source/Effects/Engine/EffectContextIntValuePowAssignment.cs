// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.Engine.EffectContextIntValuePowAssignment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a ^= b] : int", MenuItem = "a ^= b/int")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextIntValuePowAssignment : EffectContextValueAssignment<int>
  {
    protected override int Compute(int a, int b) => (int) Mathf.Pow((float) a, (float) b);

    public override string ValueView
    {
      get
      {
        return (this.a != null ? this.a.ValueView : "null") + " ^= " + (this.b != null ? this.b.ValueView : "null");
      }
    }

    public override string TypeView
    {
      get
      {
        return (this.a != null ? this.a.TypeView : "null") + " ^= " + (this.b != null ? this.b.TypeView : "null");
      }
    }
  }
}
