using System;
using System.Collections.Generic;
using System.Reflection;
using Cofe.Loggers;
using Cofe.Meta;
using Engine.Common.Reflection;

namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class InfoAttribute(string apiName, Type engineComponentType = null) : TypeAttribute 
  {
    public string ApiName = apiName;
    private static Dictionary<string, ComponentReplectionInfo> components = new();
    private static Dictionary<Type, string> names = new();

    public Type EngineComponentType { get; set; } = engineComponentType;

    public override void ComputeType(Type type)
    {
      string apiName = ApiName;
      if (EngineComponentType != null)
        names.Add(EngineComponentType, apiName);
      string dependedComponent = "";
      object[] customAttributes = type.GetCustomAttributes(typeof (DependedAttribute), false);
      if (customAttributes.Length != 0)
        dependedComponent = ((DependedAttribute) customAttributes[0]).Name;
      components.Add(apiName, new ComponentReplectionInfo(type, apiName, dependedComponent));
    }

    public static MethodInfo GetComponentMethodInfo(string componentName, string methodName)
    {
      if (!components.TryGetValue(componentName, out ComponentReplectionInfo componentReplectionInfo))
      {
        Logger.AddError("Component with name " + componentName + " not found");
        return null;
      }

      if (componentReplectionInfo.Methods.TryGetValue(methodName, out MethodInfo componentMethodInfo))
        return componentMethodInfo;
      Logger.AddError("Component with name " + componentName + " hasn't method with name " + methodName);
      return null;
    }

    public static PropertyInfo GetComponentPropertyInfo(string componentName, string propertyName)
    {
      if (!components.TryGetValue(componentName, out ComponentReplectionInfo componentReplectionInfo))
      {
        Logger.AddError("Component with name " + componentName + " not found");
        return null;
      }

      if (componentReplectionInfo.Properties.TryGetValue(propertyName, out PropertyInfo componentPropertyInfo))
        return componentPropertyInfo;
      Logger.AddError("Component with name " + componentName + " hasn't property with name " + propertyName);
      return null;
    }

    public static ComponentReplectionInfo GetComponentInfo(string name)
    {
      components.TryGetValue(name, out ComponentReplectionInfo componentInfo);
      return componentInfo;
    }

    public static IEnumerable<ComponentReplectionInfo> Components => components.Values;

    public static bool TryGetValue(string name, out ComponentReplectionInfo result)
    {
      return components.TryGetValue(name, out result);
    }

    public static string GetComponentName(Type type)
    {
      names.TryGetValue(type, out string componentName);
      return componentName;
    }
  }
}
