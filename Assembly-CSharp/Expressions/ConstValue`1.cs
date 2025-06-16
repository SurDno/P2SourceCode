// Decompiled with JetBrains decompiler
// Type: Expressions.ConstValue`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;

#nullable disable
namespace Expressions
{
  public abstract class ConstValue<T> : IValue<T> where T : struct
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true)]
    [Inspected(Name = "value", Mutable = true, Mode = ExecuteMode.Edit)]
    protected T value;

    public T GetValue(IEffect context) => this.value;

    public string ValueView => this.value.ToString();

    public string TypeView => TypeUtility.GetTypeName(this.GetType());
  }
}
