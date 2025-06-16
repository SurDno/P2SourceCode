using System;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Serialization;
using ParadoxNotion.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlowCanvas;

public class Graph {
	private bool hasInitialized;
	private List<IUpdatable> updatableNodes;

	public Type baseNodeType => typeof(FlowNode);

	private void OnGraphStarted() {
		if (hasInitialized)
			return;
		updatableNodes = new List<IUpdatable>();
		for (var index = 0; index < nodes.Count; ++index)
			if (nodes[index] is IUpdatable node)
				updatableNodes.Add(node);
		for (var index = 0; index < nodes.Count; ++index)
			if (nodes[index] is FlowNode) {
				var node = (FlowNode)nodes[index];
				node.AssignSelfInstancePort();
				node.BindPorts();
			}

		hasInitialized = true;
	}

	private void OnGraphUpdate() {
		if (updatableNodes == null)
			return;
		for (var index = 0; index < updatableNodes.Count; ++index)
			updatableNodes[index].Update();
	}

	public void Serialize(out string json, out List<Object> references) {
		var graphData = new GraphData();
		graphData.nodes = nodes;
		var connectionList = new List<Connection>();
		for (var index1 = 0; index1 < graphData.nodes.Count; ++index1) {
			for (var index2 = 0; index2 < graphData.nodes[index1].outConnections.Count; ++index2)
				connectionList.Add(graphData.nodes[index1].outConnections[index2]);
		}

		graphData.connections = connectionList;
		references = new List<Object>();
		json = JSONSerializer.Serialize(typeof(GraphData), graphData, objectReferences: references);
	}

	public void Deserialize(string serializedGraph, List<Object> objectReferences) {
		if (ReflectionTools.ContextObject == null)
			ReflectionTools.ContextObject = agent;
		var graphData = JSONSerializer.Deserialize<GraphData>(serializedGraph, objectReferences);
		ReflectionTools.ContextObject = null;
		for (var index = 0; index < graphData.connections.Count; ++index)
			if (graphData.connections[index].sourceNode == null)
				Debug.LogError("connections[i].sourceNode is null, i : " + index + " , graph : " +
				               agent.gameObject.GetFullName());
			else if (graphData.connections[index].targetNode == null)
				Debug.LogError("connections[i].targetNode is null, i : " + index + " , graph : " +
				               agent.gameObject.GetFullName());
			else {
				graphData.connections[index].sourceNode.outConnections.Add(graphData.connections[index]);
				graphData.connections[index].targetNode.inConnections.Add(graphData.connections[index]);
			}

		for (var index = 0; index < graphData.nodes.Count; ++index)
			graphData.nodes[index].graph = this;
		nodes = graphData.nodes;
		UpdateNodeIDs();
		OnValidate();
		CorrectPosition();
	}

	private void CorrectPosition() {
		zoomFactor = 1f;
		translation = Vector2.positiveInfinity;
		for (var index = 0; index < nodes.Count; ++index) {
			var node = nodes[index];
			if (node.nodePosition.x < (double)translation.x)
				translation = node.nodePosition;
		}

		if (translation == Vector2.positiveInfinity)
			translation = Vector2.zero;
		translation = new Vector2((float)(-(double)translation.x + 500.0), (float)(-(double)translation.y + 500.0));
	}

	public void OnValidate() {
		for (var index = 0; index < nodes.Count; ++index) {
			var node = nodes[index];
			try {
				node.OnValidate(this);
			} catch (Exception ex) {
				Debug.LogError(ex.ToString());
			}
		}
	}

	public bool isRunning { get; private set; }

	public bool isPaused { get; private set; }

	public List<Node> nodes { get; private set; } = new();

	public Vector2 translation { get; set; }

	public float zoomFactor { get; set; } = 1f;

	public FlowScriptController agent { get; set; }

	public string agentName { get; private set; }

	public void UpdateReferences() {
		UpdateNodeBBFields();
	}

	private void UpdateNodeBBFields() {
		for (var index = 0; index < nodes.Count; ++index)
			BBParameter.SetBBFields(nodes[index], agent.blackboard);
	}

	public void UpdateNodeIDs() {
		for (var index = 0; index < nodes.Count; ++index) {
			nodes[index].ResetRecursion();
			nodes[index].Id = index + 1;
		}
	}

