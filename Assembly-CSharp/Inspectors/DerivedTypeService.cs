using Cofe.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Inspectors
{
  public static class DerivedTypeService
  {
    private static Dictionary<Type, Type[]> types = new Dictionary<Type, Type[]>();

    public static Type[] GetDerivedTypes(Type baseType)
    {
      Type[] array;
      if (!DerivedTypeService.types.TryGetValue(baseType, out array))
      {
        List<Type> list = new List<Type>();
        if (!baseType.IsAbstract)
          list.Add(baseType);
        if (baseType.IsClass || baseType.IsInterface)
          AssemblyUtility.ComputeAssemblies(baseType.Assembly, (Action<Assembly>) (assembly =>
          {
            foreach (Type type in assembly.GetTypes())
            {
              if (type.IsClass && !type.IsAbstract && !type.Name.EndsWith("_Generated") && TypeUtility.IsAssignableFrom(baseType, type))
                list.Add(type);
            }
          }));
        list.Sort((Comparison<Type>) ((a, b) => a.Name.CompareTo(b.Name)));
        array = list.ToArray();
        DerivedTypeService.types.Add(baseType, array);
      }
      return array;
    }
  }
}
