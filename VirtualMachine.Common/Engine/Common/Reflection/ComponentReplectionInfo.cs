using System;
using System.Collections.Generic;
using System.Reflection;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace Engine.Common.Reflection;

public class ComponentReplectionInfo {
	private string dependedComponentName;
	private string name;
	private Type type;
	private Dictionary<string, EventInfo> events = new();
	private Dictionary<string, MethodInfo> methods = new();
	private Dictionary<string, PropertyInfo> properties = new();

	public Type Type => type;

	public string Name => name;

	public string DependedComponentName => dependedComponentName;

	public IDictionary<string, MethodInfo> Methods => methods;

	public IDictionary<string, EventInfo> Events => events;

	public IDictionary<string, PropertyInfo> Properties => properties;

	public ComponentReplectionInfo(Type type, string name, string dependedComponent = "") {
		this.type = type;
		this.name = name;
		dependedComponentName = dependedComponent;
		foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
		                                       BindingFlags.FlattenHierarchy))
			if (method.GetCustomAttributes(typeof(MethodAttribute), true).Length != 0)
				methods.Add(method.Name, method);
		foreach (var eventInfo in type.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
		                                         BindingFlags.FlattenHierarchy))
			if (eventInfo.GetCustomAttributes(typeof(EventAttribute), true).Length != 0)
				events.Add(eventInfo.Name, eventInfo);
		foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public |
		                                            BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
			if (property.GetCustomAttributes(typeof(PropertyAttribute), true).Length != 0)
				properties.Add(property.Name, property);
	}
}