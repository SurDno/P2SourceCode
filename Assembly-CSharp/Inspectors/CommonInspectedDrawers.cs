using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Cofe.Meta;
using Cofe.Proxies;
using Cofe.Utility;
using Engine.Assets.Internal;

namespace Inspectors;

[Initialisable]
public static class CommonInspectedDrawers {
	private static void GenerateItemMenu(
		IList contextList,
		int contextIndex,
		IContextMenu menu,
		Action<object> setter) {
		Action action1 = null;
		if (contextIndex != 0)
			action1 = (Action)(() => {
				var context = contextList[contextIndex - 1];
				contextList[contextIndex - 1] = contextList[contextIndex];
				contextList[contextIndex] = context;
				var action2 = setter;
				if (action2 == null)
					return;
				action2(context);
			});
		menu.AddItem("Up", false, action1);
		Action action3 = null;
		if (contextIndex != contextList.Count - 1)
			action3 = (Action)(() => {
				var context = contextList[contextIndex + 1];
				contextList[contextIndex + 1] = contextList[contextIndex];
				contextList[contextIndex] = context;
				var action4 = setter;
				if (action4 == null)
					return;
				action4(context);
			});
		menu.AddItem("Down", false, action3);
		menu.AddSeparator("");
		menu.AddItem("Remove", false, (Action)(() => {
			contextList.RemoveAt(contextIndex);
			var action5 = setter;
			if (action5 == null)
				return;
			action5(null);
		}));
	}

	private static void GenerateClassMenu(
		object contextObject,
		int contextIndex,
		Type baseType,
		Type type,
		object value,
		object target,
		MemberInfo member,
		IContextMenu menu,
		Action<object> setter) {
		Action action1 = null;
		var cloneable = value as ICloneable;
		if (cloneable != null)
			action1 = () => InspectedDrawerService.CopyPasteObject = cloneable.Clone();
		menu.AddItem("Copy", false, action1);
		Action action2 = null;
		var copyObject = InspectedDrawerService.CopyPasteObject;
		if (copyObject != null && TypeUtility.IsAssignableFrom(baseType, copyObject.GetType()))
			action2 = (Action)(() => {
				var obj = ((ICloneable)copyObject).Clone();
				var action3 = setter;
				if (action3 == null)
					return;
				action3(obj);
			});
		menu.AddItem("Paste", false, action2);
		menu.AddSeparator("");
		var action4 = (Action)(() => {
			object obj = null;
			var action5 = setter;
			if (action5 == null)
				return;
			action5(obj);
		});
		menu.AddItem("null", value == null, action4);
		foreach (var derivedType1 in DerivedTypeService.GetDerivedTypes(baseType)) {
			var derivedType = derivedType1;
			var action6 = (Action)(() => {
				var obj = ProxyFactory.Create(derivedType);
				var action7 = setter;
				if (action7 == null)
					return;
				action7(obj);
			});
			var type1 = ProxyFactory.GetType(type);
			var on = value != null && type1 == derivedType;
			menu.AddItem(TypeNameService.GetTypeName(derivedType, true), on, action6);
		}
	}

