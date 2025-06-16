using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

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
    private System.Collections.Generic.EqualityComparer<T> _equalityComparer;
    private ReadOnlyCollection<T> _readOnlyWrapper;

    public SRList()
    {
    }

    public SRList(int capacity) => this.Buffer = new T[capacity];

    public SRList(IEnumerable<T> source) => this.AddRange(source);

    public T[] Buffer
    {
      get => this._buffer;
      private set => this._buffer = value;
    }

    private System.Collections.Generic.EqualityComparer<T> EqualityComparer
    {
      get
      {
        if (this._equalityComparer == null)
          this._equalityComparer = System.Collections.Generic.EqualityComparer<T>.Default;
        return this._equalityComparer;
      }
    }

    public int Count
    {
      get => this._count;
      private set => this._count = value;
    }

    public IEnumerator<T> GetEnumerator()
    {
      if (this.Buffer != null)
      {
        for (int i = 0; i < this.Count; ++i)
          yield return this.Buffer[i];
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public void Add(T item)
    {
      if (this.Buffer == null || this.Count == this.Buffer.Length)
        this.Expand();
      this.Buffer[this.Count++] = item;
    }

    public void Clear() => this.Count = 0;

    public bool Contains(T item)
    {
      if (this.Buffer == null)
        return false;
      for (int index = 0; index < this.Count; ++index)
      {
        if (this.EqualityComparer.Equals(this.Buffer[index], item))
          return true;
      }
      return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      this.Trim();
      this.Buffer.CopyTo((Array) array, arrayIndex);
    }

    public bool Remove(T item)
    {
      if (this.Buffer == null)
        return false;
      int index = this.IndexOf(item);
      if (index < 0)
        return false;
      this.RemoveAt(index);
      return true;
    }

    public bool IsReadOnly => false;

    public int IndexOf(T item)
    {
      if (this.Buffer == null)
        return -1;
      for (int index = 0; index < this.Count; ++index)
      {
        if (this.EqualityComparer.Equals(this.Buffer[index], item))
          return index;
      }
      return -1;
    }

    public void Insert(int index, T item)
    {
      if (this.Buffer == null || this.Count == this.Buffer.Length)
        this.Expand();
      if (index < this.Count)
      {
        for (int count = this.Count; count > index; --count)
          this.Buffer[count] = this.Buffer[count - 1];
        this.Buffer[index] = item;
        ++this.Count;
      }
      else
        this.Add(item);
    }

    public void RemoveAt(int index)
    {
      if (this.Buffer == null || index >= this.Count)
        return;
      --this.Count;
      this.Buffer[index] = default (T);
      for (int index1 = index; index1 < this.Count; ++index1)
        this.Buffer[index1] = this.Buffer[index1 + 1];
    }

    public T this[int index]
    {
      get => this.Buffer != null ? this.Buffer[index] : throw new IndexOutOfRangeException();
      set
      {
        if (this.Buffer == null)
          throw new IndexOutOfRangeException();
        this.Buffer[index] = value;
      }
    }

    public void OnBeforeSerialize()
    {
      Debug.Log((object) "[OnBeforeSerialize] Count: {0}".Fmt((object) this._count));
      this.Clean();
    }

    public void OnAfterDeserialize()
    {
      Debug.Log((object) "[OnAfterDeserialize] Count: {0}".Fmt((object) this._count));
    }

    public void AddRange(IEnumerable<T> range)
    {
      foreach (T obj in range)
        this.Add(obj);
    }

    public void Clear(bool clean)
    {
      this.Clear();
      if (!clean)
        return;
      this.Clean();
    }

    public void Clean()
    {
      if (this.Buffer == null)
        return;
      for (int count = this.Count; count < this._buffer.Length; ++count)
        this._buffer[count] = default (T);
    }

    public ReadOnlyCollection<T> AsReadOnly()
    {
      if (this._readOnlyWrapper == null)
        this._readOnlyWrapper = new ReadOnlyCollection<T>((IList<T>) this);
      return this._readOnlyWrapper;
    }

    private void Expand()
    {
      T[] objArray = this.Buffer != null ? new T[Mathf.Max(this.Buffer.Length << 1, 32)] : new T[32];
      if (this.Buffer != null && this.Count > 0)
        this.Buffer.CopyTo((Array) objArray, 0);
      this.Buffer = objArray;
    }

    public void Trim()
    {
      if (this.Count > 0)
      {
        if (this.Count >= this.Buffer.Length)
          return;
        T[] objArray = new T[this.Count];
        for (int index = 0; index < this.Count; ++index)
          objArray[index] = this.Buffer[index];
        this.Buffer = objArray;
      }
      else
        this.Buffer = new T[0];
    }

    public void Sort(Comparison<T> comparer)
    {
      bool flag = true;
      while (flag)
      {
        flag = false;
        for (int index = 1; index < this.Count; ++index)
        {
          if (comparer(this.Buffer[index - 1], this.Buffer[index]) > 0)
          {
            T obj = this.Buffer[index];
            this.Buffer[index] = this.Buffer[index - 1];
            this.Buffer[index - 1] = obj;
            flag = true;
          }
        }
      }
    }
  }
}
