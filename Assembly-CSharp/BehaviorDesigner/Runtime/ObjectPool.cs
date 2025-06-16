using System;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime;

public static class ObjectPool {
	private static Dictionary<Type, object> poolDictionary = new();

	public static void Clear() {
		poolDictionary.Clear();
	}

	public static T Get<T>() {
		object obj;
		if (poolDictionary.TryGetValue(typeof(T), out obj)) {
			var objStack = obj as Stack<T>;
			if (objStack.Count > 0)
				return objStack.Pop();
		}

		return (T)TaskUtility.CreateInstance(typeof(T));
	}

	public static void Return<T>(T obj) {
		if (obj == null)
			return;
		object obj1;
		if (poolDictionary.TryGetValue(typeof(T), out obj1))
			(obj1 as Stack<T>).Push(obj);
		else {
			var objStack = new Stack<T>();
			objStack.Push(obj);
			poolDictionary.Add(typeof(T), objStack);
		}
	}
}