using System;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public struct fsOption<T>
  {
    private bool _hasValue;
    private T _value;
    public static fsOption<T> Empty;

    public bool HasValue => this._hasValue;

    public bool IsEmpty => !this._hasValue;

    public T Value
    {
      get
      {
        if (this.IsEmpty)
          throw new InvalidOperationException("fsOption is empty");
        return this._value;
      }
    }

    public fsOption(T value)
    {
      this._hasValue = true;
      this._value = value;
    }
  }
}
