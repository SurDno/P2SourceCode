using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsCyclicReferenceManager
  {
    private Dictionary<object, int> _objectIds = new(ObjectReferenceEqualityComparator.Instance);
    private int _nextId;
    private Dictionary<int, object> _marked = new();
    private int _depth;

    public void Enter() => ++_depth;

    public bool Exit()
    {
      --_depth;
      if (_depth == 0)
      {
        _objectIds = new Dictionary<object, int>(ObjectReferenceEqualityComparator.Instance);
        _nextId = 0;
        _marked = new Dictionary<int, object>();
      }
      if (_depth < 0)
      {
        _depth = 0;
        throw new InvalidOperationException("Internal Error - Mismatched Enter/Exit");
      }
      return _depth == 0;
    }

    public object GetReferenceObject(int id)
    {
      return _marked.ContainsKey(id) ? _marked[id] : throw new InvalidOperationException("Internal Deserialization Error - Object definition has not been encountered for object with id=" + id + "; have you reordered or modified the serialized data? If this is an issue with an unmodified Full Json implementation and unmodified serialization data, please report an issue with an included test case.");
    }

    public void AddReferenceWithId(int id, object reference) => _marked[id] = reference;

    public int GetReferenceId(object item)
    {
      if (!_objectIds.TryGetValue(item, out int referenceId))
      {
        referenceId = _nextId++;
        _objectIds[item] = referenceId;
      }
      return referenceId;
    }

    public bool IsReference(object item) => _marked.ContainsKey(GetReferenceId(item));

    public void MarkSerialized(object item)
    {
      int referenceId = GetReferenceId(item);
      if (_marked.ContainsKey(referenceId))
        throw new InvalidOperationException("Internal Error - " + item + " has already been marked as serialized");
      _marked[referenceId] = item;
    }

    private class ObjectReferenceEqualityComparator : IEqualityComparer<object>
    {
      public static readonly IEqualityComparer<object> Instance = new ObjectReferenceEqualityComparator();

      bool IEqualityComparer<object>.Equals(object x, object y) => x == y;

      int IEqualityComparer<object>.GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
  }
}
