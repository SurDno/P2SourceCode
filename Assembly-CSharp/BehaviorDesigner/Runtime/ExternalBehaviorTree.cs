using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviorDesigner.Runtime
{
  public class ExternalBehaviorTree : ScriptableObject, IBehaviorTree
  {
    [SerializeField]
    [FormerlySerializedAs("mBehaviorSource")]
    private BehaviorSource behaviorSource;

    public BehaviorSource BehaviorSource
    {
      get => this.behaviorSource;
      set => this.behaviorSource = value;
    }

    public Object GetObject() => (Object) this;

    public string GetOwnerName() => this.name;

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
      this.GetVariable(name)?.SetValue(value);
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

    private void CheckForSerialization()
    {
      this.behaviorSource.Owner = (IBehaviorTree) this;
      this.behaviorSource.CheckForSerialization(false, (BehaviorSource) null, this.GetOwnerName());
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

    int IBehaviorTree.GetInstanceID() => this.GetInstanceID();
  }
}
