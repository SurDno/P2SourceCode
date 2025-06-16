using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Scripts.Tools.Serializations.Converters;

public static class BehaviorTreeDataContext {
	public static List<Object> ContextUnityObjects;
	public static Dictionary<int, Task> Tasks;
	public static Dictionary<string, SharedVariable> Variables;

	public static int GetObjectIndex(Object object2, List<Object> unityObjects) {
		for (var index = 0; index < unityObjects.Count; ++index)
			if (unityObjects[index] == object2)
				return index;
		var count = unityObjects.Count;
		unityObjects.Add(object2);
		return count;
	}
}