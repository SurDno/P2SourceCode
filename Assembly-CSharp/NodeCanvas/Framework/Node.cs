using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FlowCanvas;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using ParadoxNotion.Services;
using UnityEngine;

namespace NodeCanvas.Framework
{
  [SpoofAOT]
  [Serializable]
  public abstract class Node
  {
    [SerializeField]
    private Vector2 _position;
    [SerializeField]
    private string _UID;
    private bool _isBreakpoint;
    private Graph _graph;
    private List<Connection> _inConnections = new List<Connection>();
    private List<Connection> _outConnections = new List<Connection>();
    private int _id;
    private Status _status = Status.Resting;
    private string _nodeName;
    private string _nodeDescription;

    public Graph graph
    {
      get => _graph;
      set => _graph = value;
    }

    public int Id
    {
      get => _id;
      set => _id = value;
    }

    public List<Connection> inConnections
    {
      get => _inConnections;
      protected set => _inConnections = value;
    }

    public List<Connection> outConnections
    {
      get => _outConnections;
      protected set => _outConnections = value;
    }

    public Vector2 nodePosition
    {
      get => _position;
      set => _position = value;
    }

    public string UID
    {
      get => string.IsNullOrEmpty(_UID) ? (_UID = Guid.NewGuid().ToString()) : _UID;
    }

    public bool isBreakpoint
    {
      get => _isBreakpoint;
      set => _isBreakpoint = value;
    }

    public virtual string name
    {
      get
      {
        if (string.IsNullOrEmpty(_nodeName))
        {
          NameAttribute attribute = GetType().RTGetAttribute<NameAttribute>(false);
          _nodeName = attribute != null ? attribute.name : GetType().FriendlyName().SplitCamelCase();
        }
        return _nodeName;
      }
    }

    public virtual string description
    {
      get
      {
        if (string.IsNullOrEmpty(_nodeDescription))
        {
          DescriptionAttribute attribute = GetType().RTGetAttribute<DescriptionAttribute>(false);
          _nodeDescription = attribute != null ? attribute.description : "No Description";
        }
        return _nodeDescription;
      }
    }

    public abstract int maxInConnections { get; }

    public abstract int maxOutConnections { get; }

    public abstract Type outConnectionType { get; }

    public abstract bool showCommentsBottom { get; }

    public Status status
    {
      get => _status;
      protected set => _status = value;
    }

    public FlowScriptController graphAgent => graph?.agent;

    public Blackboard graphBlackboard => graph?.agent.blackboard;

    private bool isChecked { get; set; }

    public static Node Create(Graph targetGraph, Type nodeType, Vector2 pos)
    {
      if (targetGraph == null)
      {
        Debug.LogError("Can't Create a Node without providing a Target Graph");
        return null;
      }
      Node instance = (Node) Activator.CreateInstance(nodeType);
      instance.graph = targetGraph;
      instance.nodePosition = pos;
      BBParameter.SetBBFields(instance, targetGraph.agent.blackboard);
      instance.OnValidate(targetGraph);
      return instance;
    }

    public Node Duplicate(Graph targetGraph)
    {
      if (targetGraph == null)
      {
        Debug.LogError("Can't duplicate a Node without providing a Target Graph");
        return null;
      }
      Node o = JSONSerializer.Deserialize<Node>(JSONSerializer.Serialize(typeof (Node), this));
      targetGraph.nodes.Add(o);
      o.inConnections.Clear();
      o.outConnections.Clear();
      if (targetGraph == graph)
        o.nodePosition += new Vector2(50f, 50f);
      o.graph = targetGraph;
      BBParameter.SetBBFields(o, targetGraph.agent.blackboard);
      o.OnValidate(targetGraph);
      return o;
    }

    public virtual void OnValidate(Graph assignedGraph)
    {
    }

    public virtual void OnDestroy()
    {
    }

    public Status Execute(FlowScriptController agent, Blackboard blackboard)
    {
      if (isChecked)
        return Error("Infinite Loop. Please check for other errors that may have caused this in the log before this.");
      isChecked = true;
      status = OnExecute(agent, blackboard);
      isChecked = false;
      return status;
    }

