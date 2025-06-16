using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Cinemachine.Utility;

[DocumentationSorting(0.0f, DocumentationSortingAttribute.Level.Undoc)]
public static class ReflectionHelpers {
	public static void CopyFields(object src, object dst,
		BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) {
		if (src == null || dst == null)
			return;
		var fields = src.GetType().GetFields(bindingAttr);
		for (var index = 0; index < fields.Length; ++index)
			if (!fields[index].IsStatic)
				fields[index].SetValue(dst, fields[index].GetValue(src));
	}

	public static T AccessInternalField<T>(this Type type, object obj, string memberName) {
		if (string.IsNullOrEmpty(memberName) || type == null)
			return default;
		var bindingFlags = BindingFlags.NonPublic;
		var bindingAttr = obj == null ? bindingFlags | BindingFlags.Static : bindingFlags | BindingFlags.Instance;
		var field = type.GetField(memberName, bindingAttr);
		return field != null && field.FieldType == typeof(T) ? (T)field.GetValue(obj) : default;
	}

	public static object GetParentObject(string path, object obj) {
		var strArray = path.Split('.');
		if (strArray.Length == 1)
			return obj;
		obj = obj.GetType().GetField(strArray[0], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			.GetValue(obj);
		return GetParentObject(string.Join(".", strArray, 1, strArray.Length - 1), obj);
	}

	public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr) {
		if (expr.Body.NodeType != ExpressionType.MemberAccess)
			throw new InvalidOperationException();
		var memberExpression = expr.Body as MemberExpression;
		var stringList = new List<string>();
		for (; memberExpression != null; memberExpression = memberExpression.Expression as MemberExpression)
			stringList.Add(memberExpression.Member.Name);
		var stringBuilder = new StringBuilder();
		for (var index = stringList.Count - 1; index >= 0; --index) {
			stringBuilder.Append(stringList[index]);
			if (index > 0)
				stringBuilder.Append('.');
		}

		return stringBuilder.ToString();
	}
}