using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Engine.Common.Reflection
{
  public class ComponentReplectionInfo
  {
    private string dependedComponentName;
    private string name;
    private Type type;
    private Dictionary<string, EventInfo> events = new Dictionary<string, EventInfo>();
    private Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
    private Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();

    public Type Type => this.type;

    public string Name => this.name;

    public string DependedComponentName => this.dependedComponentName;

    public IDictionary<string, MethodInfo> Methods
    {
      get => (IDictionary<string, MethodInfo>) this.methods;
    }

    public IDictionary<string, EventInfo> Events => (IDictionary<string, EventInfo>) this.events;

    public IDictionary<string, PropertyInfo> Properties
    {
      get => (IDictionary<string, PropertyInfo>) this.properties;
    }

    public ComponentReplectionInfo(Type type, string name, string dependedComponent = "")
    {
      this.type = type;
      this.name = name;
      this.dependedComponentName = dependedComponent;
      foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
      {
        if (method.GetCustomAttributes(typeof (MethodAttribute), true).Length != 0)
          this.methods.Add(method.Name, method);
      }
      foreach (EventInfo eventInfo in type.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
      {
        if (eventInfo.GetCustomAttributes(typeof (EventAttribute), true).Length != 0)
          this.events.Add(eventInfo.Name, eventInfo);
      }
      foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
      {
        if (property.GetCustomAttributes(typeof (PropertyAttribute), true).Length != 0)
          this.properties.Add(property.Name, property);
      }
    }
  }
}
