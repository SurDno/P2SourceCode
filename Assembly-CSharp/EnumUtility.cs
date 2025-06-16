using System;

public static class EnumUtility
{
  public static bool HasValue<T>(this T source, T value) where T : struct, IComparable, IFormattable, IConvertible
  {
    if (!typeof (T).IsEnum)
      throw new ArgumentException("T must be an enumerated type");
    return ((int) (ValueType) source & (int) (ValueType) value) != 0;
  }

  public static T SwitchValue<T>(this T source, T value) where T : struct, IComparable, IFormattable, IConvertible
  {
    if (!typeof (T).IsEnum)
      throw new ArgumentException("T must be an enumerated type");
    uint num1 = (uint) (int) (ValueType) source;
    uint num2 = (uint) (int) (ValueType) value;
    uint num3 = num1 & num2;
    uint num4 = num1 & ~num2;
    if (num3 == 0U)
      num4 |= num2;
    return (T) Enum.ToObject(typeof (T), num4);
  }

  public static T SetValue<T>(this T source, T value, bool set) where T : struct, IComparable, IFormattable, IConvertible
  {
    if (!typeof (T).IsEnum)
      throw new ArgumentException("T must be an enumerated type");
    uint num1 = (uint) (int) (ValueType) source;
    uint num2 = (uint) (int) (ValueType) value;
    uint num3 = num1 & ~num2;
    if (set)
      num3 |= num2;
    return (T) Enum.ToObject(typeof (T), num3);
  }
}
