using System;
using System.Collections.Generic;
using System.Linq;

namespace PLVirtualMachine.Common
{
  public class CSDequeue<T>
  {
    private LinkedList<T> linkedListData = new LinkedList<T>();

    public CSDequeue()
    {
    }

    public CSDequeue(List<T> list)
    {
      this.linkedListData = new LinkedList<T>((IEnumerable<T>) list);
    }

    public void PushBack(T obj) => this.linkedListData.AddLast(obj);

    public T PopBack()
    {
      T obj = this.linkedListData.Last.Value;
      this.linkedListData.RemoveLast();
      return obj;
    }

    public T GetBack() => this.linkedListData.Last.Value;

    public void MergeBack(CSDequeue<T> other)
    {
      this.linkedListData = new LinkedList<T>(this.linkedListData.Concat<T>((IEnumerable<T>) other.linkedListData));
    }

    public void PushFront(T obj) => this.linkedListData.AddFirst(obj);

    public T PopFront()
    {
      if (this.linkedListData.First == null)
        return default (T);
      T obj = this.linkedListData.First.Value;
      this.linkedListData.RemoveFirst();
      return obj;
    }

    public T GetFront() => this.linkedListData.First.Value;

    public void MergeFront(CSDequeue<T> other)
    {
      LinkedList<T> linkedListData = other.linkedListData;
      try
      {
        this.linkedListData = new LinkedList<T>(linkedListData.Concat<T>((IEnumerable<T>) this.linkedListData));
      }
      catch (Exception ex)
      {
        ex.ToString();
      }
    }

    public void Clear() => this.linkedListData.Clear();

    public int Count() => this.linkedListData.Count;

    public bool Empty() => this.linkedListData.Count == 0;

    public List<T> ToList => this.linkedListData.ToList<T>();
  }
}
