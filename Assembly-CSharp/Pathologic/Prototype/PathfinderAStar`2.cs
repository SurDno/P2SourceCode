using System.Collections.Generic;

namespace Pathologic.Prototype;

public class PathfinderAStar<T, K> {
	private Relation _cost;
	private List<Node> _estimates;
	private GetNeighbors _getNeighbors;
	private Relation _heuristic;
	private Condition _isLess;
	private Dictionary<T, Node> _sourcesAndCosts;
	private Operation _sum;
	private K _zero;

	public PathfinderAStar(
		GetNeighbors getNeighbors,
		Relation cost,
		Relation heuristic,
		Operation sum,
		Condition isLess,
		K zero) {
		_getNeighbors = getNeighbors;
		_cost = cost;
		_heuristic = heuristic;
		_sum = sum;
		_isLess = isLess;
		_zero = zero;
		_sourcesAndCosts = new Dictionary<T, Node>();
		_estimates = new List<Node>();
	}

	private T Dequeue() {
		var index1 = _estimates.Count - 1;
		for (var index2 = index1 - 1; index2 >= 0; --index2)
			if (_isLess(_estimates[index2].value, _estimates[index1].value))
				index1 = index2;
		var obj = _estimates[index1].item;
		_estimates.RemoveAt(index1);
		return obj;
	}

	public void AddReversedRoute(T start, T goal, List<T> output) {
		if (!goal.Equals(start)) {
			_estimates.Add(new Node(start, _zero));
			_sourcesAndCosts.Add(start, new Node(start, _zero));
		}

		while (_estimates.Count > 0) {
			var obj1 = Dequeue();
			if (obj1.Equals(goal)) {
				for (; !obj1.Equals(start); obj1 = _sourcesAndCosts[obj1].item)
					output.Add(obj1);
				break;
			}

			var objList = _getNeighbors(obj1);
			for (var index = 0; index < objList.Count; ++index) {
				var obj2 = objList[index];
				var k = _sum(_sourcesAndCosts[obj1].value, _cost(obj1, obj2));
				if (!_sourcesAndCosts.ContainsKey(obj2) || _isLess(k, _sourcesAndCosts[obj2].value)) {
					_sourcesAndCosts[obj2] = new Node(obj1, k);
					_estimates.Add(new Node(obj2, _sum(k, _heuristic(obj2, goal))));
				}
			}
		}

		_estimates.Clear();
		_sourcesAndCosts.Clear();
	}

	private struct Node {
		public T item;
		public K value;

		public Node(T item, K value) {
			this.item = item;
			this.value = value;
		}
	}

	public delegate bool Condition(K value0, K value1);

	public delegate IList<T> GetNeighbors(T node);

	public delegate K Operation(K value0, K value1);

	public delegate K Relation(T node0, T node1);
}