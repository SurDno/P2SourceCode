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
      linkedListData = new LinkedList<T>(list);
    }

    public void PushBack(T obj) => linkedListData.AddLast(obj);

    public T PopBack()
    {
      T obj = linkedListData.Last.Value;
      linkedListData.RemoveLast();
      return obj;
    }

    public T GetBack() => linkedListData.Last.Value;

    public void MergeBack(CSDequeue<T> other)
    {
      linkedListData = new LinkedList<T>(linkedListData.Concat(other.linkedListData));
    }

    public void PushFront(T obj) => linkedListData.AddFirst(obj);

    public T PopFront()
    {
      if (linkedListData.First == null)
        return default (T);
      T obj = linkedListData.First.Value;
      linkedListData.RemoveFirst();
      return obj;
    }

    public T GetFront() => linkedListData.First.Value;

    public void MergeFront(CSDequeue<T> other)
    {
      LinkedList<T> linkedListData = other.linkedListData;
      try
      {
        this.linkedListData = new LinkedList<T>(linkedListData.Concat(this.linkedListData));
      }
      catch (Exception ex)
      {
        ex.ToString();
      }
    }

    public void Clear() => linkedListData.Clear();

    public int Count() => linkedListData.Count;

    public bool Empty() => linkedListData.Count == 0;

    public List<T> ToList => linkedListData.ToList();
  }
}
