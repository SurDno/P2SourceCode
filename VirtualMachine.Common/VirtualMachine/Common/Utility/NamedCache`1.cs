using System.Collections.Generic;
using PLVirtualMachine.Common;

namespace VirtualMachine.Common.Utility
{
  public struct NamedCache<T> where T : class, INamed
  {
    private List<T> items;

    public void Add(T value)
    {
      if (items == null)
        items = new List<T>();
      foreach (T obj in items)
      {
        if (obj.Name == value.Name)
          return;
      }
      items.Add(value);
    }

    public bool TryGetValue(string name, out T result)
    {
      if (items != null)
      {
        foreach (T obj in items)
        {
          if (obj.Name == name)
          {
            result = obj;
            return true;
          }
        }
      }
      result = default (T);
      return false;
    }

    public void Clear()
    {
      if (items == null)
        return;
      items.Clear();
    }

    public void Reserve(int capacity)
    {
      if (items == null)
      {
        items = new List<T>(capacity);
      }
      else
      {
        if (items.Count >= capacity)
          return;
        items.Capacity = capacity;
      }
    }
  }
}
