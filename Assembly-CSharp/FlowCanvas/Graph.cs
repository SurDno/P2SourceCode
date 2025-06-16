// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Graph
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Serialization;
using ParadoxNotion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace FlowCanvas
{
  public class Graph
  {
    private bool hasInitialized;
    private List<IUpdatable> updatableNodes;

    public System.Type baseNodeType => typeof (FlowNode);

    private void OnGraphStarted()
    {
      if (this.hasInitialized)
        return;
      this.updatableNodes = new List<IUpdatable>();
      for (int index = 0; index < this.nodes.Count; ++index)
      {
        if (this.nodes[index] is IUpdatable node)
          this.updatableNodes.Add(node);
      }
      for (int index = 0; index < this.nodes.Count; ++index)
      {
        if (this.nodes[index] is FlowNode)
        {
          FlowNode node = (FlowNode) this.nodes[index];
          node.AssignSelfInstancePort();
          node.BindPorts();
        }
      }
      this.hasInitialized = true;
    }

    private void OnGraphUpdate()
    {
      if (this.updatableNodes == null)
        return;
      for (int index = 0; index < this.updatableNodes.Count; ++index)
        this.updatableNodes[index].Update();
    }

    public void Serialize(out string json, out List<UnityEngine.Object> references)
    {
      GraphData graphData = new GraphData();
      graphData.nodes = this.nodes;
      List<Connection> connectionList = new List<Connection>();
      for (int index1 = 0; index1 < graphData.nodes.Count; ++index1)
      {
        for (int index2 = 0; index2 < graphData.nodes[index1].outConnections.Count; ++index2)
          connectionList.Add(graphData.nodes[index1].outConnections[index2]);
      }
      graphData.connections = connectionList;
      references = new List<UnityEngine.Object>();
      json = JSONSerializer.Serialize(typeof (GraphData), (object) graphData, objectReferences: references);
    }

    public void Deserialize(string serializedGraph, List<UnityEngine.Object> objectReferences)
    {
      if (ReflectionTools.ContextObject == null)
        ReflectionTools.ContextObject = (object) this.agent;
      GraphData graphData = JSONSerializer.Deserialize<GraphData>(serializedGraph, objectReferences);
      ReflectionTools.ContextObject = (object) null;
      for (int index = 0; index < graphData.connections.Count; ++index)
      {
        if (graphData.connections[index].sourceNode == null)
          Debug.LogError((object) ("connections[i].sourceNode is null, i : " + (object) index + " , graph : " + this.agent.gameObject.GetFullName()));
        else if (graphData.connections[index].targetNode == null)
        {
          Debug.LogError((object) ("connections[i].targetNode is null, i : " + (object) index + " , graph : " + this.agent.gameObject.GetFullName()));
        }
        else
        {
          graphData.connections[index].sourceNode.outConnections.Add(graphData.connections[index]);
          graphData.connections[index].targetNode.inConnections.Add(graphData.connections[index]);
        }
      }
      for (int index = 0; index < graphData.nodes.Count; ++index)
        graphData.nodes[index].graph = this;
      this.nodes = graphData.nodes;
      this.UpdateNodeIDs();
      this.OnValidate();
      this.CorrectPosition();
    }

    private void CorrectPosition()
    {
      this.zoomFactor = 1f;
      this.translation = Vector2.positiveInfinity;
      for (int index = 0; index < this.nodes.Count; ++index)
      {
        NodeCanvas.Framework.Node node = this.nodes[index];
        if ((double) node.nodePosition.x < (double) this.translation.x)
          this.translation = node.nodePosition;
      }
      if (this.translation == Vector2.positiveInfinity)
        this.translation = Vector2.zero;
      this.translation = new Vector2((float) (-(double) this.translation.x + 500.0), (float) (-(double) this.translation.y + 500.0));
    }

    public void OnValidate()
    {
      for (int index = 0; index < this.nodes.Count; ++index)
      {
        NodeCanvas.Framework.Node node = this.nodes[index];
        try
        {
          node.OnValidate(this);
        }
        catch (Exception ex)
        {
          Debug.LogError((object) ex.ToString());
        }
      }
    }

    public bool isRunning { get; private set; }

    public bool isPaused { get; private set; }

    public List<NodeCanvas.Framework.Node> nodes { get; private set; } = new List<NodeCanvas.Framework.Node>();

    public Vector2 translation { get; set; }

    public float zoomFactor { get; set; } = 1f;

    public FlowScriptController agent { get; set; }

    public string agentName { get; private set; }

    public void UpdateReferences() => this.UpdateNodeBBFields();

    private void UpdateNodeBBFields()
    {
      for (int index = 0; index < this.nodes.Count; ++index)
        BBParameter.SetBBFields((object) this.nodes[index], this.agent.blackboard);
    }

    public void UpdateNodeIDs()
    {
      for (int index = 0; index < this.nodes.Count; ++index)
      {
        this.nodes[index].ResetRecursion();
        this.nodes[index].Id = index + 1;
      }
    }

    public void StartGraph()
    {
      if (this.isRunning)
        return;
      this.agentName = this.agent.name;
      this.UpdateReferences();
      this.isRunning = true;
      if (!this.isPaused)
        this.OnGraphStarted();
      for (int index = 0; index < this.nodes.Count; ++index)
      {
        if (!this.isPaused)
          this.nodes[index].OnGraphStarted();
        else
          this.nodes[index].OnGraphUnpaused();
      }
      this.isPaused = false;
      BlueprintManager.current.graphs.Add(this);
      this.UpdateGraph();
    }

    public void Stop(bool success = true)
    {
      if (!this.isRunning && !this.isPaused)
        return;
      BlueprintManager.current.graphs.Remove(this);
      this.isRunning = false;
      this.isPaused = false;
      for (int index = 0; index < this.nodes.Count; ++index)
      {
        this.nodes[index].Reset(false);
        this.nodes[index].OnGraphStoped();
      }
    }

    public void Pause()
    {
      if (!this.isRunning)
        return;
      BlueprintManager.current.graphs.Remove(this);
      this.isRunning = false;
      this.isPaused = true;
      for (int index = 0; index < this.nodes.Count; ++index)
        this.nodes[index].OnGraphPaused();
    }

    public void UpdateGraph() => this.OnGraphUpdate();

    public void SendEvent(EventData eventData)
    {
      if (!this.isRunning || eventData == null || !((UnityEngine.Object) this.agent != (UnityEngine.Object) null))
        return;
      MessageRouter component = this.agent.GetComponent<MessageRouter>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        component.Dispatch("OnCustomEvent", (object) eventData);
        component.Dispatch(eventData.name, eventData.value);
      }
    }

    public List<T> GetAllNodesOfType<T>() where T : NodeCanvas.Framework.Node
    {
      return this.nodes.OfType<T>().ToList<T>();
    }

    public BBParameter[] GetDefinedParameters()
    {
      List<BBParameter> bbParameterList = new List<BBParameter>();
      List<object> objectList = new List<object>();
      objectList.AddRange(this.nodes.Cast<object>());
      for (int index = 0; index < objectList.Count; ++index)
      {
        foreach (BBParameter objectBbParameter in BBParameter.GetObjectBBParameters(objectList[index]))
        {
          if (objectBbParameter != null && objectBbParameter.useBlackboard && !objectBbParameter.isNone)
            bbParameterList.Add(objectBbParameter);
        }
      }
      return bbParameterList.ToArray();
    }

    public void CreateDefinedParameterVariables(Blackboard bb)
    {
      foreach (BBParameter definedParameter in this.GetDefinedParameters())
        definedParameter.PromoteToVariable(bb);
    }

    public NodeCanvas.Framework.Node AddNode(System.Type nodeType, Vector2 pos)
    {
      if (!nodeType.RTIsSubclassOf(this.baseNodeType))
      {
        Debug.LogWarning((object) (nodeType.ToString() + " can't be added to " + this.GetType().FriendlyName() + " graph"));
        return (NodeCanvas.Framework.Node) null;
      }
      NodeCanvas.Framework.Node node = NodeCanvas.Framework.Node.Create(this, nodeType, pos);
      this.RecordUndo("New Node");
      this.nodes.Add(node);
      this.UpdateNodeIDs();
      return node;
    }

    public void RemoveNode(NodeCanvas.Framework.Node node, bool recordUndo = true)
    {
      if (!this.nodes.Contains(node))
      {
        Debug.LogWarning((object) "Node is not part of this graph");
      }
      else
      {
        node.OnDestroy();
        foreach (Connection connection in node.inConnections.ToArray())
          this.RemoveConnection(connection);
        foreach (Connection connection in node.outConnections.ToArray())
          this.RemoveConnection(connection);
        if (recordUndo)
          this.RecordUndo("Delete Node");
        this.nodes.Remove(node);
        this.UpdateNodeIDs();
      }
    }

    public Connection ConnectNodes(NodeCanvas.Framework.Node sourceNode, NodeCanvas.Framework.Node targetNode, int indexToInsert)
    {
      if (!targetNode.IsNewConnectionAllowed(sourceNode))
        return (Connection) null;
      this.RecordUndo("New Connection");
      Connection connection = Connection.Create(sourceNode, targetNode, indexToInsert);
      sourceNode.OnChildConnected(indexToInsert);
      targetNode.OnParentConnected(targetNode.inConnections.IndexOf(connection));
      return connection;
    }

    public void RemoveConnection(Connection connection, bool recordUndo = true)
    {
      if (Application.isPlaying)
        connection.Reset();
      if (recordUndo)
        this.RecordUndo("Delete Connection");
      connection.OnDestroy();
      connection.sourceNode.OnChildDisconnected(connection.sourceNode.outConnections.IndexOf(connection));
      connection.targetNode.OnParentDisconnected(connection.targetNode.inConnections.IndexOf(connection));
      connection.sourceNode.outConnections.Remove(connection);
      connection.targetNode.inConnections.Remove(connection);
    }

    private void RecordUndo(string name)
    {
    }

    public void OnDestroy()
    {
      foreach (NodeCanvas.Framework.Node node in this.nodes.ToList<NodeCanvas.Framework.Node>())
        node.OnDestroy();
    }
  }
}
