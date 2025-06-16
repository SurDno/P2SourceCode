using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ParadoxNotion;

public static class ReflectionTools {
	private const BindingFlags flagsEverything = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
	                                             BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	public static object ContextObject;
	private static Dictionary<string, Type> typeMap = new();
	private static Dictionary<Type, FieldInfo[]> _typeFields = new();

	public static Type GetType(
		string typeFullName,
		bool fallbackNoNamespace = false,
		Type fallbackAssignable = null) {
		if (string.IsNullOrEmpty(typeFullName))
			return null;
		Type type1 = null;
		if (typeMap.TryGetValue(typeFullName, out type1))
			return type1;
		var typeDirect = GetTypeDirect(typeFullName);
		if (typeDirect != null)
			return typeMap[typeFullName] = typeDirect;
		var type2 = TryResolveGenericType(typeFullName, fallbackNoNamespace, fallbackAssignable);
		if (type2 != null)
			return typeMap[typeFullName] = type2;
		if (fallbackNoNamespace) {
			var type3 = TryResolveWithoutNamespace(typeFullName, fallbackAssignable);
			if (type3 != null)
				return typeMap[type3.FullName] = type3;
		}

		LateLog(string.Format("<b>(Type Request)</b> Type with name '{0}' could not be resolved.", typeFullName),
			LogType.Error);
		return typeMap[typeFullName] = null;
	}

	public static string GetContext() {
		var str = "";
		var contextObject = ContextObject;
		string context;
		if (contextObject != null) {
			var @object = contextObject as Object;
			context = !(@object != null)
				? str + "context object : " + contextObject
				: str + "context unity object : " + @object.GetInfo();
		} else
			context = str + "context not found";

		return context;
	}

	private static void LateLog(object logMessage, LogType logType = LogType.Log) {
		var message = logMessage + " , " + GetContext();
		Debug.unityLogger.Log(logType, message);
	}

	private static Type GetTypeDirect(string typeFullName) {
		var type1 = Type.GetType(typeFullName);
		if (type1 != null)
			return type1;
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
			Type type2;
			try {
				type2 = assembly.GetType(typeFullName);
			} catch {
				continue;
			}

			if (type2 != null)
				return type2;
		}

