// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes.InfoAttribute
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Cofe.Meta;
using Engine.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class InfoAttribute : TypeAttribute
  {
    public string ApiName;
    private static Dictionary<string, ComponentReplectionInfo> components = new Dictionary<string, ComponentReplectionInfo>();
    private static Dictionary<Type, string> names = new Dictionary<Type, string>();

    public Type EngineComponentType { get; set; }

    public InfoAttribute(string apiName, Type engineComponentType = null)
    {
      this.ApiName = apiName;
      this.EngineComponentType = engineComponentType;
    }

    public override void ComputeType(Type type)
    {
      string apiName = this.ApiName;
      if (this.EngineComponentType != (Type) null)
        InfoAttribute.names.Add(this.EngineComponentType, apiName);
      string dependedComponent = "";
      object[] customAttributes = type.GetCustomAttributes(typeof (DependedAttribute), false);
      if (customAttributes.Length != 0)
        dependedComponent = ((DependedAttribute) customAttributes[0]).Name;
      InfoAttribute.components.Add(apiName, new ComponentReplectionInfo(type, apiName, dependedComponent));
    }

    public static MethodInfo GetComponentMethodInfo(string componentName, string methodName)
    {
      ComponentReplectionInfo componentReplectionInfo;
      if (!InfoAttribute.components.TryGetValue(componentName, out componentReplectionInfo))
      {
        Logger.AddError("Component with name " + componentName + " not found");
        return (MethodInfo) null;
      }
      MethodInfo componentMethodInfo;
      if (componentReplectionInfo.Methods.TryGetValue(methodName, out componentMethodInfo))
        return componentMethodInfo;
      Logger.AddError("Component with name " + componentName + " hasn't method with name " + methodName);
      return (MethodInfo) null;
    }

    public static PropertyInfo GetComponentPropertyInfo(string componentName, string propertyName)
    {
      ComponentReplectionInfo componentReplectionInfo;
      if (!InfoAttribute.components.TryGetValue(componentName, out componentReplectionInfo))
      {
        Logger.AddError("Component with name " + componentName + " not found");
        return (PropertyInfo) null;
      }
      PropertyInfo componentPropertyInfo;
      if (componentReplectionInfo.Properties.TryGetValue(propertyName, out componentPropertyInfo))
        return componentPropertyInfo;
      Logger.AddError("Component with name " + componentName + " hasn't property with name " + propertyName);
      return (PropertyInfo) null;
    }

    public static ComponentReplectionInfo GetComponentInfo(string name)
    {
      ComponentReplectionInfo componentInfo;
      InfoAttribute.components.TryGetValue(name, out componentInfo);
      return componentInfo;
    }

    public static IEnumerable<ComponentReplectionInfo> Components
    {
      get => (IEnumerable<ComponentReplectionInfo>) InfoAttribute.components.Values;
    }

    public static bool TryGetValue(string name, out ComponentReplectionInfo result)
    {
      return InfoAttribute.components.TryGetValue(name, out result);
    }

    public static string GetComponentName(Type type)
    {
      string componentName;
      InfoAttribute.names.TryGetValue(type, out componentName);
      return componentName;
    }
  }
}
