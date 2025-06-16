using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace ParadoxNotion.Serialization.FullSerializer.Internal;

public class fsReflectedConverter : fsConverter {
	public override bool CanProcess(Type type) {
		return !type.Resolve().IsArray && !typeof(ICollection).IsAssignableFrom(type);
	}

	public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType) {
		serialized = fsData.CreateDictionary();
		var success = fsResult.Success;
		var fsMetaType = fsMetaType.Get(Serializer.Config, instance.GetType());
		fsMetaType.EmitAotData();
		object context = null;
		if (!fsGlobalConfig.SerializeDefaultValues && !(instance is Object))
			context = fsMetaType.CreateInstance();
		for (var index = 0; index < fsMetaType.Properties.Length; ++index) {
			var property = fsMetaType.Properties[index];
			if (property.CanRead && (fsGlobalConfig.SerializeDefaultValues || context == null ||
			                         !Equals(property.Read(instance), property.Read(context)))) {
				fsData data;
				var result = Serializer.TrySerialize(property.StorageType, property.OverrideConverterType,
					property.Read(instance), out data);
				success.AddMessages(result);
				if (!result.Failed)
					serialized.AsDictionary[property.JsonName] = data;
			}
		}

		return success;
	}

	public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {
		fsResult fsResult;
		if ((fsResult = fsResult.Success + CheckType(data, fsDataType.Object)).Failed)
			return fsResult;
		if (data.AsDictionary.Count == 0)
			return fsResult.Success;
		var fsMetaType = fsMetaType.Get(Serializer.Config, storageType);
		fsMetaType.EmitAotData();
		for (var index = 0; index < fsMetaType.Properties.Length; ++index) {
			var property = fsMetaType.Properties[index];
			fsData data1;
			if (property.CanWrite && data.AsDictionary.TryGetValue(property.JsonName, out data1)) {
				object result1 = null;
				var result2 = Serializer.TryDeserialize(data1, property.StorageType, property.OverrideConverterType,
					ref result1);
				fsResult.AddMessages(result2);
				if (!result2.Failed)
					property.Write(instance, result1);
			}
		}

		return fsResult;
	}

	public override object CreateInstance(fsData data, Type storageType) {
		return fsMetaType.Get(Serializer.Config, storageType).CreateInstance();
	}
}