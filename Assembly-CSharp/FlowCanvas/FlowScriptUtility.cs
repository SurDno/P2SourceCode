// Decompiled with JetBrains decompiler
// Type: FlowCanvas.FlowScriptUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using NodeCanvas.Framework;
using System.Collections.Generic;

#nullable disable
namespace FlowCanvas
{
  public static class FlowScriptUtility
  {
    public static List<NodeCanvas.Framework.Node> CopyNodesToGraph(
      List<NodeCanvas.Framework.Node> nodes,
      Graph targetGraph)
    {
      if (targetGraph == null)
        return (List<NodeCanvas.Framework.Node>) null;
      List<NodeCanvas.Framework.Node> graph = new List<NodeCanvas.Framework.Node>();
      Dictionary<Connection, KeyValuePair<int, int>> dictionary = new Dictionary<Connection, KeyValuePair<int, int>>();
      foreach (NodeCanvas.Framework.Node node1 in nodes)
      {
        NodeCanvas.Framework.Node node2 = node1.Duplicate(targetGraph);
        graph.Add(node2);
        foreach (Connection outConnection in node1.outConnections)
          dictionary[outConnection] = new KeyValuePair<int, int>(nodes.IndexOf(outConnection.sourceNode), nodes.IndexOf(outConnection.targetNode));
      }
      foreach (KeyValuePair<Connection, KeyValuePair<int, int>> keyValuePair1 in dictionary)
      {
        KeyValuePair<int, int> keyValuePair2 = keyValuePair1.Value;
        if (keyValuePair2.Value != -1)
        {
          List<NodeCanvas.Framework.Node> nodeList1 = graph;
          keyValuePair2 = keyValuePair1.Value;
          int key = keyValuePair2.Key;
          NodeCanvas.Framework.Node newSource = nodeList1[key];
          List<NodeCanvas.Framework.Node> nodeList2 = graph;
          keyValuePair2 = keyValuePair1.Value;
          int index = keyValuePair2.Value;
          NodeCanvas.Framework.Node newTarget = nodeList2[index];
          keyValuePair1.Key.Duplicate(newSource, newTarget);
        }
      }
      return graph;
    }
  }
}
