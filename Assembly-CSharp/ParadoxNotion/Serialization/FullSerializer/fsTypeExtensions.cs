using System;
using System.Collections.Generic;
using System.Linq;

namespace ParadoxNotion.Serialization.FullSerializer;

public static class fsTypeExtensions {
	public static string CSharpName(this Type type) {
		return type.CSharpName(false);
	}

	public static string CSharpName(
		this Type type,
		bool includeNamespace,
		bool ensureSafeDeclarationName) {
		var str = type.CSharpName(includeNamespace);
		if (ensureSafeDeclarationName)
			str = str.Replace('>', '_').Replace('<', '_').Replace('.', '_');
		return str;
	}

	public static string CSharpName(this Type type, bool includeNamespace) {
		if (type == typeof(void))
			return "void";
		if (type == typeof(int))
			return "int";
		if (type == typeof(float))
			return "float";
		if (type == typeof(bool))
			return "bool";
		if (type == typeof(double))
			return "double";
		if (type == typeof(string))
			return "string";
		if (type.IsGenericParameter)
			return type.ToString();
		var str1 = "";
		IEnumerable<Type> source = type.GetGenericArguments();
		if (type.IsNested) {
			str1 = str1 + type.DeclaringType.CSharpName() + ".";
			if (type.DeclaringType.GetGenericArguments().Length != 0)
				source = source.Skip(type.DeclaringType.GetGenericArguments().Length);
		}

		var str2 = source.Any()
			? str1 + type.Name.Substring(0, type.Name.IndexOf('`')) + "<" +
			  string.Join(",", source.Select(t => t.CSharpName(includeNamespace)).ToArray()) + ">"
			: str1 + type.Name;
		if (includeNamespace && type.Namespace != null)
			str2 = type.Namespace + "." + str2;
		return str2;
	}
}