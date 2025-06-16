using System;
using System.Globalization;
using System.Text;
using Cofe.Loggers;
using Engine.Common.Binders;
using Engine.Common.Types;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common.Data;

public static class StringSerializer {
	public static string LastError { get; set; } = "";

	public static Type ReadTypeByString(string typeName) {
		var type = Type.GetType(typeName);
		Type result;
		if (null == type && EnumTypeAttribute.TryGetValue(typeName, out result))
			type = result;
		return type;
	}

	public static string WriteValue(object value) {
		if (value == null)
			return "";
		if (value.GetType() == typeof(string))
			return (string)value;
		try {
			if (typeof(IVMStringSerializable).IsAssignableFrom(value.GetType()))
				return ((IVMStringSerializable)value).Write();
			LastError = "";
			if (value.GetType() == typeof(Guid))
				return GuidUtility.GetGuidString((Guid)value);
			if (value.GetType() == typeof(HierarchyGuid))
				return ((HierarchyGuid)value).Write();
			if (value.GetType() == typeof(byte[]))
				return Encoding.UTF8.GetString((byte[])value);
			if (value.GetType() == typeof(float))
				return ((float)value).ToString(CultureInfo.InvariantCulture);
			return value.GetType() == typeof(double)
				? ((double)value).ToString(CultureInfo.InvariantCulture)
				: value.ToString();
		} catch (Exception ex) {
			LastError = string.Format("Cannot serialize value of type {0} to string: error {1} at {2}", value.GetType(),
				ex, EngineAPIManager.Instance.CurrentFSMStateInfo);
			Logger.AddError(LastError);
			return "";
		}
	}

	public static object ReadValue(string data, Type valueType) {
		LastError = "";
		if (null == valueType) {
			LastError = string.Format("value {0} conversion error: conversion type is null at {1}", data,
				EngineAPIManager.Instance.CurrentFSMStateInfo);
			Logger.AddError(LastError);
			return null;
		}

		if (!typeof(IVMStringSerializable).IsAssignableFrom(valueType))
			return ReadStringValue(data, valueType);
		if (valueType.IsInterface) {
			valueType = BaseSerializer.GetRealRefType(valueType);
			if (null == valueType) {
				LastError = string.Format("Invalid data interface type {0} at {1}", valueType,
					EngineAPIManager.Instance.CurrentFSMStateInfo);
				Logger.AddError(LastError);
				return null;
			}
		}

		var instance = (IVMStringSerializable)Activator.CreateInstance(valueType);
		instance.Read(data);
		return instance;
	}

	private static object ReadStringValue(string data, Type valueType) {
		LastError = "";
		try {
			if (valueType == typeof(string))
				return data;
			if (((data == "0" || data == "") && valueType != typeof(object) && valueType != typeof(string) &&
			     (!valueType.IsValueType || valueType == typeof(bool))) ||
			    ((data == "0" || data == "") && valueType.IsEnum))
				return BaseSerializer.GetDefaultValue(valueType);
			if (!valueType.IsEnum)
				return StringUtility.To(data, valueType);
			foreach (Enum @enum in Enum.GetValues(valueType))
				if (@enum.ToString() == data)
					return @enum;
			LastError = string.Format("Cannot convert value {0} to enum type {1} at {2}", data, valueType,
				EngineAPIManager.Instance.CurrentFSMStateInfo);
			Logger.AddError(LastError);
		} catch (Exception ex) {
			LastError = string.Format("value {0} conversion error: {1} at {2}", data, ex,
				EngineAPIManager.Instance.CurrentFSMStateInfo);
			Logger.AddError(LastError);
		}

		return BaseSerializer.GetDefaultValue(valueType);
	}
}