using FlowCanvas;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using ParadoxNotion.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private NodeCanvas.Status _status = NodeCanvas.Status.Resting;
    private string _nodeName;
    private string _nodeDescription;

    public Graph graph
    {
      get => this._graph;
      set => this._graph = value;
    }

    public int Id
    {
      get => this._id;
      set => this._id = value;
    }

    public List<Connection> inConnections
    {
      get => this._inConnections;
      protected set => this._inConnections = value;
    }

    public List<Connection> outConnections
    {
      get => this._outConnections;
      protected set => this._outConnections = value;
    }

    public Vector2 nodePosition
    {
      get => this._position;
      set => this._position = value;
    }

    public string UID
    {
      get => string.IsNullOrEmpty(this._UID) ? (this._UID = Guid.NewGuid().ToString()) : this._UID;
    }

    public bool isBreakpoint
    {
      get => this._isBreakpoint;
      set => this._isBreakpoint = value;
    }

    public virtual string name
    {
      get
      {
        if (string.IsNullOrEmpty(this._nodeName))
        {
          NameAttribute attribute = ReflectionTools.RTGetAttribute<NameAttribute>(this.GetType(), false);
          this._nodeName = attribute != null ? attribute.name : this.GetType().FriendlyName().SplitCamelCase();
        }
        return this._nodeName;
      }
    }

    public virtual string description
    {
      get
      {
        if (string.IsNullOrEmpty(this._nodeDescription))
        {
          DescriptionAttribute attribute = ReflectionTools.RTGetAttribute<DescriptionAttribute>(this.GetType(), false);
          this._nodeDescription = attribute != null ? attribute.description : "No Description";
        }
        return this._nodeDescription;
      }
    }

    public abstract int maxInConnections { get; }

    public abstract int maxOutConnections { get; }

    public abstract System.Type outConnectionType { get; }

    public abstract bool showCommentsBottom { get; }

    public NodeCanvas.Status status
    {
      get => this._status;
      protected set => this._status = value;
    }

    public FlowScriptController graphAgent => this.graph?.agent;

    public Blackboard graphBlackboard => this.graph?.agent.blackboard;

    private bool isChecked { get; set; }

    public static Node Create(Graph targetGraph, System.Type nodeType, Vector2 pos)
    {
      if (targetGraph == null)
      {
        Debug.LogError((object) "Can't Create a Node without providing a Target Graph");
        return (Node) null;
      }
      Node instance = (Node) Activator.CreateInstance(nodeType);
      instance.graph = targetGraph;
      instance.nodePosition = pos;
      BBParameter.SetBBFields((object) instance, targetGraph.agent.blackboard);
      instance.OnValidate(targetGraph);
      return instance;
    }

    public Node Duplicate(Graph targetGraph)
    {
      if (targetGraph == null)
      {
        Debug.LogError((object) "Can't duplicate a Node without providing a Target Graph");
        return (Node) null;
      }
      Node o = JSONSerializer.Deserialize<Node>(JSONSerializer.Serialize(typeof (Node), (object) this));
      targetGraph.nodes.Add(o);
      o.inConnections.Clear();
      o.outConnections.Clear();
      if (targetGraph == this.graph)
        o.nodePosition += new Vector2(50f, 50f);
      o.graph = targetGraph;
      BBParameter.SetBBFields((object) o, targetGraph.agent.blackboard);
      o.OnValidate(targetGraph);
      return o;
    }

    public virtual void OnValidate(Graph assignedGraph)
    {
    }

    public virtual void OnDestroy()
    {
    }

    public NodeCanvas.Status Execute(FlowScriptController agent, Blackboard blackboard)
    {
      if (this.isChecked)
        return this.Error("Infinite Loop. Please check for other errors that may have caused this in the log before this.");
      this.isChecked = true;
      this.status = this.OnExecute((Component) agent, blackboard);
      this.isChecked = false;
      return this.status;
    }

    private IEnumerator YieldBreak(Component agent, Blackboard blackboard)
    {
      Debug.Break();
      yield return (object) null;
      this.status = this.OnExecute(agent, blackboard);
    }

    protected NodeCanvas.Status Error(string log)
    {
      Debug.LogError((object) ("<b>Graph Error:</b> '" + log + "' On node '" + this.name + "' ID " + (object) this.Id + " | On graph '" + (object) this.graph.agent + "'"));
      return NodeCanvas.Status.Error;
    }

    public void Reset(bool recursively = true)
    {
      if (this.status == NodeCanvas.Status.Resting || this.isChecked)
        return;
      this.status = NodeCanvas.Status.Resting;
      this.isChecked = true;
      for (int index = 0; index < this.outConnections.Count; ++index)
        this.outConnections[index].Reset(recursively);
      this.isChecked = false;
    }

    public void SendEvent(EventData eventData) => this.graph.SendEvent(eventData);

    public void RegisterEvents(params string[] eventNames)
    {
      this.RegisterEvents((Component) this.graphAgent, eventNames);
    }

    public void RegisterEvents(Component targetAgent, params string[] eventNames)
    {
      if ((UnityEngine.Object) targetAgent == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "Null Agent provided for event registration");
      }
      else
      {
        MessageRouter messageRouter = targetAgent.GetComponent<MessageRouter>();
        if ((UnityEngine.Object) messageRouter == (UnityEngine.Object) null)
          messageRouter = targetAgent.gameObject.AddComponent<MessageRouter>();
        messageRouter.Register((object) this, eventNames);
      }
    }

    public void UnRegisterEvents(params string[] eventNames)
    {
      this.UnRegisterEvents((Component) this.graphAgent, eventNames);
    }

    public void UnRegisterEvents(Component targetAgent, params string[] eventNames)
    {
      if ((UnityEngine.Object) targetAgent == (UnityEngine.Object) null)
        return;
      MessageRouter component = targetAgent.GetComponent<MessageRouter>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      component.UnRegister((object) this, eventNames);
    }

    public void UnregisterAllEvents() => this.UnregisterAllEvents((Component) this.graphAgent);

    public void UnregisterAllEvents(Component targetAgent)
    {
      if ((UnityEngine.Object) targetAgent == (UnityEngine.Object) null)
        return;
      MessageRouter component = targetAgent.GetComponent<MessageRouter>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      component.UnRegister((object) this);
    }

    public bool IsNewConnectionAllowed() => this.IsNewConnectionAllowed((Node) null);

    public bool IsNewConnectionAllowed(Node sourceNode)
    {
      if (sourceNode != null)
      {
        if (this == sourceNode)
        {
          Debug.LogWarning((object) "Node can't connect to itself");
          return false;
        }
        if (sourceNode.outConnections.Count >= sourceNode.maxOutConnections && sourceNode.maxOutConnections != -1)
        {
          Debug.LogWarning((object) "Source node can have no more out connections.");
          return false;
        }
      }
      if (this.maxInConnections > this.inConnections.Count || this.maxInConnections == -1)
        return true;
      Debug.LogWarning((object) "Target node can have no more connections");
      return false;
    }

    public void ResetRecursion()
    {
      if (!this.isChecked)
        return;
      this.isChecked = false;
      for (int index = 0; index < this.outConnections.Count; ++index)
        this.outConnections[index].targetNode.ResetRecursion();
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
      return this.inConnections.Count != 0 ? this.inConnections.Select<Connection, Node>((Func<Connection, Node>) (c => c.sourceNode)).ToList<Node>() : new List<Node>();
    }

    public List<Node> GetChildNodes()
    {
      return this.outConnections.Count != 0 ? this.outConnections.Select<Connection, Node>((Func<Connection, Node>) (c => c.targetNode)).ToList<Node>() : new List<Node>();
    }

    protected virtual NodeCanvas.Status OnExecute(Component agent, Blackboard blackboard)
    {
      return this.status;
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

    public override sealed string ToString() => this.name;
  }
}
