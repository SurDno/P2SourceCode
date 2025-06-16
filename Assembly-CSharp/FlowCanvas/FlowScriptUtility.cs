using System.Collections.Generic;
using NodeCanvas.Framework;

namespace FlowCanvas;

public static class FlowScriptUtility {
	public static List<Node> CopyNodesToGraph(
		List<Node> nodes,
		Graph targetGraph) {
		if (targetGraph == null)
			return null;
		var graph = new List<Node>();
		var dictionary = new Dictionary<Connection, KeyValuePair<int, int>>();
		foreach (var node1 in nodes) {
			var node2 = node1.Duplicate(targetGraph);
			graph.Add(node2);
			foreach (var outConnection in node1.outConnections)
				dictionary[outConnection] = new KeyValuePair<int, int>(nodes.IndexOf(outConnection.sourceNode),
					nodes.IndexOf(outConnection.targetNode));
		}

		foreach (var keyValuePair1 in dictionary) {
			var keyValuePair2 = keyValuePair1.Value;
			if (keyValuePair2.Value != -1) {
				var nodeList1 = graph;
				keyValuePair2 = keyValuePair1.Value;
				var key = keyValuePair2.Key;
				var newSource = nodeList1[key];
				var nodeList2 = graph;
				keyValuePair2 = keyValuePair1.Value;
				var index = keyValuePair2.Value;
				var newTarget = nodeList2[index];
				keyValuePair1.Key.Duplicate(newSource, newTarget);
			}
		}

		return graph;
	}
}