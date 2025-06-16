using Cofe.Utility;
using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;

namespace Expressions;

public abstract class ConstValue<T> : IValue<T> where T : struct {
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy()]
	[Inspected(Header = true)]
	[Inspected(Name = "value", Mutable = true, Mode = ExecuteMode.Edit)]
	protected T value;

	public T GetValue(IEffect context) {
		return value;
	}

	public string ValueView => value.ToString();

	public string TypeView => TypeUtility.GetTypeName(GetType());
}