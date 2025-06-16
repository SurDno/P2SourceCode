using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Engine.Common.Services;

namespace BehaviorDesigner.Runtime
{
  [Serializable]
  public class BehaviorSource
  {
    private int behaviorId = -1;
    protected Task entryTask;
    protected Task rootTask;
    protected List<Task> detachedTasks;
    private List<SharedVariable> variables;
    private Dictionary<string, int> sharedVariableIndex;
    private bool hasSerialized;
    private IBehaviorTree owner;
    [SerializeField]
    [FormerlySerializedAs("mTaskData")]
    private TaskSerializationData taskData;

    public int BehaviorId
    {
      get => behaviorId;
      set => behaviorId = value;
    }

    public Task EntryTask
    {
      get => entryTask;
      set => entryTask = value;
    }

    public Task RootTask
    {
      get => rootTask;
      set => rootTask = value;
    }

    public List<Task> DetachedTasks
    {
      get => detachedTasks;
      set => detachedTasks = value;
    }

    public List<SharedVariable> Variables
    {
      get
      {
        CheckForSerialization(false, null, Name);
        return variables;
      }
      set
      {
        variables = value;
        UpdateVariablesIndex();
      }
    }

    public bool HasSerialized
    {
      get => hasSerialized;
      set => hasSerialized = value;
    }

    public TaskSerializationData TaskData
    {
      get => taskData;
      set => taskData = value;
    }

    public IBehaviorTree Owner
    {
      get => owner;
      set => owner = value;
    }

    public BehaviorSource()
    {
    }

    public BehaviorSource(IBehaviorTree owner) => Initialize(owner);

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
      if (taskData != null && !string.IsNullOrEmpty(taskData.XmlData))
      {
        XmlDeserialization.LoadXml(taskData, behaviorSource1, context);
        IOptimizationService optimizationService = ServiceCache.OptimizationService;
        if (optimizationService != null)
          optimizationService.FrameHasSpike = true;
      }
      return true;
    }

    public SharedVariable GetVariable(string name)
    {
      if (name == null)
        return null;
      CheckForSerialization(false, null, Name);
      if (variables != null)
      {
        if (sharedVariableIndex == null || sharedVariableIndex.Count != variables.Count)
          UpdateVariablesIndex();
        int index;
        if (sharedVariableIndex.TryGetValue(name, out index))
          return variables[index];
      }
      return null;
    }

    public List<SharedVariable> GetAllVariables()
    {
      CheckForSerialization(false, null, Name);
      return variables;
    }

    public void SetVariable(string name, SharedVariable sharedVariable)
    {
      if (variables == null)
        variables = new List<SharedVariable>();
      else if (sharedVariableIndex == null)
        UpdateVariablesIndex();
      sharedVariable.Name = name;
      int index;
      if (sharedVariableIndex != null && sharedVariableIndex.TryGetValue(name, out index))
      {
        SharedVariable variable = variables[index];
        if (!variable.GetType().Equals(typeof (SharedVariable)) && !variable.GetType().Equals(sharedVariable.GetType()))
          Debug.LogError((object) string.Format("Error: Unable to set SharedVariable {0} - the variable type {1} does not match the existing type {2}", name, variable.GetType(), sharedVariable.GetType()));
        else
          variable.SetValue(sharedVariable.GetValue());
      }
      else
      {
        variables.Add(sharedVariable);
        UpdateVariablesIndex();
      }
    }

    public void UpdateVariableName(SharedVariable sharedVariable, string name)
    {
      CheckForSerialization(false, null, Name);
      sharedVariable.Name = name;
      UpdateVariablesIndex();
    }

    public void SetAllVariables(List<SharedVariable> variables)
    {
      this.variables = variables;
      UpdateVariablesIndex();
    }

    private void UpdateVariablesIndex()
    {
      if (variables == null)
      {
        if (sharedVariableIndex == null)
          return;
        sharedVariableIndex = null;
      }
      else
      {
        if (sharedVariableIndex == null)
          sharedVariableIndex = new Dictionary<string, int>(variables.Count);
        else
          sharedVariableIndex.Clear();
        for (int index = 0; index < variables.Count; ++index)
        {
          if (variables[index] != null)
            sharedVariableIndex.Add(variables[index].Name, index);
        }
      }
    }

    public string Name => Owner != null ? Owner.GetOwnerName() : "Unknown";
  }
}
