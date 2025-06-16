// Decompiled with JetBrains decompiler
// Type: Expressions.PlusIntPolyOperation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Inspectors;

#nullable disable
namespace Expressions
{
  [TypeName(TypeName = "[a + ... + z] : int", MenuItem = "a + ... + z/int")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PlusIntPolyOperation : PolyOperation<int>
  {
    protected override int Compute(int a, int b) => a + b;

    protected override string OperatorView() => "+";
  }
}
