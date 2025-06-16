using System.Collections.Generic;
using Engine.Common.Generator;

namespace BehaviorDesigner.Runtime.Tasks
{
  public abstract class ParentTask : Task
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    protected List<Task> children;

    public List<Task> Children
    {
      get => children;
      private set => children = value;
    }

    public virtual int MaxChildren() => int.MaxValue;

    public virtual bool CanRunParallelChildren() => false;

    public virtual int CurrentChildIndex() => 0;

    public virtual bool CanExecute() => true;

    public virtual TaskStatus Decorate(TaskStatus status) => status;

    public virtual bool CanReevaluate() => false;

    public virtual bool OnReevaluationStarted() => false;

    public virtual void OnReevaluationEnded(TaskStatus status)
    {
    }

    public virtual void OnChildExecuted(TaskStatus childStatus)
    {
    }

    public virtual void OnChildExecuted(int childIndex, TaskStatus childStatus)
    {
    }

    public virtual void OnChildStarted()
    {
    }

    public virtual void OnChildStarted(int childIndex)
    {
    }

    public virtual TaskStatus OverrideStatus(TaskStatus status) => status;

    public virtual TaskStatus OverrideStatus() => TaskStatus.Running;

    public virtual void OnConditionalAbort(int childIndex)
    {
    }

    public override float GetUtility()
    {
      float utility = 0.0f;
      if (children != null)
      {
        for (int index = 0; index < children.Count; ++index)
        {
          if (children[index] != null && !children[index].Disabled)
            utility += children[index].GetUtility();
        }
      }
      return utility;
    }

    public override void OnDrawGizmos()
    {
      if (children == null)
        return;
      for (int index = 0; index < children.Count; ++index)
      {
        if (children[index] != null && !children[index].Disabled)
          children[index].OnDrawGizmos();
      }
    }

    public void AddChild(Task child, int index)
    {
      if (children == null)
        children = new List<Task>();
      children.Insert(index, child);
    }

    public void ReplaceAddChild(Task child, int index)
    {
      if (children != null && index < children.Count)
        children[index] = child;
      else
        AddChild(child, index);
    }
  }
}
