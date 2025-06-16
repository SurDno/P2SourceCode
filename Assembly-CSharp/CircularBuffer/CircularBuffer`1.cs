using System;
using System.Collections;
using System.Collections.Generic;

namespace CircularBuffer
{
  public class CircularBuffer<T> : IEnumerable<T>, IEnumerable
  {
    private T[] _buffer;
    private int _end;
    private int _size;
    private int _start;

    public CircularBuffer(int capacity)
      : this(capacity, new T[0])
    {
    }

    public CircularBuffer(int capacity, T[] items)
    {
      if (capacity < 1)
        throw new ArgumentException("Circular buffer cannot have negative or zero capacity.", nameof (capacity));
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      if (items.Length > capacity)
        throw new ArgumentException("Too many items to fit circular buffer", nameof (items));
      _buffer = new T[capacity];
      Array.Copy(items, _buffer, items.Length);
      _size = items.Length;
      _start = 0;
      _end = _size == capacity ? 0 : _size;
    }

    public int Capacity => _buffer.Length;

    public bool IsFull => Size == Capacity;

    public bool IsEmpty => Size == 0;

    public int Size => _size;

    public T this[int index]
    {
      get
      {
        if (IsEmpty)
          throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
        return index < _size ? _buffer[InternalIndex(index)] : throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, _size));
      }
      set
      {
        if (IsEmpty)
          throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
        if (index >= _size)
          throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, _size));
        _buffer[InternalIndex(index)] = value;
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      ArraySegment<T>[] arraySegmentArray = new ArraySegment<T>[2]
      {
        ArrayOne(),
        ArrayTwo()
      };
      for (int index = 0; index < arraySegmentArray.Length; ++index)
      {
        ArraySegment<T> segment = arraySegmentArray[index];
        for (int i = 0; i < segment.Count; ++i)
          yield return segment.Array[segment.Offset + i];
        segment = new ArraySegment<T>();
      }
      arraySegmentArray = null;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public T Front()
    {
      ThrowIfEmpty();
      return _buffer[_start];
    }

    public T Back()
    {
      ThrowIfEmpty();
      return _buffer[(_end != 0 ? _end : _size) - 1];
    }

    public void PushBack(T item)
    {
      if (IsFull)
      {
        _buffer[_end] = item;
        Increment(ref _end);
        _start = _end;
      }
      else
      {
        _buffer[_end] = item;
        Increment(ref _end);
        ++_size;
      }
    }

    public void PushFront(T item)
    {
      if (IsFull)
      {
        Decrement(ref _start);
        _end = _start;
        _buffer[_start] = item;
      }
      else
      {
        Decrement(ref _start);
        _buffer[_start] = item;
        ++_size;
      }
    }

    public void PopBack()
    {
      ThrowIfEmpty("Cannot take elements from an empty buffer.");
      Decrement(ref _end);
      _buffer[_end] = default (T);
      --_size;
    }

    public void PopFront()
    {
      ThrowIfEmpty("Cannot take elements from an empty buffer.");
      _buffer[_start] = default (T);
      Increment(ref _start);
      --_size;
    }

    public T[] ToArray()
    {
      T[] destinationArray = new T[Size];
      int destinationIndex = 0;
      ArraySegment<T>[] arraySegmentArray = new ArraySegment<T>[2]
      {
        ArrayOne(),
        ArrayTwo()
      };
      foreach (ArraySegment<T> arraySegment in arraySegmentArray)
      {
        Array.Copy(arraySegment.Array, arraySegment.Offset, destinationArray, destinationIndex, arraySegment.Count);
        destinationIndex += arraySegment.Count;
      }
      return destinationArray;
    }

    private void ThrowIfEmpty(string message = "Cannot access an empty buffer.")
    {
      if (IsEmpty)
        throw new InvalidOperationException(message);
    }

    private void Increment(ref int index)
    {
      if (++index != Capacity)
        return;
      index = 0;
    }

    private void Decrement(ref int index)
    {
      if (index == 0)
        index = Capacity;
      --index;
    }

    private int InternalIndex(int index)
    {
      return _start + (index < Capacity - _start ? index : index - Capacity);
    }

    private ArraySegment<T> ArrayOne()
    {
      return _start < _end ? new ArraySegment<T>(_buffer, _start, _end - _start) : new ArraySegment<T>(_buffer, _start, _buffer.Length - _start);
    }

    private ArraySegment<T> ArrayTwo()
    {
      return _start < _end ? new ArraySegment<T>(_buffer, _end, 0) : new ArraySegment<T>(_buffer, 0, _end);
    }
  }
}
