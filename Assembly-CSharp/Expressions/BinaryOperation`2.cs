using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;

namespace Expressions
{
  public abstract class BinaryOperation<T, TResult> : IValue<TResult>
    where T : struct
    where TResult : struct
  {
    [DataReadProxy(Name = "Left")]
    [DataWriteProxy(Name = "Left")]
    [CopyableProxy]
    [Inspected]
    [Inspected(Name = "a", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<T> a;
    [DataReadProxy(Name = "Right")]
    [DataWriteProxy(Name = "Right")]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Name = "b", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<T> b;

    protected abstract TResult Compute(T a, T b);

    protected virtual string OperatorView() => "???";

    public TResult GetValue(IEffect context)
    {
      return a != null && b != null ? Compute(a.GetValue(context), b.GetValue(context)) : default (TResult);
    }

    public virtual string ValueView
    {
      get
      {
        return "(" + (a != null ? a.ValueView : "null") + " " + OperatorView() + " " + (b != null ? b.ValueView : "null") + ")";
      }
    }

    public virtual string TypeView
    {
      get
      {
        return "(" + (a != null ? a.TypeView : "null") + " " + OperatorView() + " " + (b != null ? b.TypeView : "null") + ")";
      }
    }
  }
}
