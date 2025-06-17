using System.Collections.Generic;
using NodeCanvas.Framework;

namespace FlowCanvas
{
  public static class FlowScriptUtility
  {
    public static List<Node> CopyNodesToGraph(
      List<Node> nodes,
      Graph targetGraph)
    {
      if (targetGraph == null)
        return null;
      List<Node> graph = [];
      Dictionary<Connection, KeyValuePair<int, int>> dictionary = new Dictionary<Connection, KeyValuePair<int, int>>();
      foreach (Node node1 in nodes)
      {
        Node node2 = node1.Duplicate(targetGraph);
        graph.Add(node2);
        foreach (Connection outConnection in node1.outConnections)
          dictionary[outConnection] = new KeyValuePair<int, int>(nodes.IndexOf(outConnection.sourceNode), nodes.IndexOf(outConnection.targetNode));
      }
      foreach (KeyValuePair<Connection, KeyValuePair<int, int>> keyValuePair1 in dictionary)
      {
        KeyValuePair<int, int> keyValuePair2 = keyValuePair1.Value;
        if (keyValuePair2.Value != -1)
        {
          List<Node> nodeList1 = graph;
          keyValuePair2 = keyValuePair1.Value;
          int key = keyValuePair2.Key;
          Node newSource = nodeList1[key];
          List<Node> nodeList2 = graph;
          keyValuePair2 = keyValuePair1.Value;
          int index = keyValuePair2.Value;
          Node newTarget = nodeList2[index];
          keyValuePair1.Key.Duplicate(newSource, newTarget);
        }
      }
      return graph;
    }
  }
}
