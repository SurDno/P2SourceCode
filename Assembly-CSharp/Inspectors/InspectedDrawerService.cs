using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Inspectors;

public static class InspectedDrawerService {
	private static Dictionary<Type, DrawerHandle> drawers = new();
	private static List<KeyValuePair<Func<Type, bool>, DrawerHandle>> conditionalDrawer = new();
	private static Dictionary<Type, Type> elementTypes = new();

	public static object CopyPasteObject { get; set; }

	public static void Add(Type type, DrawerHandle action) {
		if (!drawers.ContainsKey(type))
			drawers.Add(type,
				(name, type2, value, mutable, context, drawer, target, member, setter) => action(name, type2, value,
					mutable, context, drawer, target, member, setter));
		else
			Debug.LogError("Drawer type already exist : " + type);
	}

	public static void Add<T>(DrawerHandle action) {
		Add(typeof(T), action);
	}

	public static void AddConditional(
		Func<Type, bool> condition,
		DrawerHandle action) {
		conditionalDrawer.Add(new KeyValuePair<Func<Type, bool>, DrawerHandle>(condition, action));
	}

	public static Type GetElementType(Type type) {
		Type elementType;
		if (elementTypes.TryGetValue(type, out elementType))
			return elementType;
		if (type.HasElementType)
			return type.GetElementType();
		var genericArguments = type.GetGenericArguments();
		if (genericArguments.Length == 1)
			return genericArguments[0];
		return genericArguments.Length == 2 && type.IsGenericType &&
		       (typeof(Dictionary<,>) == type.GetGenericTypeDefinition() ||
		        typeof(IDictionary<,>) == type.GetGenericTypeDefinition())
			? typeof(KeyValuePair<,>).MakeGenericType(genericArguments)
			: typeof(object);
	}

	public static DrawerHandle GetDrawer(Type type) {
		DrawerHandle drawer;
		drawers.TryGetValue(type, out drawer);
		if (drawer != null)
			return drawer;
		foreach (var keyValuePair in conditionalDrawer)
			if (keyValuePair.Key(type))
				return keyValuePair.Value;
		return null;
	}

	public static void AddElementType(Type container, Type element) {
		elementTypes.Add(container, element);
	}

	public delegate void DrawerHandle(
		string name,
		Type type,
		object value,
		bool mutable,
		IInspectedProvider context,
		IInspectedDrawer drawer,
		object target,
		MemberInfo member,
		Action<object> setter);
}