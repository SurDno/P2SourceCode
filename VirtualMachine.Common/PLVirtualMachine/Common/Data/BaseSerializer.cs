// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.Data.BaseSerializer
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Utility;
using System;
using System.Reflection;

#nullable disable
namespace PLVirtualMachine.Common.Data
{
  public static class BaseSerializer
  {
    public static object GetDefaultValue(Type valueType)
    {
      if (valueType.IsValueType)
        return TypeDefaultUtility.GetDefault(valueType);
      if (valueType == typeof (string))
        return (object) "";
      if (valueType.IsInterface)
      {
        valueType = BaseSerializer.GetRealRefType(valueType);
        if ((Type) null != valueType)
          return Activator.CreateInstance(valueType);
      }
      else
      {
        ConstructorInfo constructor = valueType.GetConstructor((Type[]) null);
        if (constructor != (ConstructorInfo) null)
          return constructor.Invoke((object[]) null);
      }
      return (object) "";
    }

    public static Type GetRealRefType(Type type)
    {
      Type result;
      VMFactoryAttribute.TryGetValue(type, out result);
      return result;
    }
  }
}
