using Cofe.Utility;
using System;
using System.Reflection;

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