    private IEnumerator YieldBreak(Component agent, Blackboard blackboard)
    {
      Debug.Break();
      yield return null;
      status = OnExecute(agent, blackboard);
    }

    protected Status Error(string log)
    {
      Debug.LogError("<b>Graph Error:</b> '" + log + "' On node '" + name + "' ID " + Id + " | On graph '" + graph.agent + "'");
      return Status.Error;
    }

    public void Reset(bool recursively = true)
    {
      if (status == Status.Resting || isChecked)
        return;
      status = Status.Resting;
      isChecked = true;
      for (int index = 0; index < outConnections.Count; ++index)
        outConnections[index].Reset(recursively);
      isChecked = false;
    }

    public void SendEvent(EventData eventData) => graph.SendEvent(eventData);

    public void RegisterEvents(params string[] eventNames)
    {
      RegisterEvents(graphAgent, eventNames);
    }

    public void RegisterEvents(Component targetAgent, params string[] eventNames)
    {
      if (targetAgent == null)
      {
        Debug.LogError("Null Agent provided for event registration");
      }
      else
      {
        MessageRouter messageRouter = targetAgent.GetComponent<MessageRouter>();
        if (messageRouter == null)
          messageRouter = targetAgent.gameObject.AddComponent<MessageRouter>();
        messageRouter.Register(this, eventNames);
      }
    }

    public void UnRegisterEvents(params string[] eventNames)
    {
      UnRegisterEvents(graphAgent, eventNames);
    }

    public void UnRegisterEvents(Component targetAgent, params string[] eventNames)
    {
      if (targetAgent == null)
        return;
      MessageRouter component = targetAgent.GetComponent<MessageRouter>();
      if (!(component != null))
        return;
      component.UnRegister(this, eventNames);
    }

    public void UnregisterAllEvents() => UnregisterAllEvents(graphAgent);

    public void UnregisterAllEvents(Component targetAgent)
    {
      if (targetAgent == null)
        return;
      MessageRouter component = targetAgent.GetComponent<MessageRouter>();
      if (!(component != null))
        return;
      component.UnRegister(this);
    }

    public bool IsNewConnectionAllowed() => IsNewConnectionAllowed(null);

    public bool IsNewConnectionAllowed(Node sourceNode)
    {
      if (sourceNode != null)
      {
        if (this == sourceNode)
        {
          Debug.LogWarning("Node can't connect to itself");
          return false;
        }
        if (sourceNode.outConnections.Count >= sourceNode.maxOutConnections && sourceNode.maxOutConnections != -1)
        {
          Debug.LogWarning("Source node can have no more out connections.");
          return false;
        }
      }
      if (maxInConnections > inConnections.Count || maxInConnections == -1)
        return true;
      Debug.LogWarning("Target node can have no more connections");
      return false;
    }

    public void ResetRecursion()
    {
      if (!isChecked)
        return;
      isChecked = false;
      for (int index = 0; index < outConnections.Count; ++index)
        outConnections[index].targetNode.ResetRecursion();
    }

    protected Coroutine StartCoroutine(IEnumerator routine)
    {
      return BlueprintManager.current.StartCoroutine(routine);
    }

    protected void StopCoroutine(Coroutine routine)
    {
      BlueprintManager.current.StopCoroutine(routine);
    }

    public List<Node> GetParentNodes()
    {
      return inConnections.Count != 0 ? inConnections.Select(c => c.sourceNode).ToList() : new List<Node>();
    }

    public List<Node> GetChildNodes()
    {
      return outConnections.Count != 0 ? outConnections.Select(c => c.targetNode).ToList() : new List<Node>();
    }

    protected virtual Status OnExecute(Component agent, Blackboard blackboard)
    {
      return status;
    }

    public virtual void OnParentConnected(int connectionIndex)
    {
    }

    public virtual void OnParentDisconnected(int connectionIndex)
    {
    }

    public virtual void OnChildConnected(int connectionIndex)
    {
    }

    public virtual void OnChildDisconnected(int connectionIndex)
    {
    }

    public virtual void OnGraphStarted()
    {
    }

    public virtual void OnGraphStoped()
    {
    }

    public virtual void OnGraphPaused()
    {
    }

    public virtual void OnGraphUnpaused()
    {
    }

    public override sealed string ToString() => name;
  }
}
