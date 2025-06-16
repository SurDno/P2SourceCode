using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

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
