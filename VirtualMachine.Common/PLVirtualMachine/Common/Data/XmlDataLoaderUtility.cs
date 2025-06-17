using System;
using System.Collections.Generic;
using Cofe.Utility;
using Engine.Common.Types;

namespace PLVirtualMachine.Common.Data
{
  public static class XmlDataLoaderUtility
  {
    private static Dictionary<EDataType, Type> objectIdToType = new();

    static XmlDataLoaderUtility() => Init();

    public static Type GetObjTypeById(ulong id)
    {
      ushort typeId = GuidUtility.GetTypeId(id);
      objectIdToType.TryGetValue((EDataType) typeId, out Type objTypeById);
      return objTypeById;
    }

    private static void Init() => ComputeTypes();

    private static void ComputeTypes()
    {
      AssemblyUtility.ComputeAssemblies(typeof (TypeDataAttribute).Assembly, assembly =>
      {
        foreach (Type type in assembly.GetTypes())
        {
          if (type.IsClass && type.IsDefined(typeof (TypeDataAttribute), false))
          {
            TypeDataAttribute customAttribute = (TypeDataAttribute) type.GetCustomAttributes(typeof (TypeDataAttribute), false)[0];
            objectIdToType.Add(customAttribute.DataType, type);
          }
        }
      });
    }
  }
}
