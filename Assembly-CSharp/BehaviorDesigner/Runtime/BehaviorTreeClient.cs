// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.BehaviorTreeClient
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public class BehaviorTreeClient
  {
    public List<Task> taskList = new List<Task>();
    public List<int> parentIndex = new List<int>();
    public List<List<int>> childrenIndex = new List<List<int>>();
    public List<int> relativeChildIndex = new List<int>();
    public List<Stack<int>> activeStack = new List<Stack<int>>();
    public List<TaskStatus> nonInstantTaskStatus = new List<TaskStatus>();
    public List<int> interruptionIndex = new List<int>();
    public List<ConditionalReevaluate> conditionalReevaluate = new List<ConditionalReevaluate>();
    public Dictionary<int, ConditionalReevaluate> conditionalReevaluateMap = new Dictionary<int, ConditionalReevaluate>();
    public List<int> parentReevaluate = new List<int>();
    public List<int> parentCompositeIndex = new List<int>();
    public List<List<int>> childConditionalIndex = new List<List<int>>();
    public int executionCount;
    public string behaviorName;
    public BehaviorTree behavior;
    public bool destroyBehavior;

    public void Initialize(BehaviorTree b)
    {
      this.behavior = b;
      this.behaviorName = b.name;
      for (int index = this.childrenIndex.Count - 1; index > -1; --index)
        ObjectPool.Return<List<int>>(this.childrenIndex[index]);
      for (int index = this.activeStack.Count - 1; index > -1; --index)
        ObjectPool.Return<Stack<int>>(this.activeStack[index]);
      for (int index = this.childConditionalIndex.Count - 1; index > -1; --index)
        ObjectPool.Return<List<int>>(this.childConditionalIndex[index]);
      this.taskList.Clear();
      this.parentIndex.Clear();
      this.childrenIndex.Clear();
      this.relativeChildIndex.Clear();
      this.activeStack.Clear();
      this.nonInstantTaskStatus.Clear();
      this.interruptionIndex.Clear();
      this.conditionalReevaluate.Clear();
      this.conditionalReevaluateMap.Clear();
      this.parentReevaluate.Clear();
      this.parentCompositeIndex.Clear();
      this.childConditionalIndex.Clear();
    }
  }
}
