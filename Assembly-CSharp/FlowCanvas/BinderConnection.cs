using System;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace FlowCanvas
{
  public class BinderConnection : Connection
  {
    [SerializeField]
    private string _sourcePortName;
    [SerializeField]
    private string _targetPortName;
    [NonSerialized]
    private Port _sourcePort;
    [NonSerialized]
    private Port _targetPort;

    public string sourcePortId
    {
      get => sourcePort != null ? sourcePort.id : _sourcePortName;
      set => _sourcePortName = value;
    }

    public string targetPortId
    {
      get => targetPort != null ? targetPort.id : _targetPortName;
      private set => _targetPortName = value;
    }

    public Port sourcePort
    {
      get
      {
        if (_sourcePort == null && sourceNode is FlowNode)
          _sourcePort = (sourceNode as FlowNode).GetOutputPort(_sourcePortName);
        return _sourcePort;
      }
    }

    public Port targetPort
    {
      get
      {
        if (_targetPort == null && targetNode is FlowNode)
          _targetPort = (targetNode as FlowNode).GetInputPort(_targetPortName);
        return _targetPort;
      }
    }

    public Type bindingType
    {
      get
      {
        return GetType().RTIsGenericType() ? GetType().RTGetGenericArguments()[0] : typeof (void);
      }
    }

    public void GatherAndValidateSourcePort()
    {
      _sourcePort = null;
      if (sourcePort != null && TypeConverter.HasConvertion(sourcePort.type, bindingType))
        ++sourcePort.connections;
      else
        graph.RemoveConnection(this, false);
    }

    public void GatherAndValidateTargetPort()
    {
      _targetPort = null;
      if (targetPort != null && targetPort.type == bindingType)
        ++targetPort.connections;
      else
        graph.RemoveConnection(this, false);
    }

    public static BinderConnection Create(Port source, Port target)
    {
      if (source == null || target == null)
      {
        Debug.LogError((object) "Source Port or Target Port is null when making a new Binder Connection");
        return null;
      }
      if (!source.CanAcceptConnections())
      {
        Debug.LogWarning((object) "Source port can accept no more connections");
        return null;
      }
      if (!target.CanAcceptConnections())
      {
        Debug.LogWarning((object) "Target port can accept no more connections");
        return null;
      }
      if (source.parent == target.parent)
      {
        Debug.LogWarning((object) "Can't connect ports on the same parent node");
        return null;
      }
      if (source is FlowOutput && !(target is FlowInput))
      {
        Debug.LogWarning((object) "Flow ports can only be connected to other Flow ports");
        return null;
      }
      int num1;
      switch (source)
      {
        case FlowInput _ when target is FlowInput:
          num1 = 1;
          break;
        case ValueInput _:
          num1 = target is ValueInput ? 1 : 0;
          break;
        default:
          num1 = 0;
          break;
      }
      if (num1 != 0)
      {
        Debug.LogWarning((object) "Can't connect input to input");
        return null;
      }
      int num2;
      switch (source)
      {
        case FlowOutput _ when target is FlowOutput:
          num2 = 1;
          break;
        case ValueOutput _:
          num2 = target is ValueOutput ? 1 : 0;
          break;
        default:
          num2 = 0;
          break;
      }
      if (num2 != 0)
      {
        Debug.LogWarning((object) "Can't connect output to output");
        return null;
      }
      if (!TypeConverter.HasConvertion(source.type, target.type))
      {
        Debug.LogWarning((object) string.Format("Can't connect ports. Type '{0}' is not assignable from Type '{1}' and there exists no internal convertion for those types.", target.type.FriendlyName(), source.type.FriendlyName()));
        return null;
      }
      if (source is FlowOutput && target is FlowInput)
      {
        BinderConnection binderConnection = new BinderConnection();
        binderConnection.OnCreate(source, target);
        return binderConnection;
      }
      if (!(source is ValueOutput) || !(target is ValueInput))
        return null;
      BinderConnection instance = (BinderConnection) Activator.CreateInstance(typeof (BinderConnection<>).RTMakeGenericType(new Type[1]
      {
        target.type
      }));
      instance.OnCreate(source, target);
      return instance;
    }

    public virtual void Bind()
    {
      if (!isActive || !(sourcePort is FlowOutput) || !(targetPort is FlowInput))
        return;
      (sourcePort as FlowOutput).BindTo((FlowInput) targetPort);
    }

    public virtual void UnBind()
    {
      if (!(sourcePort is FlowOutput))
        return;
      (sourcePort as FlowOutput).UnBind();
    }

    private void OnCreate(Port source, Port target)
    {
      sourceNode = source.parent;
      targetNode = target.parent;
      sourcePortId = source.id;
      targetPortId = target.id;
      sourceNode.outConnections.Add(this);
      targetNode.inConnections.Add(this);
      ++source.connections;
      ++target.connections;
      if (!Application.isPlaying)
        return;
      Bind();
    }

    public override void OnDestroy()
    {
      if (sourcePort != null)
        --sourcePort.connections;
      if (targetPort != null)
        --targetPort.connections;
      if (!Application.isPlaying)
        return;
      UnBind();
    }
  }
}
