namespace Engine.Source.Settings
{
  public interface IValue<T>
  {
    T Value { get; set; }

    T DefaultValue { get; }

    T MinValue { get; }

    T MaxValue { get; }
  }
}
