using System;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public sealed class fsContext
  {
    private readonly Dictionary<Type, object> _contextObjects = new();

    public void Reset() => _contextObjects.Clear();

    public void Set<T>(T obj) => _contextObjects[typeof (T)] = obj;

    public bool Has<T>() => _contextObjects.ContainsKey(typeof (T));

    public T Get<T>()
    {
      if (_contextObjects.TryGetValue(typeof (T), out object obj))
        return (T) obj;
      throw new InvalidOperationException("There is no context object of type " + typeof (T));
    }
  }
}
