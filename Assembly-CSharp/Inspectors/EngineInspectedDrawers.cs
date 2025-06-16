using System;
using System.Collections.Generic;
using System.Linq;
using AssetDatabases;
using Cofe.Meta;
using Engine.Behaviours;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Types;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Scripts.Expressions.Commons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Inspectors;

[Initialisable]
public static class EngineInspectedDrawers {
	[Initialise]
	private static void Initialise() {
		InspectedDrawerService.Add<Pair<TimeSpan, TimeSpan>>(
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var displayName = name;
				var fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer);
				if (fold.Expand) {
					var data = (Pair<TimeSpan, TimeSpan>)value;
					context.DrawInspected("Min", typeof(TimeSpan), data.Item1, mutable, target, member, value2 => {
						data.Item1 = (TimeSpan)value2;
						value = data;
						var action = setter;
						if (action == null)
							return;
						action(value);
					});
					context.DrawInspected("Max", typeof(TimeSpan), data.Item2, mutable, target, member, value2 => {
						data.Item2 = (TimeSpan)value2;
						value = data;
						var action = setter;
						if (action == null)
							return;
						action(value);
					});
				}

				InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
			});
		InspectedDrawerService.Add(typeof(SceneGameObject),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var id = (Guid)type.GetProperty("Id").GetValue(value, null);
				GameObject gameObject1 = null;
				var gameObject2 = context.ContextObject as GameObject;
				if (gameObject2 == null) {
					var contextObject = context.ContextObject as Component;
					if (contextObject != null)
						gameObject2 = contextObject.gameObject;
				}

				if (gameObject2 != null) {
					var container = SceneObjectContainer.GetContainer(gameObject2.scene);
					if (container != null)
						gameObject1 = container.GetGameObject(id);
				}

				var gameObject3 = drawer.ObjectField(name, gameObject1, typeof(GameObject)) as GameObject;
			});
		InspectedDrawerService.Add(typeof(UnitySubAsset<>),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var genericArgument = type.GetGenericArguments()[0];
				var @object = (Object)type.GetProperty("Value").GetValue(value, null);
				drawer.ObjectField(name, @object, genericArgument);
			});
		InspectedDrawerService.Add(typeof(UnityAsset<>),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var genericArgument = type.GetGenericArguments()[0];
				var @object = (Object)type.GetProperty("Value").GetValue(value, null);
				drawer.ObjectField(name, @object, genericArgument);
			});
		InspectedDrawerService.Add(typeof(Typed<>),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var genericArgument = type.GetGenericArguments()[0];
				var object1 = (IObject)type.GetProperty("Value").GetValue(value, null);
				Object object2 = null;
				if (object1 != null)
					object2 = AssetDatabaseService.Instance.Load<Object>(
						AssetDatabaseService.Instance.GetPath(object1.Id));
				drawer.ObjectField(name, object2, typeof(Object));
			});
		InspectedDrawerService.Add(typeof(Position),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var displayName = name;
				var fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer);
				if (fold.Expand) {
					var data = (Position)value;
					context.DrawInspected("X", typeof(float), data.X, mutable, target, member, value2 => {
						data.X = (float)value2;
						value = data;
						var action = setter;
						if (action == null)
							return;
						action(value);
					});
					context.DrawInspected("Y", typeof(float), data.Y, mutable, target, member, value2 => {
						data.Y = (float)value2;
						value = data;
						var action = setter;
						if (action == null)
							return;
						action(value);
					});
				}

				InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
			});
		InspectedDrawerService.Add(typeof(ExpressionViewWrapper),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				if (!(value is ExpressionViewWrapper expressionViewWrapper2))
					return;
				var str = expressionViewWrapper2.Value;
				drawer.TextField("", str);
			});
		InspectedDrawerService.Add(typeof(ComponentCollection),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var str = name;
				var componentCollection = (ComponentCollection)value;
				var components = (List<IComponent>)componentCollection.Components;
				Action contextMenu = null;
				string displayName;
				if (value != null) {
					displayName = str + " [" + components.Count + "]";
					if (mutable)
						contextMenu = (Action)(() => {
							var menu = drawer.CreateMenu();
							GenerateCopyPasteMenu(componentCollection, menu, value2 => {
								var action = setter;
								if (action == null)
									return;
								action(value);
							});
							menu.AddSeparator("");
							GenerateAddMenu(componentCollection, menu, value2 => {
								var action = setter;
								if (action == null)
									return;
								action(value);
							});
							menu.Show();
						});
				} else
					displayName = str + " [null]";

				var fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer, contextMenu);
				if (fold.Expand && componentCollection != null) {
					var elementType = InspectedDrawerService.GetElementType(type);
					for (var index = 0; index < components.Count; index++) {
						var level = drawer.BeginBox();
						var item = components[index];
						var name1 = "Element " + index;
						context.ContextIndex = index;
						context.ContextItemMenu = (Action)(() => {
							var menu = drawer.CreateMenu();
							GenerateItemMenu(componentCollection, index, item, menu, value2 => {
								var action = setter;
								if (action == null)
									return;
								action(value);
							});
							menu.AddSeparator("");
							GenerateCopyPasteItemMenu(item, menu, value2 => {
								var action = setter;
								if (action == null)
									return;
								action(value);
							});
							menu.Show();
						});
						context.DrawInspected(name1, elementType, item, mutable, target, member, value2 => {
							var action = setter;
							if (action == null)
								return;
							action(value);
						});
						drawer.EndBox(level);
					}
				}

				InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
			});
	}

	private static void GenerateAddMenu(
		ComponentCollection componentCollection,
		IContextMenu menu,
		Action<object> setter) {
		var list = componentCollection.Components.Select(o => o.GetType()).ToList();
		foreach (var type in DerivedTypeService.GetDerivedTypes(typeof(IComponent)).Except(list).ToList()) {
			var derivedType = type;
			menu.AddItem("Add/" + GetComponentName(derivedType), false, (Action)(() => {
				componentCollection.Add(derivedType);
				var action = setter;
				if (action == null)
					return;
				action(componentCollection);
			}));
		}
	}

	private static void GenerateCopyPasteMenu(
		ComponentCollection componentCollection,
		IContextMenu menu,
		Action<object> setter) {
		Action action1 = () => InspectedDrawerService.CopyPasteObject = ((ICloneable)componentCollection).Clone();
		menu.AddItem("Copy", false, action1);
		Action action2 = null;
		var copyCollection = InspectedDrawerService.CopyPasteObject as ComponentCollection;
		if (copyCollection != null)
			action2 = (Action)(() => {
				componentCollection.Clear();
				foreach (object component in copyCollection.Components)
					componentCollection.Add(component.GetType());
				foreach (var component in componentCollection.Components) {
					var item = component;
					((ICopyable)copyCollection.Components.FirstOrDefault(o => o.GetType() == item.GetType()))
						?.CopyTo(item);
				}

				var action3 = setter;
				if (action3 == null)
					return;
				action3(componentCollection);
			});
		var copyComponent = InspectedDrawerService.CopyPasteObject as IComponent;
		if (copyComponent != null)
			action2 = (Action)(() => {
				var target = componentCollection.Add(copyComponent.GetType());
				if (target != null)
					((ICopyable)copyComponent).CopyTo(target);
				var action4 = setter;
				if (action4 == null)
					return;
				action4(componentCollection);
			});
		menu.AddItem("Paste", false, action2);
	}

	private static void GenerateItemMenu(
		ComponentCollection componentCollection,
		int index,
		IComponent item,
		IContextMenu menu,
		Action<object> setter) {
		var components = (List<IComponent>)componentCollection.Components;
		var contextIndex = index;
		Action action1 = null;
		if (contextIndex != 0)
			action1 = (Action)(() => {
				var component = components[contextIndex - 1];
				components[contextIndex - 1] = components[contextIndex];
				components[contextIndex] = component;
				var action2 = setter;
				if (action2 == null)
					return;
				action2(componentCollection);
			});
		menu.AddItem("Up", false, action1);
		Action action3 = null;
		if (contextIndex != components.Count - 1)
			action3 = (Action)(() => {
				var component = components[contextIndex + 1];
				components[contextIndex + 1] = components[contextIndex];
				components[contextIndex] = component;
				var action4 = setter;
				if (action4 == null)
					return;
				action4(componentCollection);
			});
		menu.AddItem("Down", false, action3);
		menu.AddSeparator("");
		menu.AddItem("Remove", false, (Action)(() => {
			componentCollection.Remove(item.GetType());
			var action5 = setter;
			if (action5 == null)
				return;
			action5(componentCollection);
		}));
	}

	private static void GenerateCopyPasteItemMenu(
		IComponent item,
		IContextMenu menu,
		Action<object> setter) {
		Action action1 = null;
		menu.AddItem("Copy", false, () => InspectedDrawerService.CopyPasteObject = ((ICloneable)item).Clone());
		var copyComponent = InspectedDrawerService.CopyPasteObject as IComponent;
		if (copyComponent != null && copyComponent.GetType() == item.GetType())
			action1 = (Action)(() => {
				((ICopyable)copyComponent).CopyTo(item);
				var action2 = setter;
				if (action2 == null)
					return;
				action2(item);
			});
		menu.AddItem("Paste", false, action1);
	}

	private static string GetComponentName(Type type) {
		var componentName = type.Name;
		var list = type.GetInterfaces().Where(o => o != typeof(IComponent) && typeof(IComponent).IsAssignableFrom(o))
			.ToList();
		if (list.Count > 0) {
			var str = componentName + " (";
			for (var index = 0; index < list.Count; ++index) {
				var type1 = list[index];
				str += type1.Name;
				if (index != list.Count - 1)
					str += ", ";
			}

			componentName = str + ")";
		}

		return componentName;
	}
}