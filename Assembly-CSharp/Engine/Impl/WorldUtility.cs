﻿using System;
using Cofe.Utility;
using Engine.Common;

namespace Engine.Impl
{
  public static class WorldUtility
  {
    public static IVirtualMachine CreateVirtualMachine()
    {
      Type virtualMachineType = GetVirtualMachineType();
      return virtualMachineType != null ? (IVirtualMachine) Activator.CreateInstance(virtualMachineType) : null;
    }

    private static Type GetVirtualMachineType()
    {
      Type result = null;
      Type vmType = typeof (IVirtualMachine);
      AssemblyUtility.ComputeAssemblies(vmType.Assembly, assembly =>
      {
        if (result != null)
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
      });
      return result;
    }
  }
}
