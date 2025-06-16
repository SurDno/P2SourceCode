using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;

namespace Engine.Common.Commons.Converters;

public static class DefaultDataReadUtility {
	public static byte Read(IDataReader reader, string name, byte value) {
		return DefaultConverter.ParseByte(reader.ReadSimple(name));
	}

	public static sbyte Read(IDataReader reader, string name, sbyte value) {
		return DefaultConverter.ParseSbyte(reader.ReadSimple(name));
	}

	public static int Read(IDataReader reader, string name, int value) {
		return DefaultConverter.ParseInt(reader.ReadSimple(name));
	}

	public static uint Read(IDataReader reader, string name, uint value) {
		return DefaultConverter.ParseUint(reader.ReadSimple(name));
	}

	public static short Read(IDataReader reader, string name, short value) {
		return DefaultConverter.ParseShort(reader.ReadSimple(name));
	}

	public static ushort Read(IDataReader reader, string name, ushort value) {
		return DefaultConverter.ParseUshort(reader.ReadSimple(name));
	}

	public static long Read(IDataReader reader, string name, long value) {
		return DefaultConverter.ParseLong(reader.ReadSimple(name));
	}

	public static ulong Read(IDataReader reader, string name, ulong value) {
		return DefaultConverter.ParseUlong(reader.ReadSimple(name));
	}

	public static float Read(IDataReader reader, string name, float value) {
		return DefaultConverter.ParseFloat(reader.ReadSimple(name));
	}

	public static double Read(IDataReader reader, string name, double value) {
		return DefaultConverter.ParseDouble(reader.ReadSimple(name));
	}

	public static char Read(IDataReader reader, string name, char value) {
		return DefaultConverter.ParseChar(reader.ReadSimple(name));
	}

	public static bool Read(IDataReader reader, string name, bool value) {
		return DefaultConverter.ParseBool(reader.ReadSimple(name));
	}

	public static Guid Read(IDataReader reader, string name, Guid value) {
		return DefaultConverter.ParseGuid(reader.ReadSimple(name));
	}

	public static System.DateTime Read(IDataReader reader, string name, System.DateTime value) {
		return DefaultConverter.ParseDateTime(reader.ReadSimple(name));
	}

	public static TimeSpan Read(IDataReader reader, string name, TimeSpan value) {
		return DefaultConverter.ParseTimeSpan(reader.ReadSimple(name));
	}

	public static string Read(IDataReader reader, string name, string value) {
		return reader.GetChild(name)?.Read();
	}

	public static byte[] Read(IDataReader reader, string name, byte[] value) {
		var child = reader.GetChild(name);
		return child != null ? Convert.FromBase64String(child.Read()) : null;
	}

	public static T ReadEnum<T>(IDataReader reader, string name)
		where T : struct, IComparable, IConvertible, IFormattable {
		T result;
		DefaultConverter.TryParseEnum(reader.ReadSimple(name), out result);
		return result;
	}

	public static T ReadSerialize<T>(IDataReader reader) where T : class {
		var realType = MappingUtility.GetRealType(reader, typeof(T));
		var obj = (T)ProxyFactory.Create(realType);
		if (!(obj is ISerializeDataRead serializeDataRead)) {
			Logger.AddError("Type : " + obj.GetType().Name + " is not " + typeof(ISerializeDataRead));
			return default;
		}

		serializeDataRead.DataRead(reader, realType);
		return obj;
	}

	public static T ReadSerialize<T>(IDataReader reader, string name) where T : class {
		var child = reader.GetChild(name);
		return child == null ? default : ReadSerialize<T>(child);
	}

	public static List<double> ReadList(IDataReader reader, string name, List<double> value) {
		if (value == null)
			value = new List<double>();
		else
			value.Clear();
		var child1 = reader.GetChild(name);
		if (child1 == null)
			return value;
		foreach (var child2 in child1.GetChilds()) {
			var num = DefaultConverter.ParseDouble(child2.Read());
			value.Add(num);
		}

		return value;
	}

	public static List<string> ReadList(IDataReader reader, string name, List<string> value) {
		if (value == null)
			value = new List<string>();
		else
			value.Clear();
		var child1 = reader.GetChild(name);
		if (child1 == null)
			return value;
		foreach (var child2 in child1.GetChilds())
			value.Add(child2.Read());
		return value;
	}

	public static List<T> ReadListSerialize<T>(IDataReader reader, string name, List<T> value) {
		if (value == null)
			value = new List<T>();
		else
			value.Clear();
		var child1 = reader.GetChild(name);
		if (child1 == null)
			return value;
		foreach (var child2 in child1.GetChilds()) {
			var realType = MappingUtility.GetRealType(child2, typeof(T));
			var obj = (T)ProxyFactory.Create(realType);
			if (!(obj is ISerializeDataRead serializeDataRead))
				Logger.AddError("Type : " + obj.GetType().Name + " is not " + typeof(ISerializeDataRead));
			else {
				serializeDataRead.DataRead(child2, realType);
				value.Add(obj);
			}
		}

		return value;
	}

	public static List<T> ReadListEnum<T>(IDataReader reader, string name, List<T> value)
		where T : struct, IComparable, IConvertible, IFormattable {
		if (value == null)
			value = new List<T>();
		else
			value.Clear();
		var child1 = reader.GetChild(name);
		if (child1 == null)
			return value;
		foreach (var child2 in child1.GetChilds()) {
			T result;
			DefaultConverter.TryParseEnum(child2.Read(), out result);
			value.Add(result);
		}

		return value;
	}
}