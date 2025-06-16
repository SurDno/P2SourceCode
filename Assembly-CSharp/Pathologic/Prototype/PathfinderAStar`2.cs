// Decompiled with JetBrains decompiler
// Type: Pathologic.Prototype.PathfinderAStar`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace Pathologic.Prototype
{
  public class PathfinderAStar<T, K>
  {
    private PathfinderAStar<T, K>.Relation _cost;
    private List<PathfinderAStar<T, K>.Node> _estimates;
    private PathfinderAStar<T, K>.GetNeighbors _getNeighbors;
    private PathfinderAStar<T, K>.Relation _heuristic;
    private PathfinderAStar<T, K>.Condition _isLess;
    private Dictionary<T, PathfinderAStar<T, K>.Node> _sourcesAndCosts;
    private PathfinderAStar<T, K>.Operation _sum;
    private K _zero;

    public PathfinderAStar(
      PathfinderAStar<T, K>.GetNeighbors getNeighbors,
      PathfinderAStar<T, K>.Relation cost,
      PathfinderAStar<T, K>.Relation heuristic,
      PathfinderAStar<T, K>.Operation sum,
      PathfinderAStar<T, K>.Condition isLess,
      K zero)
    {
      this._getNeighbors = getNeighbors;
      this._cost = cost;
      this._heuristic = heuristic;
      this._sum = sum;
      this._isLess = isLess;
      this._zero = zero;
      this._sourcesAndCosts = new Dictionary<T, PathfinderAStar<T, K>.Node>();
      this._estimates = new List<PathfinderAStar<T, K>.Node>();
    }

    private T Dequeue()
    {
      int index1 = this._estimates.Count - 1;
      for (int index2 = index1 - 1; index2 >= 0; --index2)
      {
        if (this._isLess(this._estimates[index2].value, this._estimates[index1].value))
          index1 = index2;
      }
      T obj = this._estimates[index1].item;
      this._estimates.RemoveAt(index1);
      return obj;
    }

    public void AddReversedRoute(T start, T goal, List<T> output)
    {
      if (!goal.Equals((object) start))
      {
        this._estimates.Add(new PathfinderAStar<T, K>.Node(start, this._zero));
        this._sourcesAndCosts.Add(start, new PathfinderAStar<T, K>.Node(start, this._zero));
      }
      while (this._estimates.Count > 0)
      {
        T obj1 = this.Dequeue();
        if (obj1.Equals((object) goal))
        {
          for (; !obj1.Equals((object) start); obj1 = this._sourcesAndCosts[obj1].item)
            output.Add(obj1);
          break;
        }
        IList<T> objList = this._getNeighbors(obj1);
        for (int index = 0; index < objList.Count; ++index)
        {
          T obj2 = objList[index];
          K k = this._sum(this._sourcesAndCosts[obj1].value, this._cost(obj1, obj2));
          if (!this._sourcesAndCosts.ContainsKey(obj2) || this._isLess(k, this._sourcesAndCosts[obj2].value))
          {
            this._sourcesAndCosts[obj2] = new PathfinderAStar<T, K>.Node(obj1, k);
            this._estimates.Add(new PathfinderAStar<T, K>.Node(obj2, this._sum(k, this._heuristic(obj2, goal))));
          }
        }
      }
      this._estimates.Clear();
      this._sourcesAndCosts.Clear();
    }

    private struct Node
    {
      public T item;
      public K value;

      public Node(T item, K value)
      {
        this.item = item;
        this.value = value;
      }
    }

    public delegate bool Condition(K value0, K value1);

    public delegate IList<T> GetNeighbors(T node);

    public delegate K Operation(K value0, K value1);

    public delegate K Relation(T node0, T node1);
  }
}
