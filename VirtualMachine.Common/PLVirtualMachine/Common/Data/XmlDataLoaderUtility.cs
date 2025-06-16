using Cofe.Utility;
using Engine.Common.Types;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PLVirtualMachine.Common.Data
{
  public static class XmlDataLoaderUtility
  {
    private static Dictionary<EDataType, Type> objectIdToType = new Dictionary<EDataType, Type>();

    static XmlDataLoaderUtility() => XmlDataLoaderUtility.Init();

    public static Type GetObjTypeById(ulong id)
    {
      ushort typeId = GuidUtility.GetTypeId(id);
      Type objTypeById;
      XmlDataLoaderUtility.objectIdToType.TryGetValue((EDataType) typeId, out objTypeById);
      return objTypeById;
    }

    private static void Init() => XmlDataLoaderUtility.ComputeTypes();

    private static void ComputeTypes()
    {
      AssemblyUtility.ComputeAssemblies(typeof (TypeDataAttribute).Assembly, (Action<Assembly>) (assembly =>
      {
        foreach (Type type in assembly.GetTypes())
        {
          if (type.IsClass && type.IsDefined(typeof (TypeDataAttribute), false))
          {
            TypeDataAttribute customAttribute = (TypeDataAttribute) type.GetCustomAttributes(typeof (TypeDataAttribute), false)[0];
            XmlDataLoaderUtility.objectIdToType.Add(customAttribute.DataType, type);
          }
        }
      }));
    }
  }
}