		return null;
	}

	private static Type TryResolveGenericType(
		string typeFullName,
		bool fallbackNoNamespace = false,
		Type fallbackAssignable = null) {
		if (!typeFullName.Contains('`') || !typeFullName.Contains('['))
			return null;
		try {
			var num1 = typeFullName.IndexOf('`');
			var type1 = GetType(typeFullName.Substring(0, num1 + 2), fallbackNoNamespace, fallbackAssignable);
			if (type1 == null)
				return null;
			var int32 = Convert.ToInt32(typeFullName.Substring(num1 + 1, 1));
			string[] array;
			if (typeFullName.Substring(num1 + 2, typeFullName.Length - num1 - 2).StartsWith("[[")) {
				var startIndex = typeFullName.IndexOf("[[") + 2;
				var num2 = typeFullName.LastIndexOf("]]");
				array = typeFullName.Substring(startIndex, num2 - startIndex).Split(new string[1] {
					"],["
				}, int32, StringSplitOptions.RemoveEmptyEntries).ToArray();
			} else {
				var startIndex = typeFullName.IndexOf('[') + 1;
				var num3 = typeFullName.LastIndexOf(']');
				array = typeFullName.Substring(startIndex, num3 - startIndex).Split(new char[1] {
					','
				}, int32, StringSplitOptions.RemoveEmptyEntries).ToArray();
			}

			var typeArgs = new Type[int32];
			for (var index = 0; index < array.Length; ++index) {
				var str = array[index];
				if (!str.Contains('`') && str.Contains(','))
					str = str.Substring(0, str.IndexOf(','));
				Type fallbackAssignable1 = null;
				if (fallbackNoNamespace) {
					var parameterConstraints = type1.RTGetGenericArguments()[index].GetGenericParameterConstraints();
					fallbackAssignable1 = parameterConstraints.Length == 0 ? typeof(object) : parameterConstraints[0];
				}

				var type2 = GetType(str, fallbackNoNamespace, fallbackAssignable1);
				if (type2 == null)
					return null;
				typeArgs[index] = type2;
			}

			return type1.RTMakeGenericType(typeArgs);
		} catch (Exception ex) {
			LateLog("<b>(Type Request)</b> BUG (Please report this): " + ex.Message, LogType.Error);
			return null;
		}
	}

	private static Type TryResolveWithoutNamespace(string typeName, Type fallbackAssignable = null) {
		if (typeName.Contains('`') && typeName.Contains('['))
			return null;
		if (typeName.Contains(','))
			typeName = typeName.Substring(0, typeName.IndexOf(','));
		if (typeName.Contains('.')) {
			var startIndex = typeName.LastIndexOf('.') + 1;
			typeName = typeName.Substring(startIndex, typeName.Length - startIndex);
		}

		return null;
	}

	private static Type[] RTGetExportedTypes(this Assembly asm) {
		return asm.GetExportedTypes();
	}

	public static string FriendlyName(this Type t, bool trueSignature = false) {
		if (t == null)
			return null;
		if (!trueSignature && t == typeof(Object))
			return "UnityObject";
		var str1 = trueSignature ? t.FullName : t.Name;
		if (!trueSignature) {
			if (str1 == "Single")
				str1 = "Float";
			if (str1 == "Int32")
				str1 = "Integer";
		}

		if (!trueSignature && str1.EndsWith("Node"))
			str1 = str1.Substring(0, str1.Length - 4);
		if (t.RTIsGenericParameter())
			str1 = "T";
		if (t.RTIsGenericType()) {
			str1 = (trueSignature ? t.FullName : t.Name) ?? t.Namespace + "." + t.Name;
			var genericArguments = t.RTGetGenericArguments();
			if (genericArguments.Length != 0) {
				var str2 = str1.Replace("`" + genericArguments.Length, "") + "<";
				for (var index = 0; index < genericArguments.Length; ++index)
					str2 = str2 + (index == 0 ? "" : ", ") + genericArguments[index].FriendlyName(trueSignature);
				str1 = str2 + ">";
			}
		}

		return str1;
	}

	public static string SignatureName(this MethodInfo method) {
		var parameters = method.GetParameters();
		var str = (method.IsStatic ? "static " : "") + method.Name + " (";
		for (var index = 0; index < parameters.Length; ++index) {
			var parameterInfo = parameters[index];
			str = str + (parameterInfo.ParameterType.IsByRef ? parameterInfo.IsOut ? "out " : "ref " : "") +
			      parameterInfo.ParameterType.FriendlyName() + (index < parameters.Length - 1 ? ", " : "");
		}

		return str + ") : " + method.ReturnType.FriendlyName();
	}

	public static Type RTReflectedType(this Type type) {
		return type.ReflectedType;
	}

	public static Type RTReflectedType(this MemberInfo member) {
		return member.ReflectedType;
	}

	public static bool RTIsAssignableFrom(this Type type, Type second) {
		return type.IsAssignableFrom(second);
	}

	public static bool RTIsAbstract(this Type type) {
		return type.IsAbstract;
	}

	public static bool RTIsValueType(this Type type) {
		return type.IsValueType;
	}

	public static bool RTIsArray(this Type type) {
		return type.IsArray;
	}

	public static bool RTIsInterface(this Type type) {
		return type.IsInterface;
	}

	public static bool RTIsSubclassOf(this Type type, Type other) {
		return type.IsSubclassOf(other);
	}

	public static bool RTIsGenericParameter(this Type type) {
		return type.IsGenericParameter;
	}

	public static bool RTIsGenericType(this Type type) {
		return type.IsGenericType;
	}

	public static MethodInfo RTGetGetMethod(this PropertyInfo prop) {
		return prop.GetGetMethod();
	}

	public static MethodInfo RTGetSetMethod(this PropertyInfo prop) {
		return prop.GetSetMethod();
	}

	public static FieldInfo RTGetField(this Type type, string name) {
		return type.GetField(name,
			BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
			BindingFlags.FlattenHierarchy);
	}

	public static PropertyInfo RTGetProperty(this Type type, string name) {
		return type.GetProperty(name,
			BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
			BindingFlags.FlattenHierarchy);
	}

	public static MethodInfo RTGetMethod(this Type type, string name) {
		return type.GetMethod(name,
			BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
			BindingFlags.FlattenHierarchy);
	}

	public static MethodInfo RTGetMethod(this Type type, string name, Type[] paramTypes) {
		return type.GetMethod(name, paramTypes);
	}

	public static EventInfo RTGetEvent(this Type type, string name) {
		return type.GetEvent(name,
			BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
			BindingFlags.FlattenHierarchy);
	}

	public static MethodInfo RTGetDelegateMethodInfo(this Delegate del) {
		return del.Method;
	}

	public static FieldInfo[] RTGetFields(this Type type) {
		FieldInfo[] fields;
		if (!_typeFields.TryGetValue(type, out fields)) {
			fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
			                        BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			_typeFields[type] = fields;
		}

		return fields;
	}

	public static PropertyInfo[] RTGetProperties(this Type type) {
		return type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
		                          BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
	}

	public static MethodInfo[] RTGetMethods(this Type type) {
		return type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
		                       BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
	}

	public static T RTGetAttribute<T>(this Type type, bool inherited) where T : Attribute {
		return (T)type.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
	}

	public static T RTGetAttribute<T>(this MemberInfo member, bool inherited) where T : Attribute {
		return (T)member.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
	}

	public static Type RTMakeGenericType(this Type type, Type[] typeArgs) {
		return type.MakeGenericType(typeArgs);
	}

	public static Type[] RTGetGenericArguments(this Type type) {
		return type.GetGenericArguments();
	}

	public static Type[] RTGetEmptyTypes() {
		return Type.EmptyTypes;
	}

	public static T RTCreateDelegate<T>(this MethodInfo method, object instance) {
		return (T)(object)method.RTCreateDelegate(typeof(T), instance);
	}

	public static Delegate RTCreateDelegate(this MethodInfo method, Type type, object instance) {
		return Delegate.CreateDelegate(type, instance, method);
	}

	public static bool IsObsolete(this MemberInfo member) {
		if (member is MethodInfo) {
			var methodInfo = (MethodInfo)member;
			if (methodInfo.Name.StartsWith("get_") || methodInfo.Name.StartsWith("set_"))
				member = methodInfo.DeclaringType.RTGetProperty(methodInfo.Name.Replace("get_", "")
					.Replace("set_", ""));
		}

		return member.RTGetAttribute<ObsoleteAttribute>(true) != null;
	}

	public static bool IsReadOnly(this FieldInfo field) {
		return field.IsInitOnly || field.IsLiteral;
	}

	public static PropertyInfo GetBaseDefinition(this PropertyInfo propertyInfo) {
		var accessor = propertyInfo.GetAccessors(true)[0];
		if (accessor == null)
			return null;
		var baseDefinition = accessor.GetBaseDefinition();
		if (baseDefinition == accessor)
			return propertyInfo;
		var array = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();
		return baseDefinition.DeclaringType.GetProperty(propertyInfo.Name,
			BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
			BindingFlags.FlattenHierarchy, null, propertyInfo.PropertyType, array, null);
	}

	public static FieldInfo GetBaseDefinition(this FieldInfo fieldInfo) {
		return fieldInfo.DeclaringType.RTGetField(fieldInfo.Name);
	}

	public static Type GetEnumerableElementType(this Type enumType) {
		if (enumType == null || !typeof(IEnumerable).IsAssignableFrom(enumType))
			return null;
		if (enumType.RTIsArray())
			return enumType.GetElementType();
		foreach (var type in enumType.GetInterfaces())
			if (type.RTIsGenericType() && !(type.GetGenericTypeDefinition() != typeof(IEnumerable<>)))
				return type.RTGetGenericArguments()[0];
		return null;
	}
}