using System;
using System.Collections;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer.Internal;

public class fsDictionaryConverter : fsConverter {
	public override bool CanProcess(Type type) {
		return typeof(IDictionary).IsAssignableFrom(type);
	}

	public override object CreateInstance(fsData data, Type storageType) {
		return fsMetaType.Get(Serializer.Config, storageType).CreateInstance();
	}

	public override fsResult TryDeserialize(fsData data, ref object instance_, Type storageType) {
		var dictionary = (IDictionary)instance_;
		var fsResult1 = fsResult.Success;
		Type keyStorageType;
		Type valueStorageType;
		GetKeyValueTypes(dictionary.GetType(), out keyStorageType, out valueStorageType);
		if (data.IsList) {
			var asList = data.AsList;
			for (var index = 0; index < asList.Count; ++index) {
				var data1 = asList[index];
				fsResult fsResult2;
				var fsResult3 = fsResult2 = fsResult1 + CheckType(data1, fsDataType.Object);
				if (fsResult3.Failed)
					return fsResult2;
				fsData subitem1;
				fsResult fsResult4;
				fsResult3 = fsResult4 = fsResult2 + CheckKey(data1, "Key", out subitem1);
				if (fsResult3.Failed)
					return fsResult4;
				fsData subitem2;
				fsResult fsResult5;
				fsResult3 = fsResult5 = fsResult4 + CheckKey(data1, "Value", out subitem2);
				if (fsResult3.Failed)
					return fsResult5;
				object result1 = null;
				object result2 = null;
				fsResult fsResult6;
				fsResult3 = fsResult6 = fsResult5 + Serializer.TryDeserialize(subitem1, keyStorageType, ref result1);
				if (fsResult3.Failed)
					return fsResult6;
				fsResult3 = fsResult1 = fsResult6 + Serializer.TryDeserialize(subitem2, valueStorageType, ref result2);
				if (fsResult3.Failed)
					return fsResult1;
				AddItemToDictionary(dictionary, result1, result2);
			}
		} else if (data.IsDictionary) {
			foreach (var keyValuePair in data.AsDictionary)
				if (!fsSerializer.IsReservedKeyword(keyValuePair.Key)) {
					var data2 = new fsData(keyValuePair.Key);
					var data3 = keyValuePair.Value;
					object result3 = null;
					object result4 = null;
					var fsResult7 = fsResult1 += Serializer.TryDeserialize(data2, keyStorageType, ref result3);
					if (fsResult7.Failed)
						return fsResult1;
					fsResult7 = fsResult1 += Serializer.TryDeserialize(data3, valueStorageType, ref result4);
					if (fsResult7.Failed)
						return fsResult1;
					AddItemToDictionary(dictionary, result3, result4);
				}
		} else
			return FailExpectedType(data, fsDataType.Array, fsDataType.Object);

		return fsResult1;
	}

	public override fsResult TrySerialize(
		object instance_,
		out fsData serialized,
		Type storageType) {
		serialized = fsData.Null;
		var fsResult1 = fsResult.Success;
		var dictionary = (IDictionary)instance_;
		Type keyStorageType;
		Type valueStorageType;
		GetKeyValueTypes(dictionary.GetType(), out keyStorageType, out valueStorageType);
		var enumerator = dictionary.GetEnumerator();
		var flag = true;
		var fsDataList1 = new List<fsData>(dictionary.Count);
		var fsDataList2 = new List<fsData>(dictionary.Count);
		while (enumerator.MoveNext()) {
			fsData data1;
			fsResult fsResult2;
			var fsResult3 = fsResult2 = fsResult1 + Serializer.TrySerialize(keyStorageType, enumerator.Key, out data1);
			if (fsResult3.Failed)
				return fsResult2;
			fsData data2;
			fsResult3 = fsResult1 = fsResult2 + Serializer.TrySerialize(valueStorageType, enumerator.Value, out data2);
			if (fsResult3.Failed)
				return fsResult1;
			fsDataList1.Add(data1);
			fsDataList2.Add(data2);
			flag &= data1.IsString;
		}

		if (flag) {
			serialized = fsData.CreateDictionary();
			var asDictionary = serialized.AsDictionary;
			for (var index = 0; index < fsDataList1.Count; ++index) {
				var fsData1 = fsDataList1[index];
				var fsData2 = fsDataList2[index];
				asDictionary[fsData1.AsString] = fsData2;
			}
		} else {
			serialized = fsData.CreateList(fsDataList1.Count);
			var asList = serialized.AsList;
			for (var index = 0; index < fsDataList1.Count; ++index) {
				var fsData3 = fsDataList1[index];
				var fsData4 = fsDataList2[index];
				asList.Add(new fsData(new Dictionary<string, fsData> {
					["Key"] = fsData3,
					["Value"] = fsData4
				}));
			}
		}

		return fsResult1;
	}

	private fsResult AddItemToDictionary(IDictionary dictionary, object key, object value) {
		if (key == null || value == null) {
			var type = fsReflectionUtility.GetInterface(dictionary.GetType(), typeof(ICollection<>));
			if (type == null)
				return fsResult.Warn(dictionary.GetType() + " does not extend ICollection");
			var instance = Activator.CreateInstance(type.GetGenericArguments()[0], key, value);
			type.GetFlattenedMethod("Add").Invoke(dictionary, new object[1] {
				instance
			});
			return fsResult.Success;
		}

		dictionary[key] = value;
		return fsResult.Success;
	}

	private static void GetKeyValueTypes(
		Type dictionaryType,
		out Type keyStorageType,
		out Type valueStorageType) {
		var type = fsReflectionUtility.GetInterface(dictionaryType, typeof(IDictionary<,>));
		if (type != null) {
			var genericArguments = type.GetGenericArguments();
			keyStorageType = genericArguments[0];
			valueStorageType = genericArguments[1];
		} else {
			keyStorageType = typeof(object);
			valueStorageType = typeof(object);
		}
	}
}