	public void StartGraph() {
		if (isRunning)
			return;
		agentName = agent.name;
		UpdateReferences();
		isRunning = true;
		if (!isPaused)
			OnGraphStarted();
		for (var index = 0; index < nodes.Count; ++index)
			if (!isPaused)
				nodes[index].OnGraphStarted();
			else
				nodes[index].OnGraphUnpaused();
		isPaused = false;
		BlueprintManager.current.graphs.Add(this);
		UpdateGraph();
	}

	public void Stop(bool success = true) {
		if (!isRunning && !isPaused)
			return;
		BlueprintManager.current.graphs.Remove(this);
		isRunning = false;
		isPaused = false;
		for (var index = 0; index < nodes.Count; ++index) {
			nodes[index].Reset(false);
			nodes[index].OnGraphStoped();
		}
	}

	public void Pause() {
		if (!isRunning)
			return;
		BlueprintManager.current.graphs.Remove(this);
		isRunning = false;
		isPaused = true;
		for (var index = 0; index < nodes.Count; ++index)
			nodes[index].OnGraphPaused();
	}

	public void UpdateGraph() {
		OnGraphUpdate();
	}

	public void SendEvent(EventData eventData) {
		if (!isRunning || eventData == null || !(agent != null))
			return;
		var component = agent.GetComponent<MessageRouter>();
		if (component != null) {
			component.Dispatch("OnCustomEvent", eventData);
			component.Dispatch(eventData.name, eventData.value);
		}
	}

	public List<T> GetAllNodesOfType<T>() where T : Node {
		return nodes.OfType<T>().ToList();
	}

	public BBParameter[] GetDefinedParameters() {
		var bbParameterList = new List<BBParameter>();
		var objectList = new List<object>();
		objectList.AddRange(nodes);
		for (var index = 0; index < objectList.Count; ++index)
			foreach (var objectBbParameter in BBParameter.GetObjectBBParameters(objectList[index]))
				if (objectBbParameter != null && objectBbParameter.useBlackboard && !objectBbParameter.isNone)
					bbParameterList.Add(objectBbParameter);
		return bbParameterList.ToArray();
	}

	public void CreateDefinedParameterVariables(Blackboard bb) {
		foreach (var definedParameter in GetDefinedParameters())
			definedParameter.PromoteToVariable(bb);
	}

	public Node AddNode(Type nodeType, Vector2 pos) {
		if (!nodeType.RTIsSubclassOf(baseNodeType)) {
			Debug.LogWarning(nodeType + " can't be added to " + GetType().FriendlyName() + " graph");
			return null;
		}

		var node = Node.Create(this, nodeType, pos);
		RecordUndo("New Node");
		nodes.Add(node);
		UpdateNodeIDs();
		return node;
	}

	public void RemoveNode(Node node, bool recordUndo = true) {
		if (!nodes.Contains(node))
			Debug.LogWarning("Node is not part of this graph");
		else {
			node.OnDestroy();
			foreach (var connection in node.inConnections.ToArray())
				RemoveConnection(connection);
			foreach (var connection in node.outConnections.ToArray())
				RemoveConnection(connection);
			if (recordUndo)
				RecordUndo("Delete Node");
			nodes.Remove(node);
			UpdateNodeIDs();
		}
	}

	public Connection ConnectNodes(Node sourceNode, Node targetNode, int indexToInsert) {
		if (!targetNode.IsNewConnectionAllowed(sourceNode))
			return null;
		RecordUndo("New Connection");
		var connection = Connection.Create(sourceNode, targetNode, indexToInsert);
		sourceNode.OnChildConnected(indexToInsert);
		targetNode.OnParentConnected(targetNode.inConnections.IndexOf(connection));
		return connection;
	}

	public void RemoveConnection(Connection connection, bool recordUndo = true) {
		if (Application.isPlaying)
			connection.Reset();
		if (recordUndo)
			RecordUndo("Delete Connection");
		connection.OnDestroy();
		connection.sourceNode.OnChildDisconnected(connection.sourceNode.outConnections.IndexOf(connection));
		connection.targetNode.OnParentDisconnected(connection.targetNode.inConnections.IndexOf(connection));
		connection.sourceNode.outConnections.Remove(connection);
		connection.targetNode.inConnections.Remove(connection);
	}

	private void RecordUndo(string name) { }

	public void OnDestroy() {
		foreach (var node in nodes.ToList())
			node.OnDestroy();
	}
}