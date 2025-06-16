using Engine.Source.Commons.Effects;

namespace Expressions
{
  public interface IValueSetter<T> : IValue<T> where T : struct
  {
    void SetValue(T value, IEffect context);
  }
}
