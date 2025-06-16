using Cofe.Meta;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
  public class VMTypeAttribute : TypeAttribute
  {
    public readonly string TypeName;
    private static Dictionary<string, Type> types = new Dictionary<string, Type>();
    private static Dictionary<Type, string> names = new Dictionary<Type, string>();

    public VMTypeAttribute(string typeName) => this.TypeName = typeName;

    public override void ComputeType(Type type)
    {
      VMTypeAttribute.RegisterVMBaseType(this.TypeName, type);
    }

    private static void RegisterVMBaseType(string baseTypeName, Type vmType)
    {
      if (!VMTypeAttribute.types.ContainsKey(baseTypeName))
        VMTypeAttribute.types.Add(baseTypeName, vmType);
      else if (vmType.IsAssignableFrom(VMTypeAttribute.types[baseTypeName]))
        VMTypeAttribute.types[baseTypeName] = vmType;
      VMTypeAttribute.names.Add(vmType, baseTypeName);
    }

    public static bool TryGetValue(string name, out Type result)
    {
      return VMTypeAttribute.types.TryGetValue(name, out result);
    }

    public static bool TryGetValue(Type type, out string result)
    {
      return VMTypeAttribute.names.TryGetValue(type, out result);
    }
  }
}
