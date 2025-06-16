// Decompiled with JetBrains decompiler
// Type: FlowCanvas.FlowNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Services;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace FlowCanvas
{
  public abstract class FlowNode : NodeCanvas.Framework.Node, ISerializationCallbackReceiver
  {
    [SerializeField]
    private Dictionary<string, object> _inputPortValues;
    private Dictionary<string, Port> inputPorts = new Dictionary<string, Port>((IEqualityComparer<string>) StringComparer.Ordinal);
    protected Dictionary<string, Port> outputPorts = new Dictionary<string, Port>((IEqualityComparer<string>) StringComparer.Ordinal);

    public override sealed int maxInConnections => -1;

    public override sealed int maxOutConnections => -1;

    public override sealed System.Type outConnectionType => typeof (BinderConnection);

    public override sealed bool showCommentsBottom => true;

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
      if (this._inputPortValues == null)
        this._inputPortValues = new Dictionary<string, object>();
      foreach (ValueInput valueInput in this.inputPorts.Values.OfType<ValueInput>())
      {
        if (!valueInput.isConnected)
          this._inputPortValues[valueInput.id] = valueInput.serializedValue;
      }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
    }

    public override sealed void OnValidate(Graph flowGraph) => this.GatherPorts();

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
      for (int index = 0; index < this.outConnections.Count; ++index)
        ((BinderConnection) this.outConnections[index]).Bind();
    }

    public void UnBindPorts()
    {
      for (int index = 0; index < this.outConnections.Count; ++index)
        ((BinderConnection) this.outConnections[index]).UnBind();
    }

    public Port GetInputPort(string id)
    {
      Port inputPort = (Port) null;
      this.inputPorts.TryGetValue(id, out inputPort);
      return inputPort;
    }

    public Port GetOutputPort(string id)
    {
      Port outputPort = (Port) null;
      this.outputPorts.TryGetValue(id, out outputPort);
      return outputPort;
    }

    public BinderConnection GetInputConnectionForPortId(string id)
    {
      return this.inConnections.OfType<BinderConnection>().FirstOrDefault<BinderConnection>((Func<BinderConnection, bool>) (c => c.targetPortId == id));
    }

    public BinderConnection GetOutputConnectionForPortId(string id)
    {
      return this.outConnections.OfType<BinderConnection>().FirstOrDefault<BinderConnection>((Func<BinderConnection, bool>) (c => c.sourcePortId == id));
    }

    public Port GetFirstInputOfType(System.Type type)
    {
      return this.inputPorts.Values.OrderBy<Port, int>((Func<Port, int>) (p => !(p.GetType() == typeof (FlowInput)) ? 1 : 0)).FirstOrDefault<Port>((Func<Port, bool>) (p => p.type.RTIsAssignableFrom(type)));
    }

    public Port GetFirstOutputOfType(System.Type type)
    {
      return this.outputPorts.Values.OrderBy<Port, int>((Func<Port, int>) (p => !(p.GetType() == typeof (FlowInput)) ? 1 : 0)).FirstOrDefault<Port>((Func<Port, bool>) (p => type.RTIsAssignableFrom(p.type)));
    }

    public void AssignSelfInstancePort()
    {
      if ((UnityEngine.Object) this.graphAgent == (UnityEngine.Object) null)
        return;
      ValueInput valueInput = this.inputPorts.Values.OfType<ValueInput>().FirstOrDefault<ValueInput>();
      if (valueInput == null || valueInput.isConnected || !valueInput.isDefaultValue)
        return;
      if (valueInput.type == typeof (GameObject))
        valueInput.serializedValue = (object) this.graphAgent.gameObject;
      if (typeof (Component).RTIsAssignableFrom(valueInput.type))
        valueInput.serializedValue = (object) this.graphAgent.GetComponent(valueInput.type);
    }

    public void GatherPorts()
    {
      this.inputPorts.Clear();
      this.outputPorts.Clear();
      this.RegisterPorts();
      this.DeserializeInputPortValues();
      this.ValidateConnections();
    }

    protected virtual void RegisterPorts() => this.DoReflectionBasedRegistration();

    private void DoReflectionBasedRegistration()
    {
      MetaService.GetContainer(this.GetType()).GetHandler(FromLocatorAttribute.Id).Compute((object) this, (object) null);
      MetaService.GetContainer(this.GetType()).GetHandler(PortAttribute.Id).Compute((object) this, (object) null);
    }

    private void ValidateConnections()
    {
      foreach (Connection connection in this.outConnections.ToArray())
      {
        if (connection is BinderConnection)
          (connection as BinderConnection).GatherAndValidateSourcePort();
      }
      foreach (Connection connection in this.inConnections.ToArray())
      {
        if (connection is BinderConnection)
          (connection as BinderConnection).GatherAndValidateTargetPort();
      }
    }

    private void DeserializeInputPortValues()
    {
      if (this._inputPortValues == null)
        return;
      foreach (KeyValuePair<string, object> inputPortValue in this._inputPortValues)
      {
        Port port = (Port) null;
        if (this.inputPorts.TryGetValue(inputPortValue.Key, out port) && port is ValueInput && inputPortValue.Value != null && port.type.RTIsAssignableFrom(inputPortValue.Value.GetType()))
          (port as ValueInput).serializedValue = inputPortValue.Value;
      }
    }

    public FlowInput AddFlowInput(string name, FlowHandler pointer, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (FlowInput) (this.inputPorts[id] = (Port) new FlowInput(this, name, id, pointer));
    }

    public FlowOutput AddFlowOutput(string name, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (FlowOutput) (this.outputPorts[id] = (Port) new FlowOutput(this, name, id));
    }

    public ValueInput<T> AddValueInput<T>(string name, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (ValueInput<T>) (this.inputPorts[id] = (Port) new ValueInput<T>(this, name, id));
    }

    public ValueOutput<T> AddValueOutput<T>(string name, ValueHandler<T> getter, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (ValueOutput<T>) (this.outputPorts[id] = (Port) new ValueOutput<T>(this, name, id, getter));
    }

    public ValueInput AddValueInput(string name, System.Type type, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (ValueInput) (this.inputPorts[id] = (Port) ValueInput.CreateInstance(type, this, name, id));
    }

    public ValueOutput AddValueOutputCommon(string name, System.Type type, object getter, string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      ValueOutput instance = (ValueOutput) Activator.CreateInstance(typeof (ValueOutput<>).RTMakeGenericType(new System.Type[1]
      {
        type
      }), (object) this, (object) name, (object) id, getter);
      this.outputPorts[id] = (Port) instance;
      return instance;
    }

    public ValueOutput AddValueOutput(
      string name,
      System.Type type,
      ValueHandler<object> getter,
      string id = "")
    {
      if (string.IsNullOrEmpty(id))
        id = name;
      return (ValueOutput) (this.outputPorts[id] = (Port) ValueOutput.CreateInstance(type, this, name, id, getter));
    }

    public ValueOutput AddPropertyOutput(PropertyInfo prop, object instance)
    {
      if (!prop.CanRead)
      {
        Debug.LogError((object) "Property is write only");
        return (ValueOutput) null;
      }
      NameAttribute attribute = prop.RTGetAttribute<NameAttribute>(false);
      string key = attribute != null ? attribute.name : prop.Name.SplitCamelCase();
      System.Type type = typeof (ValueHandler<>).RTMakeGenericType(new System.Type[1]
      {
        prop.PropertyType
      });
      Delegate @delegate = prop.RTGetGetMethod().RTCreateDelegate(type, instance);
      ValueOutput instance1 = (ValueOutput) Activator.CreateInstance(typeof (ValueOutput<>).RTMakeGenericType(new System.Type[1]
      {
        prop.PropertyType
      }), (object) this, (object) key, (object) key, (object) @delegate);
      return (ValueOutput) (this.outputPorts[key] = (Port) instance1);
    }

    public void Call(FlowOutput port) => port.Call();

    public void Fail(string error = null)
    {
      this.status = NodeCanvas.Status.Failure;
      if (error == null)
        return;
      Debug.LogError((object) string.Format("<b>Flow Execution Error:</b> '{0}' - '{1}'", (object) this.name, (object) error), (UnityEngine.Object) this.graph.agent);
    }

    public void SetStatus(NodeCanvas.Status status) => this.status = status;
  }
}
