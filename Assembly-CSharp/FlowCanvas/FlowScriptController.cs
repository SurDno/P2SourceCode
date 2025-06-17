using System;
using System.Collections.Generic;
using System.Threading;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using NodeCanvas.Framework;
using ParadoxNotion;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace FlowCanvas
{
  [RequireComponent(typeof (Blackboard))]
  public class FlowScriptController : MonoBehaviour, ISerializationCallbackReceiver
  {
    [SerializeField]
    private Blackboard _blackboard;
    [SerializeField]
    private string boundGraphSerialization = "";
    [SerializeField]
    private List<Object> boundGraphObjectReferences = [];
    [HideInInspector]
    public EnableAction enableAction = EnableAction.EnableBehaviour;
    [HideInInspector]
    public DisableAction disableAction = DisableAction.DisableBehaviour;
    private Graph _graph = new();
    private bool startCalled;
    private static bool isQuiting;
    private Thread deserializeThread;
    private List<Command> commands = [];

    public event Action<FlowScriptController> DestroyEvent;

    private void Awake()
    {
      if (!(_blackboard == null))
        return;
      _blackboard = GetComponent<Blackboard>();
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
      Profiler.BeginSample("OnAfterDeserialize " + _graph.agentName);
      if (EngineApplication.MainThread == Thread.CurrentThread && ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BlueprintsInSeparateThread)
      {
        deserializeThread = new Thread(() =>
        {
          _graph.Deserialize(boundGraphSerialization, boundGraphObjectReferences);
          _graph.agent = this;
        });
        deserializeThread.Start();
      }
      else
      {
        _graph.Deserialize(boundGraphSerialization, boundGraphObjectReferences);
        _graph.agent = this;
      }
      Profiler.EndSample();
    }

    public void WaitForThreadFinish()
    {
      if (deserializeThread == null)
        return;
      deserializeThread.Join();
      deserializeThread = null;
      _blackboard.WaitForThreadFinish();
      ApplyCommands();
    }

    private void ApplyCommands()
    {
      foreach (Command command in commands)
      {
        if (command.Type == CommandEnum.StateChange)
        {
          switch (command.State)
          {
            case StateEnum.Start:
              StartBehaviour();
              break;
            case StateEnum.Stop:
              StopBehaviour();
              break;
            case StateEnum.Pause:
              PauseBehaviour();
              break;
          }
        }
        else if (command.Type == CommandEnum.Event)
          graph.SendEvent(command.EventData);
        else if (command.Type == CommandEnum.SetValue && !(_blackboard == null))
          _blackboard.SetValue(command.ValueName, command.ValueObject);
      }
      commands.Clear();
    }

    private void Update()
    {
      if (deserializeThread == null || deserializeThread.IsAlive || !_blackboard.IsReady)
        return;
      deserializeThread = null;
      ApplyCommands();
    }

    public void SetValue(string name, object value)
    {
      if (!_blackboard.IsReady)
        commands.Add(new Command {
          Type = CommandEnum.SetValue,
          ValueName = name,
          ValueObject = value
        });
      else
        _blackboard.SetValue(name, value);
    }

    public Graph graph
    {
      get
      {
        WaitForThreadFinish();
        return _graph;
      }
    }

    public Blackboard blackboard
    {
      get
      {
        _blackboard.WaitForThreadFinish();
        return _blackboard;
      }
    }

    public bool isRunning => graph.isRunning;

    public bool isPaused => graph.isPaused;

    public void StartBehaviour()
    {
      if (deserializeThread != null && deserializeThread.IsAlive || !_blackboard.IsReady)
        commands.Add(new Command {
          Type = CommandEnum.StateChange,
          State = StateEnum.Start
        });
      else
        graph.StartGraph();
    }

    public void PauseBehaviour()
    {
      if (deserializeThread != null && deserializeThread.IsAlive || !_blackboard.IsReady)
        commands.Add(new Command {
          Type = CommandEnum.StateChange,
          State = StateEnum.Pause
        });
      else
        graph.Pause();
    }

    public void StopBehaviour()
    {
      if (deserializeThread != null && deserializeThread.IsAlive || !_blackboard.IsReady)
        commands.Add(new Command {
          Type = CommandEnum.StateChange,
          State = StateEnum.Stop
        });
      else
        graph.Stop();
    }

    public void SendEvent(string eventName) => SendEvent(new EventData(eventName));

    public void SendEvent(EventData eventData)
    {
      if (deserializeThread != null && deserializeThread.IsAlive || !_blackboard.IsReady)
        commands.Add(new Command {
          Type = CommandEnum.Event,
          EventData = eventData
        });
      else
        graph.SendEvent(eventData);
    }

    private void Start()
    {
      startCalled = true;
      if (enableAction != EnableAction.EnableBehaviour)
        return;
      StartBehaviour();
    }

    private void OnEnable()
    {
      if (!startCalled || enableAction != EnableAction.EnableBehaviour)
        return;
      StartBehaviour();
    }

    private void OnDisable()
    {
      if (isQuiting)
        return;
      if (disableAction == DisableAction.DisableBehaviour)
        StopBehaviour();
      if (disableAction != DisableAction.PauseBehaviour)
        return;
      PauseBehaviour();
    }

    private void OnDestroy()
    {
      if (isQuiting)
        return;
      WaitForThreadFinish();
      ApplyCommands();
      StopBehaviour();
      graph.OnDestroy();
      Action<FlowScriptController> destroyEvent = DestroyEvent;
      if (destroyEvent == null)
        return;
      destroyEvent(this);
    }

    private void OnApplicationQuit() => isQuiting = true;

    public enum EnableAction
    {
      EnableBehaviour,
      DoNothing,
    }

    public enum DisableAction
    {
      DisableBehaviour,
      PauseBehaviour,
      DoNothing,
    }

    private enum CommandEnum
    {
      StateChange,
      Event,
      SetValue,
    }

    private enum StateEnum
    {
      DoNothing,
      Start,
      Stop,
      Pause,
    }

    private struct Command
    {
      public CommandEnum Type;
      public StateEnum State;
      public EventData EventData;
      public string ValueName;
      public object ValueObject;
    }
  }
}
