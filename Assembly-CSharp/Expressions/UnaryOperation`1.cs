using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;

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
