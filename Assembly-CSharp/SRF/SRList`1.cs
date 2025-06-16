using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SRF
{
  [Serializable]
  public class SRList<T> : 
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable,
    ISerializationCallbackReceiver
  {
    [SerializeField]
    private T[] _buffer;
    [SerializeField]
    private int _count;
    private EqualityComparer<T> _equalityComparer;
    private ReadOnlyCollection<T> _readOnlyWrapper;

    public SRList()
    {
    }

    public SRList(int capacity) => Buffer = new T[capacity];

    public SRList(IEnumerable<T> source) => AddRange(source);

    public T[] Buffer
    {
      get => _buffer;
      private set => _buffer = value;
    }

    private EqualityComparer<T> EqualityComparer
    {
      get
      {
        if (_equalityComparer == null)
          _equalityComparer = EqualityComparer<T>.Default;
        return _equalityComparer;
      }
    }

    public int Count
    {
      get => _count;
      private set => _count = value;
    }

    public IEnumerator<T> GetEnumerator()
    {
      if (Buffer != null)
      {
        for (int i = 0; i < Count; ++i)
          yield return Buffer[i];
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item)
    {
      if (Buffer == null || Count == Buffer.Length)
        Expand();
      Buffer[Count++] = item;
    }

    public void Clear() => Count = 0;

    public bool Contains(T item)
    {
      if (Buffer == null)
        return false;
      for (int index = 0; index < Count; ++index)
      {
        if (EqualityComparer.Equals(Buffer[index], item))
          return true;
      }
      return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      Trim();
      Buffer.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
      if (Buffer == null)
        return false;
      int index = IndexOf(item);
      if (index < 0)
        return false;
      RemoveAt(index);
      return true;
    }

    public bool IsReadOnly => false;

    public int IndexOf(T item)
    {
      if (Buffer == null)
        return -1;
      for (int index = 0; index < Count; ++index)
      {
        if (EqualityComparer.Equals(Buffer[index], item))
          return index;
      }
      return -1;
    }

    public void Insert(int index, T item)
    {
      if (Buffer == null || Count == Buffer.Length)
        Expand();
      if (index < Count)
      {
        for (int count = Count; count > index; --count)
          Buffer[count] = Buffer[count - 1];
        Buffer[index] = item;
        ++Count;
      }
      else
        Add(item);
    }

    public void RemoveAt(int index)
    {
      if (Buffer == null || index >= Count)
        return;
      --Count;
      Buffer[index] = default (T);
      for (int index1 = index; index1 < Count; ++index1)
        Buffer[index1] = Buffer[index1 + 1];
    }

    public T this[int index]
    {
      get => Buffer != null ? Buffer[index] : throw new IndexOutOfRangeException();
      set
      {
        if (Buffer == null)
          throw new IndexOutOfRangeException();
        Buffer[index] = value;
      }
    }

    public void OnBeforeSerialize()
    {
      Debug.Log((object) "[OnBeforeSerialize] Count: {0}".Fmt(_count));
      Clean();
    }

    public void OnAfterDeserialize()
    {
      Debug.Log((object) "[OnAfterDeserialize] Count: {0}".Fmt(_count));
    }

    public void AddRange(IEnumerable<T> range)
    {
      foreach (T obj in range)
        Add(obj);
    }

    public void Clear(bool clean)
    {
      Clear();
      if (!clean)
        return;
      Clean();
    }

    public void Clean()
    {
      if (Buffer == null)
        return;
      for (int count = Count; count < _buffer.Length; ++count)
        _buffer[count] = default (T);
    }

    public ReadOnlyCollection<T> AsReadOnly()
    {
      if (_readOnlyWrapper == null)
        _readOnlyWrapper = new ReadOnlyCollection<T>(this);
      return _readOnlyWrapper;
    }

    private void Expand()
    {
      T[] objArray = Buffer != null ? new T[Mathf.Max(Buffer.Length << 1, 32)] : new T[32];
      if (Buffer != null && Count > 0)
        Buffer.CopyTo(objArray, 0);
      Buffer = objArray;
    }

    public void Trim()
    {
      if (Count > 0)
      {
        if (Count >= Buffer.Length)
          return;
        T[] objArray = new T[Count];
        for (int index = 0; index < Count; ++index)
          objArray[index] = Buffer[index];
        Buffer = objArray;
      }
      else
        Buffer = new T[0];
    }

    public void Sort(Comparison<T> comparer)
    {
      bool flag = true;
      while (flag)
      {
        flag = false;
        for (int index = 1; index < Count; ++index)
        {
          if (comparer(Buffer[index - 1], Buffer[index]) > 0)
          {
            T obj = Buffer[index];
            Buffer[index] = Buffer[index - 1];
            Buffer[index - 1] = obj;
            flag = true;
          }
        }
      }
    }
  }
}
