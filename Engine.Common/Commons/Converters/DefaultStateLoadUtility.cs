using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Utility;

namespace Engine.Common.Commons.Converters;

public static class DefaultStateLoadUtility {
	public static T ReadSerialize<T>(IDataReader reader, string name) where T : class {
		var child = reader.GetChild(name);
		if (child == null)
			return default;
		var realType = MappingUtility.GetRealType(child, typeof(T));
		var obj = (T)ProxyFactory.Create(realType);
		if (!(obj is ISerializeStateLoad serializeStateLoad)) {
			Logger.AddError("Type : " + TypeUtility.GetTypeName(obj.GetType()) + " is not " +
			                typeof(ISerializeStateLoad));
			return default;
		}

		serializeStateLoad.StateLoad(child, realType);
		return obj;
	}

	public static List<T> ReadListSerialize<T>(IDataReader reader, string name, List<T> value) {
		value.Clear();
		foreach (var child in reader.GetChild(name).GetChilds()) {
			var realType = MappingUtility.GetRealType(child, typeof(T));
			var obj = (T)ProxyFactory.Create(realType);
			if (!(obj is ISerializeStateLoad serializeStateLoad))
				Logger.AddError("Type : " + TypeUtility.GetTypeName(obj.GetType()) + " is not " +
				                typeof(ISerializeStateLoad));
			else {
				serializeStateLoad.StateLoad(child, realType);
				value.Add(obj);
			}
		}

		return value;
	}
}