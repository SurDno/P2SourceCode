using BehaviorDesigner.Runtime.Tasks;
using Cofe.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
  public class BehaviorTree : MonoBehaviour, IBehaviorTree
  {
    [SerializeField]
    private bool startWhenEnabled = true;
    [SerializeField]
    private bool pauseWhenDisabled = false;
    [SerializeField]
    private bool restartWhenComplete = false;
    [SerializeField]
    private bool resetValuesOnRestart = false;
    [SerializeField]
    private ExternalBehaviorTree externalBehavior;
    private BehaviorSource behaviorSource;
    private bool hasInheritedVariables = false;
    private bool isPaused = false;
    private TaskStatus executionStatus = TaskStatus.Inactive;
    private bool initialized = false;
    private Dictionary<Task, Dictionary<string, object>> defaultValues;
    private Dictionary<string, object> defaultVariableValues;
    private Dictionary<string, List<TaskCoroutine>> activeTaskCoroutines = (Dictionary<string, List<TaskCoroutine>>) null;
    private Dictionary<System.Type, Dictionary<string, Delegate>> eventTable;

    public event BehaviorTree.BehaviorHandler OnBehaviorStart;

    public event BehaviorTree.BehaviorHandler OnBehaviorRestart;

    public event BehaviorTree.BehaviorHandler OnBehaviorEnd;

    public bool StartWhenEnabled
    {
      get => this.startWhenEnabled;
      set => this.startWhenEnabled = value;
    }

    public bool PauseWhenDisabled
    {
      get => this.pauseWhenDisabled;
      set => this.pauseWhenDisabled = value;
    }

    public bool RestartWhenComplete
    {
      get => this.restartWhenComplete;
      set => this.restartWhenComplete = value;
    }

    public bool ResetValuesOnRestart
    {
      get => this.resetValuesOnRestart;
      set => this.resetValuesOnRestart = value;
    }

    public ExternalBehaviorTree ExternalBehaviorTree
    {
      get => this.externalBehavior;
      set
      {
        MonoBehaviourInstance<BehaviorTreeManager>.Instance.DisableBehavior(this);
        this.behaviorSource.HasSerialized = false;
        this.initialized = false;
        this.externalBehavior = value;
        if (!this.startWhenEnabled)
          return;
        this.EnableBehavior();
      }
    }

    public bool HasInheritedVariables
    {
      get => this.hasInheritedVariables;
      set => this.hasInheritedVariables = value;
    }

    public BehaviorSource BehaviorSource
    {
      get => this.behaviorSource;
      set => this.behaviorSource = value;
    }

    public UnityEngine.Object GetObject() => (UnityEngine.Object) this;

    public string GetOwnerName() => this.gameObject.name;

    public TaskStatus ExecutionStatus
    {
      get => this.executionStatus;
      set => this.executionStatus = value;
    }

    public BehaviorTree() => this.behaviorSource = new BehaviorSource((IBehaviorTree) this);

    public void Start()
    {
      if (!this.startWhenEnabled)
        return;
      this.EnableBehavior();
    }

    public void EnableBehavior()
    {
      MonoBehaviourInstance<BehaviorTreeManager>.Instance.EnableBehavior(this);
      if (this.initialized)
        return;
      this.initialized = true;
    }

    public void DisableBehavior()
    {
      MonoBehaviourInstance<BehaviorTreeManager>.Instance.DisableBehavior(this, this.pauseWhenDisabled);
      this.isPaused = this.pauseWhenDisabled;
    }

    public void DisableBehavior(bool pause)
    {
      MonoBehaviourInstance<BehaviorTreeManager>.Instance.DisableBehavior(this, pause);
      this.isPaused = pause;
    }

    public void OnEnable()
    {
      if (this.isPaused)
      {
        MonoBehaviourInstance<BehaviorTreeManager>.Instance.EnableBehavior(this);
        this.isPaused = false;
      }
      else
      {
        if (!this.startWhenEnabled || !this.initialized)
          return;
        this.EnableBehavior();
      }
    }

    public void OnDisable() => this.DisableBehavior();

    public void OnDestroy()
    {
      MonoBehaviourInstance<BehaviorTreeManager>.Instance.DestroyBehavior(this);
    }

    public SharedVariable GetVariable(string name)
    {
      this.CheckForSerialization();
      return this.behaviorSource.GetVariable(name);
    }

    public void SetVariable(string name, SharedVariable item)
    {
      this.CheckForSerialization();
      this.behaviorSource.SetVariable(name, item);
    }

    public void SetVariableValue(string name, object value)
    {
      SharedVariable variable = this.GetVariable(name);
      if (variable != null)
      {
        if (value is SharedVariable)
        {
          SharedVariable sharedVariable = value as SharedVariable;
          if (!TypeUtility.IsAssignableFrom(variable.GetType(), sharedVariable.GetType()))
            Debug.LogError((object) ("Set wrong type, go : " + (object) this.gameObject));
          variable.SetValue(sharedVariable.GetValue());
        }
        else
          variable.SetValue(value);
      }
      else if (value is SharedVariable)
      {
        SharedVariable sharedVariable = value as SharedVariable;
        SharedVariable instance = TaskUtility.CreateInstance(sharedVariable.GetType()) as SharedVariable;
        instance.Name = sharedVariable.Name;
        instance.IsShared = sharedVariable.IsShared;
        instance.SetValue(sharedVariable.GetValue());
        this.behaviorSource.SetVariable(name, instance);
      }
      else
        Debug.LogError((object) ("Error: No variable exists with name " + name + " in: " + (object) this.gameObject));
    }

    public List<SharedVariable> GetAllVariables()
    {
      this.CheckForSerialization();
      return this.behaviorSource.GetAllVariables();
    }

    public void CheckForSerialization()
    {
      if ((UnityEngine.Object) this.externalBehavior != (UnityEngine.Object) null)
      {
        List<SharedVariable> sharedVariableList = (List<SharedVariable>) null;
        bool force = false;
        if (!this.hasInheritedVariables)
        {
          this.behaviorSource.CheckForSerialization(false, (BehaviorSource) null, this.GetOwnerName());
          sharedVariableList = this.behaviorSource.GetAllVariables();
          this.hasInheritedVariables = true;
          force = true;
        }
        this.externalBehavior.BehaviorSource.Owner = (IBehaviorTree) this.ExternalBehaviorTree;
        this.externalBehavior.BehaviorSource.CheckForSerialization(force, this.BehaviorSource, this.GetOwnerName());
        this.externalBehavior.BehaviorSource.EntryTask = this.behaviorSource.EntryTask;
        if (sharedVariableList == null)
          return;
        for (int index = 0; index < sharedVariableList.Count; ++index)
        {
          if (sharedVariableList[index] != null)
            this.behaviorSource.SetVariable(sharedVariableList[index].Name, sharedVariableList[index]);
        }
      }
      else
        this.behaviorSource.CheckForSerialization(false, (BehaviorSource) null, this.GetOwnerName());
    }

    private T FindTask<T>(Task task) where T : Task
    {
      if (task.GetType().Equals(typeof (T)))
        return (T) task;
      if (task is ParentTask parentTask && parentTask.Children != null)
      {
        for (int index = 0; index < parentTask.Children.Count; ++index)
        {
          T obj = default (T);
          T task1;
          if ((object) (task1 = this.FindTask<T>(parentTask.Children[index])) != null)
            return task1;
        }
      }
      return default (T);
    }

    private void FindTasks<T>(Task task, ref List<T> taskList) where T : Task
    {
      if (typeof (T).IsAssignableFrom(task.GetType()))
        taskList.Add((T) task);
      if (!(task is ParentTask parentTask) || parentTask.Children == null)
        return;
      for (int index = 0; index < parentTask.Children.Count; ++index)
        this.FindTasks<T>(parentTask.Children[index], ref taskList);
    }

    private Task FindTaskWithName(string taskName, Task task)
    {
      if (task.FriendlyName.Equals(taskName))
        return task;
      if (task is ParentTask parentTask && parentTask.Children != null)
      {
        for (int index = 0; index < parentTask.Children.Count; ++index)
        {
          Task taskWithName;
          if ((taskWithName = this.FindTaskWithName(taskName, parentTask.Children[index])) != null)
            return taskWithName;
        }
      }
      return (Task) null;
    }

    private void FindTasksWithName(string taskName, Task task, ref List<Task> taskList)
    {
      if (task.FriendlyName.Equals(taskName))
        taskList.Add(task);
      if (!(task is ParentTask parentTask) || parentTask.Children == null)
        return;
      for (int index = 0; index < parentTask.Children.Count; ++index)
        this.FindTasksWithName(taskName, parentTask.Children[index], ref taskList);
    }

    public Coroutine StartTaskCoroutine(Task task, string methodName)
    {
      MethodInfo method = task.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (method == (MethodInfo) null)
      {
        Debug.LogError((object) ("Unable to start coroutine " + methodName + ": method not found"));
        return (Coroutine) null;
      }
      if (this.activeTaskCoroutines == null)
        this.activeTaskCoroutines = new Dictionary<string, List<TaskCoroutine>>();
      TaskCoroutine taskCoroutine = new TaskCoroutine(this, (IEnumerator) method.Invoke((object) task, new object[0]), methodName);
      if (this.activeTaskCoroutines.ContainsKey(methodName))
      {
        List<TaskCoroutine> activeTaskCoroutine = this.activeTaskCoroutines[methodName];
        activeTaskCoroutine.Add(taskCoroutine);
        this.activeTaskCoroutines[methodName] = activeTaskCoroutine;
      }
      else
        this.activeTaskCoroutines.Add(methodName, new List<TaskCoroutine>()
        {
          taskCoroutine
        });
      return taskCoroutine.Coroutine;
    }

    public Coroutine StartTaskCoroutine(Task task, string methodName, object value)
    {
      MethodInfo method = task.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (method == (MethodInfo) null)
      {
        Debug.LogError((object) ("Unable to start coroutine " + methodName + ": method not found"));
        return (Coroutine) null;
      }
      if (this.activeTaskCoroutines == null)
        this.activeTaskCoroutines = new Dictionary<string, List<TaskCoroutine>>();
      TaskCoroutine taskCoroutine = new TaskCoroutine(this, (IEnumerator) method.Invoke((object) task, new object[1]
      {
        value
      }), methodName);
      if (this.activeTaskCoroutines.ContainsKey(methodName))
      {
        List<TaskCoroutine> activeTaskCoroutine = this.activeTaskCoroutines[methodName];
        activeTaskCoroutine.Add(taskCoroutine);
        this.activeTaskCoroutines[methodName] = activeTaskCoroutine;
      }
      else
        this.activeTaskCoroutines.Add(methodName, new List<TaskCoroutine>()
        {
          taskCoroutine
        });
      return taskCoroutine.Coroutine;
    }

    public void StopTaskCoroutine(string methodName)
    {
      if (!this.activeTaskCoroutines.ContainsKey(methodName))
        return;
      List<TaskCoroutine> activeTaskCoroutine = this.activeTaskCoroutines[methodName];
      for (int index = 0; index < activeTaskCoroutine.Count; ++index)
        activeTaskCoroutine[index].Stop();
    }

    public void StopAllTaskCoroutines()
    {
      this.StopAllCoroutines();
      foreach (KeyValuePair<string, List<TaskCoroutine>> activeTaskCoroutine in this.activeTaskCoroutines)
      {
        List<TaskCoroutine> taskCoroutineList = activeTaskCoroutine.Value;
        for (int index = 0; index < taskCoroutineList.Count; ++index)
          taskCoroutineList[index].Stop();
      }
    }

    public void TaskCoroutineEnded(TaskCoroutine taskCoroutine, string coroutineName)
    {
      if (!this.activeTaskCoroutines.ContainsKey(coroutineName))
        return;
      List<TaskCoroutine> activeTaskCoroutine = this.activeTaskCoroutines[coroutineName];
      if (activeTaskCoroutine.Count == 1)
      {
        this.activeTaskCoroutines.Remove(coroutineName);
      }
      else
      {
        activeTaskCoroutine.Remove(taskCoroutine);
        this.activeTaskCoroutines[coroutineName] = activeTaskCoroutine;
      }
    }

    public void OnBehaviorStarted()
    {
      BehaviorTree.BehaviorHandler onBehaviorStart = this.OnBehaviorStart;
      if (onBehaviorStart == null)
        return;
      onBehaviorStart(this);
    }

    public void OnBehaviorRestarted()
    {
      BehaviorTree.BehaviorHandler onBehaviorRestart = this.OnBehaviorRestart;
      if (onBehaviorRestart == null)
        return;
      onBehaviorRestart(this);
    }

    public void OnBehaviorEnded()
    {
      BehaviorTree.BehaviorHandler onBehaviorEnd = this.OnBehaviorEnd;
      if (onBehaviorEnd == null)
        return;
      onBehaviorEnd(this);
    }

    private void RegisterEvent(string name, Delegate handler)
    {
      if (this.eventTable == null)
        this.eventTable = new Dictionary<System.Type, Dictionary<string, Delegate>>();
      Dictionary<string, Delegate> dictionary;
      if (!this.eventTable.TryGetValue(handler.GetType(), out dictionary))
      {
        dictionary = new Dictionary<string, Delegate>();
        this.eventTable.Add(handler.GetType(), dictionary);
      }
      Delegate a;
      if (dictionary.TryGetValue(name, out a))
        dictionary[name] = Delegate.Combine(a, handler);
      else
        dictionary.Add(name, handler);
    }

    public void RegisterEvent(string name, System.Action handler)
    {
      this.RegisterEvent(name, (Delegate) handler);
    }

    public void RegisterEvent<T>(string name, Action<T> handler)
    {
      this.RegisterEvent(name, (Delegate) handler);
    }

    public void RegisterEvent<T, U>(string name, Action<T, U> handler)
    {
      this.RegisterEvent(name, (Delegate) handler);
    }

    public void RegisterEvent<T, U, V>(string name, Action<T, U, V> handler)
    {
      this.RegisterEvent(name, (Delegate) handler);
    }

    private Delegate GetDelegate(string name, System.Type type)
    {
      Dictionary<string, Delegate> dictionary;
      Delegate @delegate;
      return this.eventTable != null && this.eventTable.TryGetValue(type, out dictionary) && dictionary.TryGetValue(name, out @delegate) ? @delegate : (Delegate) null;
    }

    public void SendEvent(string name)
    {
      if (!(this.GetDelegate(name, typeof (System.Action)) is System.Action action))
        return;
      action();
    }

    public void SendEvent<T>(string name, T arg1)
    {
      if (!(this.GetDelegate(name, typeof (Action<T>)) is Action<T> action))
        return;
      action(arg1);
    }

    public void SendEvent<T, U>(string name, T arg1, U arg2)
    {
      if (!(this.GetDelegate(name, typeof (Action<T, U>)) is Action<T, U> action))
        return;
      action(arg1, arg2);
    }

    public void SendEvent<T, U, V>(string name, T arg1, U arg2, V arg3)
    {
      if (!(this.GetDelegate(name, typeof (Action<T, U, V>)) is Action<T, U, V> action))
        return;
      action(arg1, arg2, arg3);
    }

    private void UnregisterEvent(string name, Delegate handler)
    {
      Dictionary<string, Delegate> dictionary;
      Delegate source;
      if (this.eventTable == null || !this.eventTable.TryGetValue(handler.GetType(), out dictionary) || !dictionary.TryGetValue(name, out source))
        return;
      dictionary[name] = Delegate.Remove(source, handler);
    }

    public void UnregisterEvent(string name, System.Action handler)
    {
      this.UnregisterEvent(name, (Delegate) handler);
    }

    public void UnregisterEvent<T>(string name, Action<T> handler)
    {
      this.UnregisterEvent(name, (Delegate) handler);
    }

    public void UnregisterEvent<T, U>(string name, Action<T, U> handler)
    {
      this.UnregisterEvent(name, (Delegate) handler);
    }

    public void UnregisterEvent<T, U, V>(string name, Action<T, U, V> handler)
    {
      this.UnregisterEvent(name, (Delegate) handler);
    }

    public void SaveResetValues()
    {
      if (this.defaultValues == null)
      {
        this.CheckForSerialization();
        this.defaultValues = new Dictionary<Task, Dictionary<string, object>>();
        this.defaultVariableValues = new Dictionary<string, object>();
        this.SaveValues();
      }
      else
        this.ResetValues();
    }

    private void SaveValues()
    {
      List<SharedVariable> allVariables = this.behaviorSource.GetAllVariables();
      if (allVariables != null)
      {
        for (int index = 0; index < allVariables.Count; ++index)
          this.defaultVariableValues.Add(allVariables[index].Name, allVariables[index].GetValue());
      }
      this.SaveValue(this.behaviorSource.RootTask);
    }

    private void SaveValue(Task task)
    {
      if (task == null)
        return;
      FieldInfo[] publicFields = TaskUtility.GetPublicFields(task.GetType());
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      for (int index = 0; index < publicFields.Length; ++index)
      {
        object obj = publicFields[index].GetValue((object) task);
        if (!(obj is SharedVariable) || !(obj as SharedVariable).IsShared)
          dictionary.Add(publicFields[index].Name, publicFields[index].GetValue((object) task));
      }
      this.defaultValues.Add(task, dictionary);
      if (!(task is ParentTask))
        return;
      ParentTask parentTask = task as ParentTask;
      if (parentTask.Children != null)
      {
        for (int index = 0; index < parentTask.Children.Count; ++index)
          this.SaveValue(parentTask.Children[index]);
      }
    }

    private void ResetValues()
    {
      foreach (KeyValuePair<string, object> defaultVariableValue in this.defaultVariableValues)
        this.SetVariableValue(defaultVariableValue.Key, defaultVariableValue.Value);
      this.ResetValue(this.behaviorSource.RootTask);
    }

    private void ResetValue(Task task)
    {
      Dictionary<string, object> dictionary;
      if (task == null || !this.defaultValues.TryGetValue(task, out dictionary))
        return;
      foreach (KeyValuePair<string, object> keyValuePair in dictionary)
      {
        FieldInfo field = task.GetType().GetField(keyValuePair.Key);
        if (field != (FieldInfo) null)
          field.SetValue((object) task, keyValuePair.Value);
      }
      if (!(task is ParentTask))
        return;
      ParentTask parentTask = task as ParentTask;
      if (parentTask.Children != null)
      {
        for (int index = 0; index < parentTask.Children.Count; ++index)
          this.ResetValue(parentTask.Children[index]);
      }
    }

    public override string ToString() => this.behaviorSource.ToString();

    int IBehaviorTree.GetInstanceID() => this.GetInstanceID();

    public delegate void BehaviorHandler(BehaviorTree behavior);
  }
}
