using System;
using System.Collections.Generic;
using System.Threading;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Serialization;

namespace NodeCanvas.Framework
{
  public class Blackboard : MonoBehaviour, ISerializationCallbackReceiver
  {
    [SerializeField]
    private string _serializedBlackboard = null;
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
      if (deserializeThread == null)
        return;
      deserializeThread.Join();
      deserializeThread = null;
    }

    public bool IsReady => deserializeThread == null || !deserializeThread.IsAlive;

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
      if (EngineApplication.MainThread == Thread.CurrentThread && ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BlueprintsInSeparateThread)
      {
        deserializeThread = new Thread(() =>
        {
          _blackboard = JSONSerializer.Deserialize<BlackboardData>(_serializedBlackboard, _objectReferences);
          if (_blackboard != null)
            return;
          Debug.LogError((object) "!!!");
          _blackboard = new BlackboardData();
        });
        deserializeThread.Start();
      }
      else
      {
        _blackboard = JSONSerializer.Deserialize<BlackboardData>(_serializedBlackboard, _objectReferences);
        if (_blackboard == null)
        {
          Debug.LogError((object) "!!!");
          _blackboard = new BlackboardData();
        }
      }
    }

    public Dictionary<string, Variable> variables
    {
      get => _blackboard.variables;
      set => _blackboard.variables = value;
    }

    public Variable AddVariable(string name, Type type)
    {
      Variable variable = _blackboard.AddVariable(name, type);
      Action<Variable> onVariableAdded = this.onVariableAdded;
      if (onVariableAdded != null)
        onVariableAdded(variable);
      return variable;
    }

    public Variable AddVariable(string name, object value)
    {
      Variable variable = _blackboard.AddVariable(name, value);
      Action<Variable> onVariableAdded = this.onVariableAdded;
      if (onVariableAdded != null)
        onVariableAdded(variable);
      return variable;
    }

    public Variable RemoveVariable(string name)
    {
      Variable variable = _blackboard.RemoveVariable(name);
      Action<Variable> onVariableRemoved = this.onVariableRemoved;
      if (onVariableRemoved != null)
        onVariableRemoved(variable);
      return variable;
    }

    public Variable GetVariable(string name, Type ofType = null)
    {
      return _blackboard.GetVariable(name, ofType);
    }

    public Variable GetVariableByID(string ID) => _blackboard.GetVariableByID(ID);

    public Variable<T> GetVariable<T>(string name) => _blackboard.GetVariable<T>(name);

    public T GetValue<T>(string name) => _blackboard.GetValue<T>(name);

    public Variable SetValue(string name, object value) => _blackboard.SetValue(name, value);

    public string[] GetVariableNames(Type ofType)
    {
      return _blackboard.GetVariableNames(ofType);
    }
  }
}
