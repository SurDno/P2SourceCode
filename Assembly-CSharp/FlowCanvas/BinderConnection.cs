// Decompiled with JetBrains decompiler
// Type: FlowCanvas.BinderConnection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using NodeCanvas.Framework;
using ParadoxNotion;
using System;
using UnityEngine;

#nullable disable
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
      get => this.sourcePort != null ? this.sourcePort.id : this._sourcePortName;
      set => this._sourcePortName = value;
    }

    public string targetPortId
    {
      get => this.targetPort != null ? this.targetPort.id : this._targetPortName;
      private set => this._targetPortName = value;
    }

    public Port sourcePort
    {
      get
      {
        if (this._sourcePort == null && this.sourceNode is FlowNode)
          this._sourcePort = (this.sourceNode as FlowNode).GetOutputPort(this._sourcePortName);
        return this._sourcePort;
      }
    }

    public Port targetPort
    {
      get
      {
        if (this._targetPort == null && this.targetNode is FlowNode)
          this._targetPort = (this.targetNode as FlowNode).GetInputPort(this._targetPortName);
        return this._targetPort;
      }
    }

    public System.Type bindingType
    {
      get
      {
        return this.GetType().RTIsGenericType() ? this.GetType().RTGetGenericArguments()[0] : typeof (void);
      }
    }

    public void GatherAndValidateSourcePort()
    {
      this._sourcePort = (Port) null;
      if (this.sourcePort != null && TypeConverter.HasConvertion(this.sourcePort.type, this.bindingType))
        ++this.sourcePort.connections;
      else
        this.graph.RemoveConnection((Connection) this, false);
    }

    public void GatherAndValidateTargetPort()
    {
      this._targetPort = (Port) null;
      if (this.targetPort != null && this.targetPort.type == this.bindingType)
        ++this.targetPort.connections;
      else
        this.graph.RemoveConnection((Connection) this, false);
    }

    public static BinderConnection Create(Port source, Port target)
    {
      if (source == null || target == null)
      {
        Debug.LogError((object) "Source Port or Target Port is null when making a new Binder Connection");
        return (BinderConnection) null;
      }
      if (!source.CanAcceptConnections())
      {
        Debug.LogWarning((object) "Source port can accept no more connections");
        return (BinderConnection) null;
      }
      if (!target.CanAcceptConnections())
      {
        Debug.LogWarning((object) "Target port can accept no more connections");
        return (BinderConnection) null;
      }
      if (source.parent == target.parent)
      {
        Debug.LogWarning((object) "Can't connect ports on the same parent node");
        return (BinderConnection) null;
      }
      if (source is FlowOutput && !(target is FlowInput))
      {
        Debug.LogWarning((object) "Flow ports can only be connected to other Flow ports");
        return (BinderConnection) null;
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
        return (BinderConnection) null;
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
        return (BinderConnection) null;
      }
      if (!TypeConverter.HasConvertion(source.type, target.type))
      {
        Debug.LogWarning((object) string.Format("Can't connect ports. Type '{0}' is not assignable from Type '{1}' and there exists no internal convertion for those types.", (object) target.type.FriendlyName(), (object) source.type.FriendlyName()));
        return (BinderConnection) null;
      }
      if (source is FlowOutput && target is FlowInput)
      {
        BinderConnection binderConnection = new BinderConnection();
        binderConnection.OnCreate(source, target);
        return binderConnection;
      }
      if (!(source is ValueOutput) || !(target is ValueInput))
        return (BinderConnection) null;
      BinderConnection instance = (BinderConnection) Activator.CreateInstance(typeof (BinderConnection<>).RTMakeGenericType(new System.Type[1]
      {
        target.type
      }));
      instance.OnCreate(source, target);
      return instance;
    }

    public virtual void Bind()
    {
      if (!this.isActive || !(this.sourcePort is FlowOutput) || !(this.targetPort is FlowInput))
        return;
      (this.sourcePort as FlowOutput).BindTo((FlowInput) this.targetPort);
    }

    public virtual void UnBind()
    {
      if (!(this.sourcePort is FlowOutput))
        return;
      (this.sourcePort as FlowOutput).UnBind();
    }

    private void OnCreate(Port source, Port target)
    {
      this.sourceNode = (Node) source.parent;
      this.targetNode = (Node) target.parent;
      this.sourcePortId = source.id;
      this.targetPortId = target.id;
      this.sourceNode.outConnections.Add((Connection) this);
      this.targetNode.inConnections.Add((Connection) this);
      ++source.connections;
      ++target.connections;
      if (!Application.isPlaying)
        return;
      this.Bind();
    }

    public override void OnDestroy()
    {
      if (this.sourcePort != null)
        --this.sourcePort.connections;
      if (this.targetPort != null)
        --this.targetPort.connections;
      if (!Application.isPlaying)
        return;
      this.UnBind();
    }
  }
}
