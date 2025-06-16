using System;
using System.Collections.Generic;
using ParadoxNotion.Serialization.FullSerializer;
using Object = UnityEngine.Object;

namespace ParadoxNotion.Serialization;

public class fsUnityObjectConverter : fsConverter {
	public override bool CanProcess(Type type) {
		return typeof(Object).RTIsAssignableFrom(type);
	}

	public override bool RequestCycleSupport(Type storageType) {
		return false;
	}

	public override bool RequestInheritanceSupport(Type storageType) {
		return false;
	}

	public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType) {
		var objectList = Serializer.Context.Get<List<Object>>();
		var @object = instance as Object;
		if ((object)@object == null) {
			serialized = new fsData(0L);
			return fsResult.Success;
		}

		if (objectList.Count == 0)
			objectList.Add(null);
		var i = -1;
		for (var index = 0; index < objectList.Count; ++index)
			if (objectList[index] == (object)@object) {
				i = index;
				break;
			}

		if (i <= 0) {
			i = objectList.Count;
			objectList.Add(@object);
		}

		serialized = new fsData(i);
		return fsResult.Success;
	}

	public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {
		var objectList = Serializer.Context.Get<List<Object>>();
		var asInt64 = (int)data.AsInt64;
		if (asInt64 >= objectList.Count)
			return fsResult.Warn("A Unity Object reference has not been deserialized");
		instance = objectList[asInt64];
		return fsResult.Success;
	}

	public override object CreateInstance(fsData data, Type storageType) {
		return null;
	}
}