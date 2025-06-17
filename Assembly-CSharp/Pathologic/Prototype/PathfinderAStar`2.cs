using System.Collections.Generic;

namespace Pathologic.Prototype
{
  public class PathfinderAStar<T, K>(
    PathfinderAStar<T, K>.GetNeighbors getNeighbors,
    PathfinderAStar<T, K>.Relation cost,
    PathfinderAStar<T, K>.Relation heuristic,
    PathfinderAStar<T, K>.Operation sum,
    PathfinderAStar<T, K>.Condition isLess,
    K zero) {
    private List<Node> _estimates = [];
    private Dictionary<T, Node> _sourcesAndCosts = new();

    private T Dequeue()
    {
      int index1 = _estimates.Count - 1;
      for (int index2 = index1 - 1; index2 >= 0; --index2)
      {
        if (isLess(_estimates[index2].value, _estimates[index1].value))
          index1 = index2;
      }
      T obj = _estimates[index1].item;
      _estimates.RemoveAt(index1);
      return obj;
    }

    public void AddReversedRoute(T start, T goal, List<T> output)
    {
      if (!goal.Equals(start))
      {
        _estimates.Add(new Node(start, zero));
        _sourcesAndCosts.Add(start, new Node(start, zero));
      }
      while (_estimates.Count > 0)
      {
        T obj1 = Dequeue();
        if (obj1.Equals(goal))
        {
          for (; !obj1.Equals(start); obj1 = _sourcesAndCosts[obj1].item)
            output.Add(obj1);
          break;
        }
        IList<T> objList = getNeighbors(obj1);
        for (int index = 0; index < objList.Count; ++index)
        {
          T obj2 = objList[index];
          K k = sum(_sourcesAndCosts[obj1].value, cost(obj1, obj2));
          if (!_sourcesAndCosts.ContainsKey(obj2) || isLess(k, _sourcesAndCosts[obj2].value))
          {
            _sourcesAndCosts[obj2] = new Node(obj1, k);
            _estimates.Add(new Node(obj2, sum(k, heuristic(obj2, goal))));
          }
        }
      }
      _estimates.Clear();
      _sourcesAndCosts.Clear();
    }

    private struct Node(T item, K value) {
      public T item = item;
      public K value = value;
    }

    public delegate bool Condition(K value0, K value1);

    public delegate IList<T> GetNeighbors(T node);

    public delegate K Operation(K value0, K value1);

    public delegate K Relation(T node0, T node1);
  }
}
