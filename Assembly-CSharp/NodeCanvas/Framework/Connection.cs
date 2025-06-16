using System;
using FlowCanvas;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;

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
    private Status _status = Status.Resting;

    public Node sourceNode
    {
      get => _sourceNode;
      protected set => _sourceNode = value;
    }

    public Node targetNode
    {
      get => _targetNode;
      protected set => _targetNode = value;
    }

    public bool isActive
    {
      get => !_isDisabled;
      set
      {
        if (!_isDisabled && !value)
          Reset();
        _isDisabled = !value;
      }
    }

    public Status status
    {
      get => _status;
      set => _status = value;
    }

    protected Graph graph => sourceNode.graph;

    public static Connection Create(Node source, Node target, int sourceIndex)
    {
      if (source == null || target == null)
      {
        Debug.LogError((object) "Can't Create a Connection without providing Source and Target Nodes");
        return null;
      }
      if (source is MissingNode)
      {
        Debug.LogError((object) "Creating new Connections from a 'MissingNode' is not allowed. Please resolve the MissingNode node first");
        return null;
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
        return null;
      }
      Connection connection = JSONSerializer.Deserialize<Connection>(JSONSerializer.Serialize(typeof (Connection), this));
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
        int connectionIndex = sourceNode.outConnections.IndexOf(this);
        sourceNode.OnChildDisconnected(connectionIndex);
        newSource.OnChildConnected(connectionIndex);
        sourceNode.outConnections.Remove(this);
      }
      newSource.outConnections.Add(this);
      sourceNode = newSource;
    }

    public void SetTarget(Node newTarget, bool isRelink = true)
    {
      if (isRelink)
      {
        int connectionIndex = targetNode.inConnections.IndexOf(this);
        targetNode.OnParentDisconnected(connectionIndex);
        newTarget.OnParentConnected(connectionIndex);
        targetNode.inConnections.Remove(this);
      }
      newTarget.inConnections.Add(this);
      targetNode = newTarget;
    }

    public Status Execute(FlowScriptController agent, Blackboard blackboard)
    {
      if (!isActive)
        return Status.Resting;
      status = targetNode.Execute(agent, blackboard);
      return status;
    }

    public void Reset(bool recursively = true)
    {
      if (status == Status.Resting)
        return;
      status = Status.Resting;
      if (!recursively)
        return;
      targetNode.Reset(recursively);
    }
  }
}
