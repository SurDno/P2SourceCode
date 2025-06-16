using System.Collections.Generic;
using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;

namespace Expressions;

public abstract class PolyOperation<T> : IValue<T> where T : struct {
	[DataReadProxy(Name = "Parameters")]
	[DataWriteProxy(Name = "Parameters")]
	[CopyableProxy()]
	[Inspected]
	[Inspected(Name = "values", Mutable = true, Mode = ExecuteMode.Edit)]
	protected List<IValue<T>> values = new();

	protected abstract T Compute(T a, T b);

	public T GetValue(IEffect context) {
		var a = default(T);
		foreach (var obj in values)
			if (obj != null)
				a = Compute(a, obj.GetValue(context));
		return a;
	}

	protected abstract string OperatorView();

	public string ValueView {
		get {
			var str = "(";
			if (values.Count != 0)
				for (var index = 0; index < values.Count; ++index) {
					var obj = values[index];
					str = str + (obj != null ? obj.ValueView : "null") +
					      (index < values.Count - 1 ? " " + OperatorView() + " " : "");
				}

			return str + ")";
		}
	}

	public string TypeView {
		get {
			var str = "(";
			if (values.Count != 0)
				for (var index = 0; index < values.Count; ++index) {
					var obj = values[index];
					str = str + (obj != null ? obj.TypeView : "null") +
					      (index < values.Count - 1 ? " " + OperatorView() + " " : "");
				}

			return str + ")";
		}
	}
}