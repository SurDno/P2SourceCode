using System;
using System.Collections.Generic;

public static class TypeAliasUtility {
	private static readonly Dictionary<Type, string> aliases = new() {
		{
			typeof(byte),
			"byte"
		}, {
			typeof(sbyte),
			"sbyte"
		}, {
			typeof(short),
			"short"
		}, {
			typeof(ushort),
			"ushort"
		}, {
			typeof(int),
			"int"
		}, {
			typeof(uint),
			"uint"
		}, {
			typeof(long),
			"long"
		}, {
			typeof(ulong),
			"ulong"
		}, {
			typeof(float),
			"float"
		}, {
			typeof(double),
			"double"
		}, {
			typeof(decimal),
			"decimal"
		}, {
			typeof(object),
			"object"
		}, {
			typeof(bool),
			"bool"
		}, {
			typeof(char),
			"char"
		}, {
			typeof(string),
			"string"
		}, {
			typeof(void),
			"void"
		}
	};

	public static string GetName(this Type type) {
		string str;
		return aliases.TryGetValue(type, out str) ? str : type.Name;
	}
}