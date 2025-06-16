using System;
using System.Collections.Generic;
using System.Reflection;

namespace ParadoxNotion.Serialization.FullSerializer.Internal;

public class fsKeyValuePairConverter : fsConverter {
	public override bool CanProcess(Type type) {
		return type.Resolve().IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
	}

	public override bool RequestCycleSupport(Type storageType) {
		return false;
	}

	public override bool RequestInheritanceSupport(Type storageType) {
		return false;
	}

	public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {
		fsData subitem1;
		fsResult fsResult1;
		if ((fsResult1 = fsResult.Success + CheckKey(data, "Key", out subitem1)).Failed)
			return fsResult1;
		fsData subitem2;
		fsResult fsResult2;
		if ((fsResult2 = fsResult1 + CheckKey(data, "Value", out subitem2)).Failed)
			return fsResult2;
		var genericArguments = storageType.GetGenericArguments();
		var storageType1 = genericArguments[0];
		var storageType2 = genericArguments[1];
		object result1 = null;
		object result2 = null;
		fsResult2.AddMessages(Serializer.TryDeserialize(subitem1, storageType1, ref result1));
		fsResult2.AddMessages(Serializer.TryDeserialize(subitem2, storageType2, ref result2));
		instance = Activator.CreateInstance(storageType, result1, result2);
		return fsResult2;
	}

	public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType) {
		var declaredProperty1 = storageType.GetDeclaredProperty("Key");
		var declaredProperty2 = storageType.GetDeclaredProperty("Value");
		var instance1 = declaredProperty1.GetValue(instance, null);
		var instance2 = declaredProperty2.GetValue(instance, null);
		var genericArguments = storageType.GetGenericArguments();
		var storageType1 = genericArguments[0];
		var storageType2 = genericArguments[1];
		var success = fsResult.Success;
		fsData data1;
		success.AddMessages(Serializer.TrySerialize(storageType1, instance1, out data1));
		fsData data2;
		success.AddMessages(Serializer.TrySerialize(storageType2, instance2, out data2));
		serialized = fsData.CreateDictionary();
		if (data1 != null)
			serialized.AsDictionary["Key"] = data1;
		if (data2 != null)
			serialized.AsDictionary["Value"] = data2;
		return success;
	}
}