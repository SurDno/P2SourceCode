// Decompiled with JetBrains decompiler
// Type: Expressions.MinTimeSpanOperation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Inspectors;
using System;

#nullable disable
namespace Expressions
{
  [TypeName(TypeName = "[min(a, b)] : Time", MenuItem = "min(a, b)/Time")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MinTimeSpanOperation : BinaryOperation<TimeSpan>
  {
    protected override TimeSpan Compute(TimeSpan a, TimeSpan b) => a < b ? a : b;

    public override string ValueView
    {
      get
      {
        return "min(" + (this.a != null ? this.a.ValueView : "null") + ", " + (this.b != null ? this.b.ValueView : "null") + ")";
      }
    }
  }
}
