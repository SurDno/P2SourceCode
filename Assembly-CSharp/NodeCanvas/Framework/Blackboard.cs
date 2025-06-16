// Decompiled with JetBrains decompiler
// Type: NodeCanvas.Framework.Blackboard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Settings.External;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Serialization;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

#nullable disable
namespace NodeCanvas.Framework
{
  public class Blackboard : MonoBehaviour, ISerializationCallbackReceiver
  {
    [SerializeField]
    private string _serializedBlackboard = (string) null;
    [SerializeField]
    private List<UnityEngine.Object> _objectReferences = new List<UnityEngine.Object>();
    private BlackboardData _blackboard = new BlackboardData();
    private Thread deserializeThread;

    public event Action<Variable> onVariableAdded;

    public event Action<Variable> onVariableRemoved;

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    public void WaitForThreadFinish()
    {
      if (this.deserializeThread == null)
        return;
      this.deserializeThread.Join();
      this.deserializeThread = (Thread) null;
    }

    public bool IsReady => this.deserializeThread == null || !this.deserializeThread.IsAlive;

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
      if (EngineApplication.MainThread == Thread.CurrentThread && ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BlueprintsInSeparateThread)
      {
        this.deserializeThread = new Thread((ThreadStart) (() =>
        {
          this._blackboard = JSONSerializer.Deserialize<BlackboardData>(this._serializedBlackboard, this._objectReferences);
          if (this._blackboard != null)
            return;
          Debug.LogError((object) "!!!");
          this._blackboard = new BlackboardData();
        }));
        this.deserializeThread.Start();
      }
      else
      {
        this._blackboard = JSONSerializer.Deserialize<BlackboardData>(this._serializedBlackboard, this._objectReferences);
        if (this._blackboard == null)
        {
          Debug.LogError((object) "!!!");
          this._blackboard = new BlackboardData();
        }
      }
    }

    public Dictionary<string, Variable> variables
    {
      get => this._blackboard.variables;
      set => this._blackboard.variables = value;
    }

    public Variable AddVariable(string name, System.Type type)
    {
      Variable variable = this._blackboard.AddVariable(name, type);
      Action<Variable> onVariableAdded = this.onVariableAdded;
      if (onVariableAdded != null)
        onVariableAdded(variable);
      return variable;
    }

    public Variable AddVariable(string name, object value)
    {
      Variable variable = this._blackboard.AddVariable(name, value);
      Action<Variable> onVariableAdded = this.onVariableAdded;
      if (onVariableAdded != null)
        onVariableAdded(variable);
      return variable;
    }

    public Variable RemoveVariable(string name)
    {
      Variable variable = this._blackboard.RemoveVariable(name);
      Action<Variable> onVariableRemoved = this.onVariableRemoved;
      if (onVariableRemoved != null)
        onVariableRemoved(variable);
      return variable;
    }

    public Variable GetVariable(string name, System.Type ofType = null)
    {
      return this._blackboard.GetVariable(name, ofType);
    }

    public Variable GetVariableByID(string ID) => this._blackboard.GetVariableByID(ID);

    public Variable<T> GetVariable<T>(string name) => this._blackboard.GetVariable<T>(name);

    public T GetValue<T>(string name) => this._blackboard.GetValue<T>(name);

    public Variable SetValue(string name, object value) => this._blackboard.SetValue(name, value);

    public string[] GetVariableNames(System.Type ofType)
    {
      return this._blackboard.GetVariableNames(ofType);
    }
  }
}
