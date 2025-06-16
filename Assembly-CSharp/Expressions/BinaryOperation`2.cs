using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;

namespace Expressions
{
  public abstract class BinaryOperation<T, TResult> : IValue<TResult>
    where T : struct
    where TResult : struct
  {
    [DataReadProxy(MemberEnum.None, Name = "Left")]
    [DataWriteProxy(MemberEnum.None, Name = "Left")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Name = "a", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<T> a;
    [DataReadProxy(MemberEnum.None, Name = "Right")]
    [DataWriteProxy(MemberEnum.None, Name = "Right")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Name = "b", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<T> b;

    protected abstract TResult Compute(T a, T b);

    protected virtual string OperatorView() => "???";

    public TResult GetValue(IEffect context)
    {
      return this.a != null && this.b != null ? this.Compute(this.a.GetValue(context), this.b.GetValue(context)) : default (TResult);
    }

    public virtual string ValueView
    {
      get
      {
        return "(" + (this.a != null ? this.a.ValueView : "null") + " " + this.OperatorView() + " " + (this.b != null ? this.b.ValueView : "null") + ")";
      }
    }

    public virtual string TypeView
    {
      get
      {
        return "(" + (this.a != null ? this.a.TypeView : "null") + " " + this.OperatorView() + " " + (this.b != null ? this.b.TypeView : "null") + ")";
      }
    }
  }
}
