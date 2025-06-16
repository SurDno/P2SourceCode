using System;
using Inspectors;

namespace Engine.Source.Settings
{
  public class EnumValue<T> : IValue<T> where T : struct, IComparable, IFormattable, IConvertible
  {
    [Inspected]
    private string name;
    [Inspected]
    private T defaultValue;
    private T value;

    public EnumValue(string name, T defaultValue = default (T))
    {
      this.name = name;
      value = PlayerSettings.Instance.GetEnum(name, defaultValue);
      this.defaultValue = defaultValue;
    }

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
