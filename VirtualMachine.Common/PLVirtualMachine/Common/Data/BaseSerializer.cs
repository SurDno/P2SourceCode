using System;
using System.Reflection;
using Cofe.Utility;

namespace PLVirtualMachine.Common.Data;

public static class BaseSerializer {
	public static object GetDefaultValue(Type valueType) {
		if (valueType.IsValueType)
			return TypeDefaultUtility.GetDefault(valueType);
		if (valueType == typeof(string))
			return "";
		if (valueType.IsInterface) {
			valueType = GetRealRefType(valueType);
			if (null != valueType)
				return Activator.CreateInstance(valueType);
		} else {
			var constructor = valueType.GetConstructor(null);
			if (constructor != null)
				return constructor.Invoke(null);
		}

		return "";
	}

	public static Type GetRealRefType(Type type) {
		Type result;
		VMFactoryAttribute.TryGetValue(type, out result);
		return result;
	}
}