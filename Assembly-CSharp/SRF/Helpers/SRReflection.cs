using System;
using System.Reflection;

namespace SRF.Helpers;

public static class SRReflection {
	public static void SetPropertyValue(object obj, PropertyInfo p, object value) {
		p.GetSetMethod().Invoke(obj, new object[1] { value });
	}

	public static object GetPropertyValue(object obj, PropertyInfo p) {
		return p.GetGetMethod().Invoke(obj, null);
	}

	public static T GetAttribute<T>(MemberInfo t) where T : Attribute {
		return Attribute.GetCustomAttribute(t, typeof(T)) as T;
	}
}