	[Initialise]
	private static void Initialise() {
		InspectedDrawerService.Add<Type>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var type1 = (Type)value;
			drawer.TextField(name, TypeUtility.GetTypeFullName(type1, false));
		});
		InspectedDrawerService.Add<int>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var num1 = (int)value;
			var num2 = drawer.IntField(name, num1);
			if (!mutable || num1 == num2)
				return;
			value = num2;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<uint>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var str = value.ToString();
			drawer.TextField(name, str);
		});
		InspectedDrawerService.Add<long>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var num3 = (long)value;
			var num4 = drawer.LongField(name, num3);
			if (!mutable || num3 == num4)
				return;
			value = num4;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<ulong>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var str = value.ToString();
			drawer.TextField(name, str);
		});
		InspectedDrawerService.Add<float>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var num5 = (float)value;
			var customAttribute = member.GetCustomAttribute<SliderAttribute>();
			var num6 = customAttribute == null
				? drawer.FloatField(name, num5)
				: drawer.SliderField(name, num5, customAttribute.Min, customAttribute.Max);
			if (!mutable || num5 == (double)num6)
				return;
			value = num6;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<double>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var num7 = (double)value;
			var num8 = drawer.DoubleField(name, num7);
			if (!mutable || num7 == num8)
				return;
			value = num8;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<bool>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var flag1 = (bool)value;
			var flag2 = drawer.BoolField(name, flag1);
			if (!mutable || flag1 == flag2)
				return;
			value = flag2;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<string>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var str1 = (string)value;
			var str2 = member.GetCustomAttribute<MultiAttribute>() == null
				? drawer.TextField(name, str1)
				: drawer.MultiTextField(name, str1);
			if (!mutable || !(str1 != str2))
				return;
			value = str2;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<Guid>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var str = value.ToString();
			drawer.TextField(name, str);
		});
		InspectedDrawerService.Add<DateTime>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var str = ((DateTime)value).ToString(CultureInfo.InvariantCulture);
			drawer.TextField(name, str);
		});
		InspectedDrawerService.Add<TimeSpan>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var totalSeconds = ((TimeSpan)value).TotalSeconds;
			var num = drawer.DoubleField(name, totalSeconds);
			if (!mutable || num == totalSeconds)
				return;
			value = TimeSpan.FromSeconds(num);
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add(typeof(KeyValuePair<,>),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var str = name;
				var genericArgument1 = type.GetGenericArguments()[0];
				var obj1 = type.GetProperty("Key").GetValue(value, null);
				var displayName = str + " (" + obj1 + ")";
				var fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer);
				if (fold.Expand) {
					context.DrawInspected("Key", genericArgument1, obj1, mutable, target, member, value2 => {
						var action = setter;
						if (action == null)
							return;
						action(value);
					});
					var genericArgument2 = type.GetGenericArguments()[1];
					var obj2 = type.GetProperty("Value").GetValue(value, null);
					context.DrawInspected("Value", genericArgument2, obj2, mutable, target, member, value2 => {
						var action = setter;
						if (action == null)
							return;
						action(value);
					});
				}

				InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
			});
		InspectedDrawerService.AddConditional(type => type.IsEnum,
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var enum1 = (Enum)value;
				var enum2 = member.GetCustomAttribute<MaskAttribute>() == null
					? member.GetCustomAttribute<SortedAttribute>() == null
						? drawer.EnumPopup(name, enum1, context)
						: drawer.EnumSortedPopup(name, enum1, context)
					: drawer.EnumMaskPopup(name, enum1, context);
				if (!mutable || enum1.Equals(enum2))
					return;
				value = enum2;
				if (setter != null)
					setter(value);
			});
		InspectedDrawerService.AddConditional(type => typeof(IList).IsAssignableFrom(type),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var str = name;
				var list = (IList)value;
				var baseType = type;
				var contextIndex = context.ContextIndex;
				var contextObject = context.ContextObject;
				var contextItemMenu = context.ContextItemMenu;
				context.ContextIndex = -1;
				context.ContextItemMenu = null;
				Action contextMenu = null;
				string displayName;
				if (value != null)
					displayName = str + " [" + list.Count + "]";
				else
					displayName = str + " [null]";
				if (mutable)
					contextMenu = (Action)(() => {
						var elementType = InspectedDrawerService.GetElementType(type);
						var menu = drawer.CreateMenu();
						if (value != null) {
							var action9 = (Action)(() => {
								list.Add(TypeDefaultUtility.GetDefault(elementType));
								var action2 = setter;
								if (action2 == null)
									return;
								action2(list);
							});
							menu.AddItem("Add", false, action9);
							menu.AddSeparator("");
							Action action10 = null;
							if (list.Count > 0)
								action10 = (Action)(() => {
									list.RemoveAt(0);
									var action4 = setter;
									if (action4 == null)
										return;
									action4(list);
								});
							menu.AddItem("Remove First", false, action10);
							Action action11 = null;
							if (list.Count > 0)
								action11 = (Action)(() => {
									list.RemoveAt(list.Count - 1);
									var action6 = setter;
									if (action6 == null)
										return;
									action6(list);
								});
							menu.AddItem("Remove Last", false, action11);
							Action action12 = null;
							if (list.Count > 0)
								action12 = (Action)(() => {
									list.Clear();
									var action8 = setter;
									if (action8 == null)
										return;
									action8(list);
								});
							menu.AddItem("Clear", false, action12);
						}

						GenerateClassMenu(contextObject, contextIndex, baseType, type, value, target, member, menu,
							value2 => {
								var action = setter;
								if (action == null)
									return;
								action(value2);
							});
						menu.Show();
					});
				var fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer, contextMenu);
				if (fold.Expand && list != null) {
					var elementType = InspectedDrawerService.GetElementType(type);
					for (var index = 0; index < list.Count; ++index) {
						var obj = list[index];
						var level = drawer.BeginBox();
						var name1 = "Element " + index;
						context.ContextIndex = index;
						var captureIndex = index;
						context.DrawInspected(name1, elementType, obj, mutable, target, member, value2 => {
							if (captureIndex < list.Count)
								list[captureIndex] = value2;
							var action = setter;
							if (action == null)
								return;
							action(list);
						});
						drawer.EndBox(level);
					}
				}

				InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
			});
		InspectedDrawerService.AddConditional(type => typeof(IEnumerable).IsAssignableFrom(type),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var str = name;
				var data = (IEnumerable)value;
				var contextIndex = context.ContextIndex;
				var contextObject = context.ContextObject;
				var contextItemMenu = context.ContextItemMenu;
				context.ContextIndex = -1;
				context.ContextItemMenu = null;
				Action contextMenu = null;
				string displayName;
				if (data != null) {
					var num = 0;
					if (data is ICollection collection2)
						num = collection2.Count;
					else
						foreach (var obj in data)
							++num;
					displayName = str + " [" + num + "]";
				} else
					displayName = str + " [null]";

				var fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer, contextMenu);
				if (fold.Expand && data != null) {
					var elementType = InspectedDrawerService.GetElementType(type);
					var num = 0;
					foreach (var obj in data) {
						var level = drawer.BeginBox();
						var name2 = "Element " + num;
						context.ContextIndex = num;
						context.DrawInspected(name2, elementType, obj, mutable, target, member, value2 => {
							var action = setter;
							if (action == null)
								return;
							action(data);
						});
						drawer.EndBox(level);
						++num;
					}
				}

				InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
			});
		InspectedDrawerService.Add<object>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var baseType = type;
			context.ElementName = "";
			if (value != null) {
				type = value.GetType();
				MetaService.GetContainer(type).GetHandler(context.NameId).Compute(value, context);
			} else
				context.ElementName = "null";

			var contextIndex = context.ContextIndex;
			var contextObject = context.ContextObject;
			var contextItemMenu = context.ContextItemMenu;
			context.ContextIndex = -1;
			context.ContextItemMenu = null;
			Action contextMenu = null;
			if (mutable)
				contextMenu = contextItemMenu == null
					? (Action)(() => {
						var menu3 = drawer.CreateMenu();
						if (member.GetValue(target) is IList contextList2) {
							var contextIndex1 = contextIndex;
							var menu4 = menu3;
							GenerateItemMenu(contextList2, contextIndex1, menu4, value2 => {
								var action = setter;
								if (action == null)
									return;
								action(value2);
							});
							menu3.AddSeparator("");
						}

						GenerateClassMenu(contextObject, contextIndex, baseType, type, value, target, member, menu3,
							value2 => {
								var action = setter;
								if (action == null)
									return;
								action(value2);
							});
						menu3.Show();
					})
					: contextItemMenu;
			var str = name;
			string displayName;
			if (string.IsNullOrEmpty(context.ElementName))
				displayName = str + " (" + (value != null ? TypeNameService.GetTypeName(value.GetType()) : "") + ")";
			else
				displayName = str + " (" + (value != null ? TypeNameService.GetTypeName(value.GetType()) + " : " : "") +
				              context.ElementName + ")";
			var fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer, contextMenu);
			if (fold.Expand && value != null)
				MetaService.GetContainer(type).GetHandler(context.DrawId).Compute(value, new InspectedContext {
					Provider = context,
					Setter = value2 => {
						var action = setter;
						if (action == null)
							return;
						action(value);
					}
				});
			InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
		});
	}
}