// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.ParentTask
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  public abstract class ParentTask : Task
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    protected List<Task> children;

    public List<Task> Children
    {
      get => this.children;
      private set => this.children = value;
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
      if (this.children != null)
      {
        for (int index = 0; index < this.children.Count; ++index)
        {
          if (this.children[index] != null && !this.children[index].Disabled)
            utility += this.children[index].GetUtility();
        }
      }
      return utility;
    }

    public override void OnDrawGizmos()
    {
      if (this.children == null)
        return;
      for (int index = 0; index < this.children.Count; ++index)
      {
        if (this.children[index] != null && !this.children[index].Disabled)
          this.children[index].OnDrawGizmos();
      }
    }

    public void AddChild(Task child, int index)
    {
      if (this.children == null)
        this.children = new List<Task>();
      this.children.Insert(index, child);
    }

    public void ReplaceAddChild(Task child, int index)
    {
      if (this.children != null && index < this.children.Count)
        this.children[index] = child;
      else
        this.AddChild(child, index);
    }
  }
}
