using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Expressions;
using Inspectors;
using Scripts.Expressions.Commons;

namespace Engine.Source.Effects.Engine;

public abstract class EffectContextValueAssignment<T> : IEffectValueSetter where T : struct {
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected]
	[Inspected(Name = "a", Mutable = true, Mode = ExecuteMode.Edit)]
	protected IValueSetter<T> a;

	[DataReadProxy(Name = "Source")]
	[DataWriteProxy(Name = "Source")]
	[CopyableProxy()]
	[Inspected]
	[Inspected(Name = "b", Mutable = true, Mode = ExecuteMode.Edit)]
	protected IValue<T> b;

	private ExpressionViewWrapper view = new();

	[Inspected(Mode = ExecuteMode.EditAndRuntime)]
	private ExpressionViewWrapper ValueViewWrapper {
		get {
			view.Value = ValueView;
			return view;
		}
	}

	public abstract string ValueView { get; }

	public abstract string TypeView { get; }

	protected abstract T Compute(T a, T b);

	public void Compute(IEffect context) {
		if (b == null || a == null)
			return;
		a.SetValue(Compute(a.GetValue(context), b.GetValue(context)), context);
	}
}