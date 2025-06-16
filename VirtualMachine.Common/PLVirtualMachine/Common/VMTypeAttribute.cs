// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.VMTypeAttribute
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Meta;
using System;
using System.Collections.Generic;

#nullable disable
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
