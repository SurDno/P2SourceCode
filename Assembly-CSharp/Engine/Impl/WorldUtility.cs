using Cofe.Utility;
using Engine.Common;
using System;
using System.Reflection;

namespace Engine.Impl
{
  public static class WorldUtility
  {
    public static IVirtualMachine CreateVirtualMachine()
    {
      Type virtualMachineType = WorldUtility.GetVirtualMachineType();
      return virtualMachineType != (Type) null ? (IVirtualMachine) Activator.CreateInstance(virtualMachineType) : (IVirtualMachine) null;
    }

    private static Type GetVirtualMachineType()
    {
      Type result = (Type) null;
      Type vmType = typeof (IVirtualMachine);
      AssemblyUtility.ComputeAssemblies(vmType.Assembly, (Action<Assembly>) (assembly =>
      {
        if (result != (Type) null)
          return;
        foreach (Type type1 in assembly.GetTypes())
        {
          if (type1.IsClass && !type1.IsAbstract)
          {
            foreach (Type type2 in type1.GetInterfaces())
            {
              if (type2 == vmType)
              {
                result = type1;
                return;
              }
            }
          }
        }
      }));
      return result;
    }
  }
}
