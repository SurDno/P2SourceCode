using System;
using Inspectors;

namespace Engine.Source.Settings
{
  public class EnumValue<T>(string name, T defaultValue = default(T)) : IValue<T>
    where T : struct, IComparable, IFormattable, IConvertible {
    [Inspected]
    private string name = name;
    [Inspected]
    private T defaultValue = defaultValue;
    private T value = PlayerSettings.Instance.GetEnum(name, defaultValue);

    [Inspected(Mutable = true)]
    public T Value
    {
      get => value;
      set
      {
        if (this.value.CompareTo(value) == 0)
          return;
        this.value = value;
        PlayerSettings.Instance.SetEnum(name, value);
        PlayerSettings.Instance.Save();
      }
    }

    public T DefaultValue => defaultValue;

    public T MinValue => defaultValue;

    public T MaxValue => defaultValue;
  }
}
