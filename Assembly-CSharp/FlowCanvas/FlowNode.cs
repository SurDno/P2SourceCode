using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cofe.Meta;
using Engine.Common.Services;
using FlowCanvas.Nodes;
using NodeCanvas;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas
{
  public abstract class FlowNode : Node, ISerializationCallbackReceiver
  {
    [SerializeField]
    private Dictionary<string, object> _inputPortValues;
    private Dictionary<string, Port> inputPorts = new(StringComparer.Ordinal);
    protected Dictionary<string, Port> outputPorts = new(StringComparer.Ordinal);

    public override sealed int maxInConnections => -1;

    public override sealed int maxOutConnections => -1;

    public override sealed Type outConnectionType => typeof (BinderConnection);

    public override sealed bool showCommentsBottom => true;

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
      if (_inputPortValues == null)
        _inputPortValues = new Dictionary<string, object>();
      foreach (ValueInput valueInput in inputPorts.Values.OfType<ValueInput>())
      {
        if (!valueInput.isConnected)
          _inputPortValues[valueInput.id] = valueInput.serializedValue;
      }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
    }

    public override sealed void OnValidate(Graph flowGraph) => GatherPorts();

    public override sealed void OnParentConnected(int i)
    {
    }

    public override sealed void OnChildConnected(int i)
    {
    }

    public override sealed void OnParentDisconnected(int i)
    {
    }

    public override sealed void OnChildDisconnected(int i)
    {
    }

    private static void ConnectPorts(Port source, Port target)
    {
      BinderConnection.Create(source, target);
    }

    public void BindPorts()
    {
      for (int index = 0; index < outConnections.Count; ++index)
        ((BinderConnection) outConnections[index]).Bind();
    }

    public void UnBindPorts()
    {
      for (int index = 0; index < outConnections.Count; ++index)
        ((BinderConnection) outConnections[index]).UnBind();
    }

    public Port GetInputPort(string id)
    {
      inputPorts.TryGetValue(id, out Port inputPort);
      return inputPort;
    }

    public Port GetOutputPort(string id)
    {
      outputPorts.TryGetValue(id, out Port outputPort);
      return outputPort;
    }

    public BinderConnection GetInputConnectionForPortId(string id)
    {
      return inConnections.OfType<BinderConnection>().FirstOrDefault(c => c.targetPortId == id);
    }

    public BinderConnection GetOutputConnectionForPortId(string id)
    {
      return outConnections.OfType<BinderConnection>().FirstOrDefault(c => c.sourcePortId == id);
    }

    public Port GetFirstInputOfType(Type type)
    {
      return inputPorts.Values.OrderBy(p => !(p.GetType() == typeof (FlowInput)) ? 1 : 0).FirstOrDefault(p => p.type.RTIsAssignableFrom(type));
    }

    public Port GetFirstOutputOfType(Type type)
    {
      return outputPorts.Values.OrderBy(p => !(p.GetType() == typeof (FlowInput)) ? 1 : 0).FirstOrDefault(p => type.RTIsAssignableFrom(p.type));
    }

    public void AssignSelfInstancePort()
    {
      if (graphAgent == null)
        return;
      ValueInput valueInput = inputPorts.Values.OfType<ValueInput>().FirstOrDefault();
      if (valueInput == null || valueInput.isConnected || !valueInput.isDefaultValue)
        return;
      if (valueInput.type == typeof (GameObject))
        valueInput.serializedValue = graphAgent.gameObject;
      if (typeof (Component).RTIsAssignableFrom(valueInput.type))
        valueInput.serializedValue = graphAgent.GetComponent(valueInput.type);
    }

    public void GatherPorts()
    {
      inputPorts.Clear();
      outputPorts.Clear();
      RegisterPorts();
      DeserializeInputPortValues();
      ValidateConnections();
    }

    protected virtual void RegisterPorts() => DoReflectionBasedRegistration();

    private void DoReflectionBasedRegistration()
    {
      MetaService.GetContainer(GetType()).GetHandler(FromLocatorAttribute.Id).Compute(this, null);
      MetaService.GetContainer(GetType()).GetHandler(PortAttribute.Id).Compute(this, null);
    }

    private void ValidateConnections()
    {
      foreach (Connection connection in outConnections.ToArray())
      {
        if (connection is BinderConnection)
          (connection as BinderConnection).GatherAndValidateSourcePort();
      }
      foreach (Connection connection in inConnections.ToArray())
      {
        if (connection is BinderConnection)
          (connection as BinderConnection).GatherAndValidateTargetPort();
      }
    }

    private void DeserializeInputPortValues()
    {
      if (_inputPortValues == null)
        return;
      foreach (KeyValuePair<string, object> inputPortValue in _inputPortValues)
      {
        if (inputPorts.TryGetValue(inputPortValue.Key, out Port port) && port is ValueInput && inputPortValue.Value != null && port.type.RTIsAssignableFrom(inputPortValue.Value.GetType()))
          (port as ValueInput).serializedValue = inputPortValue.Value;
      }
    }

    public FlowInput AddFlowInput(string name, FlowHandler pointer, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (FlowInput) (inputPorts[id] = new FlowInput(this, name, id, pointer));
    }

    public FlowOutput AddFlowOutput(string name, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (FlowOutput) (outputPorts[id] = new FlowOutput(this, name, id));
    }

    public ValueInput<T> AddValueInput<T>(string name, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (ValueInput<T>) (inputPorts[id] = new ValueInput<T>(this, name, id));
    }

    public ValueOutput<T> AddValueOutput<T>(string name, ValueHandler<T> getter, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (ValueOutput<T>) (outputPorts[id] = new ValueOutput<T>(this, name, id, getter));
    }

    public ValueInput AddValueInput(string name, Type type, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (ValueInput) (inputPorts[id] = ValueInput.CreateInstance(type, this, name, id));
    }

    public ValueOutput AddValueOutputCommon(string name, Type type, object getter, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      ValueOutput instance = (ValueOutput) Activator.CreateInstance(typeof (ValueOutput<>).RTMakeGenericType([
        type
      ]), this, name, id, getter);
      outputPorts[id] = instance;
      return instance;
    }

    public ValueOutput AddValueOutput(
      string name,
      Type type,
      ValueHandler<object> getter,
      string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (ValueOutput) (outputPorts[id] = ValueOutput.CreateInstance(type, this, name, id, getter));
    }

    public ValueOutput AddPropertyOutput(PropertyInfo prop, object instance)
    {
      if (!prop.CanRead)
      {
        Debug.LogError("Property is write only");
        return null;
      }
      NameAttribute attribute = prop.RTGetAttribute<NameAttribute>(false);
      string key = attribute != null ? attribute.name : prop.Name.SplitCamelCase();
      Type type = typeof (ValueHandler<>).RTMakeGenericType([
        prop.PropertyType
      ]);
      Delegate @delegate = prop.RTGetGetMethod().RTCreateDelegate(type, instance);
      ValueOutput instance1 = (ValueOutput) Activator.CreateInstance(typeof (ValueOutput<>).RTMakeGenericType([
        prop.PropertyType
      ]), this, key, key, @delegate);
      return (ValueOutput) (outputPorts[key] = instance1);
    }

    public void Call(FlowOutput port) => port.Call();

    public void Fail(string error = null)
    {
      status = Status.Failure;
      if (error == null)
        return;
      Debug.LogError(string.Format("<b>Flow Execution Error:</b> '{0}' - '{1}'", name, error), graph.agent);
    }

    public void SetStatus(Status status) => this.status = status;
  }
}
