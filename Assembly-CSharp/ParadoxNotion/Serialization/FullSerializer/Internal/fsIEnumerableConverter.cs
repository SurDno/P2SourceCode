using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ParadoxNotion.Serialization.FullSerializer.Internal;

public class fsIEnumerableConverter : fsConverter {
	public override bool CanProcess(Type type) {
		return typeof(IEnumerable).IsAssignableFrom(type) && GetAddMethod(type) != null;
	}

	public override object CreateInstance(fsData data, Type storageType) {
		return fsMetaType.Get(Serializer.Config, storageType).CreateInstance();
	}

	public override fsResult TrySerialize(
		object instance_,
		out fsData serialized,
		Type storageType) {
		var collection = (IEnumerable)instance_;
		var success = fsResult.Success;
		var elementType = GetElementType(storageType);
		serialized = fsData.CreateList(HintSize(collection));
		var asList = serialized.AsList;
		foreach (var instance in collection) {
			fsData data;
			var result = Serializer.TrySerialize(elementType, instance, out data);
			success.AddMessages(result);
			if (!result.Failed)
				asList.Add(data);
		}

		if (IsStack(collection.GetType()))
			asList.Reverse();
		return success;
	}

	private bool IsStack(Type type) {
		return type.Resolve().IsGenericType && type.Resolve().GetGenericTypeDefinition() == typeof(Stack<>);
	}

	public override fsResult TryDeserialize(fsData data, ref object instance_, Type storageType) {
		var instance = (IEnumerable)instance_;
		fsResult fsResult;
		if ((fsResult = fsResult.Success + CheckType(data, fsDataType.Array)).Failed)
			return fsResult;
		if (data.AsList.Count == 0)
			return fsResult.Success;
		if (instance is IList) {
			var genericArguments = storageType.GetGenericArguments();
			if (genericArguments.Length == 1) {
				var list = (IList)instance;
				var storageType1 = genericArguments[0];
				for (var index = 0; index < data.AsList.Count; ++index) {
					object result = null;
					Serializer.TryDeserialize(data.AsList[index], storageType1, ref result);
					list.Add(result);
				}

				return fsResult.Success;
			}
		}

		var elementType = GetElementType(storageType);
		var addMethod = GetAddMethod(storageType);
		var flattenedMethod1 = storageType.GetFlattenedMethod("get_Item");
		var flattenedMethod2 = storageType.GetFlattenedMethod("set_Item");
		if (flattenedMethod2 == null)
			TryClear(storageType, instance);
		var existingSize = TryGetExistingSize(storageType, instance);
		var asList = data.AsList;
		for (var index = 0; index < asList.Count; ++index) {
			var data1 = asList[index];
			object result1 = null;
			if (flattenedMethod1 != null && index < existingSize)
				result1 = flattenedMethod1.Invoke(instance, new object[1] {
					index
				});
			var result2 = Serializer.TryDeserialize(data1, elementType, ref result1);
			fsResult.AddMessages(result2);
			if (!result2.Failed) {
				if (flattenedMethod2 != null && index < existingSize)
					flattenedMethod2.Invoke(instance, new object[2] {
						index,
						result1
					});
				else
					addMethod.Invoke(instance, new object[1] {
						result1
					});
			}
		}

		return fsResult;
	}

	private static int HintSize(IEnumerable collection) {
		return collection is ICollection ? ((ICollection)collection).Count : 0;
	}

	private static Type GetElementType(Type objectType) {
		if (objectType.HasElementType)
			return objectType.GetElementType();
		var type = fsReflectionUtility.GetInterface(objectType, typeof(IEnumerable<>));
		return type != null ? type.GetGenericArguments()[0] : typeof(object);
	}

	private static void TryClear(Type type, object instance) {
		var flattenedMethod = type.GetFlattenedMethod("Clear");
		if (!(flattenedMethod != null))
			return;
		flattenedMethod.Invoke(instance, null);
	}

	private static int TryGetExistingSize(Type type, object instance) {
		var flattenedProperty = type.GetFlattenedProperty("Count");
		return flattenedProperty != null ? (int)flattenedProperty.GetGetMethod().Invoke(instance, null) : 0;
	}

	private static MethodInfo GetAddMethod(Type type) {
		var type1 = fsReflectionUtility.GetInterface(type, typeof(ICollection<>));
		if (type1 != null) {
			var declaredMethod = type1.GetDeclaredMethod("Add");
			if (declaredMethod != null)
				return declaredMethod;
		}

		var addMethod = type.GetFlattenedMethod("Add");
		if ((object)addMethod == null)
			addMethod = type.GetFlattenedMethod("Push") ?? type.GetFlattenedMethod("Enqueue");
		return addMethod;
	}
}