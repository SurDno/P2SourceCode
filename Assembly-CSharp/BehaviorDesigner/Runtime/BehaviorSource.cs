using BehaviorDesigner.Runtime.Tasks;
using Engine.Common.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviorDesigner.Runtime
{
  [Serializable]
  public class BehaviorSource
  {
    private int behaviorId = -1;
    protected Task entryTask = (Task) null;
    protected Task rootTask = (Task) null;
    protected List<Task> detachedTasks = (List<Task>) null;
    private List<SharedVariable> variables = (List<SharedVariable>) null;
    private Dictionary<string, int> sharedVariableIndex;
    private bool hasSerialized = false;
    private IBehaviorTree owner;
    [SerializeField]
    [FormerlySerializedAs("mTaskData")]
    private TaskSerializationData taskData;

    public int BehaviorId
    {
      get => this.behaviorId;
      set => this.behaviorId = value;
    }

    public Task EntryTask
    {
      get => this.entryTask;
      set => this.entryTask = value;
    }

    public Task RootTask
    {
      get => this.rootTask;
      set => this.rootTask = value;
    }

    public List<Task> DetachedTasks
    {
      get => this.detachedTasks;
      set => this.detachedTasks = value;
    }

    public List<SharedVariable> Variables
    {
      get
      {
        this.CheckForSerialization(false, (BehaviorSource) null, this.Name);
        return this.variables;
      }
      set
      {
        this.variables = value;
        this.UpdateVariablesIndex();
      }
    }

    public bool HasSerialized
    {
      get => this.hasSerialized;
      set => this.hasSerialized = value;
    }

    public TaskSerializationData TaskData
    {
      get => this.taskData;
      set => this.taskData = value;
    }

    public IBehaviorTree Owner
    {
      get => this.owner;
      set => this.owner = value;
    }

    public BehaviorSource()
    {
    }

    public BehaviorSource(IBehaviorTree owner) => this.Initialize(owner);

    public void Initialize(IBehaviorTree owner) => this.owner = owner;

    public void Save(Task entryTask, Task rootTask, List<Task> detachedTasks)
    {
      this.entryTask = entryTask;
      this.rootTask = rootTask;
      this.detachedTasks = detachedTasks;
    }

    public void Load(out Task entryTask, out Task rootTask, out List<Task> detachedTasks)
    {
      entryTask = this.entryTask;
      rootTask = this.rootTask;
      detachedTasks = this.detachedTasks;
    }

    public bool CheckForSerialization(bool force, BehaviorSource behaviorSource, string context)
    {
      BehaviorSource behaviorSource1 = behaviorSource == null ? this : behaviorSource;
      if (!(!behaviorSource1.HasSerialized | force))
        return false;
      behaviorSource1.HasSerialized = true;
      if (this.taskData != null && !string.IsNullOrEmpty(this.taskData.XmlData))
      {
        XmlDeserialization.LoadXml(this.taskData, behaviorSource1, context);
        IOptimizationService optimizationService = ServiceCache.OptimizationService;
        if (optimizationService != null)
          optimizationService.FrameHasSpike = true;
      }
      return true;
    }

    public SharedVariable GetVariable(string name)
    {
      if (name == null)
        return (SharedVariable) null;
      this.CheckForSerialization(false, (BehaviorSource) null, this.Name);
      if (this.variables != null)
      {
        if (this.sharedVariableIndex == null || this.sharedVariableIndex.Count != this.variables.Count)
          this.UpdateVariablesIndex();
        int index;
        if (this.sharedVariableIndex.TryGetValue(name, out index))
          return this.variables[index];
      }
      return (SharedVariable) null;
    }

    public List<SharedVariable> GetAllVariables()
    {
      this.CheckForSerialization(false, (BehaviorSource) null, this.Name);
      return this.variables;
    }

    public void SetVariable(string name, SharedVariable sharedVariable)
    {
      if (this.variables == null)
        this.variables = new List<SharedVariable>();
      else if (this.sharedVariableIndex == null)
        this.UpdateVariablesIndex();
      sharedVariable.Name = name;
      int index;
      if (this.sharedVariableIndex != null && this.sharedVariableIndex.TryGetValue(name, out index))
      {
        SharedVariable variable = this.variables[index];
        if (!variable.GetType().Equals(typeof (SharedVariable)) && !variable.GetType().Equals(sharedVariable.GetType()))
          Debug.LogError((object) string.Format("Error: Unable to set SharedVariable {0} - the variable type {1} does not match the existing type {2}", (object) name, (object) variable.GetType(), (object) sharedVariable.GetType()));
        else
          variable.SetValue(sharedVariable.GetValue());
      }
      else
      {
        this.variables.Add(sharedVariable);
        this.UpdateVariablesIndex();
      }
    }

    public void UpdateVariableName(SharedVariable sharedVariable, string name)
    {
      this.CheckForSerialization(false, (BehaviorSource) null, this.Name);
      sharedVariable.Name = name;
      this.UpdateVariablesIndex();
    }

    public void SetAllVariables(List<SharedVariable> variables)
    {
      this.variables = variables;
      this.UpdateVariablesIndex();
    }

    private void UpdateVariablesIndex()
    {
      if (this.variables == null)
      {
        if (this.sharedVariableIndex == null)
          return;
        this.sharedVariableIndex = (Dictionary<string, int>) null;
      }
      else
      {
        if (this.sharedVariableIndex == null)
          this.sharedVariableIndex = new Dictionary<string, int>(this.variables.Count);
        else
          this.sharedVariableIndex.Clear();
        for (int index = 0; index < this.variables.Count; ++index)
        {
          if (this.variables[index] != null)
            this.sharedVariableIndex.Add(this.variables[index].Name, index);
        }
      }
    }

    public string Name => this.Owner != null ? this.Owner.GetOwnerName() : "Unknown";
  }
}
