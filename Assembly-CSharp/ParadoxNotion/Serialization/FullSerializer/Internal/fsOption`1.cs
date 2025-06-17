using System;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public struct fsOption<T>(T value) {
    private bool _hasValue = true;
    public static fsOption<T> Empty;

    public bool HasValue => _hasValue;

    public bool IsEmpty => !_hasValue;

    public T Value
    {
      get
      {
        if (IsEmpty)
          throw new InvalidOperationException("fsOption is empty");
        return value;
      }
    }
  }
}
