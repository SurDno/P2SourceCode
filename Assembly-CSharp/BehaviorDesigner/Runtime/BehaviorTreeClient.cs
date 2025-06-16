using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime;

public class BehaviorTreeClient {
	public List<Task> taskList = new();
	public List<int> parentIndex = new();
	public List<List<int>> childrenIndex = new();
	public List<int> relativeChildIndex = new();
	public List<Stack<int>> activeStack = new();
	public List<TaskStatus> nonInstantTaskStatus = new();
	public List<int> interruptionIndex = new();
	public List<ConditionalReevaluate> conditionalReevaluate = new();
	public Dictionary<int, ConditionalReevaluate> conditionalReevaluateMap = new();
	public List<int> parentReevaluate = new();
	public List<int> parentCompositeIndex = new();
	public List<List<int>> childConditionalIndex = new();
	public int executionCount;
	public string behaviorName;
	public BehaviorTree behavior;
	public bool destroyBehavior;

	public void Initialize(BehaviorTree b) {
		behavior = b;
		behaviorName = b.name;
		for (var index = childrenIndex.Count - 1; index > -1; --index)
			ObjectPool.Return(childrenIndex[index]);
		for (var index = activeStack.Count - 1; index > -1; --index)
			ObjectPool.Return(activeStack[index]);
		for (var index = childConditionalIndex.Count - 1; index > -1; --index)
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