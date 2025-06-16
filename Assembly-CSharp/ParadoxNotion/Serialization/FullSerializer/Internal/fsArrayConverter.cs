using System;
using System.Collections;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer.Internal;

public class fsArrayConverter : fsConverter {
	public override bool CanProcess(Type type) {
		return type.IsArray;
	}

	public override bool RequestCycleSupport(Type storageType) {
		return false;
	}

	public override bool RequestInheritanceSupport(Type storageType) {
		return false;
	}

	public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType) {
		IList list = (Array)instance;
		var elementType = storageType.GetElementType();
		var success = fsResult.Success;
		serialized = fsData.CreateList(list.Count);
		var asList = serialized.AsList;
		for (var index = 0; index < list.Count; ++index) {
			var instance1 = list[index];
			fsData data;
			var result = Serializer.TrySerialize(elementType, instance1, out data);
			success.AddMessages(result);
			if (!result.Failed)
				asList.Add(data);
		}

		return success;
	}

	public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {
		fsResult fsResult;
		if ((fsResult = fsResult.Success + CheckType(data, fsDataType.Array)).Failed)
			return fsResult;
		var elementType = storageType.GetElementType();
		var asList = data.AsList;
		var arrayList = new ArrayList(asList.Count);
		var count = arrayList.Count;
		for (var index = 0; index < asList.Count; ++index) {
			var data1 = asList[index];
			object result1 = null;
			if (index < count)
				result1 = arrayList[index];
			var result2 = Serializer.TryDeserialize(data1, elementType, ref result1);
			fsResult.AddMessages(result2);
			if (!result2.Failed) {
				if (index < count)
					arrayList[index] = result1;
				else
					arrayList.Add(result1);
			}
		}

		instance = arrayList.ToArray(elementType);
		return fsResult;
	}

	public override object CreateInstance(fsData data, Type storageType) {
		return fsMetaType.Get(Serializer.Config, storageType).CreateInstance();
	}
}