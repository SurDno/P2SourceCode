using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;

namespace Expressions;

public abstract class UnaryOperation<T> : IValue<T> where T : struct {
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy()]
	[Inspected]
	[Inspected(Name = "value", Mutable = true, Mode = ExecuteMode.Edit)]
	protected IValue<T> value;

	protected abstract T Compute(T value);

	public T GetValue(IEffect context) {
		return value != null ? Compute(value.GetValue(context)) : default;
	}

	protected abstract string OperatorView();

	public string ValueView => OperatorView() + (value != null ? value.ValueView : "null");

	public string TypeView => OperatorView() + (value != null ? value.TypeView : "null");
}