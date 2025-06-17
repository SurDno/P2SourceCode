using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
  public class BehaviorTreeClient
  {
    public List<Task> taskList = [];
    public List<int> parentIndex = [];
    public List<List<int>> childrenIndex = [];
    public List<int> relativeChildIndex = [];
    public List<Stack<int>> activeStack = [];
    public List<TaskStatus> nonInstantTaskStatus = [];
    public List<int> interruptionIndex = [];
    public List<ConditionalReevaluate> conditionalReevaluate = [];
    public Dictionary<int, ConditionalReevaluate> conditionalReevaluateMap = new();
    public List<int> parentReevaluate = [];
    public List<int> parentCompositeIndex = [];
    public List<List<int>> childConditionalIndex = [];
    public int executionCount;
    public string behaviorName;
    public BehaviorTree behavior;
    public bool destroyBehavior;

    public void Initialize(BehaviorTree b)
    {
      behavior = b;
      behaviorName = b.name;
      for (int index = childrenIndex.Count - 1; index > -1; --index)
        ObjectPool.Return(childrenIndex[index]);
      for (int index = activeStack.Count - 1; index > -1; --index)
        ObjectPool.Return(activeStack[index]);
      for (int index = childConditionalIndex.Count - 1; index > -1; --index)
        ObjectPool.Return(childConditionalIndex[index]);
      taskList.Clear();
      parentIndex.Clear();
      childrenIndex.Clear();
      relativeChildIndex.Clear();
      activeStack.Clear();
      nonInstantTaskStatus.Clear();
      interruptionIndex.Clear();
      conditionalReevaluate.Clear();
      conditionalReevaluateMap.Clear();
      parentReevaluate.Clear();
      parentCompositeIndex.Clear();
      childConditionalIndex.Clear();
    }
  }
}
