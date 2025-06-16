// Decompiled with JetBrains decompiler
// Type: Expressions.MinFloatOperation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Inspectors;

#nullable disable
namespace Expressions
{
  [TypeName(TypeName = "[min(a, b)] : float", MenuItem = "min(a, b)/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MinFloatOperation : BinaryOperation<float>
  {
    protected override float Compute(float a, float b) => (double) a < (double) b ? a : b;

    public override string ValueView
    {
      get
      {
        return "min(" + (this.a != null ? this.a.ValueView : "null") + ", " + (this.b != null ? this.b.ValueView : "null") + ")";
      }
    }
  }
}
