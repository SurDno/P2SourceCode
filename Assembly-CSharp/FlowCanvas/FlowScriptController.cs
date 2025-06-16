// Decompiled with JetBrains decompiler
// Type: FlowCanvas.FlowScriptController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Settings.External;
using NodeCanvas.Framework;
using ParadoxNotion;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
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
    private List<UnityEngine.Object> boundGraphObjectReferences = new List<UnityEngine.Object>();
    [HideInInspector]
    public FlowScriptController.EnableAction enableAction = FlowScriptController.EnableAction.EnableBehaviour;
    [HideInInspector]
    public FlowScriptController.DisableAction disableAction = FlowScriptController.DisableAction.DisableBehaviour;
    private Graph _graph = new Graph();
    private bool startCalled = false;
    private static bool isQuiting;
    private Thread deserializeThread;
    private List<FlowScriptController.Command> commands = new List<FlowScriptController.Command>();

    public event Action<FlowScriptController> DestroyEvent;

    private void Awake()
    {
      if (!((UnityEngine.Object) this._blackboard == (UnityEngine.Object) null))
        return;
      this._blackboard = this.GetComponent<Blackboard>();
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
      Profiler.BeginSample("OnAfterDeserialize " + this._graph.agentName);
      if (EngineApplication.MainThread == Thread.CurrentThread && ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BlueprintsInSeparateThread)
      {
        this.deserializeThread = new Thread((ThreadStart) (() =>
        {
          this._graph.Deserialize(this.boundGraphSerialization, this.boundGraphObjectReferences);
          this._graph.agent = this;
        }));
        this.deserializeThread.Start();
      }
      else
      {
        this._graph.Deserialize(this.boundGraphSerialization, this.boundGraphObjectReferences);
        this._graph.agent = this;
      }
      Profiler.EndSample();
    }

    public void WaitForThreadFinish()
    {
      if (this.deserializeThread == null)
        return;
      this.deserializeThread.Join();
      this.deserializeThread = (Thread) null;
      this._blackboard.WaitForThreadFinish();
      this.ApplyCommands();
    }

    private void ApplyCommands()
    {
      foreach (FlowScriptController.Command command in this.commands)
      {
        if (command.Type == FlowScriptController.CommandEnum.StateChange)
        {
          switch (command.State)
          {
            case FlowScriptController.StateEnum.Start:
              this.StartBehaviour();
              break;
            case FlowScriptController.StateEnum.Stop:
              this.StopBehaviour();
              break;
            case FlowScriptController.StateEnum.Pause:
              this.PauseBehaviour();
              break;
          }
        }
        else if (command.Type == FlowScriptController.CommandEnum.Event)
          this.graph.SendEvent(command.EventData);
        else if (command.Type == FlowScriptController.CommandEnum.SetValue && !((UnityEngine.Object) this._blackboard == (UnityEngine.Object) null))
          this._blackboard.SetValue(command.ValueName, command.ValueObject);
      }
      this.commands.Clear();
    }

    private void Update()
    {
      if (this.deserializeThread == null || this.deserializeThread.IsAlive || !this._blackboard.IsReady)
        return;
      this.deserializeThread = (Thread) null;
      this.ApplyCommands();
    }

    public void SetValue(string name, object value)
    {
      if (!this._blackboard.IsReady)
        this.commands.Add(new FlowScriptController.Command()
        {
          Type = FlowScriptController.CommandEnum.SetValue,
          ValueName = name,
          ValueObject = value
        });
      else
        this._blackboard.SetValue(name, value);
    }

    public Graph graph
    {
      get
      {
        this.WaitForThreadFinish();
        return this._graph;
      }
    }

    public Blackboard blackboard
    {
      get
      {
        this._blackboard.WaitForThreadFinish();
        return this._blackboard;
      }
    }

    public bool isRunning => this.graph.isRunning;

    public bool isPaused => this.graph.isPaused;

    public void StartBehaviour()
    {
      if (this.deserializeThread != null && this.deserializeThread.IsAlive || !this._blackboard.IsReady)
        this.commands.Add(new FlowScriptController.Command()
        {
          Type = FlowScriptController.CommandEnum.StateChange,
          State = FlowScriptController.StateEnum.Start
        });
      else
        this.graph.StartGraph();
    }

    public void PauseBehaviour()
    {
      if (this.deserializeThread != null && this.deserializeThread.IsAlive || !this._blackboard.IsReady)
        this.commands.Add(new FlowScriptController.Command()
        {
          Type = FlowScriptController.CommandEnum.StateChange,
          State = FlowScriptController.StateEnum.Pause
        });
      else
        this.graph.Pause();
    }

    public void StopBehaviour()
    {
      if (this.deserializeThread != null && this.deserializeThread.IsAlive || !this._blackboard.IsReady)
        this.commands.Add(new FlowScriptController.Command()
        {
          Type = FlowScriptController.CommandEnum.StateChange,
          State = FlowScriptController.StateEnum.Stop
        });
      else
        this.graph.Stop();
    }

    public void SendEvent(string eventName) => this.SendEvent(new EventData(eventName));

    public void SendEvent(EventData eventData)
    {
      if (this.deserializeThread != null && this.deserializeThread.IsAlive || !this._blackboard.IsReady)
        this.commands.Add(new FlowScriptController.Command()
        {
          Type = FlowScriptController.CommandEnum.Event,
          EventData = eventData
        });
      else
        this.graph.SendEvent(eventData);
    }

    private void Start()
    {
      this.startCalled = true;
      if (this.enableAction != FlowScriptController.EnableAction.EnableBehaviour)
        return;
      this.StartBehaviour();
    }

    private void OnEnable()
    {
      if (!this.startCalled || this.enableAction != FlowScriptController.EnableAction.EnableBehaviour)
        return;
      this.StartBehaviour();
    }

    private void OnDisable()
    {
      if (FlowScriptController.isQuiting)
        return;
      if (this.disableAction == FlowScriptController.DisableAction.DisableBehaviour)
        this.StopBehaviour();
      if (this.disableAction != FlowScriptController.DisableAction.PauseBehaviour)
        return;
      this.PauseBehaviour();
    }

    private void OnDestroy()
    {
      if (FlowScriptController.isQuiting)
        return;
      this.WaitForThreadFinish();
      this.ApplyCommands();
      this.StopBehaviour();
      this.graph.OnDestroy();
      Action<FlowScriptController> destroyEvent = this.DestroyEvent;
      if (destroyEvent == null)
        return;
      destroyEvent(this);
    }

    private void OnApplicationQuit() => FlowScriptController.isQuiting = true;

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
      public FlowScriptController.CommandEnum Type;
      public FlowScriptController.StateEnum State;
      public EventData EventData;
      public string ValueName;
      public object ValueObject;
    }
  }
}
