// Decompiled with JetBrains decompiler
// Type: Expressions.PlusTimeSpanOperation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Inspectors;
using System;

#nullable disable
namespace Expressions
{
  [TypeName(TypeName = "[a + b] : Time", MenuItem = "a + b/Time")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PlusTimeSpanOperation : BinaryOperation<TimeSpan>
  {
    protected override TimeSpan Compute(TimeSpan a, TimeSpan b) => a + b;

    protected override string OperatorView() => "+";
  }
}
