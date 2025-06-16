// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.Internal.fsCyclicReferenceManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsCyclicReferenceManager
  {
    private Dictionary<object, int> _objectIds = new Dictionary<object, int>(fsCyclicReferenceManager.ObjectReferenceEqualityComparator.Instance);
    private int _nextId;
    private Dictionary<int, object> _marked = new Dictionary<int, object>();
    private int _depth;

    public void Enter() => ++this._depth;

    public bool Exit()
    {
      --this._depth;
      if (this._depth == 0)
      {
        this._objectIds = new Dictionary<object, int>(fsCyclicReferenceManager.ObjectReferenceEqualityComparator.Instance);
        this._nextId = 0;
        this._marked = new Dictionary<int, object>();
      }
      if (this._depth < 0)
      {
        this._depth = 0;
        throw new InvalidOperationException("Internal Error - Mismatched Enter/Exit");
      }
      return this._depth == 0;
    }

    public object GetReferenceObject(int id)
    {
      return this._marked.ContainsKey(id) ? this._marked[id] : throw new InvalidOperationException("Internal Deserialization Error - Object definition has not been encountered for object with id=" + (object) id + "; have you reordered or modified the serialized data? If this is an issue with an unmodified Full Json implementation and unmodified serialization data, please report an issue with an included test case.");
    }

    public void AddReferenceWithId(int id, object reference) => this._marked[id] = reference;

    public int GetReferenceId(object item)
    {
      int referenceId;
      if (!this._objectIds.TryGetValue(item, out referenceId))
      {
        referenceId = this._nextId++;
        this._objectIds[item] = referenceId;
      }
      return referenceId;
    }

    public bool IsReference(object item) => this._marked.ContainsKey(this.GetReferenceId(item));

    public void MarkSerialized(object item)
    {
      int referenceId = this.GetReferenceId(item);
      if (this._marked.ContainsKey(referenceId))
        throw new InvalidOperationException("Internal Error - " + item + " has already been marked as serialized");
      this._marked[referenceId] = item;
    }

    private class ObjectReferenceEqualityComparator : IEqualityComparer<object>
    {
      public static readonly IEqualityComparer<object> Instance = (IEqualityComparer<object>) new fsCyclicReferenceManager.ObjectReferenceEqualityComparator();

      bool IEqualityComparer<object>.Equals(object x, object y) => x == y;

      int IEqualityComparer<object>.GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
  }
}
