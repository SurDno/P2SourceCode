// Decompiled with JetBrains decompiler
// Type: Expressions.NotEqualStammKindOperations
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Common.Generator;
using Inspectors;

#nullable disable
namespace Expressions
{
  [TypeName(TypeName = "[a != b] : StammKind", MenuItem = "a != b/StammKind")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NotEqualStammKindOperations : ComparisonOperation<StammKind>
  {
    protected override bool Compute(StammKind a, StammKind b) => a != b;

    protected override string OperatorView() => "!=";
  }
}
