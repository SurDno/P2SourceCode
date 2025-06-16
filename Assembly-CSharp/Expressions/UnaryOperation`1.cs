// Decompiled with JetBrains decompiler
// Type: Expressions.UnaryOperation`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;

#nullable disable
namespace Expressions
{
  public abstract class UnaryOperation<T> : IValue<T> where T : struct
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Name = "value", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<T> value;

    protected abstract T Compute(T value);

    public T GetValue(IEffect context)
    {
      return this.value != null ? this.Compute(this.value.GetValue(context)) : default (T);
    }

    protected abstract string OperatorView();

    public string ValueView
    {
      get => this.OperatorView() + (this.value != null ? this.value.ValueView : "null");
    }

    public string TypeView
    {
      get => this.OperatorView() + (this.value != null ? this.value.TypeView : "null");
    }
  }
}
