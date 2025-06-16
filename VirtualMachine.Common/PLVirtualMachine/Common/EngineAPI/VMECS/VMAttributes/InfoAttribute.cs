using System;
using System.Collections.Generic;
using System.Reflection;
using Cofe.Loggers;
using Cofe.Meta;
using Engine.Common.Reflection;

namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class InfoAttribute : TypeAttribute
  {
    public string ApiName;
    private static Dictionary<string, ComponentReplectionInfo> components = new Dictionary<string, ComponentReplectionInfo>();
    private static Dictionary<Type, string> names = new Dictionary<Type, string>();

    public Type EngineComponentType { get; set; }

    public InfoAttribute(string apiName, Type engineComponentType = null)
    {
      ApiName = apiName;
      EngineComponentType = engineComponentType;
    }

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
      ComponentReplectionInfo componentReplectionInfo;
      if (!components.TryGetValue(componentName, out componentReplectionInfo))
      {
        Logger.AddError("Component with name " + componentName + " not found");
        return null;
      }
      MethodInfo componentMethodInfo;
      if (componentReplectionInfo.Methods.TryGetValue(methodName, out componentMethodInfo))
        return componentMethodInfo;
      Logger.AddError("Component with name " + componentName + " hasn't method with name " + methodName);
      return null;
    }

    public static PropertyInfo GetComponentPropertyInfo(string componentName, string propertyName)
    {
      ComponentReplectionInfo componentReplectionInfo;
      if (!components.TryGetValue(componentName, out componentReplectionInfo))
      {
        Logger.AddError("Component with name " + componentName + " not found");
        return null;
      }
      PropertyInfo componentPropertyInfo;
      if (componentReplectionInfo.Properties.TryGetValue(propertyName, out componentPropertyInfo))
        return componentPropertyInfo;
      Logger.AddError("Component with name " + componentName + " hasn't property with name " + propertyName);
      return null;
    }

    public static ComponentReplectionInfo GetComponentInfo(string name)
    {
      ComponentReplectionInfo componentInfo;
      components.TryGetValue(name, out componentInfo);
      return componentInfo;
    }

    public static IEnumerable<ComponentReplectionInfo> Components
    {
      get => components.Values;
    }

    public static bool TryGetValue(string name, out ComponentReplectionInfo result)
    {
      return components.TryGetValue(name, out result);
    }

    public static string GetComponentName(Type type)
    {
      string componentName;
      names.TryGetValue(type, out componentName);
      return componentName;
    }
  }
}
