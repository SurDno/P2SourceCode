// Decompiled with JetBrains decompiler
// Type: Engine.Impl.WorldUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common;
using System;
using System.Reflection;

#nullable disable
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
