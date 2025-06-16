using Engine.Source.Commons.Effects;

namespace Expressions;

public interface IValue<T> where T : struct {
	string ValueView { get; }

	string TypeView { get; }

	T GetValue(IEffect context);
}