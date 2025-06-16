using FlowCanvas;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using System;
using UnityEngine;

namespace NodeCanvas.Framework
{
  [SpoofAOT]
  public abstract class Connection
  {
    [SerializeField]
    private Node _sourceNode;
    [SerializeField]
    private Node _targetNode;
    [SerializeField]
    private bool _isDisabled;
    [NonSerialized]
    private NodeCanvas.Status _status = NodeCanvas.Status.Resting;

    public Node sourceNode
    {
      get => this._sourceNode;
      protected set => this._sourceNode = value;
    }

    public Node targetNode
    {
      get => this._targetNode;
      protected set => this._targetNode = value;
    }

    public bool isActive
    {
      get => !this._isDisabled;
      set
      {
        if (!this._isDisabled && !value)
          this.Reset();
        this._isDisabled = !value;
      }
    }

    public NodeCanvas.Status status
    {
      get => this._status;
      set => this._status = value;
    }

    protected Graph graph => this.sourceNode.graph;

    public static Connection Create(Node source, Node target, int sourceIndex)
    {
      if (source == null || target == null)
      {
        Debug.LogError((object) "Can't Create a Connection without providing Source and Target Nodes");
        return (Connection) null;
      }
      if (source is MissingNode)
      {
        Debug.LogError((object) "Creating new Connections from a 'MissingNode' is not allowed. Please resolve the MissingNode node first");
        return (Connection) null;
      }
      Connection instance = (Connection) Activator.CreateInstance(source.outConnectionType);
      instance.sourceNode = source;
      instance.targetNode = target;
      source.outConnections.Insert(sourceIndex, instance);
      target.inConnections.Add(instance);
      instance.OnValidate(sourceIndex, target.inConnections.IndexOf(instance));
      return instance;
    }

    public Connection Duplicate(Node newSource, Node newTarget)
    {
      if (newSource == null || newTarget == null)
      {
        Debug.LogError((object) "Can't Duplicate a Connection without providing NewSource and NewTarget Nodes");
        return (Connection) null;
      }
      Connection connection = JSONSerializer.Deserialize<Connection>(JSONSerializer.Serialize(typeof (Connection), (object) this));
      connection.SetSource(newSource, false);
      connection.SetTarget(newTarget, false);
      connection.OnValidate(newSource.outConnections.IndexOf(connection), newTarget.inConnections.IndexOf(connection));
      return connection;
    }

    public virtual void OnValidate(int sourceIndex, int targetIndex)
    {
    }

    public virtual void OnDestroy()
    {
    }

    public void SetSource(Node newSource, bool isRelink = true)
    {
      if (isRelink)
      {
        int connectionIndex = this.sourceNode.outConnections.IndexOf(this);
        this.sourceNode.OnChildDisconnected(connectionIndex);
        newSource.OnChildConnected(connectionIndex);
        this.sourceNode.outConnections.Remove(this);
      }
      newSource.outConnections.Add(this);
      this.sourceNode = newSource;
    }

    public void SetTarget(Node newTarget, bool isRelink = true)
    {
      if (isRelink)
      {
        int connectionIndex = this.targetNode.inConnections.IndexOf(this);
        this.targetNode.OnParentDisconnected(connectionIndex);
        newTarget.OnParentConnected(connectionIndex);
        this.targetNode.inConnections.Remove(this);
      }
      newTarget.inConnections.Add(this);
      this.targetNode = newTarget;
    }

    public NodeCanvas.Status Execute(FlowScriptController agent, Blackboard blackboard)
    {
      if (!this.isActive)
        return NodeCanvas.Status.Resting;
      this.status = this.targetNode.Execute(agent, blackboard);
      return this.status;
    }

    public void Reset(bool recursively = true)
    {
      if (this.status == NodeCanvas.Status.Resting)
        return;
      this.status = NodeCanvas.Status.Resting;
      if (!recursively)
        return;
      this.targetNode.Reset(recursively);
    }
  }
}
