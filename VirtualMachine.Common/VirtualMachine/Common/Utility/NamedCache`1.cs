using PLVirtualMachine.Common;
using System.Collections.Generic;

namespace VirtualMachine.Common.Utility
{
  public struct NamedCache<T> where T : class, INamed
  {
    private List<T> items;

    public void Add(T value)
    {
      if (this.items == null)
        this.items = new List<T>();
      foreach (T obj in this.items)
      {
        if (obj.Name == value.Name)
          return;
      }
      this.items.Add(value);
    }

    public bool TryGetValue(string name, out T result)
    {
      if (this.items != null)
      {
        foreach (T obj in this.items)
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
      if (this.items == null)
        return;
      this.items.Clear();
    }

    public void Reserve(int capacity)
    {
      if (this.items == null)
      {
        this.items = new List<T>(capacity);
      }
      else
      {
        if (this.items.Count >= capacity)
          return;
        this.items.Capacity = capacity;
      }
    }
  }
}
